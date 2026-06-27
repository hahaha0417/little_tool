namespace hahaha
{
    partial class monitor_form
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
            label_status = new Label();
            label_status_value = new Label();
            label_pid = new Label();
            label_pid_value = new Label();
            label_cpu = new Label();
            label_cpu_value = new Label();
            label_priority = new Label();
            combo_priority = new ComboBox();
            label_affinity = new Label();
            checkedListBox_affinity = new CheckedListBox();
            button_apply = new Button();
            button_refresh = new Button();
            timer_refresh = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // label_status
            // 
            label_status.AutoSize = true;
            label_status.Location = new Point(28, 23);
            label_status.Name = "label_status";
            label_status.Size = new Size(31, 15);
            label_status.TabIndex = 0;
            label_status.Text = "狀態";
            // 
            // label_status_value
            // 
            label_status_value.AutoSize = true;
            label_status_value.Location = new Point(92, 23);
            label_status_value.Name = "label_status_value";
            label_status_value.Size = new Size(12, 15);
            label_status_value.TabIndex = 1;
            label_status_value.Text = "-";
            // 
            // label_pid
            // 
            label_pid.AutoSize = true;
            label_pid.Location = new Point(28, 52);
            label_pid.Name = "label_pid";
            label_pid.Size = new Size(26, 15);
            label_pid.TabIndex = 2;
            label_pid.Text = "PID";
            // 
            // label_pid_value
            // 
            label_pid_value.AutoSize = true;
            label_pid_value.Location = new Point(92, 52);
            label_pid_value.Name = "label_pid_value";
            label_pid_value.Size = new Size(12, 15);
            label_pid_value.TabIndex = 3;
            label_pid_value.Text = "-";
            // 
            // label_cpu
            // 
            label_cpu.AutoSize = true;
            label_cpu.Location = new Point(28, 81);
            label_cpu.Name = "label_cpu";
            label_cpu.Size = new Size(58, 15);
            label_cpu.TabIndex = 4;
            label_cpu.Text = "CPU 使用";
            // 
            // label_cpu_value
            // 
            label_cpu_value.AutoSize = true;
            label_cpu_value.Location = new Point(92, 81);
            label_cpu_value.Name = "label_cpu_value";
            label_cpu_value.Size = new Size(12, 15);
            label_cpu_value.TabIndex = 5;
            label_cpu_value.Text = "-";
            // 
            // label_priority
            // 
            label_priority.AutoSize = true;
            label_priority.Location = new Point(28, 121);
            label_priority.Name = "label_priority";
            label_priority.Size = new Size(46, 15);
            label_priority.TabIndex = 6;
            label_priority.Text = "Priority";
            // 
            // combo_priority
            // 
            combo_priority.FormattingEnabled = true;
            combo_priority.Location = new Point(92, 118);
            combo_priority.Name = "combo_priority";
            combo_priority.Size = new Size(208, 23);
            combo_priority.TabIndex = 7;
            combo_priority.SelectedIndexChanged += combo_priority_SelectedIndexChanged;
            // 
            // label_affinity
            // 
            label_affinity.AutoSize = true;
            label_affinity.Location = new Point(28, 162);
            label_affinity.Name = "label_affinity";
            label_affinity.Size = new Size(58, 15);
            label_affinity.TabIndex = 8;
            label_affinity.Text = "CPU 親和";
            // 
            // checkedListBox_affinity
            // 
            checkedListBox_affinity.CheckOnClick = true;
            checkedListBox_affinity.FormattingEnabled = true;
            checkedListBox_affinity.Location = new Point(92, 162);
            checkedListBox_affinity.Name = "checkedListBox_affinity";
            checkedListBox_affinity.Size = new Size(208, 184);
            checkedListBox_affinity.TabIndex = 9;
            checkedListBox_affinity.ItemCheck += checkedListBox_affinity_ItemCheck;
            checkedListBox_affinity.MouseDown += checkedListBox_affinity_MouseDown;
            // 
            // button_apply
            // 
            button_apply.Location = new Point(92, 364);
            button_apply.Name = "button_apply";
            button_apply.Size = new Size(97, 34);
            button_apply.TabIndex = 10;
            button_apply.Text = "套用";
            button_apply.UseVisualStyleBackColor = true;
            button_apply.Click += button_apply_Click;
            // 
            // button_refresh
            // 
            button_refresh.Location = new Point(203, 364);
            button_refresh.Name = "button_refresh";
            button_refresh.Size = new Size(97, 34);
            button_refresh.TabIndex = 11;
            button_refresh.Text = "重新整理";
            button_refresh.UseVisualStyleBackColor = true;
            button_refresh.Click += button_refresh_Click;
            // 
            // timer_refresh
            // 
            timer_refresh.Interval = 1000;
            timer_refresh.Tick += timer_refresh_Tick;
            // 
            // monitor_form
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(336, 423);
            Controls.Add(button_refresh);
            Controls.Add(button_apply);
            Controls.Add(checkedListBox_affinity);
            Controls.Add(label_affinity);
            Controls.Add(combo_priority);
            Controls.Add(label_priority);
            Controls.Add(label_cpu_value);
            Controls.Add(label_cpu);
            Controls.Add(label_pid_value);
            Controls.Add(label_pid);
            Controls.Add(label_status_value);
            Controls.Add(label_status);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "monitor_form";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Process 監控";
            FormClosed += monitor_form_FormClosed;
            Load += monitor_form_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private Label label_status;
        private Label label_status_value;
        private Label label_pid;
        private Label label_pid_value;
        private Label label_cpu;
        private Label label_cpu_value;
        private Label label_priority;
        private ComboBox combo_priority;
        private Label label_affinity;
        private CheckedListBox checkedListBox_affinity;
        private Button button_apply;
        private Button button_refresh;
        private System.Windows.Forms.Timer timer_refresh;
    }
}
