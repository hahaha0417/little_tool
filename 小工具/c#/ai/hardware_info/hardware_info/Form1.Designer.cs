namespace hardware_info;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel tableLayoutPanel1;
    private Label lblCpuTotalTitle;
    private Label lblCpuTotalValue;
    private Label lblCpuCoresTitle;
    private TextBox txtCpuCores;
    private Label lblMemoryTitle;
    private Label lblMemoryValue;
    private Label lblDiskTitle;
    private Label lblDiskValue;
    private Label lblJsonPathTitle;
    private TextBox txtJsonPath;
    private Button btnBrowseJsonPath;
    private Label lblWriteIntervalTitle;
    private NumericUpDown numWriteIntervalMs;
    private Label lblUiUpdateIntervalTitle;
    private NumericUpDown numUiUpdateIntervalMs;
    private Label lblInternalSamplingIntervalTitle;
    private NumericUpDown numInternalSamplingIntervalMs;
    private Label lblCpuSmoothingWindowTitle;
    private NumericUpDown numCpuSmoothingWindowSize;

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
        tableLayoutPanel1 = new TableLayoutPanel();
        lblJsonPathTitle = new Label();
        txtJsonPath = new TextBox();
        btnBrowseJsonPath = new Button();
        lblWriteIntervalTitle = new Label();
        numWriteIntervalMs = new NumericUpDown();
        lblUiUpdateIntervalTitle = new Label();
        numUiUpdateIntervalMs = new NumericUpDown();
        lblInternalSamplingIntervalTitle = new Label();
        numInternalSamplingIntervalMs = new NumericUpDown();
        lblCpuSmoothingWindowTitle = new Label();
        numCpuSmoothingWindowSize = new NumericUpDown();
        lblCpuTotalTitle = new Label();
        lblCpuTotalValue = new Label();
        lblCpuCoresTitle = new Label();
        txtCpuCores = new TextBox();
        lblMemoryTitle = new Label();
        lblMemoryValue = new Label();
        lblDiskTitle = new Label();
        lblDiskValue = new Label();
        tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numWriteIntervalMs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numUiUpdateIntervalMs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numInternalSamplingIntervalMs).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numCpuSmoothingWindowSize).BeginInit();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 3;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
        tableLayoutPanel1.Controls.Add(lblJsonPathTitle, 0, 0);
        tableLayoutPanel1.Controls.Add(txtJsonPath, 1, 0);
        tableLayoutPanel1.Controls.Add(btnBrowseJsonPath, 2, 0);
        tableLayoutPanel1.Controls.Add(lblWriteIntervalTitle, 0, 1);
        tableLayoutPanel1.Controls.Add(numWriteIntervalMs, 1, 1);
        tableLayoutPanel1.Controls.Add(lblUiUpdateIntervalTitle, 0, 2);
        tableLayoutPanel1.Controls.Add(numUiUpdateIntervalMs, 1, 2);
        tableLayoutPanel1.Controls.Add(lblInternalSamplingIntervalTitle, 0, 3);
        tableLayoutPanel1.Controls.Add(numInternalSamplingIntervalMs, 1, 3);
        tableLayoutPanel1.Controls.Add(lblCpuSmoothingWindowTitle, 0, 4);
        tableLayoutPanel1.Controls.Add(numCpuSmoothingWindowSize, 1, 4);
        tableLayoutPanel1.Controls.Add(lblCpuTotalTitle, 0, 5);
        tableLayoutPanel1.Controls.Add(lblCpuTotalValue, 1, 5);
        tableLayoutPanel1.Controls.Add(lblCpuCoresTitle, 0, 6);
        tableLayoutPanel1.Controls.Add(txtCpuCores, 1, 6);
        tableLayoutPanel1.Controls.Add(lblMemoryTitle, 0, 7);
        tableLayoutPanel1.Controls.Add(lblMemoryValue, 1, 7);
        tableLayoutPanel1.Controls.Add(lblDiskTitle, 0, 8);
        tableLayoutPanel1.Controls.Add(lblDiskValue, 1, 8);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(12, 12);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 9;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
        tableLayoutPanel1.Size = new Size(776, 626);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // lblJsonPathTitle
        // 
        lblJsonPathTitle.AutoSize = true;
        lblJsonPathTitle.Dock = DockStyle.Fill;
        lblJsonPathTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblJsonPathTitle.Location = new Point(3, 0);
        lblJsonPathTitle.Name = "lblJsonPathTitle";
        lblJsonPathTitle.Size = new Size(174, 50);
        lblJsonPathTitle.TabIndex = 8;
        lblJsonPathTitle.Text = "JSON 路徑";
        lblJsonPathTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtJsonPath
        // 
        txtJsonPath.Dock = DockStyle.Fill;
        txtJsonPath.Font = new Font("Microsoft JhengHei UI", 10F);
        txtJsonPath.Location = new Point(183, 3);
        txtJsonPath.Name = "txtJsonPath";
        txtJsonPath.Size = new Size(530, 24);
        txtJsonPath.TabIndex = 9;
        txtJsonPath.TextChanged += txtJsonPath_TextChanged;
        // 
        // btnBrowseJsonPath
        // 
        btnBrowseJsonPath.Dock = DockStyle.Fill;
        btnBrowseJsonPath.Font = new Font("Microsoft JhengHei UI", 10F);
        btnBrowseJsonPath.Location = new Point(719, 3);
        btnBrowseJsonPath.Name = "btnBrowseJsonPath";
        btnBrowseJsonPath.Size = new Size(54, 44);
        btnBrowseJsonPath.TabIndex = 10;
        btnBrowseJsonPath.Text = "...";
        btnBrowseJsonPath.UseVisualStyleBackColor = true;
        btnBrowseJsonPath.Click += btnBrowseJsonPath_Click;
        // 
        // lblWriteIntervalTitle
        // 
        lblWriteIntervalTitle.AutoSize = true;
        lblWriteIntervalTitle.Dock = DockStyle.Fill;
        lblWriteIntervalTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblWriteIntervalTitle.Location = new Point(3, 50);
        lblWriteIntervalTitle.Name = "lblWriteIntervalTitle";
        lblWriteIntervalTitle.Size = new Size(174, 50);
        lblWriteIntervalTitle.TabIndex = 11;
        lblWriteIntervalTitle.Text = "寫入間隔(ms)";
        lblWriteIntervalTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // numWriteIntervalMs
        // 
        tableLayoutPanel1.SetColumnSpan(numWriteIntervalMs, 2);
        numWriteIntervalMs.Dock = DockStyle.Left;
        numWriteIntervalMs.Font = new Font("Microsoft JhengHei UI", 10F);
        numWriteIntervalMs.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        numWriteIntervalMs.Location = new Point(183, 53);
        numWriteIntervalMs.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
        numWriteIntervalMs.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        numWriteIntervalMs.Name = "numWriteIntervalMs";
        numWriteIntervalMs.Size = new Size(140, 24);
        numWriteIntervalMs.TabIndex = 12;
        numWriteIntervalMs.ThousandsSeparator = true;
        numWriteIntervalMs.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        numWriteIntervalMs.ValueChanged += numWriteIntervalMs_ValueChanged;
        // 
        // lblUiUpdateIntervalTitle
        // 
        lblUiUpdateIntervalTitle.AutoSize = true;
        lblUiUpdateIntervalTitle.Dock = DockStyle.Fill;
        lblUiUpdateIntervalTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblUiUpdateIntervalTitle.Location = new Point(3, 100);
        lblUiUpdateIntervalTitle.Name = "lblUiUpdateIntervalTitle";
        lblUiUpdateIntervalTitle.Size = new Size(174, 50);
        lblUiUpdateIntervalTitle.TabIndex = 13;
        lblUiUpdateIntervalTitle.Text = "畫面更新(ms)";
        lblUiUpdateIntervalTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // numUiUpdateIntervalMs
        // 
        tableLayoutPanel1.SetColumnSpan(numUiUpdateIntervalMs, 2);
        numUiUpdateIntervalMs.Dock = DockStyle.Left;
        numUiUpdateIntervalMs.Font = new Font("Microsoft JhengHei UI", 10F);
        numUiUpdateIntervalMs.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        numUiUpdateIntervalMs.Location = new Point(183, 103);
        numUiUpdateIntervalMs.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
        numUiUpdateIntervalMs.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        numUiUpdateIntervalMs.Name = "numUiUpdateIntervalMs";
        numUiUpdateIntervalMs.Size = new Size(140, 24);
        numUiUpdateIntervalMs.TabIndex = 14;
        numUiUpdateIntervalMs.ThousandsSeparator = true;
        numUiUpdateIntervalMs.Value = new decimal(new int[] { 1000, 0, 0, 0 });
        numUiUpdateIntervalMs.ValueChanged += numUiUpdateIntervalMs_ValueChanged;
        // 
        // lblInternalSamplingIntervalTitle
        // 
        lblInternalSamplingIntervalTitle.AutoSize = true;
        lblInternalSamplingIntervalTitle.Dock = DockStyle.Fill;
        lblInternalSamplingIntervalTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblInternalSamplingIntervalTitle.Location = new Point(3, 150);
        lblInternalSamplingIntervalTitle.Name = "lblInternalSamplingIntervalTitle";
        lblInternalSamplingIntervalTitle.Size = new Size(174, 50);
        lblInternalSamplingIntervalTitle.TabIndex = 15;
        lblInternalSamplingIntervalTitle.Text = "內部採樣(ms)";
        lblInternalSamplingIntervalTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // numInternalSamplingIntervalMs
        // 
        tableLayoutPanel1.SetColumnSpan(numInternalSamplingIntervalMs, 2);
        numInternalSamplingIntervalMs.Dock = DockStyle.Left;
        numInternalSamplingIntervalMs.Font = new Font("Microsoft JhengHei UI", 10F);
        numInternalSamplingIntervalMs.Increment = new decimal(new int[] { 50, 0, 0, 0 });
        numInternalSamplingIntervalMs.Location = new Point(183, 153);
        numInternalSamplingIntervalMs.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
        numInternalSamplingIntervalMs.Minimum = new decimal(new int[] { 250, 0, 0, 0 });
        numInternalSamplingIntervalMs.Name = "numInternalSamplingIntervalMs";
        numInternalSamplingIntervalMs.Size = new Size(140, 24);
        numInternalSamplingIntervalMs.TabIndex = 16;
        numInternalSamplingIntervalMs.ThousandsSeparator = true;
        numInternalSamplingIntervalMs.Value = new decimal(new int[] { 250, 0, 0, 0 });
        numInternalSamplingIntervalMs.ValueChanged += numInternalSamplingIntervalMs_ValueChanged;
        // 
        // lblCpuSmoothingWindowTitle
        // 
        lblCpuSmoothingWindowTitle.AutoSize = true;
        lblCpuSmoothingWindowTitle.Dock = DockStyle.Fill;
        lblCpuSmoothingWindowTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblCpuSmoothingWindowTitle.Location = new Point(3, 200);
        lblCpuSmoothingWindowTitle.Name = "lblCpuSmoothingWindowTitle";
        lblCpuSmoothingWindowTitle.Size = new Size(174, 50);
        lblCpuSmoothingWindowTitle.TabIndex = 17;
        lblCpuSmoothingWindowTitle.Text = "CPU 平滑樣本數";
        lblCpuSmoothingWindowTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // numCpuSmoothingWindowSize
        // 
        tableLayoutPanel1.SetColumnSpan(numCpuSmoothingWindowSize, 2);
        numCpuSmoothingWindowSize.Dock = DockStyle.Left;
        numCpuSmoothingWindowSize.Font = new Font("Microsoft JhengHei UI", 10F);
        numCpuSmoothingWindowSize.Location = new Point(183, 203);
        numCpuSmoothingWindowSize.Maximum = new decimal(new int[] { 60, 0, 0, 0 });
        numCpuSmoothingWindowSize.Minimum = new decimal(new int[] { 3, 0, 0, 0 });
        numCpuSmoothingWindowSize.Name = "numCpuSmoothingWindowSize";
        numCpuSmoothingWindowSize.Size = new Size(140, 24);
        numCpuSmoothingWindowSize.TabIndex = 18;
        numCpuSmoothingWindowSize.Value = new decimal(new int[] { 5, 0, 0, 0 });
        numCpuSmoothingWindowSize.ValueChanged += numCpuSmoothingWindowSize_ValueChanged;
        // 
        // lblCpuTotalTitle
        // 
        lblCpuTotalTitle.AutoSize = true;
        lblCpuTotalTitle.Dock = DockStyle.Fill;
        lblCpuTotalTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblCpuTotalTitle.Location = new Point(3, 250);
        lblCpuTotalTitle.Name = "lblCpuTotalTitle";
        lblCpuTotalTitle.Size = new Size(174, 50);
        lblCpuTotalTitle.TabIndex = 0;
        lblCpuTotalTitle.Text = "CPU 總使用率";
        lblCpuTotalTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblCpuTotalValue
        // 
        lblCpuTotalValue.AutoSize = true;
        tableLayoutPanel1.SetColumnSpan(lblCpuTotalValue, 2);
        lblCpuTotalValue.Dock = DockStyle.Fill;
        lblCpuTotalValue.Font = new Font("Microsoft JhengHei UI", 12F);
        lblCpuTotalValue.Location = new Point(183, 250);
        lblCpuTotalValue.Name = "lblCpuTotalValue";
        lblCpuTotalValue.Size = new Size(590, 50);
        lblCpuTotalValue.TabIndex = 1;
        lblCpuTotalValue.Text = "-";
        lblCpuTotalValue.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblCpuCoresTitle
        // 
        lblCpuCoresTitle.AutoSize = true;
        lblCpuCoresTitle.Dock = DockStyle.Fill;
        lblCpuCoresTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblCpuCoresTitle.Location = new Point(3, 300);
        lblCpuCoresTitle.Name = "lblCpuCoresTitle";
        lblCpuCoresTitle.Size = new Size(174, 226);
        lblCpuCoresTitle.TabIndex = 2;
        lblCpuCoresTitle.Text = "CPU 各核心使用率";
        // 
        // txtCpuCores
        // 
        tableLayoutPanel1.SetColumnSpan(txtCpuCores, 2);
        txtCpuCores.Dock = DockStyle.Fill;
        txtCpuCores.Font = new Font("Consolas", 11F);
        txtCpuCores.Location = new Point(183, 303);
        txtCpuCores.Multiline = true;
        txtCpuCores.Name = "txtCpuCores";
        txtCpuCores.ReadOnly = true;
        txtCpuCores.ScrollBars = ScrollBars.Vertical;
        txtCpuCores.Size = new Size(590, 220);
        txtCpuCores.TabIndex = 3;
        // 
        // lblMemoryTitle
        // 
        lblMemoryTitle.AutoSize = true;
        lblMemoryTitle.Dock = DockStyle.Fill;
        lblMemoryTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblMemoryTitle.Location = new Point(3, 526);
        lblMemoryTitle.Name = "lblMemoryTitle";
        lblMemoryTitle.Size = new Size(174, 50);
        lblMemoryTitle.TabIndex = 4;
        lblMemoryTitle.Text = "Memory 使用量 / 總量";
        lblMemoryTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblMemoryValue
        // 
        lblMemoryValue.AutoSize = true;
        tableLayoutPanel1.SetColumnSpan(lblMemoryValue, 2);
        lblMemoryValue.Dock = DockStyle.Fill;
        lblMemoryValue.Font = new Font("Microsoft JhengHei UI", 12F);
        lblMemoryValue.Location = new Point(183, 526);
        lblMemoryValue.Name = "lblMemoryValue";
        lblMemoryValue.Size = new Size(590, 50);
        lblMemoryValue.TabIndex = 5;
        lblMemoryValue.Text = "-";
        lblMemoryValue.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblDiskTitle
        // 
        lblDiskTitle.AutoSize = true;
        lblDiskTitle.Dock = DockStyle.Fill;
        lblDiskTitle.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold);
        lblDiskTitle.Location = new Point(3, 576);
        lblDiskTitle.Name = "lblDiskTitle";
        lblDiskTitle.Size = new Size(174, 50);
        lblDiskTitle.TabIndex = 6;
        lblDiskTitle.Text = "Disk 使用量 / 容量";
        lblDiskTitle.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // lblDiskValue
        // 
        lblDiskValue.AutoSize = true;
        tableLayoutPanel1.SetColumnSpan(lblDiskValue, 2);
        lblDiskValue.Dock = DockStyle.Fill;
        lblDiskValue.Font = new Font("Microsoft JhengHei UI", 12F);
        lblDiskValue.Location = new Point(183, 576);
        lblDiskValue.Name = "lblDiskValue";
        lblDiskValue.Size = new Size(590, 50);
        lblDiskValue.TabIndex = 7;
        lblDiskValue.Text = "-";
        lblDiskValue.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 650);
        Controls.Add(tableLayoutPanel1);
        Name = "Form1";
        Padding = new Padding(12);
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Hardware Info";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numWriteIntervalMs).EndInit();
        ((System.ComponentModel.ISupportInitialize)numUiUpdateIntervalMs).EndInit();
        ((System.ComponentModel.ISupportInitialize)numInternalSamplingIntervalMs).EndInit();
        ((System.ComponentModel.ISupportInitialize)numCpuSmoothingWindowSize).EndInit();
        ResumeLayout(false);
    }

    #endregion
}
