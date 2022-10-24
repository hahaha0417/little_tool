namespace hahaha
{
    partial class server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(server));
            this.panel_server = new System.Windows.Forms.Panel();
            this.panel_light = new System.Windows.Forms.Panel();
            this.button_send = new System.Windows.Forms.Button();
            this.label_send = new System.Windows.Forms.Label();
            this.text_box_send = new System.Windows.Forms.TextBox();
            this.label_receive = new System.Windows.Forms.Label();
            this.text_box_receive = new System.Windows.Forms.TextBox();
            this.button_close = new System.Windows.Forms.Button();
            this.button_start = new System.Windows.Forms.Button();
            this.label_port = new System.Windows.Forms.Label();
            this.text_box_port = new System.Windows.Forms.TextBox();
            this.panel_server.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_server
            // 
            this.panel_server.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel_server.Controls.Add(this.panel_light);
            this.panel_server.Controls.Add(this.button_send);
            this.panel_server.Controls.Add(this.label_send);
            this.panel_server.Controls.Add(this.text_box_send);
            this.panel_server.Controls.Add(this.label_receive);
            this.panel_server.Controls.Add(this.text_box_receive);
            this.panel_server.Controls.Add(this.button_close);
            this.panel_server.Controls.Add(this.button_start);
            this.panel_server.Controls.Add(this.label_port);
            this.panel_server.Controls.Add(this.text_box_port);
            this.panel_server.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_server.Location = new System.Drawing.Point(0, 0);
            this.panel_server.Name = "panel_server";
            this.panel_server.Size = new System.Drawing.Size(486, 301);
            this.panel_server.TabIndex = 0;
            // 
            // panel_light
            // 
            this.panel_light.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel_light.Location = new System.Drawing.Point(201, 55);
            this.panel_light.Name = "panel_light";
            this.panel_light.Size = new System.Drawing.Size(92, 79);
            this.panel_light.TabIndex = 11;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(22, 223);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(75, 64);
            this.button_send.TabIndex = 10;
            this.button_send.Text = "送出";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // label_send
            // 
            this.label_send.AutoSize = true;
            this.label_send.Location = new System.Drawing.Point(22, 186);
            this.label_send.Name = "label_send";
            this.label_send.Size = new System.Drawing.Size(31, 15);
            this.label_send.TabIndex = 9;
            this.label_send.Text = "送出";
            // 
            // text_box_send
            // 
            this.text_box_send.Location = new System.Drawing.Point(111, 183);
            this.text_box_send.Name = "text_box_send";
            this.text_box_send.Size = new System.Drawing.Size(355, 23);
            this.text_box_send.TabIndex = 8;
            // 
            // label_receive
            // 
            this.label_receive.AutoSize = true;
            this.label_receive.Location = new System.Drawing.Point(22, 147);
            this.label_receive.Name = "label_receive";
            this.label_receive.Size = new System.Drawing.Size(31, 15);
            this.label_receive.TabIndex = 7;
            this.label_receive.Text = "接收";
            // 
            // text_box_receive
            // 
            this.text_box_receive.Location = new System.Drawing.Point(111, 144);
            this.text_box_receive.Name = "text_box_receive";
            this.text_box_receive.Size = new System.Drawing.Size(355, 23);
            this.text_box_receive.TabIndex = 6;
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(103, 60);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 64);
            this.button_close.TabIndex = 5;
            this.button_close.Text = "關閉";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(22, 60);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(75, 64);
            this.button_start.TabIndex = 4;
            this.button_start.Text = "啟動";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(22, 22);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(31, 15);
            this.label_port.TabIndex = 3;
            this.label_port.Text = "port";
            // 
            // text_box_port
            // 
            this.text_box_port.Location = new System.Drawing.Point(111, 19);
            this.text_box_port.Name = "text_box_port";
            this.text_box_port.Size = new System.Drawing.Size(355, 23);
            this.text_box_port.TabIndex = 2;
            // 
            // server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(486, 301);
            this.Controls.Add(this.panel_server);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "server";
            this.Text = "server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.server_FormClosed);
            this.Load += new System.EventHandler(this.server_Load);
            this.panel_server.ResumeLayout(false);
            this.panel_server.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel_server;
        private Panel panel_light;
        private Button button_send;
        private Label label_send;
        private TextBox text_box_send;
        private Label label_receive;
        private TextBox text_box_receive;
        private Button button_close;
        private Button button_start;
        private Label label_port;
        private TextBox text_box_port;
    }
}