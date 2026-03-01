namespace hahaha
{
    partial class form_auto_run_multiple
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label command_label;
        private TextBox command;
        private Panel panel_is_run;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_auto_run_multiple));
            command_label = new Label();
            command = new TextBox();
            panel_is_run = new Panel();
            parameter = new TextBox();
            label_parameter = new Label();
            panel1 = new Panel();
            panel5 = new Panel();
            box_item = new ListBox();
            name_item = new TextBox();
            panel7 = new Panel();
            button_copy_item = new Button();
            button_down_item = new Button();
            button_up_item = new Button();
            button_delete_item = new Button();
            button_rename_item = new Button();
            button_add_item = new Button();
            panel4 = new Panel();
            box_class = new ListBox();
            name_class = new TextBox();
            panel6 = new Panel();
            button_copy_class = new Button();
            button_down_class = new Button();
            button_up_class = new Button();
            button_delete_class = new Button();
            button_rename_class = new Button();
            button_add_class = new Button();
            panel2 = new Panel();
            label5 = new Label();
            label4 = new Label();
            check_box_create_no_window = new CheckBox();
            check_box_use_shell_excute = new CheckBox();
            button_kill_process = new Button();
            button_run = new Button();
            button_close = new Button();
            check_box_auto_reload = new CheckBox();
            panel3 = new Panel();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            panel8 = new Panel();
            panel12 = new Panel();
            label6 = new Label();
            label7 = new Label();
            check_box_create_no_window_select = new CheckBox();
            check_box_use_shell_excute_select = new CheckBox();
            button_kill_process_select = new Button();
            button_un_select = new Button();
            check_box_auto_reload_select = new CheckBox();
            button_run_select = new Button();
            button_close_select = new Button();
            tabPage2 = new TabPage();
            panel9 = new Panel();
            box_display = new RichTextBox();
            tabPage3 = new TabPage();
            panel10 = new Panel();
            button_reset_all = new Button();
            button_load_all = new Button();
            button_save_all = new Button();
            label3 = new Label();
            count_line = new TextBox();
            label1 = new Label();
            time_display = new TextBox();
            label2 = new Label();
            tabPage4 = new TabPage();
            panel11 = new Panel();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            panel7.SuspendLayout();
            panel4.SuspendLayout();
            panel6.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            panel8.SuspendLayout();
            panel12.SuspendLayout();
            tabPage2.SuspendLayout();
            panel9.SuspendLayout();
            tabPage3.SuspendLayout();
            panel10.SuspendLayout();
            tabPage4.SuspendLayout();
            SuspendLayout();
            // 
            // command_label
            // 
            command_label.AutoSize = true;
            command_label.Location = new Point(18, 29);
            command_label.Name = "command_label";
            command_label.Size = new Size(31, 15);
            command_label.TabIndex = 0;
            command_label.Text = "指令";
            command_label.UseMnemonic = false;
            // 
            // command
            // 
            command.Location = new Point(81, 26);
            command.Name = "command";
            command.Size = new Size(434, 23);
            command.TabIndex = 1;
            command.TextChanged += command_TextChanged;
            // 
            // panel_is_run
            // 
            panel_is_run.Location = new Point(376, 183);
            panel_is_run.Name = "panel_is_run";
            panel_is_run.Size = new Size(148, 133);
            panel_is_run.TabIndex = 5;
            // 
            // parameter
            // 
            parameter.Location = new Point(81, 55);
            parameter.Name = "parameter";
            parameter.Size = new Size(434, 23);
            parameter.TabIndex = 7;
            parameter.TextChanged += parameter_TextChanged;
            // 
            // label_parameter
            // 
            label_parameter.AutoSize = true;
            label_parameter.Location = new Point(18, 58);
            label_parameter.Name = "label_parameter";
            label_parameter.Size = new Size(31, 15);
            label_parameter.TabIndex = 6;
            label_parameter.Text = "參數";
            label_parameter.UseMnemonic = false;
            // 
            // panel1
            // 
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel4);
            panel1.Dock = DockStyle.Left;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(726, 611);
            panel1.TabIndex = 8;
            // 
            // panel5
            // 
            panel5.Controls.Add(box_item);
            panel5.Controls.Add(name_item);
            panel5.Controls.Add(panel7);
            panel5.Dock = DockStyle.Left;
            panel5.Location = new Point(363, 0);
            panel5.Name = "panel5";
            panel5.Size = new Size(360, 611);
            panel5.TabIndex = 11;
            // 
            // box_item
            // 
            box_item.Dock = DockStyle.Fill;
            box_item.FormattingEnabled = true;
            box_item.Location = new Point(0, 0);
            box_item.Name = "box_item";
            box_item.Size = new Size(360, 528);
            box_item.TabIndex = 1;
            box_item.SelectedIndexChanged += box_item_SelectedIndexChanged;
            // 
            // name_item
            // 
            name_item.Dock = DockStyle.Bottom;
            name_item.Location = new Point(0, 528);
            name_item.Name = "name_item";
            name_item.Size = new Size(360, 23);
            name_item.TabIndex = 13;
            // 
            // panel7
            // 
            panel7.Controls.Add(button_copy_item);
            panel7.Controls.Add(button_down_item);
            panel7.Controls.Add(button_up_item);
            panel7.Controls.Add(button_delete_item);
            panel7.Controls.Add(button_rename_item);
            panel7.Controls.Add(button_add_item);
            panel7.Dock = DockStyle.Bottom;
            panel7.Location = new Point(0, 551);
            panel7.Name = "panel7";
            panel7.Size = new Size(360, 60);
            panel7.TabIndex = 12;
            // 
            // button_copy_item
            // 
            button_copy_item.Dock = DockStyle.Left;
            button_copy_item.Location = new Point(300, 0);
            button_copy_item.Name = "button_copy_item";
            button_copy_item.Size = new Size(60, 60);
            button_copy_item.TabIndex = 21;
            button_copy_item.Text = "複製";
            button_copy_item.UseVisualStyleBackColor = true;
            button_copy_item.Click += button_copy_item_Click;
            // 
            // button_down_item
            // 
            button_down_item.Dock = DockStyle.Left;
            button_down_item.Location = new Point(240, 0);
            button_down_item.Name = "button_down_item";
            button_down_item.Size = new Size(60, 60);
            button_down_item.TabIndex = 20;
            button_down_item.Text = "v";
            button_down_item.UseVisualStyleBackColor = true;
            button_down_item.Click += button_down_item_Click;
            // 
            // button_up_item
            // 
            button_up_item.Dock = DockStyle.Left;
            button_up_item.Location = new Point(180, 0);
            button_up_item.Name = "button_up_item";
            button_up_item.Size = new Size(60, 60);
            button_up_item.TabIndex = 19;
            button_up_item.Text = "^";
            button_up_item.UseVisualStyleBackColor = true;
            button_up_item.Click += button_up_item_Click;
            // 
            // button_delete_item
            // 
            button_delete_item.Dock = DockStyle.Left;
            button_delete_item.Location = new Point(120, 0);
            button_delete_item.Name = "button_delete_item";
            button_delete_item.Size = new Size(60, 60);
            button_delete_item.TabIndex = 18;
            button_delete_item.Text = "刪除";
            button_delete_item.UseVisualStyleBackColor = true;
            button_delete_item.Click += button_delete_item_Click;
            // 
            // button_rename_item
            // 
            button_rename_item.Dock = DockStyle.Left;
            button_rename_item.Location = new Point(60, 0);
            button_rename_item.Name = "button_rename_item";
            button_rename_item.Size = new Size(60, 60);
            button_rename_item.TabIndex = 17;
            button_rename_item.Text = "更名";
            button_rename_item.UseVisualStyleBackColor = true;
            button_rename_item.Click += button_rename_item_Click;
            // 
            // button_add_item
            // 
            button_add_item.Dock = DockStyle.Left;
            button_add_item.Location = new Point(0, 0);
            button_add_item.Name = "button_add_item";
            button_add_item.Size = new Size(60, 60);
            button_add_item.TabIndex = 16;
            button_add_item.Text = "新增";
            button_add_item.UseVisualStyleBackColor = true;
            button_add_item.Click += button_add_item_Click;
            // 
            // panel4
            // 
            panel4.Controls.Add(box_class);
            panel4.Controls.Add(name_class);
            panel4.Controls.Add(panel6);
            panel4.Dock = DockStyle.Left;
            panel4.Location = new Point(0, 0);
            panel4.Name = "panel4";
            panel4.Size = new Size(363, 611);
            panel4.TabIndex = 11;
            // 
            // box_class
            // 
            box_class.Dock = DockStyle.Fill;
            box_class.FormattingEnabled = true;
            box_class.Location = new Point(0, 0);
            box_class.Name = "box_class";
            box_class.Size = new Size(363, 528);
            box_class.TabIndex = 0;
            box_class.SelectedIndexChanged += box_class_SelectedIndexChanged;
            // 
            // name_class
            // 
            name_class.Dock = DockStyle.Bottom;
            name_class.Location = new Point(0, 528);
            name_class.Name = "name_class";
            name_class.Size = new Size(363, 23);
            name_class.TabIndex = 11;
            // 
            // panel6
            // 
            panel6.Controls.Add(button_copy_class);
            panel6.Controls.Add(button_down_class);
            panel6.Controls.Add(button_up_class);
            panel6.Controls.Add(button_delete_class);
            panel6.Controls.Add(button_rename_class);
            panel6.Controls.Add(button_add_class);
            panel6.Dock = DockStyle.Bottom;
            panel6.Location = new Point(0, 551);
            panel6.Name = "panel6";
            panel6.Size = new Size(363, 60);
            panel6.TabIndex = 11;
            // 
            // button_copy_class
            // 
            button_copy_class.Dock = DockStyle.Left;
            button_copy_class.Location = new Point(300, 0);
            button_copy_class.Name = "button_copy_class";
            button_copy_class.Size = new Size(60, 60);
            button_copy_class.TabIndex = 16;
            button_copy_class.Text = "複製";
            button_copy_class.UseVisualStyleBackColor = true;
            button_copy_class.Click += button_copy_class_Click;
            // 
            // button_down_class
            // 
            button_down_class.Dock = DockStyle.Left;
            button_down_class.Location = new Point(240, 0);
            button_down_class.Name = "button_down_class";
            button_down_class.Size = new Size(60, 60);
            button_down_class.TabIndex = 15;
            button_down_class.Text = "v";
            button_down_class.UseVisualStyleBackColor = true;
            button_down_class.Click += button_down_class_Click;
            // 
            // button_up_class
            // 
            button_up_class.Dock = DockStyle.Left;
            button_up_class.Location = new Point(180, 0);
            button_up_class.Name = "button_up_class";
            button_up_class.Size = new Size(60, 60);
            button_up_class.TabIndex = 14;
            button_up_class.Text = "^";
            button_up_class.UseVisualStyleBackColor = true;
            button_up_class.Click += button_up_class_Click;
            // 
            // button_delete_class
            // 
            button_delete_class.Dock = DockStyle.Left;
            button_delete_class.Location = new Point(120, 0);
            button_delete_class.Name = "button_delete_class";
            button_delete_class.Size = new Size(60, 60);
            button_delete_class.TabIndex = 13;
            button_delete_class.Text = "刪除";
            button_delete_class.UseVisualStyleBackColor = true;
            button_delete_class.Click += button_delete_class_Click;
            // 
            // button_rename_class
            // 
            button_rename_class.Dock = DockStyle.Left;
            button_rename_class.Location = new Point(60, 0);
            button_rename_class.Name = "button_rename_class";
            button_rename_class.Size = new Size(60, 60);
            button_rename_class.TabIndex = 12;
            button_rename_class.Text = "更名";
            button_rename_class.UseVisualStyleBackColor = true;
            button_rename_class.Click += button_rename_class_Click;
            // 
            // button_add_class
            // 
            button_add_class.Dock = DockStyle.Left;
            button_add_class.Location = new Point(0, 0);
            button_add_class.Name = "button_add_class";
            button_add_class.Size = new Size(60, 60);
            button_add_class.TabIndex = 11;
            button_add_class.Text = "新增";
            button_add_class.UseVisualStyleBackColor = true;
            button_add_class.Click += button_add_class_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(label5);
            panel2.Controls.Add(label4);
            panel2.Controls.Add(check_box_create_no_window);
            panel2.Controls.Add(check_box_use_shell_excute);
            panel2.Controls.Add(button_kill_process);
            panel2.Controls.Add(button_run);
            panel2.Controls.Add(button_close);
            panel2.Controls.Add(check_box_auto_reload);
            panel2.Controls.Add(command);
            panel2.Controls.Add(command_label);
            panel2.Controls.Add(parameter);
            panel2.Controls.Add(label_parameter);
            panel2.Controls.Add(panel_is_run);
            panel2.Dock = DockStyle.Fill;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(553, 348);
            panel2.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(290, 121);
            label5.Name = "label5";
            label5.Size = new Size(99, 15);
            label5.TabIndex = 18;
            label5.Text = "只對console有效";
            label5.UseMnemonic = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(290, 94);
            label4.Name = "label4";
            label4.Size = new Size(79, 15);
            label4.TabIndex = 17;
            label4.Text = "開文字檔網址";
            label4.UseMnemonic = false;
            // 
            // check_box_create_no_window
            // 
            check_box_create_no_window.AutoSize = true;
            check_box_create_no_window.Location = new Point(421, 117);
            check_box_create_no_window.Name = "check_box_create_no_window";
            check_box_create_no_window.Size = new Size(86, 19);
            check_box_create_no_window.TabIndex = 16;
            check_box_create_no_window.Text = "不建立視窗";
            check_box_create_no_window.UseVisualStyleBackColor = true;
            check_box_create_no_window.CheckedChanged += check_box_create_no_window_CheckedChanged;
            // 
            // check_box_use_shell_excute
            // 
            check_box_use_shell_excute.AutoSize = true;
            check_box_use_shell_excute.Location = new Point(421, 90);
            check_box_use_shell_excute.Name = "check_box_use_shell_excute";
            check_box_use_shell_excute.Size = new Size(77, 19);
            check_box_use_shell_excute.TabIndex = 15;
            check_box_use_shell_excute.Text = "Shell執行";
            check_box_use_shell_excute.UseVisualStyleBackColor = true;
            check_box_use_shell_excute.CheckedChanged += check_box_use_shell_excute_CheckedChanged;
            // 
            // button_kill_process
            // 
            button_kill_process.Location = new Point(18, 201);
            button_kill_process.Name = "button_kill_process";
            button_kill_process.Size = new Size(80, 60);
            button_kill_process.TabIndex = 14;
            button_kill_process.Text = "刪除進程";
            button_kill_process.UseVisualStyleBackColor = true;
            button_kill_process.Click += button_kill_process_Click;
            // 
            // button_run
            // 
            button_run.Location = new Point(18, 135);
            button_run.Name = "button_run";
            button_run.Size = new Size(80, 60);
            button_run.TabIndex = 12;
            button_run.Text = "執行";
            button_run.UseVisualStyleBackColor = true;
            button_run.Click += button_run_Click;
            // 
            // button_close
            // 
            button_close.Location = new Point(104, 135);
            button_close.Name = "button_close";
            button_close.Size = new Size(80, 60);
            button_close.TabIndex = 13;
            button_close.Text = "關閉";
            button_close.UseVisualStyleBackColor = true;
            button_close.Click += button_close_Click;
            // 
            // check_box_auto_reload
            // 
            check_box_auto_reload.AutoSize = true;
            check_box_auto_reload.Location = new Point(18, 93);
            check_box_auto_reload.Name = "check_box_auto_reload";
            check_box_auto_reload.Size = new Size(74, 19);
            check_box_auto_reload.TabIndex = 11;
            check_box_auto_reload.Text = "自動重載";
            check_box_auto_reload.UseVisualStyleBackColor = true;
            check_box_auto_reload.CheckedChanged += check_box_auto_reload_CheckedChanged;
            // 
            // panel3
            // 
            panel3.Controls.Add(tabControl1);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(726, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(567, 611);
            panel3.TabIndex = 10;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(567, 611);
            tabControl1.TabIndex = 12;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(panel8);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(559, 583);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "設定";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel8
            // 
            panel8.Controls.Add(panel2);
            panel8.Controls.Add(panel12);
            panel8.Dock = DockStyle.Fill;
            panel8.Location = new Point(3, 3);
            panel8.Name = "panel8";
            panel8.Size = new Size(553, 577);
            panel8.TabIndex = 11;
            // 
            // panel12
            // 
            panel12.Controls.Add(label6);
            panel12.Controls.Add(label7);
            panel12.Controls.Add(check_box_create_no_window_select);
            panel12.Controls.Add(check_box_use_shell_excute_select);
            panel12.Controls.Add(button_kill_process_select);
            panel12.Controls.Add(button_un_select);
            panel12.Controls.Add(check_box_auto_reload_select);
            panel12.Controls.Add(button_run_select);
            panel12.Controls.Add(button_close_select);
            panel12.Dock = DockStyle.Bottom;
            panel12.Location = new Point(0, 348);
            panel12.Name = "panel12";
            panel12.Size = new Size(553, 229);
            panel12.TabIndex = 15;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(290, 48);
            label6.Name = "label6";
            label6.Size = new Size(99, 15);
            label6.TabIndex = 20;
            label6.Text = "只對console有效";
            label6.UseMnemonic = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(290, 21);
            label7.Name = "label7";
            label7.Size = new Size(79, 15);
            label7.TabIndex = 19;
            label7.Text = "開文字檔網址";
            label7.UseMnemonic = false;
            // 
            // check_box_create_no_window_select
            // 
            check_box_create_no_window_select.AutoSize = true;
            check_box_create_no_window_select.Location = new Point(414, 47);
            check_box_create_no_window_select.Name = "check_box_create_no_window_select";
            check_box_create_no_window_select.Size = new Size(110, 19);
            check_box_create_no_window_select.TabIndex = 17;
            check_box_create_no_window_select.Text = "選擇不建立視窗";
            check_box_create_no_window_select.UseVisualStyleBackColor = true;
            check_box_create_no_window_select.CheckedChanged += check_box_create_no_window_select_CheckedChanged;
            // 
            // check_box_use_shell_excute_select
            // 
            check_box_use_shell_excute_select.AutoSize = true;
            check_box_use_shell_excute_select.Location = new Point(414, 17);
            check_box_use_shell_excute_select.Name = "check_box_use_shell_excute_select";
            check_box_use_shell_excute_select.Size = new Size(101, 19);
            check_box_use_shell_excute_select.TabIndex = 16;
            check_box_use_shell_excute_select.Text = "選擇Shell執行";
            check_box_use_shell_excute_select.UseVisualStyleBackColor = true;
            check_box_use_shell_excute_select.CheckedChanged += check_box_use_shell_excute_select_CheckedChanged;
            // 
            // button_kill_process_select
            // 
            button_kill_process_select.Location = new Point(18, 123);
            button_kill_process_select.Name = "button_kill_process_select";
            button_kill_process_select.Size = new Size(80, 60);
            button_kill_process_select.TabIndex = 15;
            button_kill_process_select.Text = "選擇刪除進程";
            button_kill_process_select.UseVisualStyleBackColor = true;
            button_kill_process_select.Click += button_kill_process_select_Click;
            // 
            // button_un_select
            // 
            button_un_select.Location = new Point(190, 57);
            button_un_select.Name = "button_un_select";
            button_un_select.Size = new Size(80, 60);
            button_un_select.TabIndex = 11;
            button_un_select.Text = "不選取";
            button_un_select.UseVisualStyleBackColor = true;
            button_un_select.Click += button_un_select_Click;
            // 
            // check_box_auto_reload_select
            // 
            check_box_auto_reload_select.AutoSize = true;
            check_box_auto_reload_select.Location = new Point(18, 23);
            check_box_auto_reload_select.Name = "check_box_auto_reload_select";
            check_box_auto_reload_select.Size = new Size(98, 19);
            check_box_auto_reload_select.TabIndex = 10;
            check_box_auto_reload_select.Text = "選擇自動重載";
            check_box_auto_reload_select.UseVisualStyleBackColor = true;
            check_box_auto_reload_select.CheckedChanged += check_box_auto_reload_select_CheckedChanged;
            // 
            // button_run_select
            // 
            button_run_select.Location = new Point(18, 57);
            button_run_select.Name = "button_run_select";
            button_run_select.Size = new Size(80, 60);
            button_run_select.TabIndex = 8;
            button_run_select.Text = "選擇全部執行";
            button_run_select.UseVisualStyleBackColor = true;
            button_run_select.Click += button_run_select_Click;
            // 
            // button_close_select
            // 
            button_close_select.Location = new Point(104, 57);
            button_close_select.Name = "button_close_select";
            button_close_select.Size = new Size(80, 60);
            button_close_select.TabIndex = 9;
            button_close_select.Text = "選擇全部關閉";
            button_close_select.UseVisualStyleBackColor = true;
            button_close_select.Click += button_close_select_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(panel9);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(559, 583);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "輸出";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            panel9.Controls.Add(box_display);
            panel9.Dock = DockStyle.Fill;
            panel9.Location = new Point(3, 3);
            panel9.Name = "panel9";
            panel9.Size = new Size(553, 577);
            panel9.TabIndex = 12;
            // 
            // box_display
            // 
            box_display.Dock = DockStyle.Fill;
            box_display.Location = new Point(0, 0);
            box_display.Name = "box_display";
            box_display.ReadOnly = true;
            box_display.Size = new Size(553, 577);
            box_display.TabIndex = 0;
            box_display.Text = "";
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(panel10);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(559, 583);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "系統設定";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel10
            // 
            panel10.Controls.Add(button_reset_all);
            panel10.Controls.Add(button_load_all);
            panel10.Controls.Add(button_save_all);
            panel10.Controls.Add(label3);
            panel10.Controls.Add(count_line);
            panel10.Controls.Add(label1);
            panel10.Controls.Add(time_display);
            panel10.Controls.Add(label2);
            panel10.Dock = DockStyle.Fill;
            panel10.Location = new Point(3, 3);
            panel10.Name = "panel10";
            panel10.Size = new Size(553, 577);
            panel10.TabIndex = 13;
            // 
            // button_reset_all
            // 
            button_reset_all.Location = new Point(191, 88);
            button_reset_all.Name = "button_reset_all";
            button_reset_all.Size = new Size(80, 60);
            button_reset_all.TabIndex = 16;
            button_reset_all.Text = "重設";
            button_reset_all.UseVisualStyleBackColor = true;
            button_reset_all.Click += button_reset_all_Click;
            // 
            // button_load_all
            // 
            button_load_all.Location = new Point(19, 88);
            button_load_all.Name = "button_load_all";
            button_load_all.Size = new Size(80, 60);
            button_load_all.TabIndex = 15;
            button_load_all.Text = "載入";
            button_load_all.UseVisualStyleBackColor = true;
            button_load_all.Click += button_load_all_Click;
            // 
            // button_save_all
            // 
            button_save_all.Location = new Point(105, 88);
            button_save_all.Name = "button_save_all";
            button_save_all.Size = new Size(80, 60);
            button_save_all.TabIndex = 14;
            button_save_all.Text = "儲存";
            button_save_all.UseVisualStyleBackColor = true;
            button_save_all.Click += button_save_all_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(522, 52);
            label3.Name = "label3";
            label3.Size = new Size(23, 15);
            label3.TabIndex = 12;
            label3.Text = "ms";
            label3.UseMnemonic = false;
            // 
            // count_line
            // 
            count_line.Location = new Point(82, 20);
            count_line.Name = "count_line";
            count_line.Size = new Size(434, 23);
            count_line.TabIndex = 9;
            count_line.Text = "800";
            count_line.TextChanged += count_line_TextChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(19, 23);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 8;
            label1.Text = "行數";
            label1.UseMnemonic = false;
            // 
            // time_display
            // 
            time_display.Location = new Point(82, 49);
            time_display.Name = "time_display";
            time_display.Size = new Size(434, 23);
            time_display.TabIndex = 11;
            time_display.Text = "500";
            time_display.TextChanged += time_display_TextChanged;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(19, 52);
            label2.Name = "label2";
            label2.Size = new Size(43, 15);
            label2.TabIndex = 10;
            label2.Text = "更新率";
            label2.UseMnemonic = false;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(panel11);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(559, 583);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "tabPage4";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel11
            // 
            panel11.Dock = DockStyle.Fill;
            panel11.Location = new Point(3, 3);
            panel11.Name = "panel11";
            panel11.Size = new Size(553, 577);
            panel11.TabIndex = 14;
            // 
            // form_auto_run_multiple
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1293, 611);
            Controls.Add(panel3);
            Controls.Add(panel1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "form_auto_run_multiple";
            Text = "auto run multiple";
            FormClosed += form_auto_run_FormClosed;
            Load += form_auto_run_Load;
            panel1.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            panel7.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel6.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            panel8.ResumeLayout(false);
            panel12.ResumeLayout(false);
            panel12.PerformLayout();
            tabPage2.ResumeLayout(false);
            panel9.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            panel10.ResumeLayout(false);
            panel10.PerformLayout();
            tabPage4.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion

        private TextBox parameter;
        private Label label_parameter;
        private Panel panel1;
        private Panel panel2;
        private Panel panel5;
        private ListBox box_item;
        private Panel panel4;
        private ListBox box_class;
        private Panel panel3;
        private TextBox name_item;
        private Panel panel7;
        private TextBox name_class;
        private Panel panel6;
        private Button button_add_class;
        private Button button_down_item;
        private Button button_up_item;
        private Button button_delete_item;
        private Button button_rename_item;
        private Button button_add_item;
        private Button button_down_class;
        private Button button_up_class;
        private Button button_delete_class;
        private Button button_rename_class;
        private Button button_copy_item;
        private Button button_copy_class;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private Panel panel8;
        private TabPage tabPage2;
        private Panel panel9;
        private TabPage tabPage3;
        private Panel panel10;
        private Panel panel12;
        private CheckBox check_box_auto_reload_select;
        private Button button_run_select;
        private Button button_close_select;
        private RichTextBox box_display;
        private TabPage tabPage4;
        private Panel panel11;
        private Label label3;
        private TextBox count_line;
        private Label label1;
        private TextBox time_display;
        private Label label2;
        private CheckBox check_box_auto_reload;
        private Button button_run;
        private Button button_close;
        private Button button_load_all;
        private Button button_save_all;
        private Button button_reset_all;
        private Button button_un_select;
        private Button button_kill_process;
        private Button button_kill_process_select;
        private CheckBox check_box_use_shell_excute;
        private CheckBox check_box_use_shell_excute_select;
        private CheckBox check_box_create_no_window;
        private CheckBox check_box_create_no_window_select;
        private Label label5;
        private Label label4;
        private Label label6;
        private Label label7;
    }
}