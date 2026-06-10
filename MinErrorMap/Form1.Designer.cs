namespace MinErrorMap
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            btnLoad = new Button();
            btnSave = new Button();
            btnShuffle = new Button();
            btnErrors = new Button();
            dgvMatrix = new DataGridView();
            btnGenerate = new Button();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            numErrors = new NumericUpDown();
            numCols = new NumericUpDown();
            numRows = new NumericUpDown();
            tabPage2 = new TabPage();
            chartProgress = new System.Windows.Forms.DataVisualization.Charting.Chart();
            lblStatus = new Label();
            btnStop = new Button();
            btnPause = new Button();
            btnStartSearch = new Button();
            label4 = new Label();
            label5 = new Label();
            txtTabuTenure = new TextBox();
            txtMaxIter = new TextBox();
            tabPage3 = new TabPage();
            dgvResults = new DataGridView();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numErrors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCols).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRows).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)chartProgress).BeginInit();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(984, 581);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(btnLoad);
            tabPage1.Controls.Add(btnSave);
            tabPage1.Controls.Add(btnShuffle);
            tabPage1.Controls.Add(btnErrors);
            tabPage1.Controls.Add(dgvMatrix);
            tabPage1.Controls.Add(btnGenerate);
            tabPage1.Controls.Add(label3);
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(numErrors);
            tabPage1.Controls.Add(numCols);
            tabPage1.Controls.Add(numRows);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(976, 553);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Generator instancji";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnLoad
            // 
            btnLoad.Location = new Point(33, 448);
            btnLoad.Name = "btnLoad";
            btnLoad.Size = new Size(144, 36);
            btnLoad.TabIndex = 14;
            btnLoad.Text = "Wczytaj z pliku";
            btnLoad.UseVisualStyleBackColor = true;
            btnLoad.Click += btnLoad_Click;
            // 
            // btnSave
            // 
            btnSave.Location = new Point(33, 406);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(144, 36);
            btnSave.TabIndex = 13;
            btnSave.Text = "Zapisz do pliku";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnShuffle
            // 
            btnShuffle.Location = new Point(33, 318);
            btnShuffle.Name = "btnShuffle";
            btnShuffle.Size = new Size(144, 36);
            btnShuffle.TabIndex = 11;
            btnShuffle.Text = "Przetasuj kolumny";
            btnShuffle.UseVisualStyleBackColor = true;
            btnShuffle.Click += btnShuffle_Click;
            // 
            // btnErrors
            // 
            btnErrors.Location = new Point(33, 262);
            btnErrors.Name = "btnErrors";
            btnErrors.Size = new Size(144, 36);
            btnErrors.TabIndex = 9;
            btnErrors.Text = "Wprowadź błędy";
            btnErrors.UseVisualStyleBackColor = true;
            btnErrors.Click += btnErrors_Click;
            // 
            // dgvMatrix
            // 
            dgvMatrix.AllowUserToAddRows = false;
            dgvMatrix.AllowUserToOrderColumns = true;
            dgvMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatrix.Location = new Point(331, 64);
            dgvMatrix.Name = "dgvMatrix";
            dgvMatrix.RowHeadersWidth = 62;
            dgvMatrix.Size = new Size(619, 465);
            dgvMatrix.TabIndex = 8;
            dgvMatrix.CellValueChanged += dgvMatrix_CellValueChanged;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(33, 203);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(144, 36);
            btnGenerate.TabIndex = 7;
            btnGenerate.Text = "Generuj Macierz";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(30, 150);
            label3.Name = "label3";
            label3.Size = new Size(85, 15);
            label3.TabIndex = 6;
            label3.Text = "Liczba błędów:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 107);
            label2.Name = "label2";
            label2.Size = new Size(105, 15);
            label2.TabIndex = 5;
            label2.Text = "Liczba kolumn (n):";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 64);
            label1.Name = "label1";
            label1.Size = new Size(106, 15);
            label1.TabIndex = 4;
            label1.Text = "Liczba wierszy (m):";
            // 
            // numErrors
            // 
            numErrors.Location = new Point(176, 150);
            numErrors.Name = "numErrors";
            numErrors.Size = new Size(120, 23);
            numErrors.TabIndex = 3;
            numErrors.Value = new decimal(new int[] { 3, 0, 0, 0 });
            // 
            // numCols
            // 
            numCols.Location = new Point(176, 107);
            numCols.Name = "numCols";
            numCols.Size = new Size(120, 23);
            numCols.TabIndex = 2;
            numCols.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // numRows
            // 
            numRows.Location = new Point(176, 66);
            numRows.Name = "numRows";
            numRows.Size = new Size(120, 23);
            numRows.TabIndex = 1;
            numRows.Value = new decimal(new int[] { 20, 0, 0, 0 });
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(chartProgress);
            tabPage2.Controls.Add(lblStatus);
            tabPage2.Controls.Add(btnStop);
            tabPage2.Controls.Add(btnPause);
            tabPage2.Controls.Add(btnStartSearch);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(txtTabuTenure);
            tabPage2.Controls.Add(txtMaxIter);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(976, 553);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tabu search";
            tabPage2.UseVisualStyleBackColor = true;
            tabPage2.Click += tabPage2_Click;
            // 
            // chartProgress
            // 
            chartArea1.Name = "ChartArea1";
            chartProgress.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            chartProgress.Legends.Add(legend1);
            chartProgress.Location = new Point(324, 65);
            chartProgress.Name = "chartProgress";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            chartProgress.Series.Add(series1);
            chartProgress.Size = new Size(611, 372);
            chartProgress.TabIndex = 9;
            chartProgress.Text = "chart1";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.FlatStyle = FlatStyle.Flat;
            lblStatus.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblStatus.Location = new Point(324, 15);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(243, 27);
            lblStatus.TabIndex = 7;
            lblStatus.Text = "Czekam na uruchomienie...";
            lblStatus.Click += lblStatus_Click;
            // 
            // btnStop
            // 
            btnStop.Font = new Font("Segoe UI", 20F);
            btnStop.Location = new Point(733, 481);
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(151, 53);
            btnStop.TabIndex = 6;
            btnStop.Text = "STOP";
            btnStop.UseVisualStyleBackColor = true;
            btnStop.Click += btnStop_Click;
            // 
            // btnPause
            // 
            btnPause.Font = new Font("Segoe UI", 20F);
            btnPause.Location = new Point(421, 481);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(151, 53);
            btnPause.TabIndex = 5;
            btnPause.Text = "PAUZA";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // btnStartSearch
            // 
            btnStartSearch.Font = new Font("Segoe UI", 20F);
            btnStartSearch.Location = new Point(113, 481);
            btnStartSearch.Name = "btnStartSearch";
            btnStartSearch.Size = new Size(151, 53);
            btnStartSearch.TabIndex = 4;
            btnStartSearch.Text = "START";
            btnStartSearch.UseVisualStyleBackColor = true;
            btnStartSearch.Click += btnStartSearch_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(29, 56);
            label4.Name = "label4";
            label4.Size = new Size(87, 15);
            label4.TabIndex = 3;
            label4.Text = "Kadencja Tabu:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(29, 27);
            label5.Name = "label5";
            label5.Size = new Size(142, 15);
            label5.TabIndex = 2;
            label5.Text = "Ilość iteracji bez poprawy:";
            // 
            // txtTabuTenure
            // 
            txtTabuTenure.Location = new Point(193, 53);
            txtTabuTenure.Name = "txtTabuTenure";
            txtTabuTenure.Size = new Size(100, 23);
            txtTabuTenure.TabIndex = 1;
            txtTabuTenure.Text = "5";
            // 
            // txtMaxIter
            // 
            txtMaxIter.Location = new Point(193, 24);
            txtMaxIter.Name = "txtMaxIter";
            txtMaxIter.Size = new Size(100, 23);
            txtMaxIter.TabIndex = 0;
            txtMaxIter.Text = "50";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(dgvResults);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(976, 553);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Wyniki";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // dgvResults
            // 
            dgvResults.AllowUserToAddRows = false;
            dgvResults.AllowUserToDeleteRows = false;
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResults.Location = new Point(302, 26);
            dgvResults.Name = "dgvResults";
            dgvResults.ReadOnly = true;
            dgvResults.Size = new Size(645, 501);
            dgvResults.TabIndex = 0;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1017, 605);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Form1";
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
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private NumericUpDown numErrors;
        private NumericUpDown numCols;
        private NumericUpDown numRows;
        private TabPage tabPage3;
        private Button btnGenerate;
        private Label label3;
        private Label label2;
        private Label label1;
        private DataGridView dgvMatrix;
        private Button btnShuffle;
        private Button btnErrors;
        private Button btnLoad;
        private Button btnSave;
        private TextBox txtMaxIter;
        private Label label4;
        private Label label5;
        private TextBox txtTabuTenure;
        private Button btnStartSearch;
        private Button btnStop;
        private Button btnPause;
        private Label lblStatus;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartProgress;
        private DataGridView dgvResults;
    }
}