namespace hahaha
{
    partial class form_auto_run
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label command_label;
        private TextBox command;
        private Button button_run;
        private CheckBox check_autoload;
        private Button button_close;
        private Panel panel_autoload;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(form_auto_run));
            this.command_label = new System.Windows.Forms.Label();
            this.command = new System.Windows.Forms.TextBox();
            this.button_run = new System.Windows.Forms.Button();
            this.check_autoload = new System.Windows.Forms.CheckBox();
            this.button_close = new System.Windows.Forms.Button();
            this.panel_autoload = new System.Windows.Forms.Panel();
            this.parameter = new System.Windows.Forms.TextBox();
            this.label_parameter = new System.Windows.Forms.Label();
            this.button_monitor = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // command_label
            // 
            this.command_label.AutoSize = true;
            this.command_label.Location = new System.Drawing.Point(34, 24);
            this.command_label.Name = "command_label";
            this.command_label.Size = new System.Drawing.Size(31, 15);
            this.command_label.TabIndex = 0;
            this.command_label.Text = "指令";
            this.command_label.UseMnemonic = false;
            // 
            // command
            // 
            this.command.Location = new System.Drawing.Point(97, 21);
            this.command.Name = "command";
            this.command.Size = new System.Drawing.Size(452, 23);
            this.command.TabIndex = 1;
            // 
            // button_run
            // 
            this.button_run.Location = new System.Drawing.Point(34, 147);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(107, 81);
            this.button_run.TabIndex = 2;
            this.button_run.Text = "執行";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // check_autoload
            // 
            this.check_autoload.AutoSize = true;
            this.check_autoload.Location = new System.Drawing.Point(34, 103);
            this.check_autoload.Name = "check_autoload";
            this.check_autoload.Size = new System.Drawing.Size(74, 19);
            this.check_autoload.TabIndex = 3;
            this.check_autoload.Text = "自動重載";
            this.check_autoload.UseVisualStyleBackColor = true;
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(157, 147);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(107, 81);
            this.button_close.TabIndex = 4;
            this.button_close.Text = "關閉";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // panel_autoload
            // 
            this.panel_autoload.Location = new System.Drawing.Point(401, 97);
            this.panel_autoload.Name = "panel_autoload";
            this.panel_autoload.Size = new System.Drawing.Size(148, 131);
            this.panel_autoload.TabIndex = 5;
            // 
            // parameter
            // 
            this.parameter.Location = new System.Drawing.Point(97, 50);
            this.parameter.Name = "parameter";
            this.parameter.Size = new System.Drawing.Size(452, 23);
            this.parameter.TabIndex = 7;
            // 
            // label_parameter
            // 
            this.label_parameter.AutoSize = true;
            this.label_parameter.Location = new System.Drawing.Point(34, 53);
            this.label_parameter.Name = "label_parameter";
            this.label_parameter.Size = new System.Drawing.Size(31, 15);
            this.label_parameter.TabIndex = 6;
            this.label_parameter.Text = "參數";
            this.label_parameter.UseMnemonic = false;
            // 
            // button_monitor
            // 
            this.button_monitor.Location = new System.Drawing.Point(280, 147);
            this.button_monitor.Name = "button_monitor";
            this.button_monitor.Size = new System.Drawing.Size(107, 81);
            this.button_monitor.TabIndex = 8;
            this.button_monitor.Text = "監控";
            this.button_monitor.UseVisualStyleBackColor = true;
            this.button_monitor.Click += new System.EventHandler(this.button_monitor_Click);
            // 
            // form_auto_run
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 248);
            this.Controls.Add(this.button_monitor);
            this.Controls.Add(this.parameter);
            this.Controls.Add(this.label_parameter);
            this.Controls.Add(this.panel_autoload);
            this.Controls.Add(this.button_close);
            this.Controls.Add(this.check_autoload);
            this.Controls.Add(this.button_run);
            this.Controls.Add(this.command);
            this.Controls.Add(this.command_label);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "form_auto_run";
            this.Text = "auto run";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.form_auto_run_FormClosed);
            this.Load += new System.EventHandler(this.form_auto_run_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox parameter;
        private Label label_parameter;
        private Button button_monitor;
    }
}
