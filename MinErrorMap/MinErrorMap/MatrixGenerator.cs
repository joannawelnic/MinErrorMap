using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MinErrorMap
{
    public class MatrixGenerator
    {
        // Używamy jednego obiektu Random dla całej klasy, aby losowania były poprawne
        private Random _random = new Random();
        private int nRows;
        private int nCols;

        /// <summary>
        /// Generuje macierz z ukrytym rozwiązaniem optymalnym (C1P) i błędami.
        /// </summary>
        public int[,] GenerateMatrix(int rows, int cols)
        {
            nRows = rows;
            nCols = cols;

            // Zabezpieczenie na wypadek wpisania np. 1 kolumny przez użytkownika
            if (cols < 2)
            {
                throw new ArgumentException("Aby spełnić warunek min. 2 jedynek, macierz musi mieć co najmniej 2 kolumny.");
            }

            int[,] matrix = new int[rows, cols];
            bool isValid = false; // Flaga sprawdzająca, czy macierz jest poprawna

            // Powtarzamy generowanie, dopóki nie uzyskamy macierzy bez pustych kolumn
            while (!isValid)
            {
                matrix = new int[rows, cols]; // Tworzymy nową, czystą macierz

                for (int i = 0; i < rows; i++)
                {
                    // Losujemy początek, ale upewniamy się, że zostaje miejsce na min. 2 jedynki
                    int start = _random.Next(0, cols - 1);

                    // Długość bloku to minimum 2, a maksimum to ile zostało do końca wiersza
                    int length = _random.Next(2, cols - start + 1);

                    for (int j = start; j < start + length; j++)
                    {
                        matrix[i, j] = 1;
                    }
                }

                // Sprawdzamy, czy wygenerowana macierz ma jakieś puste kolumny (same zera)
                isValid = true; // Zakładamy na start, że jest ok
                for (int j = 0; j < cols; j++)
                {
                    bool hasOneInColumn = false;
                    for (int i = 0; i < rows; i++)
                    {
                        if (matrix[i, j] == 1)
                        {
                            hasOneInColumn = true;
                            break; // Znaleźliśmy jedynkę, nie musimy dalej sprawdzać tej kolumny
                        }
                    }

                    // Jeśli po przejrzeniu całego wiersza w tej kolumnie nie ma jedynki
                    if (!hasOneInColumn)
                    {
                        isValid = false; // Macierz jest niepoprawna, pętla while uruchomi się ponownie
                        break; // Przerywamy sprawdzanie kolejnych kolumn
                    }
                }
            }
            return matrix;
        }

        // KROK 2: Wprowadza błędy do istniejącej macierzy
        public void ApplyErrors(int[,] matrix, int errors)
        {

            int errorsApplied = 0;
            int maxAttempts = errors * 200; // zabezpieczenie przed nieskończoną pętlą
            int attempts = 0;

            while (errorsApplied < errors && attempts < maxAttempts)
            {
                attempts++;
                int r = _random.Next(0, nRows);
                int c = _random.Next(0, nCols);

                if (matrix[r, c] == 1)
                {
                    // 1→0 tylko gdy obie sąsiednie komórki to też 1 (środek bloku)
                    bool leftIsOne = (c > 0) && matrix[r, c - 1] == 1;
                    bool rightIsOne = (c < nCols - 1) && matrix[r, c + 1] == 1;

                    if (leftIsOne && rightIsOne)
                    {
                        matrix[r, c] = 0;
                        errorsApplied++;
                    }
                }
                else
                {
                    // 0→1 tylko gdy żaden sąsiad nie jest 1 (izolowana pozycja)
                    bool leftIsOne = (c > 0) && matrix[r, c - 1] == 1;
                    bool rightIsOne = (c < nCols - 1) && matrix[r, c + 1] == 1;

                    if (!leftIsOne && !rightIsOne)
                    {
                        matrix[r, c] = 1;
                        errorsApplied++;
                    }
                }
            }

            // Informacja jeśli nie udało się wprowadzić wszystkich błędów
            if (errorsApplied < errors)
            {
                // Możesz tu rzucić wyjątek, wyświetlić MessageBox, lub po prostu zalogować
                System.Diagnostics.Debug.WriteLine(
                    $"Uwaga: wprowadzono tylko {errorsApplied}/{errors} realnych błędów.");
            }
        }

        /// <summary>
        /// Zmienia losowo kolejność kolumn w macierzy.
        /// </summary>
        // KROK 3: Tasuje kolumny istniejącej macierzy i zwraca nową
        public int[,] ShuffleColumns(int[,] originalMatrix)
        {
            int rows = originalMatrix.GetLength(0);
            int cols = originalMatrix.GetLength(1);
            int[] colIndices = new int[cols];

            for (int i = 0; i < cols; i++) colIndices[i] = i;

            for (int i = cols - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                int temp = colIndices[i];
                colIndices[i] = colIndices[j];
                colIndices[j] = temp;
            }

            int[,] shuffledMatrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    shuffledMatrix[i, j] = originalMatrix[i, colIndices[j]];
                }
            }
            return shuffledMatrix;
        }
    }
}
