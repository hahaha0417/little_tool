namespace hahaha
{ 
    partial class Socket_Client
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Socket_Client));
            this.label_ip = new System.Windows.Forms.Label();
            this.ip = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.TextBox();
            this.label_port = new System.Windows.Forms.Label();
            this.send = new System.Windows.Forms.TextBox();
            this.label_send = new System.Windows.Forms.Label();
            this.button_send = new System.Windows.Forms.Button();
            this.button_connect = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.receive = new System.Windows.Forms.TextBox();
            this.label_receive = new System.Windows.Forms.Label();
            this.panel_connect = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // label_ip
            // 
            this.label_ip.AutoSize = true;
            this.label_ip.Location = new System.Drawing.Point(23, 25);
            this.label_ip.Name = "label_ip";
            this.label_ip.Size = new System.Drawing.Size(18, 15);
            this.label_ip.TabIndex = 0;
            this.label_ip.Text = "ip";
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(109, 22);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(415, 23);
            this.ip.TabIndex = 1;
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(109, 51);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(415, 23);
            this.port.TabIndex = 3;
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(23, 54);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(31, 15);
            this.label_port.TabIndex = 2;
            this.label_port.Text = "port";
            // 
            // send
            // 
            this.send.Location = new System.Drawing.Point(109, 80);
            this.send.Name = "send";
            this.send.Size = new System.Drawing.Size(415, 23);
            this.send.TabIndex = 5;
            // 
            // label_send
            // 
            this.label_send.AutoSize = true;
            this.label_send.Location = new System.Drawing.Point(23, 83);
            this.label_send.Name = "label_send";
            this.label_send.Size = new System.Drawing.Size(31, 15);
            this.label_send.TabIndex = 4;
            this.label_send.Text = "送出";
            // 
            // button_send
            // 
            this.button_send.Location = new System.Drawing.Point(40, 165);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(124, 92);
            this.button_send.TabIndex = 6;
            this.button_send.Text = "送出";
            this.button_send.UseVisualStyleBackColor = true;
            this.button_send.Click += new System.EventHandler(this.button_send_Click);
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(186, 165);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(124, 92);
            this.button_connect.TabIndex = 7;
            this.button_connect.Text = "連線";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(333, 165);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(124, 92);
            this.button_stop.TabIndex = 8;
            this.button_stop.Text = "關閉";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // receive
            // 
            this.receive.Location = new System.Drawing.Point(109, 109);
            this.receive.Name = "receive";
            this.receive.Size = new System.Drawing.Size(415, 23);
            this.receive.TabIndex = 10;
            // 
            // label_receive
            // 
            this.label_receive.AutoSize = true;
            this.label_receive.Location = new System.Drawing.Point(23, 112);
            this.label_receive.Name = "label_receive";
            this.label_receive.Size = new System.Drawing.Size(31, 15);
            this.label_receive.TabIndex = 9;
            this.label_receive.Text = "收到";
            // 
            // panel_connect
            // 
            this.panel_connect.Location = new System.Drawing.Point(552, 38);
            this.panel_connect.Name = "panel_connect";
            this.panel_connect.Size = new System.Drawing.Size(229, 183);
            this.panel_connect.TabIndex = 23;
            // 
            // Socket_Client
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 272);
            this.Controls.Add(this.panel_connect);
            this.Controls.Add(this.receive);
            this.Controls.Add(this.label_receive);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.send);
            this.Controls.Add(this.label_send);
            this.Controls.Add(this.port);
            this.Controls.Add(this.label_port);
            this.Controls.Add(this.ip);
            this.Controls.Add(this.label_ip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Socket_Client";
            this.Text = "Socket Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Socket_Client_FormClosed);
            this.Load += new System.EventHandler(this.Socket_Client_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label_ip;
        private TextBox ip;
        private TextBox port;
        private Label label_port;
        private TextBox send;
        private Label label_send;
        private Button button_send;
        private Button button_connect;
        private Button button_stop;
        private TextBox receive;
        private Label label_receive;
        private Panel panel_connect;
    }
}