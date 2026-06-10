using System.Threading;
using System.Threading.Tasks;

namespace MinErrorMap
{
    public partial class Form1 : Form
    {
        private MatrixGenerator _generator;
        private int[,] _currentMatrix; // Przechowujemy aktualną macierz w pamięci
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;
        private bool _isPaused = false;
        private bool _isUpdatingGrid = false; 

        public Form1()
        {
            InitializeComponent();
            _generator = new MatrixGenerator();
            dgvMatrix.CellFormatting += DgvMatrix_CellFormatting;
            dgvResults.CellFormatting += DgvMatrix_CellFormatting;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            // Pobieranie danych od użytkownika z zabezpieczeniem przed pustymi/złymi wartościami
            if (int.TryParse(numRows.Text, out int rows) &&
                int.TryParse(numCols.Text, out int cols))
            {
                // Generujemy i wyświetlamy TYLKO idealny wzorzec
                _currentMatrix = _generator.GenerateMatrix(rows, cols);
                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
            }
            else
            {
                MessageBox.Show("Wprowadź poprawne wartości dla wierszy i kolumn!");
            }
        }

        // Metoda pomocnicza do rysowania macierzy w DataGridView
        private void DisplayMatrixInGrid(DataGridView grid, int[,] matrix, int rows, int cols)
        {
            _isUpdatingGrid = true; // ← blokujemy event przed nadpisaniem

            grid.Rows.Clear();
            grid.Columns.Clear();

            // Dodawanie kolumn do siatki
            for (int i = 0; i < cols; i++)
            {
                grid.Columns.Add($"Col{i}", $"{i}");
                grid.Columns[i].Width = 40; // Ustawienie szerokości kolumn
            }

            // Dodawanie wierszy i wypełnianie ich danymi
            for (int i = 0; i < rows; i++)
            {
                grid.Rows.Add();
                for (int j = 0; j < cols; j++)
                {
                    grid.Rows[i].Cells[j].Value = matrix[i, j];
                }
            }
            _isUpdatingGrid = false;
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null) { MessageBox.Show("brak macierzy"); return; }

            if (int.TryParse(numErrors.Text, out int errors))
            {

                _generator.ApplyErrors(_currentMatrix, errors);


                int rows = _currentMatrix.GetLength(0);
                int cols = _currentMatrix.GetLength(1);
                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);

            }
            else
            {
                MessageBox.Show("TryParse zwróciło false - wartość: " + numErrors.Text);
            }
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            if (_currentMatrix != null)
            {
                _currentMatrix = _generator.ShuffleColumns(_currentMatrix);
                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));
            }
        }

        // Zapis do pliku

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null)
            {
                MessageBox.Show("Nie ma macierzy do zapisania!");
                return;
            }

            // Najpierw pobieramy ewentualne ręczne zmiany z tabeli!
            //SyncMatrixWithGrid();

            // Otwieramy okno dialogowe do wyboru miejsca zapisu
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Plik tekstowy (*.txt)|*.txt";
            sfd.Title = "Zapisz instancję macierzy";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                int rows = _currentMatrix.GetLength(0);
                int cols = _currentMatrix.GetLength(1);

                // Używamy StreamWriter do zapisu linijka po linijce
                using (StreamWriter sw = new StreamWriter(sfd.FileName))
                {
                    for (int i = 0; i < rows; i++)
                    {
                        string[] rowValues = new string[cols];

                        for (int j = 0; j < cols; j++)
                        {
                            rowValues[j] = _currentMatrix[i, j].ToString();
                        }
                        // Łączymy liczby spacją i zapisujemy wiersz
                        sw.WriteLine(string.Join(" ", rowValues));
                    }
                }
                MessageBox.Show("Macierz została pomyślnie zapisana!", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            // Otwieramy okno dialogowe do wyboru pliku
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Plik tekstowy (*.txt)|*.txt";
            ofd.Title = "Wczytaj instancję macierzy";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Wczytujemy wszystkie linie tekstu z pliku
                string[] lines = File.ReadAllLines(ofd.FileName);

                if (lines.Length == 0)
                {
                    MessageBox.Show("Wybrany plik jest pusty.");
                    return;
                }

                // Ustalamy wymiary nowej macierzy
                int rows = lines.Length;
                // Dzielimy pierwszą linię po spacjach, aby sprawdzić ile jest kolumn
                string[] firstRow = lines[0].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int cols = firstRow.Length;

                // Inicjalizujemy nową macierz

                _currentMatrix = new int[rows, cols];
                // Wypełniamy macierz danymi z pliku
                for (int i = 0; i < rows; i++)
                {
                    string[] cells = lines[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < cols; j++)
                    {
                        // Odczytujemy liczbę, upewniając się, że nie wykraczamy poza zakres
                        if (j < cells.Length && int.TryParse(cells[j], out int val) && (val == 0 || val == 1))
                        {
                            _currentMatrix[i, j] = val;
                        }
                    }
                }

                // Aktualizujemy pola tekstowe (TextBoxy) z wymiarami
                numRows.Text = rows.ToString();
                numCols.Text = cols.ToString();

                // Wyświetlamy wczytaną macierz w siatce
                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
            }
        }

        private void dgvMatrix_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingGrid) return; // ← ignorujemy eventy podczas rysowania

            // 1. Zabezpieczenie: Sprawdzamy, czy macierz istnieje i czy zmiana nie dotyczy nagłówków (indeks -1)
            if (_currentMatrix != null && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // 2. Pobieramy wpisaną wartość z konkretnej, zmienionej komórki
                var cellValue = dgvMatrix.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                // 3. Sprawdzamy, czy wpisano poprawną liczbę - i sprawdza czy jest to 0 lub 1
                if (int.TryParse(cellValue?.ToString(), out int val) && (val == 0 || val == 1))
                {
                    // 4. Aktualizujemy tylko to jedno konkretne pole w naszej tablicy w pamięci!
                    _currentMatrix[e.RowIndex, e.ColumnIndex] = val;
                }
            }
        }

        // --- SEKCJA TABU SEARCH I WYKRESU ---

        // Ta metoda będzie naszym delegatem wywoływanym przez algorytm w tle
        private void UpdateProgressUI(int iteration, int bestScore)
        {
            // --- NOWE ZABEZPIECZENIE: Jeśli wciśnięto STOP, ignorujemy wszelkie zaległe komunikaty! ---
            if (_cts != null && _cts.IsCancellationRequested) return;

            // Zabezpieczenie wielowątkowe: Jeśli ta metoda jest wywoływana z innego wątku niż główny,
            // prosimy formularz (Invoke), aby wykonał ją u siebie bezpiecznie.
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, int>(UpdateProgressUI), iteration, bestScore);
                return; // Zatrzymujemy wykonanie w wątku w tle
            }

            // --- Ten kod wykonuje się już BEZPIECZNIE w głównym wątku ---

            // Dodajemy nowy punkt do naszego wykresu na osi X (iteracja) i osi Y (wynik funkcji celu)
            chartProgress.Series[0].Points.AddXY(iteration, bestScore);

            // Aktualizujemy tekst etykiety
            lblStatus.Text = $"Iteracja: {iteration} | Najlepszy wynik (błędy): {bestScore}";
        }

        private async void btnStartSearch_Click(object sender, EventArgs e)
        {
            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true); // true = szlaban podniesiony na start
            _isPaused = false;
            btnPause.Text = "Pauza"; // Upewniamy się, że przycisk ma właściwy tekst na start

            // 1. Zabezpieczenia i pobranie danych
            if (_currentMatrix == null)
            {
                MessageBox.Show("Przejdź do pierwszej zakładki i wygeneruj macierz początkową!", "Brak danych");
                return;
            }

            if (!int.TryParse(txtMaxIter.Text, out int maxIter) || !int.TryParse(txtTabuTenure.Text, out int tenure))
            {
                MessageBox.Show("Podaj poprawne parametry dla algorytmu (liczby całkowite)!");
                return;
            }

            // 2. Przygotowanie interfejsu i WYKRESU przed startem
            btnStartSearch.Enabled = false;

            // --- KONFIGURACJA WYKRESU ---
            chartProgress.Series[0].Points.Clear();
            chartProgress.Series[0].Name = "Liczba błędów"; // Nazwa dla legendy
            chartProgress.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine; // Zmiana na ciągłą linię (bardzo ważne!)
            chartProgress.Series[0].BorderWidth = 2; // Grubsza, lepiej widoczna linia
            chartProgress.Series[0].Color = System.Drawing.Color.Blue; // Kolor linii

            // Opisy osi
            chartProgress.ChartAreas[0].AxisX.Title = "Numer iteracji";
            chartProgress.ChartAreas[0].AxisY.Title = "Wartość funkcji celu (błędy)";
            // -----------------------------

            TabuSearchAlgorithm tabuSearch = new TabuSearchAlgorithm();
            int[] bestColumnOrder = null;

            // 3. URUCHOMIENIE W TLE (Wielowątkowość)

            await Task.Run(() =>
            {
                // UWAGA: Przekazujemy _cts.Token oraz _pauseEvent do algorytmu!
                bestColumnOrder = tabuSearch.RunTabuSearch(_currentMatrix, maxIter, tenure, UpdateProgressUI, _cts.Token, _pauseEvent);
            });


            // 4. PO ZAKOŃCZENIU PRACY W TLE
            // PO (poprawna kolejność):
            if (bestColumnOrder != null)
            {
                _currentMatrix = ReorderMatrixColumns(_currentMatrix, bestColumnOrder);

                // Wpisujemy dane PRZED przełączeniem zakładki
                DisplayMatrixInGrid(dgvResults, _currentMatrix, _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));

                // Dopiero teraz przełączamy zakładkę
                tabControl1.SelectedTab = tabControl1.TabPages[2];

                dgvResults.Refresh();
            }

            btnStartSearch.Enabled = true;

            if (!_cts.IsCancellationRequested)
            {
                lblStatus.Text += " -> ZAKOŃCZONO!";
            }
        }

        // Funkcja pomocnicza, która tworzy nową macierz z odpowiednio ułożonymi kolumnami
        private int[,] ReorderMatrixColumns(int[,] matrix, int[] columnOrder)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] newMatrix = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    newMatrix[i, j] = matrix[i, columnOrder[j]];
                }
            }
            return newMatrix;
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (_pauseEvent == null) return; // Zabezpieczenie, jeśli algorytm nie działa

            if (_isPaused)
            {
                // Wznawiamy działanie
                _pauseEvent.Set(); // Podnosimy szlaban
                btnPause.Text = "Pauza";
            }
            else
            {
                // Zatrzymujemy działanie
                _pauseEvent.Reset(); // Opuszczamy szlaban
                btnPause.Text = "Wznów";
            }
            _isPaused = !_isPaused;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel(); // Wysyłamy sygnał do przerwania pracy
                lblStatus.Text = "PRZERWANO PRZEZ UŻYTKOWNIKA!";

                // Odblokowujemy szlaban, na wypadek gdyby algorytm był zapauzowany (inaczej utknie na zawsze)
                if (_pauseEvent != null) _pauseEvent.Set();
            }
        }
        private void DgvMatrix_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            if (e.Value.ToString() == "1")
            {
                e.CellStyle.BackColor = Color.LightGreen;
                e.CellStyle.ForeColor = Color.White;
            }
            else if (e.Value.ToString() == "0")
            {
                e.CellStyle.BackColor = Color.LightPink;
                e.CellStyle.ForeColor = Color.Black;
            }
        }

        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
    }
}
