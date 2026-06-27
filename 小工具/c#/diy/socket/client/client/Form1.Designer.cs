namespace hahaha
{
    partial class client
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(client));
            this.panel_client = new System.Windows.Forms.Panel();
            this.label_ip = new System.Windows.Forms.Label();
            this.text_box_ip = new System.Windows.Forms.TextBox();
            this.panel_light = new System.Windows.Forms.Panel();
            this.button_send = new System.Windows.Forms.Button();
            this.label_send = new System.Windows.Forms.Label();
            this.text_box_send = new System.Windows.Forms.TextBox();
            this.label_receive = new System.Windows.Forms.Label();
            this.text_box_receive = new System.Windows.Forms.TextBox();
            this.button_close = new System.Windows.Forms.Button();
            this.button_connect = new System.Windows.Forms.Button();
            this.label_port = new System.Windows.Forms.Label();
            this.text_box_port = new System.Windows.Forms.TextBox();
            this.panel_client.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_client
            // 
            this.panel_client.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel_client.Controls.Add(this.label_ip);
            this.panel_client.Controls.Add(this.text_box_ip);
            this.panel_client.Controls.Add(this.panel_light);
            this.panel_client.Controls.Add(this.button_send);
            this.panel_client.Controls.Add(this.label_send);
            this.panel_client.Controls.Add(this.text_box_send);
            this.panel_client.Controls.Add(this.label_receive);
            this.panel_client.Controls.Add(this.text_box_receive);
            this.panel_client.Controls.Add(this.button_close);
            this.panel_client.Controls.Add(this.button_connect);
            this.panel_client.Controls.Add(this.label_port);
            this.panel_client.Controls.Add(this.text_box_port);
            this.panel_client.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_client.Location = new System.Drawing.Point(0, 0);
            this.panel_client.Name = "panel_client";
            this.panel_client.Size = new System.Drawing.Size(495, 344);
            this.panel_client.TabIndex = 1;
            // 
            // label_ip
            // 
            this.label_ip.AutoSize = true;
            this.label_ip.Location = new System.Drawing.Point(29, 21);
            this.label_ip.Name = "label_ip";
            this.label_ip.Size = new System.Drawing.Size(18, 15);
            this.label_ip.TabIndex = 13;
            this.label_ip.Text = "ip";
            // 
            // text_box_ip
            // 
            this.text_box_ip.Location = new System.Drawing.Point(118, 18);
            this.text_box_ip.Name = "text_box_ip";
            this.text_box_ip.Size = new System.Drawing.Size(355, 23);
            this.text_box_ip.TabIndex = 12;
            // 
            // panel_light
            // 
            this.panel_light.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.panel_light.Location = new System.Drawing.Point(209, 96);
            this.panel_light.Name = "panel_light";
            this.panel_light.Size = new System.Drawing.Size(92, 79);
            this.panel_light.TabIndex = 11;
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(30, 267);
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
            this.label_send.Location = new System.Drawing.Point(30, 230);
            this.label_send.Name = "label_send";
            this.label_send.Size = new System.Drawing.Size(31, 15);
            this.label_send.TabIndex = 9;
            this.label_send.Text = "送出";
            // 
            // text_box_send
            // 
            this.text_box_send.Location = new System.Drawing.Point(119, 227);
            this.text_box_send.Name = "text_box_send";
            this.text_box_send.Size = new System.Drawing.Size(355, 23);
            this.text_box_send.TabIndex = 8;
            // 
            // label_receive
            // 
            this.label_receive.AutoSize = true;
            this.label_receive.Location = new System.Drawing.Point(30, 191);
            this.label_receive.Name = "label_receive";
            this.label_receive.Size = new System.Drawing.Size(31, 15);
            this.label_receive.TabIndex = 7;
            this.label_receive.Text = "接收";
            // 
            // text_box_receive
            // 
            this.text_box_receive.Location = new System.Drawing.Point(119, 188);
            this.text_box_receive.Name = "text_box_receive";
            this.text_box_receive.Size = new System.Drawing.Size(355, 23);
            this.text_box_receive.TabIndex = 6;
            // 
            // button_close
            // 
            this.button_close.Location = new System.Drawing.Point(111, 104);
            this.button_close.Name = "button_close";
            this.button_close.Size = new System.Drawing.Size(75, 64);
            this.button_close.TabIndex = 5;
            this.button_close.Text = "關閉";
            this.button_close.UseVisualStyleBackColor = true;
            this.button_close.Click += new System.EventHandler(this.button_close_Click);
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(30, 104);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(75, 64);
            this.button_connect.TabIndex = 4;
            this.button_connect.Text = "連線";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(29, 61);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(31, 15);
            this.label_port.TabIndex = 3;
            this.label_port.Text = "port";
            // 
            // text_box_port
            // 
            this.text_box_port.Location = new System.Drawing.Point(118, 58);
            this.text_box_port.Name = "text_box_port";
            this.text_box_port.Size = new System.Drawing.Size(355, 23);
            this.text_box_port.TabIndex = 2;
            // 
            // client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 344);
            this.Controls.Add(this.panel_client);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "client";
            this.Text = "client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.client_FormClosed);
            this.Load += new System.EventHandler(this.client_Load);
            this.panel_client.ResumeLayout(false);
            this.panel_client.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel_client;
        private Label label_ip;
        private TextBox text_box_ip;
        private Panel panel_light;
        private Button button_send;
        private Label label_send;
        private TextBox text_box_send;
        private Label label_receive;
        private TextBox text_box_receive;
        private Button button_close;
        private Button button_connect;
        private Label label_port;
        private TextBox text_box_port;
    }
}