namespace cache_analysis
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
            components = new System.ComponentModel.Container();
            labelSettingsFile = new Label();
            cboSettingsFiles = new ComboBox();
            btnLoadSettings = new Button();
            btnSaveSettings = new Button();
            btnNewSettings = new Button();
            btnRenameSettings = new Button();
            btnDeleteSettings = new Button();
            labelProjectPath = new Label();
            txtProjectPath = new TextBox();
            btnBrowseProject = new Button();
            labelOutputFolder = new Label();
            txtOutputFolder = new TextBox();
            btnBrowseOutputFolder = new Button();
            labelParallelism = new Label();
            numParallelism = new NumericUpDown();
            btnGenerate = new Button();
            txtPreview = new TextBox();
            lblStatus = new Label();
            ((System.ComponentModel.ISupportInitialize)numParallelism).BeginInit();
            SuspendLayout();
            // 
            // labelSettingsFile
            // 
            labelSettingsFile.AutoSize = true;
            labelSettingsFile.Location = new Point(14, 18);
            labelSettingsFile.Name = "labelSettingsFile";
            labelSettingsFile.Size = new Size(85, 15);
            labelSettingsFile.TabIndex = 0;
            labelSettingsFile.Text = "setting 設定檔";
            // 
            // cboSettingsFiles
            // 
            cboSettingsFiles.DropDownStyle = ComboBoxStyle.DropDownList;
            cboSettingsFiles.FormattingEnabled = true;
            cboSettingsFiles.Location = new Point(14, 36);
            cboSettingsFiles.Name = "cboSettingsFiles";
            cboSettingsFiles.Size = new Size(340, 23);
            cboSettingsFiles.TabIndex = 1;
            cboSettingsFiles.DropDown += cboSettingsFiles_DropDown;
            // 
            // btnLoadSettings
            // 
            btnLoadSettings.Location = new Point(370, 18);
            btnLoadSettings.Name = "btnLoadSettings";
            btnLoadSettings.Size = new Size(80, 60);
            btnLoadSettings.TabIndex = 2;
            btnLoadSettings.Text = "載入";
            btnLoadSettings.UseVisualStyleBackColor = true;
            btnLoadSettings.Click += btnLoadSettings_Click;
            // 
            // btnSaveSettings
            // 
            btnSaveSettings.Location = new Point(456, 18);
            btnSaveSettings.Name = "btnSaveSettings";
            btnSaveSettings.Size = new Size(80, 60);
            btnSaveSettings.TabIndex = 3;
            btnSaveSettings.Text = "儲存";
            btnSaveSettings.UseVisualStyleBackColor = true;
            btnSaveSettings.Click += btnSaveSettings_Click;
            // 
            // btnNewSettings
            // 
            btnNewSettings.Location = new Point(542, 18);
            btnNewSettings.Name = "btnNewSettings";
            btnNewSettings.Size = new Size(80, 60);
            btnNewSettings.TabIndex = 4;
            btnNewSettings.Text = "新增";
            btnNewSettings.UseVisualStyleBackColor = true;
            btnNewSettings.Click += btnNewSettings_Click;
            // 
            // btnRenameSettings
            // 
            btnRenameSettings.Location = new Point(628, 18);
            btnRenameSettings.Name = "btnRenameSettings";
            btnRenameSettings.Size = new Size(80, 60);
            btnRenameSettings.TabIndex = 5;
            btnRenameSettings.Text = "更名";
            btnRenameSettings.UseVisualStyleBackColor = true;
            btnRenameSettings.Click += btnRenameSettings_Click;
            // 
            // btnDeleteSettings
            // 
            btnDeleteSettings.Location = new Point(714, 18);
            btnDeleteSettings.Name = "btnDeleteSettings";
            btnDeleteSettings.Size = new Size(80, 60);
            btnDeleteSettings.TabIndex = 6;
            btnDeleteSettings.Text = "刪除";
            btnDeleteSettings.UseVisualStyleBackColor = true;
            btnDeleteSettings.Click += btnDeleteSettings_Click;
            // 
            // labelProjectPath
            // 
            labelProjectPath.AutoSize = true;
            labelProjectPath.Location = new Point(14, 94);
            labelProjectPath.Name = "labelProjectPath";
            labelProjectPath.Size = new Size(67, 15);
            labelProjectPath.TabIndex = 7;
            labelProjectPath.Text = "專案資料夾";
            // 
            // txtProjectPath
            // 
            txtProjectPath.Location = new Point(14, 112);
            txtProjectPath.Name = "txtProjectPath";
            txtProjectPath.Size = new Size(690, 23);
            txtProjectPath.TabIndex = 8;
            // 
            // btnBrowseProject
            // 
            btnBrowseProject.Location = new Point(714, 94);
            btnBrowseProject.Name = "btnBrowseProject";
            btnBrowseProject.Size = new Size(80, 60);
            btnBrowseProject.TabIndex = 9;
            btnBrowseProject.Text = "選擇資料夾";
            btnBrowseProject.UseVisualStyleBackColor = true;
            btnBrowseProject.Click += btnBrowseProject_Click;
            // 
            // labelOutputFolder
            // 
            labelOutputFolder.AutoSize = true;
            labelOutputFolder.Location = new Point(14, 170);
            labelOutputFolder.Name = "labelOutputFolder";
            labelOutputFolder.Size = new Size(91, 15);
            labelOutputFolder.TabIndex = 10;
            labelOutputFolder.Text = "輸出資料夾";
            // 
            // txtOutputFolder
            // 
            txtOutputFolder.Location = new Point(14, 188);
            txtOutputFolder.Name = "txtOutputFolder";
            txtOutputFolder.Size = new Size(690, 23);
            txtOutputFolder.TabIndex = 11;
            // 
            // btnBrowseOutputFolder
            // 
            btnBrowseOutputFolder.Location = new Point(714, 170);
            btnBrowseOutputFolder.Name = "btnBrowseOutputFolder";
            btnBrowseOutputFolder.Size = new Size(80, 60);
            btnBrowseOutputFolder.TabIndex = 12;
            btnBrowseOutputFolder.Text = "選擇資料夾";
            btnBrowseOutputFolder.UseVisualStyleBackColor = true;
            btnBrowseOutputFolder.Click += btnBrowseOutputFolder_Click;
            // 
            // labelParallelism
            // 
            labelParallelism.AutoSize = true;
            labelParallelism.Location = new Point(14, 246);
            labelParallelism.Name = "labelParallelism";
            labelParallelism.Size = new Size(67, 15);
            labelParallelism.TabIndex = 13;
            labelParallelism.Text = "平行核心數";
            // 
            // numParallelism
            // 
            numParallelism.Location = new Point(87, 244);
            numParallelism.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
            numParallelism.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numParallelism.Name = "numParallelism";
            numParallelism.Size = new Size(88, 23);
            numParallelism.TabIndex = 14;
            numParallelism.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(195, 226);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(120, 60);
            btnGenerate.TabIndex = 15;
            btnGenerate.Text = "產生低 Token 分析";
            btnGenerate.UseVisualStyleBackColor = true;
            btnGenerate.Click += btnGenerate_Click;
            // 
            // txtPreview
            // 
            txtPreview.Location = new Point(14, 304);
            txtPreview.Multiline = true;
            txtPreview.Name = "txtPreview";
            txtPreview.ReadOnly = true;
            txtPreview.ScrollBars = ScrollBars.Both;
            txtPreview.Size = new Size(780, 300);
            txtPreview.TabIndex = 16;
            txtPreview.WordWrap = false;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(334, 248);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(103, 15);
            lblStatus.TabIndex = 17;
            lblStatus.Text = "等待指定專案路徑";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(814, 624);
            Controls.Add(lblStatus);
            Controls.Add(txtPreview);
            Controls.Add(btnGenerate);
            Controls.Add(numParallelism);
            Controls.Add(labelParallelism);
            Controls.Add(btnBrowseOutputFolder);
            Controls.Add(txtOutputFolder);
            Controls.Add(labelOutputFolder);
            Controls.Add(btnBrowseProject);
            Controls.Add(txtProjectPath);
            Controls.Add(labelProjectPath);
            Controls.Add(btnDeleteSettings);
            Controls.Add(btnRenameSettings);
            Controls.Add(btnNewSettings);
            Controls.Add(btnSaveSettings);
            Controls.Add(btnLoadSettings);
            Controls.Add(cboSettingsFiles);
            Controls.Add(labelSettingsFile);
            MinimumSize = new Size(830, 670);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Low Token Cache Analyzer";
            ((System.ComponentModel.ISupportInitialize)numParallelism).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelSettingsFile;
        private ComboBox cboSettingsFiles;
        private Button btnLoadSettings;
        private Button btnSaveSettings;
        private Button btnNewSettings;
        private Button btnRenameSettings;
        private Button btnDeleteSettings;
        private Label labelProjectPath;
        private TextBox txtProjectPath;
        private Button btnBrowseProject;
        private Label labelOutputFolder;
        private TextBox txtOutputFolder;
        private Button btnBrowseOutputFolder;
        private Label labelParallelism;
        private NumericUpDown numParallelism;
        private Button btnGenerate;
        private TextBox txtPreview;
        private Label lblStatus;
    }
}
