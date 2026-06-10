using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MinErrorMap
{
    /// <summary>
    /// Dane przekazywane do UI po każdej iteracji algorytmu.
    /// </summary>
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

    /// <summary>
    /// Wynik działania algorytmu Tabu Search.
    /// </summary>
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

        // ─────────────────────────────────────────────────────────────────────
        // FUNKCJA CELU  –  O(m·n) zamiast O(m·n³)
        //
        // Klasyczne podejście iteruje po wszystkich parach (start, end) → O(n²) na wiersz,
        // a wewnętrzna pętla liczy koszty → łącznie O(n³) na wiersz.
        //
        // Kluczowa obserwacja (specjalizacja algorytmu):
        //   cost(start,end) = (end−start+1) + totalOnes − 2·ones_inside(start,end)
        //
        // Minimalizacja kosztu ↔ maksymalizacja sumy podciągu tablicy a[k] = 2·val[k]−1,
        // gdzie a[k] = +1 gdy val[k]=1, a[k] = −1 gdy val[k]=0.
        // To jest klasyczny problem MAXIMUM SUBARRAY SUM → Algorytm Kadane'a → O(n) na wiersz.
        //
        // Łączna złożoność ewaluacji: O(m·n).
        // Łączna złożoność jednej iteracji TS: O(n²·m·n) = O(m·n³).
        // ─────────────────────────────────────────────────────────────────────
        public int CalculateObjectiveFunction(int[,] matrix, int[] columnOrder)
        {
            int total = 0;
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            for (int i = 0; i < rows; i++)
                total += CalculateRowErrorKadane(matrix, i, columnOrder, cols);
            return total;
        }

        private int CalculateRowErrorKadane(int[,] matrix, int rowIndex, int[] columnOrder, int cols)
        {
            // Policz jedynki w wierszu
            int totalOnes = 0;
            for (int k = 0; k < cols; k++)
                totalOnes += matrix[rowIndex, columnOrder[k]];

            if (totalOnes == 0) return 0; // wiersz bez jedynek – trywialnie C1P

            // Algorytm Kadane'a: szukamy maksymalnej sumy podciągu tablicy (+1 lub -1)
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

        // ─────────────────────────────────────────────────────────────────────
        //  LISTA TABU
        // ─────────────────────────────────────────────────────────────────────
        public void InitializeTabuList(int numberOfColumns, int tabuTenure)
        {
            _tabuList = new int[numberOfColumns, numberOfColumns];
            _tabuTenure = tabuTenure;
        }

        public bool IsTabu(int col1, int col2, int currentIteration)
            => _tabuList[col1, col2] >= currentIteration;

        public void MakeTabu(int col1, int col2, int currentIteration)
        {
            int expiration = currentIteration + _tabuTenure;
            _tabuList[col1, col2] = expiration;
            _tabuList[col2, col1] = expiration;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  PERTURBACJA – losowe k swapów na kopii rozwiązania
        //  Cel: dywersyfikacja; wyrzucenie algorytmu z basenu lokalnego optimum
        //  przy zachowaniu dotychczas znalezionego globalnego rekordu.
        // ─────────────────────────────────────────────────────────────────────
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

        // ─────────────────────────────────────────────────────────────────────
        //  GŁÓWNA FUNKCJA ALGORYTMU
        //
        //  Parametry:
        //    maxIterationsWithoutImprovement – warunek stopu na fazę
        //    tabuTenure                      – kadencja Tabu (stała liczba iteracji)
        //    numberOfRestarts                – ile razy restartujemy z perturbacją
        //    perturbationSize                – ile losowych swapów przy restarcie
        //    neighborhoodSamplePct           – 0.01–1.0 (1.0 = pełne sąsiedztwo)
        // ─────────────────────────────────────────────────────────────────────
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

            // Pre-generacja wszystkich par (i,j) z i<j – używana do próbkowania sąsiedztwa
            int totalPairs = cols * (cols - 1) / 2;
            var allPairs = new List<(int posI, int posJ)>(totalPairs);
            for (int i = 0; i < cols - 1; i++)
                for (int j = i + 1; j < cols; j++)
                    allPairs.Add((i, j));

            int sampleSize = Math.Max(1, (int)Math.Round(totalPairs * neighborhoodSamplePct));
            bool fullNeighborhood = sampleSize >= totalPairs;

            // Inicjalne rozwiązanie: kolejność tożsamościowa [0,1,2,...,n-1]
            int[] currentOrder = new int[cols];
            for (int i = 0; i < cols; i++) currentOrder[i] = i;

            int initialScore = CalculateObjectiveFunction(matrix, currentOrder);
            int bestGlobalScore = initialScore;
            int[] bestGlobalOrder = (int[])currentOrder.Clone();
            int totalIterations = 0;

            // ── PĘTLA RESTARTÓW ──────────────────────────────────────────────
            // restart=0: pierwsze uruchomienie od rozwiązania tożsamościowego
            // restart>0: perturbacja najlepszego globalnego rozwiązania → nowy start
            for (int restart = 0; restart <= numberOfRestarts; restart++)
            {
                if (cancellationToken.IsCancellationRequested) break;

                if (restart > 0)
                {
                    // Perturbacja: losowe swappy na najlepszym dotychczas rozwiązaniu
                    currentOrder = ApplyPerturbation(bestGlobalOrder, perturbationSize);
                }

                int currentScore = CalculateObjectiveFunction(matrix, currentOrder);
                InitializeTabuList(cols, tabuTenure); // świeża lista Tabu na każdą fazę
                int iterationsWithoutImprovement = 0;
                int phaseIteration = 0;

                // ── PĘTLA TABU SEARCH W FAZIE ───────────────────────────────
                while (iterationsWithoutImprovement < maxIterationsWithoutImprovement)
                {
                    // Obsługa pauzy i stopu
                    try { pauseEvent.Wait(cancellationToken); }
                    catch (OperationCanceledException) { goto Done; }
                    if (cancellationToken.IsCancellationRequested) goto Done;

                    phaseIteration++;
                    totalIterations++;

                    int bestNeighborScore = int.MaxValue;
                    int[] bestNeighborOrder = null;
                    int bestSwapRealCol1 = -1, bestSwapRealCol2 = -1;

                    // Próbkowanie sąsiedztwa: częściowy Fisher-Yates dla podzbioru par
                    if (!fullNeighborhood)
                    {
                        for (int k = 0; k < sampleSize; k++)
                        {
                            int rndIdx = k + _random.Next(totalPairs - k);
                            (allPairs[k], allPairs[rndIdx]) = (allPairs[rndIdx], allPairs[k]);
                        }
                    }

                    // Ocena sąsiadów w próbce
                    for (int pairIdx = 0; pairIdx < sampleSize; pairIdx++)
                    {
                        var (posI, posJ) = allPairs[pairIdx];

                        // Tworzenie sąsiada przez swap pozycji posI i posJ
                        int[] neighbor = (int[])currentOrder.Clone();
                        (neighbor[posI], neighbor[posJ]) = (neighbor[posJ], neighbor[posI]);

                        int neighborScore = CalculateObjectiveFunction(matrix, neighbor);

                        // Prawdziwe indeksy kolumn (do tablicy Tabu)
                        int realCol1 = currentOrder[posI];
                        int realCol2 = currentOrder[posJ];

                        bool isTabu      = IsTabu(realCol1, realCol2, phaseIteration);
                        bool beatsGlobal = neighborScore < bestGlobalScore; // kryterium aspiracji

                        // Akceptacja: ruch dozwolony LUB bije globalny rekord (aspiracja)
                        if (!isTabu || beatsGlobal)
                        {
                            if (neighborScore < bestNeighborScore)
                            {
                                bestNeighborScore = neighborScore;
                                bestNeighborOrder = neighbor;
                                bestSwapRealCol1  = realCol1;
                                bestSwapRealCol2  = realCol2;
                            }
                        }
                    }

                    if (bestNeighborOrder == null) break; // wszyscy sąsiedzi zablokowani

                    currentOrder = bestNeighborOrder;
                    currentScore = bestNeighborScore;
                    MakeTabu(bestSwapRealCol1, bestSwapRealCol2, phaseIteration);

                    // Aktualizacja globalnego rekordu
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

                    // Raport do UI
                    onProgressUpdate?.Invoke(new ProgressInfo
                    {
                        TotalIterations              = totalIterations,
                        BestScore                    = bestGlobalScore,
                        InitialScore                 = initialScore,
                        CurrentRestart               = restart,
                        TotalRestarts                = numberOfRestarts,
                        ElapsedMs                    = stopwatch.ElapsedMilliseconds,
                        IterationsWithoutImprovement = iterationsWithoutImprovement,
                        MaxIterationsWithoutImprovement = maxIterationsWithoutImprovement
                    });
                }
            }

            Done:
            stopwatch.Stop();
            return new SearchResult
            {
                BestOrder           = bestGlobalOrder,
                BestScore           = bestGlobalScore,
                TotalIterations     = totalIterations,
                ElapsedMs           = stopwatch.ElapsedMilliseconds,
                RestartsPerformed   = numberOfRestarts
            };
        }
    }
}
