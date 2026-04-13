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
            int[,] matrix = new int[rows, cols];

            // ETAP 1: Generowanie wzorca (własność consecutive 1s)
            for (int i = 0; i < rows; i++)
            {
                // cols to ilosc kolumn czyli dlugosc wiersza
                int start = _random.Next(0, cols); // numer - Losowy początek bloku jedynek
                // Długość bloku to minimum 1, maksimum tyle, ile zostało miejsca do końca wiersza
                int length = _random.Next(1, cols - start + 1); // dl wiersza - startowy punkt

                for (int j = start; j < start + length; j++)
                {
                    matrix[i, j] = 1;
                }
            }
            return matrix;
        }

        // KROK 2: Wprowadza błędy do istniejącej macierzy
        public void ApplyErrors(int[,] matrix, int errors)
        {
            int errorsApplied = 0;
            while (errorsApplied < errors)
            {
                int r = _random.Next(0, nRows);
                int c = _random.Next(0, nCols);

                // Odwracamy bit: jeśli jest 1 to robimy 0, jeśli 0 to robimy 1
                if (matrix[r, c] == 0)
                {
                    matrix[r, c] = 1;
                }
                else
                {
                    matrix[r, c] = 0;
                }
                errorsApplied++;
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
