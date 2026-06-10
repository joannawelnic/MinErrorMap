using System;
using System.Collections.Generic;

namespace MinErrorMap
{
    public class MatrixGenerator
    {
        private Random _random = new Random();
        private int _nRows;
        private int _nCols;

        /// <summary>
        /// Liczba rzeczywiście wprowadzonych błędów do macierzy.
        /// Stanowi dolną granicę wartości optimum po przetasowaniu kolumn.
        /// </summary>
        public int KnownErrors { get; private set; } = 0;

        /// <summary>
        /// Generuje losową macierz binarną spełniającą własność C1P (consecutive ones property).
        /// Każdy wiersz ma ciągły blok jedynek o długości co najmniej 2.
        /// Gwarantuje brak kolumn złożonych wyłącznie z zer.
        /// </summary>
        public int[,] GenerateMatrix(int rows, int cols)
        {
            if (cols < 2)
                throw new ArgumentException("Macierz musi mieć co najmniej 2 kolumny.");

            _nRows = rows;
            _nCols = cols;
            KnownErrors = 0;

            int[,] matrix;
            bool isValid;

            do
            {
                matrix = new int[rows, cols];
                isValid = true;

                for (int i = 0; i < rows; i++)
                {
                    // Start bloku: 0 do cols-2 (zostaje miejsce na min. 2 jedynki)
                    int start = _random.Next(0, cols - 1);
                    // Długość bloku: min 2, max tyle ile zostało do końca
                    int length = _random.Next(2, cols - start + 1);
                    for (int j = start; j < start + length; j++)
                        matrix[i, j] = 1;
                }

                // Walidacja: żadna kolumna nie może być złożona z samych zer
                for (int j = 0; j < cols && isValid; j++)
                {
                    bool hasOne = false;
                    for (int i = 0; i < rows && !hasOne; i++)
                        hasOne = matrix[i, j] == 1;
                    if (!hasOne) isValid = false;
                }
            } while (!isValid);

            return matrix;
        }

        /// <summary>
        /// Tworzy pustą macierz (same zera) do ręcznego wypełnienia przez użytkownika.
        /// </summary>
        public int[,] CreateEmptyMatrix(int rows, int cols)
        {
            _nRows = rows;
            _nCols = cols;
            KnownErrors = 0;
            return new int[rows, cols];
        }

        /// <summary>
        /// Wprowadza DOKŁADNIE errorsRequested realnych błędów do macierzy C1P.
        ///
        /// Zasada błędu realnego (nie naruszającego struktury w wymyślny sposób):
        ///   1→0 : tylko jeśli oba sąsiedzi (lewy i prawy) to 1 – środek bloku jedynek.
        ///          Zamiana końca bloku 1→0 jedynie skraca blok, co nie jest prawdziwym błędem.
        ///   0→1 : tylko jeśli żaden sąsiad nie jest 1 – izolowana pozycja.
        ///          Zamiana zera 0→1 obok końca bloku jedynie ten blok rozszerza.
        ///
        /// Metoda pre-skanuje wszystkie dozwolone pozycje, tasuje je i stosuje pierwsze N.
        /// Rzuca wyjątek jeśli macierz nie ma wystarczająco dużo dozwolonych pozycji.
        /// </summary>
        public void ApplyErrors(int[,] matrix, int errorsRequested)
        {
            _nRows = matrix.GetLength(0);
            _nCols = matrix.GetLength(1);

            // Zbierz wszystkie pozycje gdzie można wprowadzić realny błąd
            var validPositions = new List<(int r, int c, int newVal)>();

            for (int r = 0; r < _nRows; r++)
            {
                for (int c = 0; c < _nCols; c++)
                {
                    bool leftIsOne  = (c > 0)          && matrix[r, c - 1] == 1;
                    bool rightIsOne = (c < _nCols - 1) && matrix[r, c + 1] == 1;

                    if (matrix[r, c] == 1 && leftIsOne && rightIsOne)
                    {
                        // Środek bloku jedynek – zamiana 1→0 tworzy "dziurę" (realny błąd)
                        validPositions.Add((r, c, 0));
                    }
                    else if (matrix[r, c] == 0 && !leftIsOne && !rightIsOne)
                    {
                        // Izolowana pozycja – zamiana 0→1 tworzy samotną jedynkę (realny błąd)
                        validPositions.Add((r, c, 1));
                    }
                }
            }

            if (validPositions.Count < errorsRequested)
                throw new InvalidOperationException(
                    $"Można wprowadzić maksymalnie {validPositions.Count} realnych błędów " +
                    $"do tej macierzy. Zmniejsz liczbę błędów lub zwiększ rozmiar macierzy.");

            // Tasuj Fisherem-Yatesem i wybierz pierwsze errorsRequested pozycji
            for (int i = validPositions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (validPositions[i], validPositions[j]) = (validPositions[j], validPositions[i]);
            }

            for (int k = 0; k < errorsRequested; k++)
            {
                var (r, c, newVal) = validPositions[k];
                matrix[r, c] = newVal;
            }

            KnownErrors = errorsRequested;
        }

        /// <summary>
        /// Tasuje kolumny macierzy algorytmem Fisher-Yates.
        /// Zwraca nową macierz z przetasowanymi kolumnami.
        /// </summary>
        public int[,] ShuffleColumns(int[,] originalMatrix)
        {
            int rows = originalMatrix.GetLength(0);
            int cols = originalMatrix.GetLength(1);

            int[] colIndices = new int[cols];
            for (int i = 0; i < cols; i++) colIndices[i] = i;

            for (int i = cols - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (colIndices[i], colIndices[j]) = (colIndices[j], colIndices[i]);
            }

            int[,] shuffled = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    shuffled[i, j] = originalMatrix[i, colIndices[j]];

            return shuffled;
        }
    }
}