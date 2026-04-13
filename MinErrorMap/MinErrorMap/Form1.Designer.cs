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
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            dgvMatrix = new DataGridView();
            btnGenerate = new Button();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            numErrors = new NumericUpDown();
            numCols = new NumericUpDown();
            numRows = new NumericUpDown();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            btnErrors = new Button();
            btnShuffle = new Button();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvMatrix).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numErrors).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numCols).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numRows).BeginInit();
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
            tabControl1.Size = new Size(870, 524);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
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
            tabPage1.Size = new Size(862, 496);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Generator instancji";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // dgvMatrix
            // 
            dgvMatrix.AllowUserToAddRows = false;
            dgvMatrix.AllowUserToOrderColumns = true;
            dgvMatrix.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvMatrix.Location = new Point(331, 64);
            dgvMatrix.Name = "dgvMatrix";
            dgvMatrix.Size = new Size(488, 399);
            dgvMatrix.TabIndex = 8;
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
            // 
            // numCols
            // 
            numCols.Location = new Point(176, 107);
            numCols.Name = "numCols";
            numCols.Size = new Size(120, 23);
            numCols.TabIndex = 2;
            // 
            // numRows
            // 
            numRows.Location = new Point(176, 66);
            numRows.Name = "numRows";
            numRows.Size = new Size(120, 23);
            numRows.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(862, 496);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Tabu search";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(862, 496);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Wyniki";
            tabPage3.UseVisualStyleBackColor = true;
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
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(903, 548);
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
    }
}
