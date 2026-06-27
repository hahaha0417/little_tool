using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Threading;
using hahahalib;
using System;
using System.Diagnostics;
using System.Text;

namespace hahaha
{
    public partial class client : Form
    {
        Socket socket_listener_;
        int max_length = 1024;

        public client()
        {
            InitializeComponent();

            panel_light.BackColor = Color.Red;
            panel_client.BackColor = Color.FromArgb(255, 200, 200);
        }

        private void client_Load(object sender, EventArgs e)
        {
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String ip_ = ini.ReadINI("client", "ip");
            String port_ = ini.ReadINI("client", "port");
            // ------------------------------------------------------------------ 

            text_box_ip.Text = ip_;
            text_box_port.Text = port_;
        }

        private void client_FormClosed(object sender, FormClosedEventArgs e)
        {
            String ip_ = text_box_ip.Text;
            String port_ = text_box_port.Text;
            
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            ini.WriteINI("client", "ip", ip_);
            ini.WriteINI("client", "port", port_);
            // ------------------------------------------------------------------ 
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            ConnectServer();
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            socket_listener_.Shutdown(SocketShutdown.Both);
            
            // socket_listener_.Disconnect(false);
            socket_listener_.Close();
            // socket_listener_.Dispose();

        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (socket_listener_.Connected)
            {
                SckSSend();
            }
            
        }

        // 連線

        private void ConnectServer()
        {
            String ip_ = text_box_ip.Text;
            String port_ = text_box_port.Text;
            try
            {
                socket_listener_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket_listener_.Connect(new IPEndPoint(IPAddress.Parse(ip_), Int32.Parse(port_)));

                // RmIp和SPort分別為string和int型態, 前者為Server端的IP, 後者為Server端的Port
                // 同 Server 端一樣要另外開一個執行緒用來等待接收來自 Server 端傳來的資料, 與Server概念同

                Thread SckSReceiveTd = new Thread(socket_listener_receive_proc);
                SckSReceiveTd.Start();

            }
            catch { }

        }

        private void socket_listener_receive_proc()
        {
            try
            {
                long IntAcceptData;
                byte[] clientData = new byte[max_length];

                while (true && socket_listener_.Connected)
                {
                    // 程式會被 hand 在此, 等待接收來自 Server 端傳來的資料
                    IntAcceptData = socket_listener_.Receive(clientData);

                    // 往下就自己寫接收到來自Server端的資料後要做什麼事唄~^^”
                    string S = Encoding.Default.GetString(clientData);
                    Console.WriteLine(S);
                    // https://tw.coderbridge.com/questions/183da228fcac4da481fb227d3c0e5b60
                    updateText(S, text_box_receive);
                }

            }
            catch
            {

            }
        }

        private delegate void UpdateUI(String str, Control ctl); //宣告委派

        private void updateText(String str, Control ctl)
        {
            if (this.InvokeRequired)
            {
                UpdateUI uu = new UpdateUI(updateText);
                this.Invoke(uu, str, ctl);
            }
            else
            {
                ctl.Text = str;
            }
        }


        // 當然 Client 端也可以傳送資料給Server端~ 和 Server 端的SckSSend一樣, 只差在Client端只有一個Socket
        private void SckSSend()
        {
            try
            {
                string SendS = text_box_send.Text.ToString();
                socket_listener_.Send(Encoding.ASCII.GetBytes(SendS));

            }
            catch
            {

            }

        }








    }
}