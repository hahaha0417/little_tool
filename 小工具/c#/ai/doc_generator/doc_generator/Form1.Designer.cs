#nullable disable

namespace CppDocGenerator;

partial class Form1
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblProfile = new Label();
        cmbProfiles = new ComboBox();
        btnAddProfile = new Button();
        btnDeleteProfile = new Button();
        btnRenameProfile = new Button();
        lblSourceFolder = new Label();
        txtSourceFolder = new TextBox();
        btnBrowseSource = new Button();
        lblOutputFile = new Label();
        txtOutputFile = new TextBox();
        btnBrowseOutput = new Button();
        lblGitHubRepo = new Label();
        txtGitHubRepo = new TextBox();
        lblGitReference = new Label();
        txtGitReference = new TextBox();
        lblParallelism = new Label();
        numParallelism = new NumericUpDown();
        btnGenerate = new Button();
        btnGenerateHtml = new Button();
        txtLog = new TextBox();
        lblJsonPreview = new Label();
        txtJsonPreview = new TextBox();
        ((System.ComponentModel.ISupportInitialize)numParallelism).BeginInit();
        SuspendLayout();
        // 
        // lblProfile
        // 
        lblProfile.AutoSize = true;
        lblProfile.Location = new Point(20, 20);
        lblProfile.Name = "lblProfile";
        lblProfile.Size = new Size(45, 15);
        lblProfile.TabIndex = 0;
        lblProfile.Text = "設定檔:";
        // 
        // cmbProfiles
        // 
        cmbProfiles.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbProfiles.FormattingEnabled = true;
        cmbProfiles.Location = new Point(20, 40);
        cmbProfiles.Name = "cmbProfiles";
        cmbProfiles.Size = new Size(340, 23);
        cmbProfiles.TabIndex = 1;
        cmbProfiles.SelectedIndexChanged += cmbProfiles_SelectedIndexChanged;
        // 
        // btnAddProfile
        // 
        btnAddProfile.Location = new Point(375, 39);
        btnAddProfile.Name = "btnAddProfile";
        btnAddProfile.Size = new Size(70, 25);
        btnAddProfile.TabIndex = 2;
        btnAddProfile.Text = "新增";
        btnAddProfile.UseVisualStyleBackColor = true;
        btnAddProfile.Click += btnAddProfile_Click;
        // 
        // btnDeleteProfile
        // 
        btnDeleteProfile.Location = new Point(455, 39);
        btnDeleteProfile.Name = "btnDeleteProfile";
        btnDeleteProfile.Size = new Size(70, 25);
        btnDeleteProfile.TabIndex = 3;
        btnDeleteProfile.Text = "刪除";
        btnDeleteProfile.UseVisualStyleBackColor = true;
        btnDeleteProfile.Click += btnDeleteProfile_Click;
        // 
        // btnRenameProfile
        // 
        btnRenameProfile.Location = new Point(535, 39);
        btnRenameProfile.Name = "btnRenameProfile";
        btnRenameProfile.Size = new Size(90, 25);
        btnRenameProfile.TabIndex = 4;
        btnRenameProfile.Text = "重新命名";
        btnRenameProfile.UseVisualStyleBackColor = true;
        btnRenameProfile.Click += btnRenameProfile_Click;
        // 
        // lblSourceFolder
        // 
        lblSourceFolder.AutoSize = true;
        lblSourceFolder.Location = new Point(20, 80);
        lblSourceFolder.Name = "lblSourceFolder";
        lblSourceFolder.Size = new Size(86, 15);
        lblSourceFolder.TabIndex = 5;
        lblSourceFolder.Text = "C++ Source：";
        // 
        // txtSourceFolder
        // 
        txtSourceFolder.Location = new Point(20, 100);
        txtSourceFolder.Name = "txtSourceFolder";
        txtSourceFolder.Size = new Size(620, 23);
        txtSourceFolder.TabIndex = 6;
        // 
        // btnBrowseSource
        // 
        btnBrowseSource.Location = new Point(655, 99);
        btnBrowseSource.Name = "btnBrowseSource";
        btnBrowseSource.Size = new Size(110, 25);
        btnBrowseSource.TabIndex = 7;
        btnBrowseSource.Text = "選擇資料夾";
        btnBrowseSource.UseVisualStyleBackColor = true;
        btnBrowseSource.Click += btnBrowseSource_Click;
        // 
        // lblOutputFile
        // 
        lblOutputFile.AutoSize = true;
        lblOutputFile.Location = new Point(20, 140);
        lblOutputFile.Name = "lblOutputFile";
        lblOutputFile.Size = new Size(72, 15);
        lblOutputFile.TabIndex = 8;
        lblOutputFile.Text = "輸出資料夾：";
        // 
        // txtOutputFile
        // 
        txtOutputFile.Location = new Point(20, 160);
        txtOutputFile.Name = "txtOutputFile";
        txtOutputFile.Size = new Size(620, 23);
        txtOutputFile.TabIndex = 9;
        // 
        // btnBrowseOutput
        // 
        btnBrowseOutput.Location = new Point(655, 159);
        btnBrowseOutput.Name = "btnBrowseOutput";
        btnBrowseOutput.Size = new Size(110, 25);
        btnBrowseOutput.TabIndex = 10;
        btnBrowseOutput.Text = "選擇資料夾";
        btnBrowseOutput.UseVisualStyleBackColor = true;
        btnBrowseOutput.Click += btnBrowseOutput_Click;
        // 
        // lblGitHubRepo
        // 
        lblGitHubRepo.AutoSize = true;
        lblGitHubRepo.Location = new Point(20, 200);
        lblGitHubRepo.Name = "lblGitHubRepo";
        lblGitHubRepo.Size = new Size(100, 15);
        lblGitHubRepo.TabIndex = 11;
        lblGitHubRepo.Text = "GitHub Repo URL:";
        // 
        // txtGitHubRepo
        // 
        txtGitHubRepo.Location = new Point(20, 220);
        txtGitHubRepo.Name = "txtGitHubRepo";
        txtGitHubRepo.Size = new Size(480, 23);
        txtGitHubRepo.TabIndex = 12;
        // 
        // lblGitReference
        // 
        lblGitReference.AutoSize = true;
        lblGitReference.Location = new Point(520, 200);
        lblGitReference.Name = "lblGitReference";
        lblGitReference.Size = new Size(72, 15);
        lblGitReference.TabIndex = 13;
        lblGitReference.Text = "Branch/Tag:";
        // 
        // txtGitReference
        // 
        txtGitReference.Location = new Point(520, 220);
        txtGitReference.Name = "txtGitReference";
        txtGitReference.Size = new Size(245, 23);
        txtGitReference.TabIndex = 14;
        // 
        // lblParallelism
        // 
        lblParallelism.AutoSize = true;
        lblParallelism.Location = new Point(20, 255);
        lblParallelism.Name = "lblParallelism";
        lblParallelism.Size = new Size(91, 15);
        lblParallelism.TabIndex = 15;
        lblParallelism.Text = "平行執行緒數：";
        // 
        // numParallelism
        // 
        numParallelism.Location = new Point(20, 275);
        numParallelism.Maximum = new decimal(new int[] { 128, 0, 0, 0 });
        numParallelism.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numParallelism.Name = "numParallelism";
        numParallelism.Size = new Size(120, 23);
        numParallelism.TabIndex = 16;
        numParallelism.Value = new decimal(new int[] { 1, 0, 0, 0 });
        // 
        // btnGenerate
        // 
        btnGenerate.Location = new Point(170, 268);
        btnGenerate.Name = "btnGenerate";
        btnGenerate.Size = new Size(160, 34);
        btnGenerate.TabIndex = 17;
        btnGenerate.Text = "產生 C++ 文件 JSON 資料夾";
        btnGenerate.UseVisualStyleBackColor = true;
        btnGenerate.Click += btnGenerate_Click;
        // 
        // btnGenerateHtml
        // 
        btnGenerateHtml.Location = new Point(345, 268);
        btnGenerateHtml.Name = "btnGenerateHtml";
        btnGenerateHtml.Size = new Size(160, 34);
        btnGenerateHtml.TabIndex = 18;
        btnGenerateHtml.Text = "產生 C++ Doc HTML";
        btnGenerateHtml.UseVisualStyleBackColor = true;
        btnGenerateHtml.Click += btnGenerateHtml_Click;
        // 
        // txtLog
        // 
        txtLog.Location = new Point(20, 320);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(360, 255);
        txtLog.TabIndex = 19;
        // 
        // lblJsonPreview
        // 
        lblJsonPreview.AutoSize = true;
        lblJsonPreview.Location = new Point(405, 300);
        lblJsonPreview.Name = "lblJsonPreview";
        lblJsonPreview.Size = new Size(75, 15);
        lblJsonPreview.TabIndex = 20;
        lblJsonPreview.Text = "JSON 預覽：";
        // 
        // txtJsonPreview
        // 
        txtJsonPreview.Location = new Point(405, 320);
        txtJsonPreview.Multiline = true;
        txtJsonPreview.Name = "txtJsonPreview";
        txtJsonPreview.ReadOnly = true;
        txtJsonPreview.ScrollBars = ScrollBars.Both;
        txtJsonPreview.Size = new Size(360, 255);
        txtJsonPreview.TabIndex = 21;
        txtJsonPreview.WordWrap = false;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 590);
        Controls.Add(txtJsonPreview);
        Controls.Add(lblJsonPreview);
        Controls.Add(txtLog);
        Controls.Add(btnGenerateHtml);
        Controls.Add(btnGenerate);
        Controls.Add(numParallelism);
        Controls.Add(lblParallelism);
        Controls.Add(btnRenameProfile);
        Controls.Add(btnDeleteProfile);
        Controls.Add(btnAddProfile);
        Controls.Add(cmbProfiles);
        Controls.Add(lblProfile);
        Controls.Add(txtGitReference);
        Controls.Add(lblGitReference);
        Controls.Add(txtGitHubRepo);
        Controls.Add(lblGitHubRepo);
        Controls.Add(btnBrowseOutput);
        Controls.Add(txtOutputFile);
        Controls.Add(lblOutputFile);
        Controls.Add(btnBrowseSource);
        Controls.Add(txtSourceFolder);
        Controls.Add(lblSourceFolder);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "C++ Doc Generator";
        ((System.ComponentModel.ISupportInitialize)numParallelism).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblProfile;
    private ComboBox cmbProfiles;
    private Button btnAddProfile;
    private Button btnDeleteProfile;
    private Button btnRenameProfile;
    private Label lblSourceFolder;
    private TextBox txtSourceFolder;
    private Button btnBrowseSource;
    private Label lblOutputFile;
    private TextBox txtOutputFile;
    private Button btnBrowseOutput;
    private Label lblGitHubRepo;
    private TextBox txtGitHubRepo;
    private Label lblGitReference;
    private TextBox txtGitReference;
    private Label lblParallelism;
    private NumericUpDown numParallelism;
    private Button btnGenerate;
    private Button btnGenerateHtml;
    private TextBox txtLog;
    private Label lblJsonPreview;
    private TextBox txtJsonPreview;
}

#nullable restore
