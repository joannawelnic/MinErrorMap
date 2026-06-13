namespace MinErrorMap
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 =
                new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 =
                new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 =
                new System.Windows.Forms.DataVisualization.Charting.Series();

            // ── Deklaracje ────────────────────────────────────────────────
            tabControl1       = new TabControl();
            // Tab 1
            tabPage1          = new TabPage();
            btnLoad           = new Button();
            btnSave           = new Button();
            btnShuffle        = new Button();
            btnErrors         = new Button();
            dgvMatrix         = new DataGridView();
            btnGenerate       = new Button();
            btnCreateManual   = new Button();
            label1            = new Label();
            label2            = new Label();
            label3            = new Label();
            lblKnownErrors    = new Label();
            numErrors         = new NumericUpDown();
            numCols           = new NumericUpDown();
            numRows           = new NumericUpDown();
            // Tab 2
            tabPage2          = new TabPage();
            chartProgress     = new System.Windows.Forms.DataVisualization.Charting.Chart();
            lblStatus         = new Label();
            btnStop           = new Button();
            btnPause          = new Button();
            btnStartSearch    = new Button();
            label4            = new Label();
            label5            = new Label();
            label6            = new Label();
            label7            = new Label();
            label8            = new Label();
            txtTabuTenure     = new TextBox();
            txtMaxIter        = new TextBox();
            numRestarts       = new NumericUpDown();
            numPerturbation   = new NumericUpDown();
            numNeighborhoodPct= new NumericUpDown();
            progressBarAlgorithm = new ProgressBar();
            lblRestartInfo    = new Label();
            lblTimeElapsed    = new Label();
            lblImprovementPct = new Label();
            // Tab 3
            tabPage3          = new TabPage();
            dgvResults        = new DataGridView();
            gbSummary         = new GroupBox();
            lblSumSize        = new Label();
            lblSumKE          = new Label();
            lblSumScore       = new Label();
            lblSumRelErr      = new Label();
            lblSumDist        = new Label();
            lblSumIter        = new Label();
            lblSumTime        = new Label();
            // Tab 4
            tabPage4          = new TabPage();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();

            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numErrors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCols).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRows).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartProgress).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRestarts).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numPerturbation).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numNeighborhoodPct).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
            SuspendLayout();

            // ── tabControl1 ───────────────────────────────────────────────
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Location     = new Point(12, 12);
            tabControl1.Name         = "tabControl1";
            tabControl1.SelectedIndex= 0;
            tabControl1.Size         = new Size(984, 581);
            tabControl1.TabIndex     = 0;

            // ── tabPage1 – Generator instancji ────────────────────────────
            tabPage1.Controls.Add(btnLoad);
            tabPage1.Controls.Add(btnSave);
            tabPage1.Controls.Add(btnShuffle);
            tabPage1.Controls.Add(btnErrors);
            tabPage1.Controls.Add(dgvMatrix);
            tabPage1.Controls.Add(btnGenerate);
            tabPage1.Controls.Add(btnCreateManual);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(lblKnownErrors);
            tabPage1.Controls.Add(numErrors);
            tabPage1.Controls.Add(numCols);
            tabPage1.Controls.Add(numRows);
            tabPage1.Location        = new Point(4, 24);
            tabPage1.Name            = "tabPage1";
            tabPage1.Padding         = new Padding(3);
            tabPage1.Size            = new Size(976, 553);
            tabPage1.TabIndex        = 0;
            tabPage1.Text            = "Generator instancji";
            tabPage1.UseVisualStyleBackColor = true;

            // label1 – wiersze
            label1.AutoSize  = true;
            label1.Location  = new Point(30, 64);
            label1.Name      = "label1";
            label1.Text      = "Liczba wierszy (m):";

            // numRows
            numRows.Location = new Point(176, 62);
            numRows.Maximum  = new decimal(new int[] { 500, 0, 0, 0 });
            numRows.Minimum  = new decimal(new int[] { 2, 0, 0, 0 });
            numRows.Name     = "numRows";
            numRows.Size     = new Size(120, 23);
            numRows.TabIndex = 1;
            numRows.Value    = new decimal(new int[] { 20, 0, 0, 0 });

            // label2 – kolumny
            label2.AutoSize  = true;
            label2.Location  = new Point(30, 107);
            label2.Name      = "label2";
            label2.Text      = "Liczba kolumn (n):";

            // numCols
            numCols.Location = new Point(176, 105);
            numCols.Maximum  = new decimal(new int[] { 500, 0, 0, 0 });
            numCols.Minimum  = new decimal(new int[] { 2, 0, 0, 0 });
            numCols.Name     = "numCols";
            numCols.Size     = new Size(120, 23);
            numCols.TabIndex = 2;
            numCols.Value    = new decimal(new int[] { 20, 0, 0, 0 });

            // label3 – błędy
            label3.AutoSize  = true;
            label3.Location  = new Point(30, 150);
            label3.Name      = "label3";
            label3.Text      = "Liczba błędów:";

            // numErrors
            numErrors.Location = new Point(176, 148);
            numErrors.Maximum  = new decimal(new int[] { 10000, 0, 0, 0 });
            numErrors.Name     = "numErrors";
            numErrors.Size     = new Size(120, 23);
            numErrors.TabIndex = 3;
            numErrors.Value    = new decimal(new int[] { 3, 0, 0, 0 });

            // btnGenerate
            btnGenerate.Location = new Point(33, 188);
            btnGenerate.Name     = "btnGenerate";
            btnGenerate.Size     = new Size(144, 36);
            btnGenerate.TabIndex = 7;
            btnGenerate.Text     = "Generuj macierz";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click   += btnGenerate_Click;

            // btnCreateManual – nowy przycisk
            btnCreateManual.Location = new Point(33, 234);
            btnCreateManual.Name     = "btnCreateManual";
            btnCreateManual.Size     = new Size(144, 36);
            btnCreateManual.TabIndex = 8;
            btnCreateManual.Text     = "Utwórz ręcznie";
            btnCreateManual.UseVisualStyleBackColor = true;
            btnCreateManual.Click   += btnCreateManual_Click;

            // btnErrors
            btnErrors.Location = new Point(33, 284);
            btnErrors.Name     = "btnErrors";
            btnErrors.Size     = new Size(144, 36);
            btnErrors.TabIndex = 9;
            btnErrors.Text     = "Wprowadź błędy";
            btnErrors.UseVisualStyleBackColor = true;
            btnErrors.Click   += btnErrors_Click;

            // lblKnownErrors – informacja o błędach bieżącej instancji
            lblKnownErrors.AutoSize   = true;
            lblKnownErrors.Font       = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblKnownErrors.ForeColor  = Color.DarkBlue;
            lblKnownErrors.Location   = new Point(30, 330);
            lblKnownErrors.Name       = "lblKnownErrors";
            lblKnownErrors.Size       = new Size(260, 15);
            lblKnownErrors.Text       = "Znane błędy instancji: —";

            // btnShuffle
            btnShuffle.Location = new Point(33, 354);
            btnShuffle.Name     = "btnShuffle";
            btnShuffle.Size     = new Size(144, 36);
            btnShuffle.TabIndex = 11;
            btnShuffle.Text     = "Przetasuj kolumny";
            btnShuffle.UseVisualStyleBackColor = true;
            btnShuffle.Click   += btnShuffle_Click;

            // btnSave
            btnSave.Location = new Point(33, 406);
            btnSave.Name     = "btnSave";
            btnSave.Size     = new Size(144, 36);
            btnSave.TabIndex = 13;
            btnSave.Text     = "Zapisz do pliku";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click   += btnSave_Click;

            // btnLoad
            btnLoad.Location = new Point(33, 448);
            btnLoad.Name     = "btnLoad";
            btnLoad.Size     = new Size(144, 36);
            btnLoad.TabIndex = 14;
            btnLoad.Text     = "Wczytaj z pliku";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click   += btnLoad_Click;

            // dgvMatrix
            dgvMatrix.AllowUserToAddRows         = false;
            dgvMatrix.AllowUserToOrderColumns    = true;
            dgvMatrix.ColumnHeadersHeightSizeMode= DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatrix.Location   = new Point(331, 64);
            dgvMatrix.Name       = "dgvMatrix";
            dgvMatrix.RowHeadersWidth = 62;
            dgvMatrix.Size       = new Size(619, 465);
            dgvMatrix.TabIndex   = 8;
            dgvMatrix.CellValueChanged += dgvMatrix_CellValueChanged;

            // ── tabPage2 – Tabu Search ────────────────────────────────────
            tabPage2.Controls.Add(chartProgress);
            tabPage2.Controls.Add(lblStatus);
            tabPage2.Controls.Add(btnStop);
            tabPage2.Controls.Add(btnPause);
            tabPage2.Controls.Add(btnStartSearch);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(label6);
            tabPage2.Controls.Add(label7);
            tabPage2.Controls.Add(label8);
            tabPage2.Controls.Add(txtMaxIter);
            tabPage2.Controls.Add(txtTabuTenure);
            tabPage2.Controls.Add(numRestarts);
            tabPage2.Controls.Add(numPerturbation);
            tabPage2.Controls.Add(numNeighborhoodPct);
            tabPage2.Controls.Add(progressBarAlgorithm);
            tabPage2.Controls.Add(lblRestartInfo);
            tabPage2.Controls.Add(lblTimeElapsed);
            tabPage2.Controls.Add(lblImprovementPct);
            tabPage2.Location        = new Point(4, 24);
            tabPage2.Name            = "tabPage2";
            tabPage2.Padding         = new Padding(3);
            tabPage2.Size            = new Size(976, 553);
            tabPage2.TabIndex        = 1;
            tabPage2.Text            = "Tabu Search";
            tabPage2.UseVisualStyleBackColor = true;
            tabPage2.Click          += tabPage2_Click;

            // label5 – max iteracji
            label5.AutoSize  = true;
            label5.Location  = new Point(10, 27);
            label5.Name      = "label5";
            label5.Text      = "Iteracji bez poprawy:";

            // txtMaxIter
            txtMaxIter.Location = new Point(175, 24);
            txtMaxIter.Name     = "txtMaxIter";
            txtMaxIter.Size     = new Size(80, 23);
            txtMaxIter.TabIndex = 0;
            txtMaxIter.Text     = "100";

            // label4 – kadencja Tabu
            label4.AutoSize  = true;
            label4.Location  = new Point(10, 57);
            label4.Name      = "label4";
            label4.Text      = "Kadencja Tabu:";

            // txtTabuTenure
            txtTabuTenure.Location = new Point(175, 54);
            txtTabuTenure.Name     = "txtTabuTenure";
            txtTabuTenure.Size     = new Size(80, 23);
            txtTabuTenure.TabIndex = 1;
            txtTabuTenure.Text     = "5";

            // label6 – restartów
            label6.AutoSize  = true;
            label6.Location  = new Point(10, 87);
            label6.Name      = "label6";
            label6.Text      = "Liczba restartów:";

            // numRestarts
            numRestarts.Location = new Point(175, 84);
            numRestarts.Maximum  = new decimal(new int[] { 50, 0, 0, 0 });
            numRestarts.Name     = "numRestarts";
            numRestarts.Size     = new Size(80, 23);
            numRestarts.TabIndex = 2;
            numRestarts.Value    = new decimal(new int[] { 3, 0, 0, 0 });

            // label7 – perturbacja
            label7.AutoSize  = true;
            label7.Location  = new Point(10, 117);
            label7.Name      = "label7";
            label7.Text      = "Perturbacja (swapów):";

            // numPerturbation
            numPerturbation.Location = new Point(175, 114);
            numPerturbation.Maximum  = new decimal(new int[] { 200, 0, 0, 0 });
            numPerturbation.Minimum  = new decimal(new int[] { 1, 0, 0, 0 });
            numPerturbation.Name     = "numPerturbation";
            numPerturbation.Size     = new Size(80, 23);
            numPerturbation.TabIndex = 3;
            numPerturbation.Value    = new decimal(new int[] { 3, 0, 0, 0 });

            // label8 – sąsiedztwo
            label8.AutoSize  = true;
            label8.Location  = new Point(10, 147);
            label8.Name      = "label8";
            label8.Text      = "Sąsiedztwo (%):";

            // numNeighborhoodPct
            numNeighborhoodPct.Location = new Point(175, 144);
            numNeighborhoodPct.Maximum  = new decimal(new int[] { 100, 0, 0, 0 });
            numNeighborhoodPct.Minimum  = new decimal(new int[] { 1, 0, 0, 0 });
            numNeighborhoodPct.Name     = "numNeighborhoodPct";
            numNeighborhoodPct.Size     = new Size(80, 23);
            numNeighborhoodPct.TabIndex = 4;
            numNeighborhoodPct.Value    = new decimal(new int[] { 100, 0, 0, 0 });

            // progressBarAlgorithm
            progressBarAlgorithm.Location  = new Point(10, 178);
            progressBarAlgorithm.Name      = "progressBarAlgorithm";
            progressBarAlgorithm.Size      = new Size(295, 18);
            progressBarAlgorithm.TabIndex  = 10;

            // lblRestartInfo
            lblRestartInfo.AutoSize  = true;
            lblRestartInfo.Font      = new Font("Segoe UI", 9F);
            lblRestartInfo.Location  = new Point(10, 202);
            lblRestartInfo.Name      = "lblRestartInfo";
            lblRestartInfo.Text      = "Restart: —";

            // lblTimeElapsed
            lblTimeElapsed.AutoSize  = true;
            lblTimeElapsed.Font      = new Font("Segoe UI", 9F);
            lblTimeElapsed.Location  = new Point(120, 202);
            lblTimeElapsed.Name      = "lblTimeElapsed";
            lblTimeElapsed.Text      = "Czas: — ms";

            // lblImprovementPct
            lblImprovementPct.AutoSize  = true;
            lblImprovementPct.Font      = new Font("Segoe UI", 9F);
            lblImprovementPct.ForeColor = Color.DarkGreen;
            lblImprovementPct.Location  = new Point(10, 222);
            lblImprovementPct.Name      = "lblImprovementPct";
            lblImprovementPct.Text      = "Poprawa: —";

            // lblStatus
            lblStatus.AutoSize    = true;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.Font        = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblStatus.Location    = new Point(324, 15);
            lblStatus.Name        = "lblStatus";
            lblStatus.Text        = "Czekam na uruchomienie...";

            // chartProgress
            chartArea1.Name = "ChartArea1";
            chartProgress.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chartProgress.Legends.Add(legend1);
            chartProgress.Location = new Point(324, 50);
            chartProgress.Name     = "chartProgress";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
            series1.Legend    = "Legend1";
            series1.Name      = "Series1";
            chartProgress.Series.Add(series1);
            chartProgress.Size     = new Size(611, 400);
            chartProgress.TabIndex = 9;
            chartProgress.Text     = "chart1";

            // btnStartSearch
            btnStartSearch.Font     = new Font("Segoe UI", 18F);
            btnStartSearch.Location = new Point(10, 470);
            btnStartSearch.Name     = "btnStartSearch";
            btnStartSearch.Size     = new Size(130, 50);
            btnStartSearch.TabIndex = 4;
            btnStartSearch.Text     = "START";
            btnStartSearch.UseVisualStyleBackColor = true;
            btnStartSearch.Click   += btnStartSearch_Click;

            // btnPause
            btnPause.Font     = new Font("Segoe UI", 18F);
            btnPause.Location = new Point(430, 470);
            btnPause.Name     = "btnPause";
            btnPause.Size     = new Size(130, 50);
            btnPause.TabIndex = 5;
            btnPause.Text     = "PAUZA";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click   += btnPause_Click;

            // btnStop
            btnStop.Font     = new Font("Segoe UI", 18F);
            btnStop.Location = new Point(740, 470);
            btnStop.Name     = "btnStop";
            btnStop.Size     = new Size(130, 50);
            btnStop.TabIndex = 6;
            btnStop.Text     = "STOP";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click   += btnStop_Click;

            // ── tabPage3 – Wyniki ─────────────────────────────────────────
            tabPage3.Controls.Add(dgvResults);
            tabPage3.Controls.Add(gbSummary);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name     = "tabPage3";
            tabPage3.Padding  = new Padding(3);
            tabPage3.Size     = new Size(976, 553);
            tabPage3.TabIndex = 2;
            tabPage3.Text     = "Wyniki";
            tabPage3.UseVisualStyleBackColor = true;

            dgvResults.AllowUserToAddRows        = false;
            dgvResults.AllowUserToDeleteRows     = false;
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResults.Location    = new Point(302, 26);
            dgvResults.Name        = "dgvResults";
            dgvResults.ReadOnly    = true;
            dgvResults.Size        = new Size(645, 501);
            dgvResults.TabIndex    = 0;

            // ── gbSummary – panel podsumowania pojedynczego uruchomienia ──
            gbSummary.Location  = new Point(6, 6);
            gbSummary.Name      = "gbSummary";
            gbSummary.Size      = new Size(286, 268);
            gbSummary.TabStop   = false;
            gbSummary.Text      = "Podsumowanie wyników";
            gbSummary.Font      = new Font("Segoe UI", 9F, FontStyle.Bold);

            Action<Label, Label, string, int> addSumRow = (cap, val, text, y) =>
            {
                cap.AutoSize  = true;
                cap.Font      = new Font("Segoe UI", 9F);
                cap.Location  = new Point(8, y);
                cap.Text      = text;
                val.AutoSize  = false;
                val.Font      = new Font("Segoe UI", 10F, FontStyle.Bold);
                val.ForeColor = Color.DarkBlue;
                val.Location  = new Point(158, y - 2);
                val.Size      = new Size(120, 20);
                val.Text      = "—";
                gbSummary.Controls.Add(cap);
                gbSummary.Controls.Add(val);
            };

            addSumRow(new Label(), lblSumSize,   "Rozmiar macierzy:",    26);
            addSumRow(new Label(), lblSumKE,     "Znane błędy:",         58);
            addSumRow(new Label(), lblSumScore,  "Wynik końcowy:",        90);
            addSumRow(new Label(), lblSumRelErr, "Błąd względny:",       122);
            addSumRow(new Label(), lblSumDist,   "Odległość od opt.:",   154);
            addSumRow(new Label(), lblSumIter,   "Liczba iteracji:",     186);
            addSumRow(new Label(), lblSumTime,   "Czas:",                218);

            // ── tabPage4 – Testy automatyczne (kontrolki budowane w Form1.cs) ──
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name     = "tabPage4";
            tabPage4.Padding  = new Padding(3);
            tabPage4.Size     = new Size(976, 553);
            tabPage4.TabIndex = 3;
            tabPage4.Text     = "Testy automatyczne";
            tabPage4.UseVisualStyleBackColor = true;

            // ── Form ──────────────────────────────────────────────────────
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode       = AutoScaleMode.Font;
            ClientSize          = new Size(1017, 605);
            Controls.Add(tabControl1);
            Name = "MinErrorMap – Tabu Search";
            Text = "MinErrorMap – Tabu Search";

            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).EndInit();
            ((System.ComponentModel.ISupportInitialize)numErrors).EndInit();
            ((System.ComponentModel.ISupportInitialize)numCols).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRows).EndInit();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)chartProgress).EndInit();
            ((System.ComponentModel.ISupportInitialize)numRestarts).EndInit();
            ((System.ComponentModel.ISupportInitialize)numPerturbation).EndInit();
            ((System.ComponentModel.ISupportInitialize)numNeighborhoodPct).EndInit();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
            ResumeLayout(false);
        }

        #endregion

        // ── Tab 1 ────────────────────────────────────────────────────────
        private TabControl    tabControl1;
        private TabPage       tabPage1;
        private NumericUpDown numErrors;
        private NumericUpDown numCols;
        private NumericUpDown numRows;
        private DataGridView  dgvMatrix;
        private Button        btnGenerate;
        private Button        btnCreateManual;
        private Button        btnErrors;
        private Button        btnShuffle;
        private Button        btnSave;
        private Button        btnLoad;
        private Label         label1;
        private Label         label2;
        private Label         label3;
        private Label         lblKnownErrors;
        // ── Tab 2 ────────────────────────────────────────────────────────
        private TabPage       tabPage2;
        private TextBox       txtMaxIter;
        private TextBox       txtTabuTenure;
        private NumericUpDown numRestarts;
        private NumericUpDown numPerturbation;
        private NumericUpDown numNeighborhoodPct;
        private Label         label4;
        private Label         label5;
        private Label         label6;
        private Label         label7;
        private Label         label8;
        private Button        btnStartSearch;
        private Button        btnPause;
        private Button        btnStop;
        private Label         lblStatus;
        private ProgressBar   progressBarAlgorithm;
        private Label         lblRestartInfo;
        private Label         lblTimeElapsed;
        private Label         lblImprovementPct;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartProgress;
        // ── Tab 3 ────────────────────────────────────────────────────────
        private TabPage       tabPage3;
        private DataGridView  dgvResults;
        // ── Tab 4 ────────────────────────────────────────────────────────
        private TabPage       tabPage4;
        // Tab 3 – panel podsumowania
        private GroupBox      gbSummary;
        private Label         lblSumSize;
        private Label         lblSumKE;
        private Label         lblSumScore;
        private Label         lblSumRelErr;
        private Label         lblSumDist;
        private Label         lblSumIter;
        private Label         lblSumTime;
        // ── Misc ─────────────────────────────────────────────────────────
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}
