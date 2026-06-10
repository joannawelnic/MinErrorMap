using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace MinErrorMap
{
    public class TabuSearchAlgorithm
    {
        // Dwuwymiarowa tablica przechowująca informacje o zakazach
        private int[,] _tabuList;

        // Zmienna określająca, przez ile iteracji dany ruch ma być zakazany (kadencja Tabu)
        private int _tabuTenure;

        /// <summary>
        /// Główna funkcja celu. Ocenia jakość podanego układu kolumn.
        /// </summary>
        /// <param name="matrix">Oryginalna macierz z błędami.</param>
        /// <param name="columnOrder">Tablica reprezentująca aktualnie testowaną kolejność kolumn (np. [3, 0, 1, 2]).</param>
        /// <returns>Zwraca łączną liczbę błędów do poprawy (im mniej, tym lepiej).</returns>
        public int CalculateObjectiveFunction(int[,] matrix, int[] columnOrder)
        {
            int totalErrors = 0;
            // 0 oznacza 1 wymiar tablicy a 1 drugi wymiar tablicy- liczba wierszy i kolumn
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            // Sprawdzamy każdy wiersz po kolei
            for (int i = 0; i < rows; i++)
            {
                // Dodajemy do sumy minimalną liczbę błędów znalezioną dla danego wiersza
                totalErrors += CalculateMinErrorsForRow(matrix, i, columnOrder, cols);
            }

            return totalErrors;
        }

        /// <summary>
        /// Funkcja pomocnicza: liczy minimalne błędy dla jednego konkretnego wiersza.
        /// </summary>
        private int CalculateMinErrorsForRow(int[,] matrix, int rowIndex, int[] columnOrder, int cols)
        {
            // zakładamy na start najgorszy scenariusz: musimy zmienić wszystkie komórki w wierszu
            int minFlips = cols;

            // sprawdzamy każdy możliwy indeks początkowy dla "idealnego" bloku jedynek - sprawdzanie tylko do przedostatniej kolumny
            for (int start = 0; start < cols - 1; start++)
            {
                // Sprawdzamy każdy możliwy indeks końcowy dla "idealnego" bloku jedynek
                // ZMIANA: Koniec bloku (end) musi być przynajmniej o 1 większy od startu
                // co gwarantuje, że długość ocenianego bloku to minimum 2
                for (int end = start + 1; end < cols; end++)
                {
                    int currentFlips = 0; // licznik błędów dla tej konkretnej kombinacji (start-end)

                    // przechodzimy przez wszystkie kolumny w wierszu zgodnie z testowaną kolejnością (columnOrder)
                    for (int k = 0; k < cols; k++)
                    {
                        // pobieramy prawdziwy indeks kolumny z naszego rozwiązania
                        int realColIndex = columnOrder[k];
                        // odczytujemy wartość komórki
                        int cellValue = matrix[rowIndex, realColIndex];

                        if (k >= start && k <= end)
                        {
                            // jesteśmy WEWNĄTRZ założonego bloku jedynek
                            // jeśli jest tu zero (0) to traktujemy to jako błąd do poprawy
                            if (cellValue == 0) currentFlips++;
                        }
                        else
                        {
                            // jesteśmy POZA założonym blokiem jedynek (powinny tu być same zera)
                            // jeśli jest tu jedynka (1) to traktujemy to jako błąd do usunięcia
                            if (cellValue == 1) currentFlips++;
                        }
                    }

                    // jeśli ta kombinacja wymaga mniej poprawek zapisujemy ją jako nasz nowy rekord dla tego wiersza
                    if (currentFlips < minFlips)
                    {
                        minFlips = currentFlips;
                    }
                }
            }

            return minFlips;
        }

        /// <summary>
        /// Generuje wszystkich możliwych sąsiadów dla aktualnego ułożenia kolumn.
        /// Sąsiad powstaje przez zamianę miejscami dwóch dowolnych kolumn.
        /// </summary>
        /// <param name="currentOrder">Aktualna kolejność kolumn (np. [0, 1, 2, 3]).</param>
        /// <returns>Lista nowych, sąsiednich ułożeń kolumn.</returns>
        public List<int[]> GenerateNeighborhood(int[] currentOrder)
        {
            // przygotowujemy pustą listę do której będziemy wrzucać nowych sąsiadów
            List<int[]> neighborhood = new List<int[]>();
            int numberOfColumns = currentOrder.Length;

            // Używamy dwóch pętli for aby wygenerować każdą możliwą unikalną parę do zamiany
            for (int i = 0; i < numberOfColumns - 1; i++)
            {
                for (int j = i + 1; j < numberOfColumns; j++)
                {
                    // BARDZO WAŻNE: Tworzymy kopię aktualnego ułożenia! 
                    // Jeśli byśmy tego nie zrobili, zmienialibyśmy oryginalną tablicę.
                    int[] neighbor = (int[])currentOrder.Clone();

                    // Wykonujemy operację Swap (zamiana miejscami wartości pod indeksami i oraz j)
                    int temp = neighbor[i];
                    neighbor[i] = neighbor[j];
                    neighbor[j] = temp;

                    // Dodajemy nowo utworzonego "sąsiada" do naszej listy
                    neighborhood.Add(neighbor);
                }
            }

            return neighborhood; // Zwracamy gotową listę wszystkich sąsiadów
        }

        /// <summary>
        /// Inicjalizuje lub resetuje Listę Tabu przed startem algorytmu.
        /// </summary>
        /// <param name="numberOfColumns">Liczba kolumn w naszej macierzy (rozmiar listy).</param>
        /// <param name="tabuTenure">Jak długo dany ruch ma być zakazany.</param>
        public void InitializeTabuList(int numberOfColumns, int tabuTenure)
        {
            // Tworzymy nową, pustą tablicę o wymiarach liczba kolumn x liczba kolumn
            _tabuList = new int[numberOfColumns, numberOfColumns];
            _tabuTenure = tabuTenure;
        }

        /// <summary>
        /// Sprawdza, czy zamiana dwóch konkretnych kolumn jest w tej chwili zakazana.
        /// </summary>
        /// <param name="col1">Indeks pierwszej kolumny do zamiany.</param>
        /// <param name="col2">Indeks drugiej kolumny do zamiany.</param>
        /// <param name="currentIteration">Numer aktualnie wykonywanej iteracji algorytmu.</param>
        /// <returns>Zwraca true, jeśli ruch jest zablokowany, w przeciwnym razie false.</returns>
        public bool IsTabu(int col1, int col2, int currentIteration)
        {
            // Jeśli aktualna iteracja nie przekroczyła zapisanej daty wygaśnięcia zakazu, ruch jest Tabu
            return _tabuList[col1, col2] >= currentIteration;
        }

        /// <summary>
        /// Wpisuje ruch na Listę Tabu po jego wykonaniu.
        /// </summary>
        /// <param name="col1">Indeks pierwszej zamienionej kolumny.</param>
        /// <param name="col2">Indeks drugiej zamienionej kolumny.</param>
        /// <param name="currentIteration">Numer iteracji, w której wykonano ruch.</param>
        public void MakeTabu(int col1, int col2, int currentIteration)
        {
            // Obliczamy "datę wygaśnięcia" zakazu
            int expirationIteration = currentIteration + _tabuTenure;

            // Zapisujemy zakaz w obie strony. 
            // Zamiana kolumny 1 z 2 to to samo co 2 z 1, więc blokujemy obie kombinacje.
            _tabuList[col1, col2] = expirationIteration;
            _tabuList[col2, col1] = expirationIteration;
        }

        /// <summary>
        /// Uruchamia główną pętlę algorytmu Tabu Search.
        /// </summary>
        /// <param name="matrix">Macierz, którą chcemy uporządkować.</param>
        /// <param name="maxIterationsWithoutImprovement">Warunek stopu: ile iteracji bez poprawy kończy algorytm.</param>
        /// <param name="tabuTenure">Kadencja Tabu: na jak długo blokujemy ruch.</param>
        /// <param name="onProgressUpdate">Delegat pozwalający przekazać dane do paska postępu w UI (nr iteracji, najlepszy wynik).</param>
        /// <returns>Zwraca tablicę z najlepszą znalezioną kolejnością kolumn.</returns>
        public int[] RunTabuSearch(int[,] matrix, int maxIterationsWithoutImprovement, int tabuTenure, Action<int, int> onProgressUpdate, CancellationToken cancellationToken, ManualResetEventSlim pauseEvent)
        {
            int cols = matrix.GetLength(1);

            // Przygotowanie pamięci (Listy Tabu)
            InitializeTabuList(cols, tabuTenure);

            // Utworzenie początkowego ułożenia kolumn (np. [0, 1, 2, 3])
            int[] currentOrder = new int[cols];
            for (int i = 0; i < cols; i++) currentOrder[i] = i;

            // Ocena początkowego układu i zapisanie go jako nasz "globalny rekord"
            // zapisanie poczatkowej wartosci funkcji celu i ustawienia kolumn
            int bestGlobalScore = CalculateObjectiveFunction(matrix, currentOrder);
            int[] bestGlobalOrder = (int[])currentOrder.Clone();

            int currentScore = bestGlobalScore;
            int iterationsWithoutImprovement = 0;
            int currentIteration = 0;

            // GŁÓWNA PĘTLA - działa dopóki brak poprawy nie osiągnie limitu
            while (iterationsWithoutImprovement < maxIterationsWithoutImprovement)
            {
                // NOWE: Sprawdzamy czy mamy zatrzymać algorytm lub go zapauzować

                // jeśli kliknięto Pauzę algorytm "zaśnie" w tym miejscu i poczeka na wznowienie
                // BEZPIECZNA PAUZA I STOP
                try
                {
                    pauseEvent.Wait(cancellationToken); // czekamy jeśli włączono pauzę
                }
                catch (OperationCanceledException)
                {
                    break; // przerwano algorytm przyciskiem Stop podczas pauzy!
                }

                // jeśli kliknięto Stop wychodzimy z pętli
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                currentIteration++;

                // dla aktualnego ulozenia kolumn zapisujemy tu najmniejsza wartosc funkcji celu i ulozenie
                int bestNeighborScore = int.MaxValue;
                int[] bestNeighborOrder = null;
                int bestSwappedCol1 = -1;
                int bestSwappedCol2 = -1;

                // dla aktualnego ustawienia sprawdzamy wszystkie możliwe zamiany dwóch kolumn i szukamy najlepszego rozwiazania
                for (int i = 0; i < cols - 1; i++)
                {
                    for (int j = i + 1; j < cols; j++)
                    {
                        // tworzymy sąsiada przez zamianę
                        int[] neighbor = (int[])currentOrder.Clone();
                        int temp = neighbor[i];
                        neighbor[i] = neighbor[j];
                        neighbor[j] = temp;

                        // obliczamy błędy dla tego sąsiada
                        int neighborScore = CalculateObjectiveFunction(matrix, neighbor);

                        // UWAGA: sprawdzamy JAKIE konkretnie kolumny zamieniliśmy (ich prawdziwe indeksy)
                        int realCol1 = currentOrder[i];
                        int realCol2 = currentOrder[j];

                        bool isTabu = IsTabu(realCol1, realCol2, currentIteration);
                        bool beatsGlobalBest = neighborScore < bestGlobalScore; // Kryterium aspiracji

                        // akceptujemy sąsiada tylko jeśli nie jest Tabu ALBO bije globalny rekord i wtedy moze byc TABU
                        if (!isTabu || beatsGlobalBest)
                        {
                            // czy to lokalnie byla najlepsza zamiana
                            if (neighborScore < bestNeighborScore)
                            {
                                bestNeighborScore = neighborScore;
                                bestNeighborOrder = neighbor;
                                bestSwappedCol1 = realCol1;
                                bestSwappedCol2 = realCol2;
                            }
                        }
                    }
                }

                // Po lokalnym wyszukaniu najlepszej zamiany wykonujemy najlepszy dozwolony ruch
                currentOrder = bestNeighborOrder;
                currentScore = bestNeighborScore;
                // 
                MakeTabu(bestSwappedCol1, bestSwappedCol2, currentIteration);

                // Sprawdzamy, czy pobiliśmy historyczny rekord
                if (currentScore < bestGlobalScore)
                {
                    bestGlobalScore = currentScore;
                    bestGlobalOrder = (int[])currentOrder.Clone();
                    iterationsWithoutImprovement = 0; // Znaleźliśmy poprawę, więc zerujemy licznik!
                }
                else
                {
                    iterationsWithoutImprovement++; // Brak poprawy, licznik rośnie
                }
                // NAPRAWA ZAWIESZANIA UI (Wysyłamy dane tylko co 10 iteracji)
                //if (currentIteration % 3 == 0)
                //{
                    onProgressUpdate?.Invoke(currentIteration, bestGlobalScore);
                //}

            }
            // Wysyłamy informację do interfejsu użytkownika (jeśli podano delegat)
            onProgressUpdate?.Invoke(currentIteration, bestGlobalScore);
            // Zwracamy najlepsze znalezione ułożenie
            return bestGlobalOrder;
        }

    }
}
