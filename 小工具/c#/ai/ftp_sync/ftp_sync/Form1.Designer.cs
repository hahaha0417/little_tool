namespace ftp_sync;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    private Panel pnlTop = null!;
    private Panel pnlSettings = null!;
    private Panel pnlSettingsLeft = null!;
    private Panel pnlSettingsRight = null!;
    private Panel pnlToolbar = null!;
    private Panel pnlStatus = null!;
    private Panel pnlLog = null!;
    private Panel pnlStatusCard = null!;
    private Panel pnlStatusTitle = null!;
    private Panel pnlStatusValuePanel = null!;
    private Panel pnlSummaryCard = null!;
    private Panel pnlSummaryTitle = null!;
    private Panel pnlSummaryValuePanel = null!;
    private Panel pnlProfileRow = null!;
    private Panel pnlHostRow = null!;
    private Panel pnlUserRow = null!;
    private Panel pnlPasswordRow = null!;
    private Panel pnlLocalRow = null!;
    private Panel pnlRemoteRow = null!;
    private Panel pnlExcludeRow = null!;
    private Panel pnlParallelRow = null!;
    private Panel pnlPassiveSslRow = null!;
    private Panel pnlMirrorRow = null!;
    private Panel pnlEnvRow = null!;
    private Panel pnlVendorRow = null!;
    private Panel pnlHighSpeedRow = null!;
    private Panel pnlDryRunRow = null!;
    private FlowLayoutPanel flpActions = null!;
    private Label lblHost = null!;
    private Label lblProfile = null!;
    private TextBox txtHost = null!;
    private ComboBox cboProfiles = null!;
    private Label lblPort = null!;
    private NumericUpDown numPort = null!;
    private Label lblUsername = null!;
    private TextBox txtUsername = null!;
    private Label lblPassword = null!;
    private TextBox txtPassword = null!;
    private Label lblLocalPath = null!;
    private TextBox txtLocalPath = null!;
    private Button btnBrowseLocal = null!;
    private Button btnSaveProfile = null!;
    private Button btnDeleteProfile = null!;
    private Button btnSwitchProfile = null!;
    private Label lblRemotePath = null!;
    private TextBox txtRemotePath = null!;
    private Label lblExcludePatterns = null!;
    private TextBox txtExcludePatterns = null!;
    private Label lblParallelUploads = null!;
    private NumericUpDown numParallelUploads = null!;
    private CheckBox chkPassiveMode = null!;
    private CheckBox chkUseSsl = null!;
    private CheckBox chkMirrorMode = null!;
    private CheckBox chkSyncEnvFiles = null!;
    private CheckBox chkSyncVendorFiles = null!;
    private CheckBox chkHighSpeedMode = null!;
    private CheckBox chkDryRun = null!;
    private Button btnExcludeHelp = null!;
    private Button btnTestConnection = null!;
    private Button btnStart = null!;
    private Button btnReverseSync = null!;
    private Button btnClearSyncHistory = null!;
    private Button btnClearReverseSyncHistory = null!;
    private Button btnCancel = null!;
    private ProgressBar progressBar = null!;
    private Label lblStatus = null!;
    private Label lblStatusValue = null!;
    private Label lblSummary = null!;
    private Label lblSummaryValue = null!;
    private TextBox txtLog = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        pnlStatus = new Panel();
        pnlSummaryCard = new Panel();
        pnlSummaryValuePanel = new Panel();
        lblSummaryValue = new Label();
        pnlSummaryTitle = new Panel();
        lblSummary = new Label();
        pnlStatusCard = new Panel();
        pnlStatusValuePanel = new Panel();
        lblStatusValue = new Label();
        pnlStatusTitle = new Panel();
        lblStatus = new Label();
        pnlToolbar = new Panel();
        progressBar = new ProgressBar();
        flpActions = new FlowLayoutPanel();
        btnStart = new Button();
        btnReverseSync = new Button();
        btnClearSyncHistory = new Button();
        btnClearReverseSyncHistory = new Button();
        btnCancel = new Button();
        pnlSettings = new Panel();
        pnlSettingsRight = new Panel();
        pnlDryRunRow = new Panel();
        chkDryRun = new CheckBox();
        pnlHighSpeedRow = new Panel();
        chkHighSpeedMode = new CheckBox();
        pnlVendorRow = new Panel();
        chkSyncVendorFiles = new CheckBox();
        pnlEnvRow = new Panel();
        btnTestConnection = new Button();
        chkSyncEnvFiles = new CheckBox();
        pnlMirrorRow = new Panel();
        btnExcludeHelp = new Button();
        chkMirrorMode = new CheckBox();
        pnlPassiveSslRow = new Panel();
        chkUseSsl = new CheckBox();
        chkPassiveMode = new CheckBox();
        pnlParallelRow = new Panel();
        numParallelUploads = new NumericUpDown();
        lblParallelUploads = new Label();
        pnlSettingsLeft = new Panel();
        pnlExcludeRow = new Panel();
        txtExcludePatterns = new TextBox();
        lblExcludePatterns = new Label();
        pnlRemoteRow = new Panel();
        txtRemotePath = new TextBox();
        lblRemotePath = new Label();
        pnlLocalRow = new Panel();
        txtLocalPath = new TextBox();
        btnBrowseLocal = new Button();
        lblLocalPath = new Label();
        pnlPasswordRow = new Panel();
        txtPassword = new TextBox();
        lblPassword = new Label();
        pnlUserRow = new Panel();
        txtUsername = new TextBox();
        lblUsername = new Label();
        pnlHostRow = new Panel();
        numPort = new NumericUpDown();
        lblPort = new Label();
        txtHost = new TextBox();
        lblHost = new Label();
        pnlProfileRow = new Panel();
        cboProfiles = new ComboBox();
        btnSaveProfile = new Button();
        btnDeleteProfile = new Button();
        btnSwitchProfile = new Button();
        lblProfile = new Label();
        pnlLog = new Panel();
        txtLog = new TextBox();
        pnlTop.SuspendLayout();
        pnlStatus.SuspendLayout();
        pnlSummaryCard.SuspendLayout();
        pnlSummaryValuePanel.SuspendLayout();
        pnlSummaryTitle.SuspendLayout();
        pnlStatusCard.SuspendLayout();
        pnlStatusValuePanel.SuspendLayout();
        pnlStatusTitle.SuspendLayout();
        pnlToolbar.SuspendLayout();
        flpActions.SuspendLayout();
        pnlSettings.SuspendLayout();
        pnlSettingsRight.SuspendLayout();
        pnlDryRunRow.SuspendLayout();
        pnlHighSpeedRow.SuspendLayout();
        pnlVendorRow.SuspendLayout();
        pnlEnvRow.SuspendLayout();
        pnlMirrorRow.SuspendLayout();
        pnlPassiveSslRow.SuspendLayout();
        pnlParallelRow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numParallelUploads).BeginInit();
        pnlSettingsLeft.SuspendLayout();
        pnlExcludeRow.SuspendLayout();
        pnlRemoteRow.SuspendLayout();
        pnlLocalRow.SuspendLayout();
        pnlPasswordRow.SuspendLayout();
        pnlUserRow.SuspendLayout();
        pnlHostRow.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numPort).BeginInit();
        pnlProfileRow.SuspendLayout();
        pnlLog.SuspendLayout();
        SuspendLayout();
        // 
        // pnlTop
        // 
        pnlTop.Controls.Add(pnlStatus);
        pnlTop.Controls.Add(pnlToolbar);
        pnlTop.Controls.Add(pnlSettings);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Location = new Point(0, 0);
        pnlTop.Margin = new Padding(3, 2, 3, 2);
        pnlTop.Name = "pnlTop";
        pnlTop.Padding = new Padding(10, 9, 10, 0);
        pnlTop.Size = new Size(861, 527);
        pnlTop.TabIndex = 0;
        // 
        // pnlStatus
        // 
        pnlStatus.Controls.Add(pnlSummaryCard);
        pnlStatus.Controls.Add(pnlStatusCard);
        pnlStatus.Dock = DockStyle.Top;
        pnlStatus.Location = new Point(10, 461);
        pnlStatus.Margin = new Padding(3, 2, 3, 2);
        pnlStatus.Name = "pnlStatus";
        pnlStatus.Padding = new Padding(0, 8, 0, 8);
        pnlStatus.Size = new Size(841, 66);
        pnlStatus.TabIndex = 2;
        // 
        // pnlSummaryCard
        // 
        pnlSummaryCard.Controls.Add(pnlSummaryValuePanel);
        pnlSummaryCard.Controls.Add(pnlSummaryTitle);
        pnlSummaryCard.Dock = DockStyle.Fill;
        pnlSummaryCard.Location = new Point(220, 8);
        pnlSummaryCard.Margin = new Padding(3, 2, 3, 2);
        pnlSummaryCard.Name = "pnlSummaryCard";
        pnlSummaryCard.Padding = new Padding(10, 8, 10, 8);
        pnlSummaryCard.Size = new Size(621, 50);
        pnlSummaryCard.TabIndex = 1;
        // 
        // pnlSummaryValuePanel
        // 
        pnlSummaryValuePanel.Controls.Add(lblSummaryValue);
        pnlSummaryValuePanel.Dock = DockStyle.Fill;
        pnlSummaryValuePanel.Location = new Point(74, 8);
        pnlSummaryValuePanel.Margin = new Padding(3, 2, 3, 2);
        pnlSummaryValuePanel.Name = "pnlSummaryValuePanel";
        pnlSummaryValuePanel.Size = new Size(537, 34);
        pnlSummaryValuePanel.TabIndex = 1;
        // 
        // lblSummaryValue
        // 
        lblSummaryValue.Dock = DockStyle.Fill;
        lblSummaryValue.Location = new Point(0, 0);
        lblSummaryValue.Name = "lblSummaryValue";
        lblSummaryValue.Size = new Size(537, 34);
        lblSummaryValue.TabIndex = 0;
        lblSummaryValue.Text = "尚未執行";
        lblSummaryValue.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlSummaryTitle
        // 
        pnlSummaryTitle.Controls.Add(lblSummary);
        pnlSummaryTitle.Dock = DockStyle.Left;
        pnlSummaryTitle.Location = new Point(10, 8);
        pnlSummaryTitle.Margin = new Padding(3, 2, 3, 2);
        pnlSummaryTitle.Name = "pnlSummaryTitle";
        pnlSummaryTitle.Size = new Size(64, 34);
        pnlSummaryTitle.TabIndex = 0;
        // 
        // lblSummary
        // 
        lblSummary.Dock = DockStyle.Fill;
        lblSummary.Location = new Point(0, 0);
        lblSummary.Name = "lblSummary";
        lblSummary.Size = new Size(64, 34);
        lblSummary.TabIndex = 0;
        lblSummary.Text = "統計";
        lblSummary.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlStatusCard
        // 
        pnlStatusCard.Controls.Add(pnlStatusValuePanel);
        pnlStatusCard.Controls.Add(pnlStatusTitle);
        pnlStatusCard.Dock = DockStyle.Left;
        pnlStatusCard.Location = new Point(0, 8);
        pnlStatusCard.Margin = new Padding(3, 2, 3, 2);
        pnlStatusCard.Name = "pnlStatusCard";
        pnlStatusCard.Padding = new Padding(10, 8, 10, 8);
        pnlStatusCard.Size = new Size(220, 50);
        pnlStatusCard.TabIndex = 0;
        // 
        // pnlStatusValuePanel
        // 
        pnlStatusValuePanel.Controls.Add(lblStatusValue);
        pnlStatusValuePanel.Dock = DockStyle.Fill;
        pnlStatusValuePanel.Location = new Point(74, 8);
        pnlStatusValuePanel.Margin = new Padding(3, 2, 3, 2);
        pnlStatusValuePanel.Name = "pnlStatusValuePanel";
        pnlStatusValuePanel.Size = new Size(136, 34);
        pnlStatusValuePanel.TabIndex = 1;
        // 
        // lblStatusValue
        // 
        lblStatusValue.Dock = DockStyle.Fill;
        lblStatusValue.Location = new Point(0, 0);
        lblStatusValue.Name = "lblStatusValue";
        lblStatusValue.Size = new Size(136, 34);
        lblStatusValue.TabIndex = 0;
        lblStatusValue.Text = "待命";
        lblStatusValue.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlStatusTitle
        // 
        pnlStatusTitle.Controls.Add(lblStatus);
        pnlStatusTitle.Dock = DockStyle.Left;
        pnlStatusTitle.Location = new Point(10, 8);
        pnlStatusTitle.Margin = new Padding(3, 2, 3, 2);
        pnlStatusTitle.Name = "pnlStatusTitle";
        pnlStatusTitle.Size = new Size(64, 34);
        pnlStatusTitle.TabIndex = 0;
        // 
        // lblStatus
        // 
        lblStatus.Dock = DockStyle.Fill;
        lblStatus.Location = new Point(0, 0);
        lblStatus.Name = "lblStatus";
        lblStatus.Size = new Size(64, 34);
        lblStatus.TabIndex = 0;
        lblStatus.Text = "狀態";
        lblStatus.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlToolbar
        // 
        pnlToolbar.Controls.Add(progressBar);
        pnlToolbar.Controls.Add(flpActions);
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Location = new Point(10, 382);
        pnlToolbar.Margin = new Padding(3, 2, 3, 2);
        pnlToolbar.Name = "pnlToolbar";
        pnlToolbar.Padding = new Padding(0, 6, 0, 0);
        pnlToolbar.Size = new Size(841, 79);
        pnlToolbar.TabIndex = 1;
        // 
        // progressBar
        // 
        progressBar.Dock = DockStyle.Fill;
        progressBar.Location = new Point(430, 6);
        progressBar.Margin = new Padding(3, 2, 3, 2);
        progressBar.MarqueeAnimationSpeed = 25;
        progressBar.Name = "progressBar";
        progressBar.Size = new Size(411, 73);
        progressBar.TabIndex = 1;
        // 
        // flpActions
        // 
        flpActions.AutoSize = true;
        flpActions.Controls.Add(btnStart);
        flpActions.Controls.Add(btnReverseSync);
        flpActions.Controls.Add(btnClearSyncHistory);
        flpActions.Controls.Add(btnClearReverseSyncHistory);
        flpActions.Controls.Add(btnCancel);
        flpActions.Dock = DockStyle.Left;
        flpActions.Location = new Point(0, 6);
        flpActions.Margin = new Padding(3, 2, 3, 2);
        flpActions.Name = "flpActions";
        flpActions.Size = new Size(430, 73);
        flpActions.TabIndex = 0;
        flpActions.WrapContents = false;
        // 
        // btnStart
        // 
        btnStart.Location = new Point(3, 2);
        btnStart.Margin = new Padding(3, 2, 3, 2);
        btnStart.Name = "btnStart";
        btnStart.Size = new Size(80, 60);
        btnStart.TabIndex = 0;
        btnStart.Text = "增量同步";
        btnStart.UseVisualStyleBackColor = true;
        btnStart.Click += btnStart_Click;
        // 
        // btnReverseSync
        // 
        btnReverseSync.Location = new Point(89, 2);
        btnReverseSync.Margin = new Padding(3, 2, 3, 2);
        btnReverseSync.Name = "btnReverseSync";
        btnReverseSync.Size = new Size(80, 60);
        btnReverseSync.TabIndex = 1;
        btnReverseSync.Text = "反向增量同步";
        btnReverseSync.UseVisualStyleBackColor = true;
        btnReverseSync.Click += btnReverseSync_Click;
        // 
        // btnClearSyncHistory
        // 
        btnClearSyncHistory.Location = new Point(175, 2);
        btnClearSyncHistory.Margin = new Padding(3, 2, 3, 2);
        btnClearSyncHistory.Name = "btnClearSyncHistory";
        btnClearSyncHistory.Size = new Size(80, 60);
        btnClearSyncHistory.TabIndex = 2;
        btnClearSyncHistory.Text = "清空正向紀錄";
        btnClearSyncHistory.UseVisualStyleBackColor = true;
        btnClearSyncHistory.Click += btnClearSyncHistory_Click;
        // 
        // btnClearReverseSyncHistory
        // 
        btnClearReverseSyncHistory.Location = new Point(261, 2);
        btnClearReverseSyncHistory.Margin = new Padding(3, 2, 3, 2);
        btnClearReverseSyncHistory.Name = "btnClearReverseSyncHistory";
        btnClearReverseSyncHistory.Size = new Size(80, 60);
        btnClearReverseSyncHistory.TabIndex = 3;
        btnClearReverseSyncHistory.Text = "清空反向紀錄";
        btnClearReverseSyncHistory.UseVisualStyleBackColor = true;
        btnClearReverseSyncHistory.Click += btnClearReverseSyncHistory_Click;
        // 
        // btnCancel
        // 
        btnCancel.Location = new Point(347, 2);
        btnCancel.Margin = new Padding(3, 2, 3, 2);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(80, 60);
        btnCancel.TabIndex = 4;
        btnCancel.Text = "取消";
        btnCancel.UseVisualStyleBackColor = true;
        btnCancel.Click += btnCancel_Click;
        // 
        // pnlSettings
        // 
        pnlSettings.BorderStyle = BorderStyle.FixedSingle;
        pnlSettings.Controls.Add(pnlSettingsRight);
        pnlSettings.Controls.Add(pnlSettingsLeft);
        pnlSettings.Dock = DockStyle.Top;
        pnlSettings.Location = new Point(10, 9);
        pnlSettings.Margin = new Padding(3, 2, 3, 2);
        pnlSettings.Name = "pnlSettings";
        pnlSettings.Padding = new Padding(10, 9, 10, 9);
        pnlSettings.Size = new Size(841, 373);
        pnlSettings.TabIndex = 0;
        // 
        // pnlSettingsRight
        // 
        pnlSettingsRight.Controls.Add(pnlDryRunRow);
        pnlSettingsRight.Controls.Add(pnlHighSpeedRow);
        pnlSettingsRight.Controls.Add(pnlVendorRow);
        pnlSettingsRight.Controls.Add(pnlEnvRow);
        pnlSettingsRight.Controls.Add(pnlMirrorRow);
        pnlSettingsRight.Controls.Add(pnlPassiveSslRow);
        pnlSettingsRight.Controls.Add(pnlParallelRow);
        pnlSettingsRight.Dock = DockStyle.Fill;
        pnlSettingsRight.Location = new Point(482, 9);
        pnlSettingsRight.Margin = new Padding(3, 2, 3, 2);
        pnlSettingsRight.Name = "pnlSettingsRight";
        pnlSettingsRight.Padding = new Padding(14, 0, 0, 0);
        pnlSettingsRight.Size = new Size(347, 353);
        pnlSettingsRight.TabIndex = 1;
        // 
        // pnlDryRunRow
        // 
        pnlDryRunRow.Controls.Add(chkDryRun);
        pnlDryRunRow.Dock = DockStyle.Top;
        pnlDryRunRow.Location = new Point(14, 304);
        pnlDryRunRow.Margin = new Padding(3, 2, 3, 2);
        pnlDryRunRow.Name = "pnlDryRunRow";
        pnlDryRunRow.Padding = new Padding(0, 6, 0, 0);
        pnlDryRunRow.Size = new Size(333, 36);
        pnlDryRunRow.TabIndex = 4;
        // 
        // chkDryRun
        // 
        chkDryRun.AutoSize = true;
        chkDryRun.Location = new Point(0, 10);
        chkDryRun.Margin = new Padding(3, 2, 3, 2);
        chkDryRun.Name = "chkDryRun";
        chkDryRun.Size = new Size(168, 19);
        chkDryRun.TabIndex = 0;
        chkDryRun.Text = "Dry Run 預演 (不實際變更)";
        chkDryRun.UseVisualStyleBackColor = true;
        // 
        // pnlHighSpeedRow
        // 
        pnlHighSpeedRow.Controls.Add(chkHighSpeedMode);
        pnlHighSpeedRow.Dock = DockStyle.Top;
        pnlHighSpeedRow.Location = new Point(14, 268);
        pnlHighSpeedRow.Margin = new Padding(3, 2, 3, 2);
        pnlHighSpeedRow.Name = "pnlHighSpeedRow";
        pnlHighSpeedRow.Padding = new Padding(0, 6, 0, 0);
        pnlHighSpeedRow.Size = new Size(333, 36);
        pnlHighSpeedRow.TabIndex = 5;
        // 
        // chkHighSpeedMode
        // 
        chkHighSpeedMode.AutoSize = true;
        chkHighSpeedMode.Location = new Point(0, 10);
        chkHighSpeedMode.Margin = new Padding(3, 2, 3, 2);
        chkHighSpeedMode.Name = "chkHighSpeedMode";
        chkHighSpeedMode.Size = new Size(151, 19);
        chkHighSpeedMode.TabIndex = 0;
        chkHighSpeedMode.Text = "高速模式 (降低 UI 更新)";
        chkHighSpeedMode.UseVisualStyleBackColor = true;
        // 
        // pnlVendorRow
        // 
        pnlVendorRow.Controls.Add(chkSyncVendorFiles);
        pnlVendorRow.Dock = DockStyle.Top;
        pnlVendorRow.Location = new Point(14, 232);
        pnlVendorRow.Margin = new Padding(3, 2, 3, 2);
        pnlVendorRow.Name = "pnlVendorRow";
        pnlVendorRow.Padding = new Padding(0, 6, 0, 0);
        pnlVendorRow.Size = new Size(333, 36);
        pnlVendorRow.TabIndex = 4;
        // 
        // chkSyncVendorFiles
        // 
        chkSyncVendorFiles.AutoSize = true;
        chkSyncVendorFiles.Location = new Point(0, 10);
        chkSyncVendorFiles.Margin = new Padding(3, 2, 3, 2);
        chkSyncVendorFiles.Name = "chkSyncVendorFiles";
        chkSyncVendorFiles.Size = new Size(216, 19);
        chkSyncVendorFiles.TabIndex = 0;
        chkSyncVendorFiles.Text = "所有 vendor 下檔案和資料夾也同步";
        chkSyncVendorFiles.UseVisualStyleBackColor = true;
        // 
        // pnlEnvRow
        // 
        pnlEnvRow.Controls.Add(btnTestConnection);
        pnlEnvRow.Controls.Add(chkSyncEnvFiles);
        pnlEnvRow.Dock = DockStyle.Top;
        pnlEnvRow.Location = new Point(14, 172);
        pnlEnvRow.Margin = new Padding(3, 2, 3, 2);
        pnlEnvRow.Name = "pnlEnvRow";
        pnlEnvRow.Size = new Size(333, 60);
        pnlEnvRow.TabIndex = 3;
        // 
        // btnTestConnection
        // 
        btnTestConnection.Dock = DockStyle.Right;
        btnTestConnection.Location = new Point(253, 0);
        btnTestConnection.Margin = new Padding(3, 2, 3, 2);
        btnTestConnection.Name = "btnTestConnection";
        btnTestConnection.Size = new Size(80, 60);
        btnTestConnection.TabIndex = 1;
        btnTestConnection.Text = "測試連線";
        btnTestConnection.UseVisualStyleBackColor = true;
        btnTestConnection.Click += btnTestConnection_Click;
        // 
        // chkSyncEnvFiles
        // 
        chkSyncEnvFiles.AutoSize = true;
        chkSyncEnvFiles.Location = new Point(0, 20);
        chkSyncEnvFiles.Margin = new Padding(3, 2, 3, 2);
        chkSyncEnvFiles.Name = "chkSyncEnvFiles";
        chkSyncEnvFiles.Size = new Size(163, 19);
        chkSyncEnvFiles.TabIndex = 0;
        chkSyncEnvFiles.Text = "所有 .env 開頭檔案也同步";
        chkSyncEnvFiles.UseVisualStyleBackColor = true;
        // 
        // pnlMirrorRow
        // 
        pnlMirrorRow.Controls.Add(btnExcludeHelp);
        pnlMirrorRow.Controls.Add(chkMirrorMode);
        pnlMirrorRow.Dock = DockStyle.Top;
        pnlMirrorRow.Location = new Point(14, 112);
        pnlMirrorRow.Margin = new Padding(3, 2, 3, 2);
        pnlMirrorRow.Name = "pnlMirrorRow";
        pnlMirrorRow.Size = new Size(333, 60);
        pnlMirrorRow.TabIndex = 2;
        // 
        // btnExcludeHelp
        // 
        btnExcludeHelp.Dock = DockStyle.Right;
        btnExcludeHelp.Location = new Point(253, 0);
        btnExcludeHelp.Margin = new Padding(3, 2, 3, 2);
        btnExcludeHelp.Name = "btnExcludeHelp";
        btnExcludeHelp.Size = new Size(80, 60);
        btnExcludeHelp.TabIndex = 1;
        btnExcludeHelp.Text = "規則範例";
        btnExcludeHelp.UseVisualStyleBackColor = true;
        btnExcludeHelp.Click += btnExcludeHelp_Click;
        // 
        // chkMirrorMode
        // 
        chkMirrorMode.AutoSize = true;
        chkMirrorMode.Location = new Point(0, 20);
        chkMirrorMode.Margin = new Padding(3, 2, 3, 2);
        chkMirrorMode.Name = "chkMirrorMode";
        chkMirrorMode.Size = new Size(181, 19);
        chkMirrorMode.TabIndex = 0;
        chkMirrorMode.Text = "鏡像同步 (刪除遠端多餘檔案)";
        chkMirrorMode.UseVisualStyleBackColor = true;
        // 
        // pnlPassiveSslRow
        // 
        pnlPassiveSslRow.Controls.Add(chkUseSsl);
        pnlPassiveSslRow.Controls.Add(chkPassiveMode);
        pnlPassiveSslRow.Dock = DockStyle.Top;
        pnlPassiveSslRow.Location = new Point(14, 56);
        pnlPassiveSslRow.Margin = new Padding(3, 2, 3, 2);
        pnlPassiveSslRow.Name = "pnlPassiveSslRow";
        pnlPassiveSslRow.Padding = new Padding(0, 6, 0, 0);
        pnlPassiveSslRow.Size = new Size(333, 56);
        pnlPassiveSslRow.TabIndex = 1;
        // 
        // chkUseSsl
        // 
        chkUseSsl.AutoSize = true;
        chkUseSsl.Location = new Point(144, 18);
        chkUseSsl.Margin = new Padding(3, 2, 3, 2);
        chkUseSsl.Name = "chkUseSsl";
        chkUseSsl.Size = new Size(105, 19);
        chkUseSsl.TabIndex = 1;
        chkUseSsl.Text = "使用 FTPS/SSL";
        chkUseSsl.UseVisualStyleBackColor = true;
        // 
        // chkPassiveMode
        // 
        chkPassiveMode.AutoSize = true;
        chkPassiveMode.Checked = true;
        chkPassiveMode.CheckState = CheckState.Checked;
        chkPassiveMode.Location = new Point(0, 18);
        chkPassiveMode.Margin = new Padding(3, 2, 3, 2);
        chkPassiveMode.Name = "chkPassiveMode";
        chkPassiveMode.Size = new Size(104, 19);
        chkPassiveMode.TabIndex = 0;
        chkPassiveMode.Text = "Passive Mode";
        chkPassiveMode.UseVisualStyleBackColor = true;
        // 
        // pnlParallelRow
        // 
        pnlParallelRow.Controls.Add(numParallelUploads);
        pnlParallelRow.Controls.Add(lblParallelUploads);
        pnlParallelRow.Dock = DockStyle.Top;
        pnlParallelRow.Location = new Point(14, 0);
        pnlParallelRow.Margin = new Padding(3, 2, 3, 2);
        pnlParallelRow.Name = "pnlParallelRow";
        pnlParallelRow.Size = new Size(333, 56);
        pnlParallelRow.TabIndex = 0;
        // 
        // numParallelUploads
        // 
        numParallelUploads.Location = new Point(94, 14);
        numParallelUploads.Margin = new Padding(3, 2, 3, 2);
        numParallelUploads.Maximum = new decimal(new int[] { int.MaxValue, 0, 0, 0 });
        numParallelUploads.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numParallelUploads.Name = "numParallelUploads";
        numParallelUploads.Size = new Size(84, 23);
        numParallelUploads.TabIndex = 1;
        numParallelUploads.Value = new decimal(new int[] { 4, 0, 0, 0 });
        // 
        // lblParallelUploads
        // 
        lblParallelUploads.AutoSize = true;
        lblParallelUploads.Location = new Point(0, 18);
        lblParallelUploads.Name = "lblParallelUploads";
        lblParallelUploads.Size = new Size(67, 15);
        lblParallelUploads.TabIndex = 0;
        lblParallelUploads.Text = "同步執行緒";
        // 
        // pnlSettingsLeft
        // 
        pnlSettingsLeft.Controls.Add(pnlExcludeRow);
        pnlSettingsLeft.Controls.Add(pnlRemoteRow);
        pnlSettingsLeft.Controls.Add(pnlLocalRow);
        pnlSettingsLeft.Controls.Add(pnlPasswordRow);
        pnlSettingsLeft.Controls.Add(pnlUserRow);
        pnlSettingsLeft.Controls.Add(pnlHostRow);
        pnlSettingsLeft.Controls.Add(pnlProfileRow);
        pnlSettingsLeft.Dock = DockStyle.Left;
        pnlSettingsLeft.Location = new Point(10, 9);
        pnlSettingsLeft.Margin = new Padding(3, 2, 3, 2);
        pnlSettingsLeft.Name = "pnlSettingsLeft";
        pnlSettingsLeft.Size = new Size(472, 353);
        pnlSettingsLeft.TabIndex = 0;
        // 
        // pnlExcludeRow
        // 
        pnlExcludeRow.Controls.Add(txtExcludePatterns);
        pnlExcludeRow.Controls.Add(lblExcludePatterns);
        pnlExcludeRow.Dock = DockStyle.Fill;
        pnlExcludeRow.Location = new Point(0, 260);
        pnlExcludeRow.Margin = new Padding(3, 2, 3, 2);
        pnlExcludeRow.Name = "pnlExcludeRow";
        pnlExcludeRow.Padding = new Padding(0, 6, 0, 0);
        pnlExcludeRow.Size = new Size(472, 93);
        pnlExcludeRow.TabIndex = 5;
        // 
        // txtExcludePatterns
        // 
        txtExcludePatterns.Dock = DockStyle.Fill;
        txtExcludePatterns.Location = new Point(84, 6);
        txtExcludePatterns.Margin = new Padding(3, 2, 3, 2);
        txtExcludePatterns.Multiline = true;
        txtExcludePatterns.Name = "txtExcludePatterns";
        txtExcludePatterns.ScrollBars = ScrollBars.Vertical;
        txtExcludePatterns.Size = new Size(388, 87);
        txtExcludePatterns.TabIndex = 1;
        // 
        // lblExcludePatterns
        // 
        lblExcludePatterns.Dock = DockStyle.Left;
        lblExcludePatterns.Location = new Point(0, 6);
        lblExcludePatterns.Name = "lblExcludePatterns";
        lblExcludePatterns.Size = new Size(84, 87);
        lblExcludePatterns.TabIndex = 0;
        lblExcludePatterns.Text = "排除規則";
        lblExcludePatterns.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlRemoteRow
        // 
        pnlRemoteRow.Controls.Add(txtRemotePath);
        pnlRemoteRow.Controls.Add(lblRemotePath);
        pnlRemoteRow.Dock = DockStyle.Top;
        pnlRemoteRow.Location = new Point(0, 226);
        pnlRemoteRow.Margin = new Padding(3, 2, 3, 2);
        pnlRemoteRow.Name = "pnlRemoteRow";
        pnlRemoteRow.Padding = new Padding(0, 6, 0, 0);
        pnlRemoteRow.Size = new Size(472, 34);
        pnlRemoteRow.TabIndex = 4;
        // 
        // txtRemotePath
        // 
        txtRemotePath.Dock = DockStyle.Fill;
        txtRemotePath.Location = new Point(84, 6);
        txtRemotePath.Margin = new Padding(3, 2, 3, 2);
        txtRemotePath.Name = "txtRemotePath";
        txtRemotePath.PlaceholderText = "/public_html/upload";
        txtRemotePath.Size = new Size(388, 23);
        txtRemotePath.TabIndex = 1;
        // 
        // lblRemotePath
        // 
        lblRemotePath.Dock = DockStyle.Left;
        lblRemotePath.Location = new Point(0, 6);
        lblRemotePath.Name = "lblRemotePath";
        lblRemotePath.Size = new Size(84, 28);
        lblRemotePath.TabIndex = 0;
        lblRemotePath.Text = "遠端資料夾";
        lblRemotePath.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlLocalRow
        // 
        pnlLocalRow.Controls.Add(txtLocalPath);
        pnlLocalRow.Controls.Add(btnBrowseLocal);
        pnlLocalRow.Controls.Add(lblLocalPath);
        pnlLocalRow.Dock = DockStyle.Top;
        pnlLocalRow.Location = new Point(0, 166);
        pnlLocalRow.Margin = new Padding(3, 2, 3, 2);
        pnlLocalRow.Name = "pnlLocalRow";
        pnlLocalRow.Padding = new Padding(0, 6, 0, 0);
        pnlLocalRow.Size = new Size(472, 60);
        pnlLocalRow.TabIndex = 3;
        // 
        // txtLocalPath
        // 
        txtLocalPath.Dock = DockStyle.Fill;
        txtLocalPath.Location = new Point(84, 6);
        txtLocalPath.Margin = new Padding(3, 2, 3, 2);
        txtLocalPath.Name = "txtLocalPath";
        txtLocalPath.PlaceholderText = "C:\\deploy\\website";
        txtLocalPath.Size = new Size(308, 23);
        txtLocalPath.TabIndex = 1;
        // 
        // btnBrowseLocal
        // 
        btnBrowseLocal.Dock = DockStyle.Right;
        btnBrowseLocal.Location = new Point(392, 6);
        btnBrowseLocal.Margin = new Padding(3, 2, 3, 2);
        btnBrowseLocal.Name = "btnBrowseLocal";
        btnBrowseLocal.Size = new Size(80, 54);
        btnBrowseLocal.TabIndex = 2;
        btnBrowseLocal.Text = "瀏覽...";
        btnBrowseLocal.UseVisualStyleBackColor = true;
        btnBrowseLocal.Click += btnBrowseLocal_Click;
        // 
        // lblLocalPath
        // 
        lblLocalPath.Dock = DockStyle.Left;
        lblLocalPath.Location = new Point(0, 6);
        lblLocalPath.Name = "lblLocalPath";
        lblLocalPath.Size = new Size(84, 54);
        lblLocalPath.TabIndex = 0;
        lblLocalPath.Text = "本機資料夾";
        lblLocalPath.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlPasswordRow
        // 
        pnlPasswordRow.Controls.Add(txtPassword);
        pnlPasswordRow.Controls.Add(lblPassword);
        pnlPasswordRow.Dock = DockStyle.Top;
        pnlPasswordRow.Location = new Point(0, 130);
        pnlPasswordRow.Margin = new Padding(3, 2, 3, 2);
        pnlPasswordRow.Name = "pnlPasswordRow";
        pnlPasswordRow.Padding = new Padding(0, 6, 0, 0);
        pnlPasswordRow.Size = new Size(472, 36);
        pnlPasswordRow.TabIndex = 2;
        // 
        // txtPassword
        // 
        txtPassword.Dock = DockStyle.Fill;
        txtPassword.Location = new Point(84, 6);
        txtPassword.Margin = new Padding(3, 2, 3, 2);
        txtPassword.Name = "txtPassword";
        txtPassword.PasswordChar = '*';
        txtPassword.Size = new Size(388, 23);
        txtPassword.TabIndex = 1;
        // 
        // lblPassword
        // 
        lblPassword.Dock = DockStyle.Left;
        lblPassword.Location = new Point(0, 6);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(84, 30);
        lblPassword.TabIndex = 0;
        lblPassword.Text = "密碼";
        lblPassword.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlUserRow
        // 
        pnlUserRow.Controls.Add(txtUsername);
        pnlUserRow.Controls.Add(lblUsername);
        pnlUserRow.Dock = DockStyle.Top;
        pnlUserRow.Location = new Point(0, 95);
        pnlUserRow.Margin = new Padding(3, 2, 3, 2);
        pnlUserRow.Name = "pnlUserRow";
        pnlUserRow.Padding = new Padding(0, 6, 0, 0);
        pnlUserRow.Size = new Size(472, 35);
        pnlUserRow.TabIndex = 1;
        // 
        // txtUsername
        // 
        txtUsername.Dock = DockStyle.Fill;
        txtUsername.Location = new Point(84, 6);
        txtUsername.Margin = new Padding(3, 2, 3, 2);
        txtUsername.Name = "txtUsername";
        txtUsername.Size = new Size(388, 23);
        txtUsername.TabIndex = 1;
        // 
        // lblUsername
        // 
        lblUsername.Dock = DockStyle.Left;
        lblUsername.Location = new Point(0, 6);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(84, 29);
        lblUsername.TabIndex = 0;
        lblUsername.Text = "帳號";
        lblUsername.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlHostRow
        // 
        pnlHostRow.Controls.Add(numPort);
        pnlHostRow.Controls.Add(lblPort);
        pnlHostRow.Controls.Add(txtHost);
        pnlHostRow.Controls.Add(lblHost);
        pnlHostRow.Dock = DockStyle.Top;
        pnlHostRow.Location = new Point(0, 60);
        pnlHostRow.Margin = new Padding(3, 2, 3, 2);
        pnlHostRow.Name = "pnlHostRow";
        pnlHostRow.Size = new Size(472, 35);
        pnlHostRow.TabIndex = 0;
        // 
        // numPort
        // 
        numPort.Dock = DockStyle.Right;
        numPort.Location = new Point(346, 0);
        numPort.Margin = new Padding(3, 2, 3, 2);
        numPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
        numPort.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
        numPort.Name = "numPort";
        numPort.Size = new Size(84, 23);
        numPort.TabIndex = 3;
        numPort.Value = new decimal(new int[] { 21, 0, 0, 0 });
        // 
        // lblPort
        // 
        lblPort.Dock = DockStyle.Right;
        lblPort.Location = new Point(430, 0);
        lblPort.Name = "lblPort";
        lblPort.Size = new Size(42, 35);
        lblPort.TabIndex = 2;
        lblPort.Text = "Port";
        lblPort.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // txtHost
        // 
        txtHost.Dock = DockStyle.Fill;
        txtHost.Location = new Point(84, 0);
        txtHost.Margin = new Padding(3, 2, 3, 2);
        txtHost.Name = "txtHost";
        txtHost.PlaceholderText = "ftp.example.com";
        txtHost.Size = new Size(388, 23);
        txtHost.TabIndex = 1;
        // 
        // lblHost
        // 
        lblHost.Dock = DockStyle.Left;
        lblHost.Location = new Point(0, 0);
        lblHost.Name = "lblHost";
        lblHost.Size = new Size(84, 35);
        lblHost.TabIndex = 0;
        lblHost.Text = "FTP 主機";
        lblHost.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlProfileRow
        // 
        pnlProfileRow.Controls.Add(cboProfiles);
        pnlProfileRow.Controls.Add(btnSaveProfile);
        pnlProfileRow.Controls.Add(btnDeleteProfile);
        pnlProfileRow.Controls.Add(btnSwitchProfile);
        pnlProfileRow.Controls.Add(lblProfile);
        pnlProfileRow.Dock = DockStyle.Top;
        pnlProfileRow.Location = new Point(0, 0);
        pnlProfileRow.Margin = new Padding(3, 2, 3, 2);
        pnlProfileRow.Name = "pnlProfileRow";
        pnlProfileRow.Padding = new Padding(0, 6, 0, 0);
        pnlProfileRow.Size = new Size(472, 60);
        pnlProfileRow.TabIndex = 0;
        // 
        // cboProfiles
        // 
        cboProfiles.Dock = DockStyle.Fill;
        cboProfiles.FormattingEnabled = true;
        cboProfiles.Location = new Point(84, 6);
        cboProfiles.Margin = new Padding(3, 2, 3, 2);
        cboProfiles.Name = "cboProfiles";
        cboProfiles.Size = new Size(148, 23);
        cboProfiles.TabIndex = 1;
        // 
        // btnSaveProfile
        // 
        btnSaveProfile.Dock = DockStyle.Right;
        btnSaveProfile.Location = new Point(232, 6);
        btnSaveProfile.Margin = new Padding(3, 2, 3, 2);
        btnSaveProfile.Name = "btnSaveProfile";
        btnSaveProfile.Size = new Size(80, 54);
        btnSaveProfile.TabIndex = 2;
        btnSaveProfile.Text = "儲存";
        btnSaveProfile.UseVisualStyleBackColor = true;
        btnSaveProfile.Click += btnSaveProfile_Click;
        // 
        // btnDeleteProfile
        // 
        btnDeleteProfile.Dock = DockStyle.Right;
        btnDeleteProfile.Location = new Point(312, 6);
        btnDeleteProfile.Margin = new Padding(3, 2, 3, 2);
        btnDeleteProfile.Name = "btnDeleteProfile";
        btnDeleteProfile.Size = new Size(80, 54);
        btnDeleteProfile.TabIndex = 3;
        btnDeleteProfile.Text = "刪除";
        btnDeleteProfile.UseVisualStyleBackColor = true;
        btnDeleteProfile.Click += btnDeleteProfile_Click;
        // 
        // btnSwitchProfile
        // 
        btnSwitchProfile.Dock = DockStyle.Right;
        btnSwitchProfile.Location = new Point(392, 6);
        btnSwitchProfile.Margin = new Padding(3, 2, 3, 2);
        btnSwitchProfile.Name = "btnSwitchProfile";
        btnSwitchProfile.Size = new Size(80, 54);
        btnSwitchProfile.TabIndex = 4;
        btnSwitchProfile.Text = "選擇";
        btnSwitchProfile.UseVisualStyleBackColor = true;
        btnSwitchProfile.Click += btnSwitchProfile_Click;
        // 
        // lblProfile
        // 
        lblProfile.Dock = DockStyle.Left;
        lblProfile.Location = new Point(0, 6);
        lblProfile.Name = "lblProfile";
        lblProfile.Size = new Size(84, 54);
        lblProfile.TabIndex = 0;
        lblProfile.Text = "設定檔";
        lblProfile.TextAlign = ContentAlignment.MiddleLeft;
        // 
        // pnlLog
        // 
        pnlLog.Controls.Add(txtLog);
        pnlLog.Dock = DockStyle.Fill;
        pnlLog.Location = new Point(0, 527);
        pnlLog.Margin = new Padding(3, 2, 3, 2);
        pnlLog.Name = "pnlLog";
        pnlLog.Padding = new Padding(10, 9, 10, 9);
        pnlLog.Size = new Size(861, 194);
        pnlLog.TabIndex = 1;
        // 
        // txtLog
        // 
        txtLog.Dock = DockStyle.Fill;
        txtLog.Font = new Font("Consolas", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
        txtLog.Location = new Point(10, 9);
        txtLog.Margin = new Padding(3, 2, 3, 2);
        txtLog.Multiline = true;
        txtLog.Name = "txtLog";
        txtLog.ReadOnly = true;
        txtLog.ScrollBars = ScrollBars.Vertical;
        txtLog.Size = new Size(841, 176);
        txtLog.TabIndex = 0;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(861, 721);
        Controls.Add(pnlLog);
        Controls.Add(pnlTop);
        Margin = new Padding(3, 2, 3, 2);
        MinimumSize = new Size(877, 760);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "FTP 增量同步工具";
        pnlTop.ResumeLayout(false);
        pnlStatus.ResumeLayout(false);
        pnlSummaryCard.ResumeLayout(false);
        pnlSummaryValuePanel.ResumeLayout(false);
        pnlSummaryTitle.ResumeLayout(false);
        pnlStatusCard.ResumeLayout(false);
        pnlStatusValuePanel.ResumeLayout(false);
        pnlStatusTitle.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        flpActions.ResumeLayout(false);
        pnlSettings.ResumeLayout(false);
        pnlSettingsRight.ResumeLayout(false);
        pnlDryRunRow.ResumeLayout(false);
        pnlDryRunRow.PerformLayout();
        pnlHighSpeedRow.ResumeLayout(false);
        pnlHighSpeedRow.PerformLayout();
        pnlVendorRow.ResumeLayout(false);
        pnlVendorRow.PerformLayout();
        pnlEnvRow.ResumeLayout(false);
        pnlEnvRow.PerformLayout();
        pnlMirrorRow.ResumeLayout(false);
        pnlMirrorRow.PerformLayout();
        pnlPassiveSslRow.ResumeLayout(false);
        pnlPassiveSslRow.PerformLayout();
        pnlParallelRow.ResumeLayout(false);
        pnlParallelRow.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numParallelUploads).EndInit();
        pnlSettingsLeft.ResumeLayout(false);
        pnlExcludeRow.ResumeLayout(false);
        pnlExcludeRow.PerformLayout();
        pnlRemoteRow.ResumeLayout(false);
        pnlRemoteRow.PerformLayout();
        pnlLocalRow.ResumeLayout(false);
        pnlLocalRow.PerformLayout();
        pnlPasswordRow.ResumeLayout(false);
        pnlPasswordRow.PerformLayout();
        pnlUserRow.ResumeLayout(false);
        pnlUserRow.PerformLayout();
        pnlHostRow.ResumeLayout(false);
        pnlHostRow.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numPort).EndInit();
        pnlProfileRow.ResumeLayout(false);
        pnlLog.ResumeLayout(false);
        pnlLog.PerformLayout();
        ResumeLayout(false);
    }

    #endregion
}
