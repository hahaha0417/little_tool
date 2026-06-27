namespace hahaha
{
    partial class Socket_Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Socket_Server));
            this.receive = new System.Windows.Forms.TextBox();
            this.label_receive = new System.Windows.Forms.Label();
            this.button_stop = new System.Windows.Forms.Button();
            this.button_connect = new System.Windows.Forms.Button();
            this.port = new System.Windows.Forms.TextBox();
            this.label_port = new System.Windows.Forms.Label();
            this.panel_connect = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // receive
            // 
            this.receive.Location = new System.Drawing.Point(108, 52);
            this.receive.Name = "receive";
            this.receive.Size = new System.Drawing.Size(415, 23);
            this.receive.TabIndex = 21;
            // 
            // label_receive
            // 
            this.label_receive.AutoSize = true;
            this.label_receive.Location = new System.Drawing.Point(22, 55);
            this.label_receive.Name = "label_receive";
            this.label_receive.Size = new System.Drawing.Size(31, 15);
            this.label_receive.TabIndex = 20;
            this.label_receive.Text = "收到";
            // 
            // button_stop
            // 
            this.button_stop.Location = new System.Drawing.Point(399, 110);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(124, 92);
            this.button_stop.TabIndex = 19;
            this.button_stop.Text = "關閉";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // button_connect
            // 
            this.button_connect.Location = new System.Drawing.Point(252, 110);
            this.button_connect.Name = "button_connect";
            this.button_connect.Size = new System.Drawing.Size(124, 92);
            this.button_connect.TabIndex = 18;
            this.button_connect.Text = "連線";
            this.button_connect.UseVisualStyleBackColor = true;
            this.button_connect.Click += new System.EventHandler(this.button_connect_Click);
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(108, 23);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(415, 23);
            this.port.TabIndex = 14;
            // 
            // label_port
            // 
            this.label_port.AutoSize = true;
            this.label_port.Location = new System.Drawing.Point(22, 26);
            this.label_port.Name = "label_port";
            this.label_port.Size = new System.Drawing.Size(31, 15);
            this.label_port.TabIndex = 13;
            this.label_port.Text = "port";
            // 
            // panel_connect
            // 
            this.panel_connect.Location = new System.Drawing.Point(545, 36);
            this.panel_connect.Name = "panel_connect";
            this.panel_connect.Size = new System.Drawing.Size(229, 183);
            this.panel_connect.TabIndex = 22;
            // 
            // Socket_Server
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 248);
            this.Controls.Add(this.panel_connect);
            this.Controls.Add(this.receive);
            this.Controls.Add(this.label_receive);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_connect);
            this.Controls.Add(this.port);
            this.Controls.Add(this.label_port);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Socket_Server";
            this.Text = "Socket Server";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Socket_Server_FormClosed);
            this.Load += new System.EventHandler(this.Socket_Server_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox receive;
        private Label label_receive;
        private Button button_stop;
        private Button button_connect;
        private TextBox port;
        private Label label_port;
        private Panel panel_connect;
    }
}