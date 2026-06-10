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
        // ── Stan aplikacji ─────────────────────────────────────────────────
        private MatrixGenerator _generator = new MatrixGenerator();
        private int[,] _currentMatrix;
        private int _knownErrors = 0;
        private bool _isUpdatingGrid = false;

        // ── Kontrola algorytmu (Tab 2) ─────────────────────────────────────
        private CancellationTokenSource _cts;
        private ManualResetEventSlim _pauseEvent;
        private bool _isPaused = false;

        // ── Kontrola testów automatycznych (Tab 4) ─────────────────────────
        private CancellationTokenSource _testCts;

        // ── Kontrolki Tab 4 (budowane programowo) ──────────────────────────
        private ComboBox cbTestParameter;
        private TextBox txtTestValues;
        private TextBox txtMatrixSizes;
        private NumericUpDown numBaseMaxIter;
        private NumericUpDown numBaseTenure;
        private NumericUpDown numBaseRestarts;
        private NumericUpDown numBasePerturbation;
        private NumericUpDown numBaseNeighborhood;
        private NumericUpDown numTestErrorPct;
        private NumericUpDown numRepetitions;
        private Button btnRunTests;
        private Button btnStopTests;
        private Button btnExportCsv;
        private ProgressBar progressBarTests;
        private Label lblTestStatus;
        private DataGridView dgvTestResults;
        private List<AggregatedResult> _lastTestResults;

        // ══════════════════════════════════════════════════════════════════
        public Form1()
        {
            InitializeComponent();
            dgvMatrix.CellFormatting += DgvMatrix_CellFormatting;
            dgvResults.CellFormatting += DgvMatrix_CellFormatting;
            InitializeTestingTab();
        }

        // ══════════════════════════════════════════════════════════════════
        //  TAB 1 – Generator instancji
        // ══════════════════════════════════════════════════════════════════

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
            UpdateKnownErrorsLabel("Ręczna macierz – kliknij komórki aby wpisać 1");
            DisplayMatrixInGrid(dgvMatrix, _currentMatrix, rows, cols);
        }

        private void btnErrors_Click(object sender, EventArgs e)
        {
            if (_currentMatrix == null) { MessageBox.Show("Najpierw wygeneruj lub wczytaj macierz."); return; }

            int errors = (int)numErrors.Value;
            try
            {
                _generator.ApplyErrors(_currentMatrix, errors);
                _knownErrors = _generator.KnownErrors;
                UpdateKnownErrorsLabel($"Znane błędy (introduced): {_knownErrors}  " +
                    $"({100.0 * _knownErrors / (_currentMatrix.GetLength(0) * _currentMatrix.GetLength(1)):F1}% komórek)");
                int rows = _currentMatrix.GetLength(0);
                int cols = _currentMatrix.GetLength(1);
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
            DisplayMatrixInGrid(dgvMatrix, _currentMatrix,
                _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));
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
            // Pierwsza linia: metadata
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

            // Parsuj opcjonalną linię metadanych
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

            // Oblicz błędy bieżącej macierzy funkcją celu (permutacja tożsamościowa)
            var ts = new TabuSearchAlgorithm();
            int[] id = new int[cols];
            for (int j = 0; j < cols; j++) id[j] = j;
            int calcErrors = ts.CalculateObjectiveFunction(_currentMatrix, id);

            if (_knownErrors > 0)
                UpdateKnownErrorsLabel($"Znane błędy (z pliku): {_knownErrors}  |  f.celu (tożs.): {calcErrors}");
            else
                UpdateKnownErrorsLabel($"Błędy f.celu (permutacja tożsamościowa): {calcErrors}");
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

        // ══════════════════════════════════════════════════════════════════
        //  TAB 2 – Tabu Search
        // ══════════════════════════════════════════════════════════════════

        private void UpdateProgressUI(ProgressInfo info)
        {
            if (_cts != null && _cts.IsCancellationRequested) return;
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<ProgressInfo>(UpdateProgressUI), info);
                return;
            }

            // Wykres
            chartProgress.Series[0].Points.AddXY(info.TotalIterations, info.BestScore);

            // Status
            double impPct = info.InitialScore > 0
                ? (info.InitialScore - info.BestScore) * 100.0 / info.InitialScore
                : 0;
            lblStatus.Text = $"Iter: {info.TotalIterations}  |  Najlepszy wynik: {info.BestScore}";

            // Pasek postępu fazy (jak blisko stopu)
            progressBarAlgorithm.Maximum = info.MaxIterationsWithoutImprovement;
            progressBarAlgorithm.Value = Math.Min(info.IterationsWithoutImprovement,
                                                     info.MaxIterationsWithoutImprovement);

            // Etykiety szczegółowe
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

            // Wykres: reset
            chartProgress.Series[0].Points.Clear();
            chartProgress.Series[0].Name = "Błędy";
            chartProgress.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            chartProgress.Series[0].BorderWidth = 2;
            chartProgress.Series[0].Color = System.Drawing.Color.DodgerBlue;
            chartProgress.ChartAreas[0].AxisX.Title = "Numer iteracji";
            chartProgress.ChartAreas[0].AxisY.Title = "Wartość funkcji celu";

            progressBarAlgorithm.Value = 0;
            lblStatus.Text = "Uruchamianie...";

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
                DisplayMatrixInGrid(dgvResults, _currentMatrix,
                    _currentMatrix.GetLength(0), _currentMatrix.GetLength(1));
                tabControl1.SelectedTab = tabPage3;
                dgvResults.Refresh();

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
            if (_isPaused) { _pauseEvent.Set(); btnPause.Text = "PAUZA"; }
            else { _pauseEvent.Reset(); btnPause.Text = "WZNÓW"; }
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

        // ══════════════════════════════════════════════════════════════════
        //  TAB 4 – Testy automatyczne
        // ══════════════════════════════════════════════════════════════════

        private void InitializeTestingTab()
        {
            int lx = 10;  // left column X
            int rx = 175; // right column X (controls)
            int y = 10;

            // ── Parametr do testowania ──
            var lblParam = new Label { AutoSize = true, Location = new Point(lx, y), Text = "Parametr testowany:" };
            cbTestParameter = new ComboBox
            {
                Location = new Point(lx, y + 20),
                Size = new Size(280, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbTestParameter.Items.AddRange(new string[]
            {
                "Kadencja Tabu (tabuTenure)",
                "Maks. iteracji bez poprawy",
                "Liczba restartów",
                "Rozmiar perturbacji",
                "Sąsiedztwo (%)",
                "Rozmiar macierzy m×n",
                "Procent błędów"
            });
            cbTestParameter.SelectedIndex = 0;
            y += 52;

            var lblVals = new Label { AutoSize = true, Location = new Point(lx, y), Text = "Wartości (np. 3,5,10,20):" };
            txtTestValues = new TextBox { Location = new Point(lx, y + 20), Size = new Size(280, 23), Text = "3,5,10,20,30" };
            y += 52;

            var lblSizes = new Label { AutoSize = true, Location = new Point(lx, y), Text = "Rozmiary macierzy (np. 10x10,20x20):" };
            txtMatrixSizes = new TextBox { Location = new Point(lx, y + 20), Size = new Size(280, 23), Text = "10x10,20x20,30x30,50x30,50x50" };
            y += 52;

            // ── Bazowe parametry ──
            var lblBase = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Location = new Point(lx, y),
                Text = "── Parametry bazowe ──"
            };
            y += 22;

            Func<string, int, int, int, (Label lbl, NumericUpDown num)> addParam = (txt, val, min, max) =>
            {
                var lbl = new Label { AutoSize = true, Location = new Point(lx, y + 3), Text = txt };
                var num = new NumericUpDown
                {
                    Location = new Point(rx, y),
                    Size = new Size(80, 23),
                    Minimum = min,
                    Maximum = max,
                    Value = val
                };
                y += 28;
                return (lbl, num);
            };

            (var lblBMaxIter, numBaseMaxIter) = addParam("Max iter. bez poprawy:", 100, 1, 5000);
            (var lblBTenure, numBaseTenure) = addParam("Kadencja Tabu:", 5, 1, 100);
            (var lblBRestarts, numBaseRestarts) = addParam("Restartów:", 3, 0, 50);
            (var lblBPert, numBasePerturbation) = addParam("Perturbacja:", 3, 1, 100);
            (var lblBNeigh, numBaseNeighborhood) = addParam("Sąsiedztwo (%):", 100, 1, 100);
            (var lblBErr, numTestErrorPct) = addParam("Błędów (%):", 3, 1, 50);
            (var lblBRep, numRepetitions) = addParam("Powtórzeń:", 10, 1, 50);
            y += 6;

            // ── Przyciski ──
            btnRunTests = new Button { Location = new Point(lx, y), Size = new Size(130, 30), Text = "▶ Uruchom testy" };
            btnStopTests = new Button { Location = new Point(lx + 140, y), Size = new Size(90, 30), Text = "■ Stop", Enabled = false };
            y += 38;

            progressBarTests = new ProgressBar { Location = new Point(lx, y), Size = new Size(280, 16) };
            y += 22;

            lblTestStatus = new Label
            {
                AutoSize = false,
                Location = new Point(lx, y),
                Size = new Size(280, 32),
                Text = "Gotowy."
            };
            y += 38;

            btnExportCsv = new Button
            {
                Location = new Point(lx, y),
                Size = new Size(130, 28),
                Text = "Eksportuj CSV",
                Enabled = false
            };

            // ── DataGridView wyników ──
            dgvTestResults = new DataGridView
            {
                Location = new Point(310, 10),
                Size = new Size(650, 520),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
            };

            // Dodaj kontrolki do Tab 4
            tabPage4.Controls.AddRange(new Control[]
            {
                lblParam, cbTestParameter,
                lblVals, txtTestValues,
                lblSizes, txtMatrixSizes,
                lblBase,
                lblBMaxIter,  numBaseMaxIter,
                lblBTenure,   numBaseTenure,
                lblBRestarts, numBaseRestarts,
                lblBPert,     numBasePerturbation,
                lblBNeigh,    numBaseNeighborhood,
                lblBErr,      numTestErrorPct,
                lblBRep,      numRepetitions,
                btnRunTests, btnStopTests,
                progressBarTests, lblTestStatus,
                btnExportCsv,
                dgvTestResults
            });

            // Eventy
            btnRunTests.Click += btnRunTests_Click;
            btnStopTests.Click += (s, e) => _testCts?.Cancel();
            btnExportCsv.Click += btnExportCsv_Click;
        }

        private async void btnRunTests_Click(object sender, EventArgs e)
        {
            _testCts = new CancellationTokenSource();
            btnRunTests.Enabled = false;
            btnStopTests.Enabled = true;
            btnExportCsv.Enabled = false;
            dgvTestResults.Rows.Clear();
            dgvTestResults.Columns.Clear();
            progressBarTests.Value = 0;

            var configs = BuildTestConfigs();
            if (configs == null || configs.Count == 0)
            {
                MessageBox.Show("Nie udało się zbudować konfiguracji testów. Sprawdź wartości.");
                btnRunTests.Enabled = true;
                btnStopTests.Enabled = false;
                return;
            }

            int reps = (int)numRepetitions.Value;
            progressBarTests.Maximum = configs.Count * reps;
            lblTestStatus.Text = $"Uruchamiam {configs.Count} konfiguracji × {reps} powtórzeń…";

            List<AggregatedResult> results = null;
            var tester = new AutomatedTester();

            await Task.Run(() =>
            {
                results = tester.RunTestSeries(configs, reps,
                    (done, total, msg) =>
                    {
                        this.Invoke((Action)(() =>
                        {
                            progressBarTests.Value = Math.Min(done, progressBarTests.Maximum);
                            lblTestStatus.Text = msg;
                        }));
                    },
                    _testCts.Token);
            });

            _lastTestResults = results;
            if (results != null && results.Count > 0)
                DisplayTestResults(results);

            lblTestStatus.Text = _testCts.IsCancellationRequested
                ? $"Przerwano. Zebrano {results?.Count ?? 0} wyników."
                : $"Zakończono. {results?.Count ?? 0} konfiguracji.";

            btnRunTests.Enabled = true;
            btnStopTests.Enabled = false;
            btnExportCsv.Enabled = results != null && results.Count > 0;
        }

        /// <summary>
        /// Buduje listę TestConfig na podstawie wybranego parametru i podanych wartości.
        /// </summary>
        private List<TestConfig> BuildTestConfigs()
        {
            var configs = new List<TestConfig>();
            string param = cbTestParameter.SelectedItem?.ToString() ?? "";

            int baseMaxIter = (int)numBaseMaxIter.Value;
            int baseTenure = (int)numBaseTenure.Value;
            int baseRest = (int)numBaseRestarts.Value;
            int basePert = (int)numBasePerturbation.Value;
            double baseNeigh = (double)numBaseNeighborhood.Value / 100.0;
            double baseErrPct = (double)numTestErrorPct.Value / 100.0;

            // Parsuj rozmiary macierzy (zawsze)
            var sizes = new List<(int r, int c)>();
            foreach (var s in txtMatrixSizes.Text.Split(','))
            {
                var parts = s.Trim().ToLower().Split('x');
                if (parts.Length == 2 && int.TryParse(parts[0], out int r) && int.TryParse(parts[1], out int c))
                    sizes.Add((r, c));
            }
            if (sizes.Count == 0) sizes.Add((20, 20));

            // Parsuj testowane wartości
            var vals = new List<double>();
            foreach (var v in txtTestValues.Text.Split(','))
                if (double.TryParse(v.Trim(), System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out double d))
                    vals.Add(d);
            if (vals.Count == 0) return configs;

            var (defR, defC) = sizes[0]; // domyślny rozmiar dla testów parametrycznych

            foreach (var val in vals)
            {
                int tenure = baseTenure;
                int maxIter = baseMaxIter;
                int restarts = baseRest;
                int pert = basePert;
                double neigh = baseNeigh;
                double errPct = baseErrPct;
                int rows = defR, cols = defC;
                string label;

                switch (param)
                {
                    case var p when p.StartsWith("Kadencja"):
                        tenure = (int)val;
                        label = $"tenure={tenure}";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Maks."):
                        maxIter = (int)val;
                        label = $"maxIter={maxIter}";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Liczba rest"):
                        restarts = (int)val;
                        label = $"restarts={restarts}";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Rozmiar pert"):
                        pert = (int)val;
                        label = $"pert={pert}";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Sąsiedztwo"):
                        neigh = val / 100.0;
                        label = $"neigh={val}%";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Procent"):
                        errPct = val / 100.0;
                        label = $"err={val}%";
                        configs.Add(MakeConfig(label, rows, cols, errPct, tenure, maxIter, restarts, pert, neigh));
                        break;
                    case var p when p.StartsWith("Rozmiar mac"):
                        // W trybie "Rozmiar macierzy" iterujemy po sizes, wartości w txtTestValues ignorujemy
                        // (zamiast tego używamy txtMatrixSizes)
                        break;
                }
            }

            // Specjalny tryb: testowanie rozmiarów macierzy
            if (param.StartsWith("Rozmiar mac"))
            {
                foreach (var (r, c) in sizes)
                {
                    string label = $"{r}x{c}";
                    configs.Add(MakeConfig(label, r, c, baseErrPct, baseTenure, baseMaxIter,
                        baseRest, basePert, baseNeigh));
                }
            }

            return configs;
        }

        private static TestConfig MakeConfig(string label, int r, int c, double errPct,
            int tenure, int maxIter, int restarts, int pert, double neigh) =>
            new TestConfig
            {
                Label = label,
                Rows = r,
                Cols = c,
                ErrorPercent = errPct,
                TabuTenure = tenure,
                MaxIterations = maxIter,
                Restarts = restarts,
                PerturbationSize = pert,
                NeighborhoodPct = neigh
            };

        private void DisplayTestResults(List<AggregatedResult> results)
        {
            dgvTestResults.Columns.Clear();
            dgvTestResults.Rows.Clear();

            string[] cols = {
                "Opis", "m", "n", "Zn.błędy", "Błędy%",
                "Tenure", "MaxIter", "Restarts", "Pert.", "Sąs.%",
                "Powt.", "Śr.Wynik", "Std.Dev", "Min", "Max",
                "Śr.Odl.Opt", "Śr.Błąd%", "Śr.Iter", "Śr.Czas[ms]"
            };
            foreach (var col in cols)
                dgvTestResults.Columns.Add(col, col);

            foreach (var r in results)
            {
                dgvTestResults.Rows.Add(
                    r.Label, r.Rows, r.Cols, r.KnownErrors, $"{r.ErrorPct:F1}%",
                    r.TabuTenure, r.MaxIterations, r.Restarts, r.PerturbationSize, r.NeighborhoodPct,
                    r.Repetitions, r.AvgScore, r.StdDev, r.BestScore, r.WorstScore,
                    r.AvgDistFromOpt, $"{r.AvgRelErrPct:F1}%", r.AvgIterations, r.AvgTimeMs
                );
            }
        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (_lastTestResults == null || _lastTestResults.Count == 0)
            {
                MessageBox.Show("Brak wyników do eksportu.");
                return;
            }

            using var sfd = new SaveFileDialog
            {
                Filter = "CSV (*.csv)|*.csv",
                FileName = "wyniki_testow.csv",
                Title = "Eksportuj wyniki do CSV"
            };
            if (sfd.ShowDialog() != DialogResult.OK) return;

            var sb = new StringBuilder();
            sb.AppendLine("Opis,m,n,ZnaneBłędy,Błędy%,Tenure,MaxIter,Restarts,Pert,Sąs%," +
                          "Powt,ŚrWynik,StdDev,Min,Max,ŚrOdlOpt,ŚrBłąd%,ŚrIter,ŚrCzasMs");
            foreach (var r in _lastTestResults)
            {
                sb.AppendLine(
                    $"{r.Label},{r.Rows},{r.Cols},{r.KnownErrors},{r.ErrorPct:F1}," +
                    $"{r.TabuTenure},{r.MaxIterations},{r.Restarts},{r.PerturbationSize},{r.NeighborhoodPct}," +
                    $"{r.Repetitions},{r.AvgScore},{r.StdDev},{r.BestScore},{r.WorstScore}," +
                    $"{r.AvgDistFromOpt},{r.AvgRelErrPct:F1},{r.AvgIterations},{r.AvgTimeMs}");
            }

            File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
            MessageBox.Show($"Eksportowano {_lastTestResults.Count} wierszy.", "CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ══════════════════════════════════════════════════════════════════
        //  Pomocnicze
        // ══════════════════════════════════════════════════════════════════

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