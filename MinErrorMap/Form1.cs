using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinErrorMap
{
    public partial class Form1 : Form
    {
        // aplikacja
        private MatrixGenerator _generator = new MatrixGenerator();
        private int[,] _currentMatrix;
        private int _knownErrors = 0;
        private bool _isUpdatingGrid = false;

        // kontrola algorytmu tabu
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;
        private bool _isPaused = false;

        public Form1()
        {
            InitializeComponent();
            dgvMatrix.CellFormatting += DgvMatrix_CellFormatting;
            dgvResults.CellFormatting += DgvMatrix_CellFormatting;
        }


        //  TAB 1 – Generator instancji

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(numRows.Value.ToString(), out int rows) ||
                !int.TryParse(numCols.Value.ToString(), out int cols))
            {
                MessageBox.Show("Wprowadź poprawne wartości dla wierszy i kolumn!");
                return;
            }
            try
            {
                _currentMatrix = _generator.GenerateMatrix(rows, cols);
                _knownErrors = 0;
                UpdateKnownErrorsLabel("Znane błędy: 0 (brak błędów, macierz C1P)");
                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd generowania: " + ex.Message);
            }
        }

        private void btnCreateManual_Click(object sender, EventArgs e)
        {
            int rows = (int)numRows.Value;
            int cols = (int)numCols.Value;

            _currentMatrix = _generator.CreateEmptyMatrix(rows, cols);
            _knownErrors = 0;
            UpdateKnownErrorsLabel("Ręczna macierz - kliknij komórki aby wpisać 1");
            DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null) { MessageBox.Show("Najpierw wygeneruj lub wczytaj macierz."); return; }

            int rows = _currentMatrix.GetLength(0);
            int cols = _currentMatrix.GetLength(1);

            // liczenie procentu na liczbe błedów
            int errors = (int)Math.Round(rows * cols * (double)numErrors.Value / 100.0);
            if (errors == 0)
            {
                MessageBox.Show("Podany procent zaokrągla się do 0 błędów. Zwiększ wartość.",
                    "Za mało błędów", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _generator.ApplyErrors(_currentMatrix, errors);

                // za każdym kliknięciem dodawaj bledy do poprzednich
                _knownErrors += _generator.KnownErrors;

                double pct = 100.0 * _knownErrors / (rows * cols);
                UpdateKnownErrorsLabel(
                    $"Znane błędy (łącznie): {_knownErrors}  ({pct:F1}% komórek)  ");

                DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Za dużo błędów", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null) return;
            _currentMatrix = _generator.ShuffleColumns(_currentMatrix);
            DisplayMatrixInGrid(dgvMatrix, _currentMatrix, _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null) { MessageBox.Show("Nie ma macierzy do zapisania!"); return; }

            using var sfd = new SaveFileDialog
            {
                Filter = "Plik tekstowy (*.txt)|*.txt",
                Title = "Zapisz instancję macierzy"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            int rows = _currentMatrix.GetLength(0);
            int cols = _currentMatrix.GetLength(1);
            using var sw = new StreamWriter(sfd.FileName);
            // metadata
            sw.WriteLine($"# rows={rows} cols={cols} knownErrors={_knownErrors}");
            for (int i = 0; i < rows; i++)
            {
                var parts = new string[cols];
                for (int j = 0; j < cols; j++) parts[j] = _currentMatrix[i, j].ToString();
                sw.WriteLine(string.Join(" ", parts));
            }
            MessageBox.Show("Macierz zapisana.", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog
            {
                Filter = "Plik tekstowy (*.txt)|*.txt",
                Title = "Wczytaj instancję macierzy"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;

            string[] lines = File.ReadAllLines(ofd.FileName);
            if (lines.Length == 0) { MessageBox.Show("Plik jest pusty."); return; }

            // parsuj opcjonalna linię metadata
            int startLine = 0;
            _knownErrors = 0;
            if (lines[0].StartsWith("#"))
            {
                var meta = lines[0];
                var keMatch = System.Text.RegularExpressions.Regex.Match(meta, @"knownErrors=(\d+)");
                if (keMatch.Success) _knownErrors = int.Parse(keMatch.Groups[1].Value);
                startLine = 1;
            }

            int dataRows = lines.Length - startLine;
            if (dataRows <= 0) { MessageBox.Show("Brak danych."); return; }

            string[] firstRow = lines[startLine].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            int cols = firstRow.Length;
            _currentMatrix = new int[dataRows, cols];

            for (int i = 0; i < dataRows; i++)
            {
                string[] cells = lines[startLine + i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < cols; j++)
                {
                    if (j < cells.Length && int.TryParse(cells[j], out int val) && (val == 0 || val == 1))
                        _currentMatrix[i, j] = val;
                }
            }

            numRows.Value = dataRows;
            numCols.Value = cols;
            DisplayMatrixInGrid(dgvMatrix, _currentMatrix, dataRows, cols);

            // oblicz błedy bieżącej macierzy funkcją celu
            var ts = new TabuSearchAlgorithm();
            int[] id = new int[cols];
            for (int j = 0; j < cols; j++) id[j] = j;
            int calcErrors = ts.CalculateObjectiveFunction(_currentMatrix, id);

            if (_knownErrors > 0)
                UpdateKnownErrorsLabel($"Znane błędy (z pliku): {_knownErrors}  |  f.celu: {calcErrors}");
            else
                UpdateKnownErrorsLabel($"Błędy f.celu: {calcErrors}");
        }

        private void dgvMatrix_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_isUpdatingGrid) return;
            if (_currentMatrix == null || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var cellValue = dgvMatrix.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            if (int.TryParse(cellValue?.ToString(), out int val) && (val == 0 || val == 1))
            {
                _currentMatrix[e.RowIndex, e.ColumnIndex] = val;

                // Przelicz błędy po ręcznej zmianie
                var ts = new TabuSearchAlgorithm();
                int cols = _currentMatrix.GetLength(1);
                int[] id = new int[cols];
                for (int j = 0; j < cols; j++) id[j] = j;
                int calcErrors = ts.CalculateObjectiveFunction(_currentMatrix, id);
                UpdateKnownErrorsLabel($"Błędy f.celu (po edycji): {calcErrors}");
            }
        }

        //  TAB 2 – Tabu Search

        private void UpdateProgressUI(ProgressInfo info)
        {
            if (_cts != null && _cts.IsCancellationRequested) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ProgressInfo>(UpdateProgressUI), info);
                return;
            }

            // wykres
            chartProgress.Series[0].Points.AddXY(info.TotalIterations, info.BestScore);

            // status
            double impPct = info.InitialScore > 0 ? (info.InitialScore - info.BestScore) * 100.0 / info.InitialScore: 0;

            lblStatus.Text = $"Iter: {info.TotalIterations}  |  Najlepszy wynik: {info.BestScore}";

            // pasek postepu fazy (jak blisko stopu)
            progressBarAlgorithm.Maximum = info.MaxIterationsWithoutImprovement;
            progressBarAlgorithm.Value = Math.Min(info.IterationsWithoutImprovement,
                                                  info.MaxIterationsWithoutImprovement);

            // etykiety
            lblRestartInfo.Text = $"Restart: {info.CurrentRestart}/{info.TotalRestarts}";
            lblTimeElapsed.Text = $"Czas: {info.ElapsedMs} ms";
            lblImprovementPct.Text = $"Poprawa: {impPct:F1}%";
        }

        private async void btnStartSearch_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null)
            {
                MessageBox.Show("Przejdź do zakładki 1 i wygeneruj macierz!", "Brak danych");
                return;
            }
            if (!int.TryParse(txtMaxIter.Text, out int maxIter) ||
                !int.TryParse(txtTabuTenure.Text, out int tenure))
            {
                MessageBox.Show("Podaj poprawne wartości Iteracji i Kadencji Tabu (liczby całkowite).");
                return;
            }

            int restarts = (int)numRestarts.Value;
            int perturbation = (int)numPerturbation.Value;
            double neighborPct = (double)numNeighborhoodPct.Value / 100.0;

            _cts = new CancellationTokenSource();
            _pauseEvent = new ManualResetEventSlim(true);
            _isPaused = false;
            btnPause.Text = "PAUZA";

            btnStartSearch.Enabled = false;

            // wykres reset
            chartProgress.Series[0].Points.Clear();
            chartProgress.Series[0].Name = "Błędy";
            chartProgress.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chartProgress.Series[0].BorderWidth = 2;
            chartProgress.Series[0].Color = System.Drawing.Color.DodgerBlue;
            chartProgress.ChartAreas[0].AxisX.Title = "Numer iteracji";
            chartProgress.ChartAreas[0].AxisY.Title = "Wartość funkcji celu";

            progressBarAlgorithm.Value = 0;
            lblStatus.Text = "Uruchamianie...";
            ResetSummaryPanel();

            var ts = new TabuSearchAlgorithm();
            SearchResult result = null;

            await Task.Run(() =>
            {
                result = ts.RunTabuSearch(
                    _currentMatrix, maxIter, tenure, restarts, perturbation, neighborPct,
                    UpdateProgressUI, _cts.Token, _pauseEvent);
            });

            if (result?.BestOrder != null)
            {
                _currentMatrix = ReorderMatrixColumns(_currentMatrix, result.BestOrder);
                DisplayMatrixInGrid(dgvResults, _currentMatrix,_currentMatrix.GetLength(0), _currentMatrix.GetLength(1));

                tabControl1.SelectedTab = tabPage3;
                dgvResults.Refresh();
                UpdateSummaryPanel(result);

                int dist = result.BestScore - _knownErrors;
                string extra = _knownErrors > 0
                    ? $"  |  Odl. od opt.: {dist}  ({(dist * 100.0 / Math.Max(1, _knownErrors)):F0}%)"
                    : "";
                lblStatus.Text = _cts.IsCancellationRequested
                    ? "PRZERWANO"
                    : $"ZAKOŃCZONO – wynik: {result.BestScore}{extra}  czas: {result.ElapsedMs} ms";
            }

            btnStartSearch.Enabled = true;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (_pauseEvent == null) return;
            if (_isPaused) {
                _pauseEvent.Set(); btnPause.Text = "PAUZA"; 
            }
            else { 
                _pauseEvent.Reset(); btnPause.Text = "WZNÓW"; 
            }
            _isPaused = !_isPaused;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_cts == null || _cts.IsCancellationRequested) return;
            _cts.Cancel();
            _pauseEvent?.Set();
            lblStatus.Text = "PRZERWANO PRZEZ UŻYTKOWNIKA";
        }

        private void tabPage2_Click(object sender, EventArgs e) { }


        // pomocnicze

        private void DisplayMatrixInGrid(DataGridView grid, int[,] matrix, int rows, int cols)
        {
            _isUpdatingGrid = true;
            grid.Rows.Clear();
            grid.Columns.Clear();

            for (int i = 0; i < cols; i++)
            {
                grid.Columns.Add($"Col{i}", $"{i}");
                grid.Columns[i].Width = 38;
            }
            for (int i = 0; i < rows; i++)
            {
                grid.Rows.Add();
                for (int j = 0; j < cols; j++)
                    grid.Rows[i].Cells[j].Value = matrix[i, j];
            }
            _isUpdatingGrid = false;
        }

        private int[,] ReorderMatrixColumns(int[,] matrix, int[] columnOrder)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            int[,] newMatrix = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    newMatrix[i, j] = matrix[i, columnOrder[j]];
            return newMatrix;
        }

        private void UpdateKnownErrorsLabel(string text)
        {
            lblKnownErrors.Text = text;
        }

        private void UpdateSummaryPanel(SearchResult result)
        {
            int rows = _currentMatrix.GetLength(0);
            int cols = _currentMatrix.GetLength(1);

            lblSumSize.Text = $"{rows} × {cols}";
            lblSumKE.Text = _knownErrors > 0 ? _knownErrors.ToString() : "N/D";
            lblSumScore.Text = result.BestScore.ToString();

            if (_knownErrors > 0)
            {
                int dist = result.BestScore - _knownErrors;
                double relErr = dist * 100.0 / _knownErrors;
                lblSumDist.Text = dist.ToString();
                lblSumRelErr.Text = $"{relErr:F1}%";
                lblSumDist.ForeColor = dist <= 0 ? Color.DarkGreen : Color.DarkBlue;
                lblSumRelErr.ForeColor = dist <= 0 ? Color.DarkGreen : Color.DarkBlue;
            }
            else
            {
                lblSumDist.Text = "N/D";
                lblSumRelErr.Text = "N/D";
            }

            lblSumIter.Text = result.TotalIterations.ToString();
            lblSumTime.Text = $"{result.ElapsedMs / 1000.0:F2} s";
        }

        private void ResetSummaryPanel()
        {
            lblSumSize.Text = lblSumKE.Text = lblSumScore.Text =
            lblSumRelErr.Text = lblSumDist.Text = lblSumIter.Text = lblSumTime.Text = "—";
        }

        // kolorowanie macierzy 1 na zielono, 0 na rozowo
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
    }
}
