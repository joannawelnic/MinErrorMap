namespace MinErrorMap
{
    public partial class Form1 : Form
    {
        private MatrixGenerator _generator;
        private int[,] _currentMatrix; // Przechowujemy aktualn¹ macierz w pamiêci

        public Form1()
        {
            InitializeComponent();
            _generator = new MatrixGenerator();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Pobieranie danych od u¿ytkownika z zabezpieczeniem przed pustymi/z³ymi wartoœciami
            if (int.TryParse(numRows.Text, out int rows) &&
                int.TryParse(numCols.Text, out int cols))
            {
                // Generujemy i wyœwietlamy TYLKO idealny wzorzec
                _currentMatrix = _generator.GenerateMatrix(rows, cols);
                DisplayMatrixInGrid(_currentMatrix, rows, cols);
            }
            else
            {
                MessageBox.Show("WprowadŸ poprawne wartoœci dla wierszy i kolumn!");
            }
        }

        // Metoda pomocnicza do rysowania macierzy w DataGridView
        private void DisplayMatrixInGrid(int[,] matrix, int rows, int cols)
        {
            dgvMatrix.Rows.Clear();
            dgvMatrix.Columns.Clear();

            // Dodawanie kolumn do siatki
            for (int i = 0; i < cols; i++)
            {
                dgvMatrix.Columns.Add($"Col{i}", $"{i}");
                dgvMatrix.Columns[i].Width = 40; // Ustawienie szerokoœci kolumn
            }

            // Dodawanie wierszy i wype³nianie ich danymi
            for (int i = 0; i < rows; i++)
            {
                dgvMatrix.Rows.Add();
                for (int j = 0; j < cols; j++)
                {
                    dgvMatrix.Rows[i].Cells[j].Value = matrix[i, j];
                }
            }
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            // 1. Zabezpieczenie: Sprawdzamy, czy macierz w ogóle istnieje
            if (_currentMatrix == null)
            {
                MessageBox.Show("Najpierw wygeneruj wzorzec macierzy, klikaj¹c 'Generuj Macierz'!", "Brak macierzy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return; // Zatrzymujemy dalsze wykonywanie kodu
            }
            if (int.TryParse(numErrors.Text, out int errors))
            {
                // 3. Logika: Wywo³ujemy metodê z naszej klasy generatora.
                // Zauwa¿, ¿e przekazujemy _currentMatrix. Metoda zmodyfikuje tê macierz "w miejscu".
                _generator.ApplyErrors(_currentMatrix, errors);

                // 4. Odœwie¿enie interfejsu: Rysujemy zaktualizowan¹ macierz od nowa w DataGridView
                int rows = _currentMatrix.GetLength(0);
                int cols = _currentMatrix.GetLength(1);
                DisplayMatrixInGrid(_currentMatrix, rows, cols);
            }
            else
            {
                // Komunikat b³êdu, jeœli u¿ytkownik wpisze np. litery zamiast cyfr lub wartoœæ 0
                MessageBox.Show("WprowadŸ poprawn¹, ca³kowit¹ liczbê b³êdów wiêksz¹ od zera!", "B³¹d danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            if (_currentMatrix != null)
            {
                _currentMatrix = _generator.ShuffleColumns(_currentMatrix);
                DisplayMatrixInGrid(_currentMatrix, _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));
            }
        }
    }
}
