using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MinErrorMap
{
    // dane przekazywane do interfejsu po każdej iteracji algorytmu
    public class ProgressInfo
    {
        public int TotalIterations { get; set; }
        public int BestScore { get; set; }
        public int InitialScore { get; set; }
        public int CurrentRestart { get; set; }
        public int TotalRestarts { get; set; }
        public long ElapsedMs { get; set; }
        public int IterationsWithoutImprovement { get; set; }
        public int MaxIterationsWithoutImprovement { get; set; }
    }

    // wynik działania algorytmu Tabu Search
    public class SearchResult
    {
        public int[] BestOrder { get; set; }
        public int BestScore { get; set; }
        public int TotalIterations { get; set; }
        public long ElapsedMs { get; set; }
        public int RestartsPerformed { get; set; }
    }

    public class TabuSearchAlgorithm
    {
        private int[,] _tabuList;
        private int _tabuTenure;
        private Random _random = new Random();

        // FUNKCJA CELU
        public int CalculateObjectiveFunction(int[,] matrix, int[] columnOrder)
        {
            int total = 0;
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
                // uruchomienie algorytmu na maksymalną sume podciagu
                total += CalculateRowErrorKadane(matrix, i, columnOrder, cols);
            return total;
        }

        private int CalculateRowErrorKadane(int[,] matrix, int rowIndex, int[] columnOrder, int cols)
        {
            // liczenie jedynek w wierszu
            int totalOnes = 0;
            for (int k = 0; k < cols; k++)
                totalOnes += matrix[rowIndex, columnOrder[k]];

            // szukanie maksymalnej sumy podciągu tablicy (+1 lub -1)
            int maxSubarraySum = int.MinValue;
            int currentSum = 0;

            for (int k = 0; k < cols; k++)
            {
                int element = 2 * matrix[rowIndex, columnOrder[k]] - 1; // +1 dla 1, -1 dla 0
                currentSum = Math.Max(element, currentSum + element);
                if (currentSum > maxSubarraySum) maxSubarraySum = currentSum;
            }

            // min_cost = totalOnes − max_subarray_sum (min 0)
            return Math.Max(0, totalOnes - maxSubarraySum);
        }


        //  LISTA TABU
        public void InitializeTabuList(int numberOfColumns, int tabuTenure)
        {
            _tabuList = new int[numberOfColumns, numberOfColumns];
            _tabuTenure = tabuTenure;
        }

        public bool IsTabu(int col1, int col2, int currentIteration)
        {
            if (_tabuList[col1, col2] >= currentIteration)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void MakeTabu(int col1, int col2, int currentIteration)
        {
            int expiration = currentIteration + _tabuTenure;
            _tabuList[col1, col2] = expiration;
            _tabuList[col2, col1] = expiration;
        }

        //  PERTURBACJA – losowe k swapów na kopii rozwiązania
        private int[] ApplyPerturbation(int[] order, int perturbationSize)
        {
            int cols = order.Length;
            int[] perturbed = (int[])order.Clone();

            for (int p = 0; p < perturbationSize; p++)
            {
                int i = _random.Next(cols);
                int j;
                do { j = _random.Next(cols); } while (j == i);
                (perturbed[i], perturbed[j]) = (perturbed[j], perturbed[i]);
            }
            return perturbed;
        }

        //  TABU SEARCH ALGORYTM
        public SearchResult RunTabuSearch(
            int[,] matrix,
            int maxIterationsWithoutImprovement,
            int tabuTenure,
            int numberOfRestarts,
            int perturbationSize,
            double neighborhoodSamplePct,
            Action<ProgressInfo> onProgressUpdate,
            CancellationToken cancellationToken,
            ManualResetEventSlim pauseEvent)
        {
            int cols = matrix.GetLength(1);
            var stopwatch = Stopwatch.StartNew();

            // poczatkowa generacja wszystkich par (i,j) z i<j – do probkowania sasiedztwa
            int totalPairs = cols * (cols - 1) / 2;
            var allPairs = new List<(int posI, int posJ)>(totalPairs);
            for (int i = 0; i < cols - 1; i++)
                for (int j = i + 1; j < cols; j++)
                    allPairs.Add((i, j));

            int sampleSize = Math.Max(1, (int)Math.Round(totalPairs * neighborhoodSamplePct));
            bool fullNeighborhood = sampleSize >= totalPairs;

            // inicjalne rozwiązanie - pierwotna kolejnosc
            int[] currentOrder = new int[cols];
            for (int i = 0; i < cols; i++) currentOrder[i] = i;

            // obliczenie poczatkowej funkcji celu
            int initialScore = CalculateObjectiveFunction(matrix, currentOrder);
            int bestGlobalScore = initialScore;
            int[] bestGlobalOrder = (int[])currentOrder.Clone();
            int totalIterations = 0;

            // PĘTLA RESTARTÓW
            // restart=0 pierwsze uruchomienie dla inicjalnego rozwiazania
            // restart>0 perturbacja najlepszego globalnego rozwiazania, nowy start
            for (int restart = 0; restart <= numberOfRestarts; restart++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                if (restart > 0)
                {
                    // perturbacja - losowe swapy na najlepszym dotychczas rozwiazaniu
                    currentOrder = ApplyPerturbation(bestGlobalOrder, perturbationSize);
                }

                int currentScore = CalculateObjectiveFunction(matrix, currentOrder);
                InitializeTabuList(cols, tabuTenure); // świeża lista Tabu na każdą fazę
                int iterationsWithoutImprovement = 0;
                int phaseIteration = 0;

                // PĘTLA TABU SEARCH W FAZIE
                while (iterationsWithoutImprovement < maxIterationsWithoutImprovement)
                {
                    // pauza i stop
                    try { pauseEvent.Wait(cancellationToken); }
                    catch (OperationCanceledException) { goto Done; }
                    if (cancellationToken.IsCancellationRequested) goto Done;

                    phaseIteration++;
                    totalIterations++;

                    int bestNeighborScore = int.MaxValue;
                    int[] bestNeighborOrder = null;
                    int bestSwapRealCol1 = -1, bestSwapRealCol2 = -1;

                    // próbkowanie sasiedztwa
                    if (!fullNeighborhood)
                    {
                        for (int k = 0; k < sampleSize; k++)
                        {
                            int rndIdx = k + _random.Next(totalPairs - k);
                            (allPairs[k], allPairs[rndIdx]) = (allPairs[rndIdx], allPairs[k]);
                        }
                    }

                    // ocena sasiadów w próbce
                    for (int pairIdx = 0; pairIdx < sampleSize; pairIdx++)
                    {
                        var (posI, posJ) = allPairs[pairIdx];

                        // tworzenie sąsiada przez swap pozycji posI i posJ
                        int[] neighbor = (int[])currentOrder.Clone();
                        (neighbor[posI], neighbor[posJ]) = (neighbor[posJ], neighbor[posI]);

                        int neighborScore = CalculateObjectiveFunction(matrix, neighbor);

                        // prawdziwe indeksy kolumn do listy Tabu
                        int realCol1 = currentOrder[posI];
                        int realCol2 = currentOrder[posJ];

                        bool isTabu = IsTabu(realCol1, realCol2, phaseIteration);
                        bool beatsGlobal = neighborScore < bestGlobalScore; // kryterium aspiracji

                        // akceptacja gdy ruch dozwolony LUB bije globalny rekord (kryterium aspiracji)
                        if (!isTabu || beatsGlobal)
                        {
                            if (neighborScore < bestNeighborScore)
                            {
                                bestNeighborScore = neighborScore;
                                bestNeighborOrder = neighbor;
                                bestSwapRealCol1 = realCol1;
                                bestSwapRealCol2 = realCol2;
                            }
                        }
                    }

                    if (bestNeighborOrder == null) break; // wszyscy sąsiedzi zablokowani

                    currentOrder = bestNeighborOrder;
                    currentScore = bestNeighborScore;
                    MakeTabu(bestSwapRealCol1, bestSwapRealCol2, phaseIteration);

                    // aktualizacja globalnego rekordu
                    if (currentScore < bestGlobalScore)
                    {
                        bestGlobalScore = currentScore;
                        bestGlobalOrder = (int[])currentOrder.Clone();
                        iterationsWithoutImprovement = 0;
                    }
                    else
                    {
                        iterationsWithoutImprovement++;
                    }

                    // raport do interfejsu
                    onProgressUpdate?.Invoke(new ProgressInfo 
                    {
                        TotalIterations = totalIterations,
                        BestScore = bestGlobalScore,
                        InitialScore = initialScore,
                        CurrentRestart = restart,
                        TotalRestarts = numberOfRestarts,
                        ElapsedMs = stopwatch.ElapsedMilliseconds,
                        IterationsWithoutImprovement = iterationsWithoutImprovement,
                        MaxIterationsWithoutImprovement = maxIterationsWithoutImprovement
                    });
                }
            }
            Done:
            stopwatch.Stop();
            return new SearchResult
            {
                BestOrder = bestGlobalOrder,
                BestScore = bestGlobalScore,
                TotalIterations = totalIterations,
                ElapsedMs = stopwatch.ElapsedMilliseconds,
                RestartsPerformed = numberOfRestarts
            };
        }
    }
}
