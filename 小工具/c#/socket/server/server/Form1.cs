using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.Reflection.Metadata;
using System.Data.Common;
using hahahalib;
using System;
using System.Diagnostics;
using System.Text;

namespace hahaha
{
    // https://keeppracticing.pixnet.net/blog/post/40203548
    public partial class server : Form
    {
        //建立一個監聽用Socket
        Socket[] socket_listener_;
        int socket_listener_index_ = 0;
        int max_length = 1024;

        public server()
        {
            InitializeComponent();

            panel_light.BackColor = Color.Red;
            panel_server.BackColor = Color.FromArgb(255, 200, 200);
        }

     

        private void server_Load(object sender, EventArgs e)
        {
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String port_ = ini.ReadINI("server", "port");
            // ------------------------------------------------------------------ 

            text_box_port.Text = port_;
        }

        private void server_FormClosed(object sender, FormClosedEventArgs e)
        {
            String port_ = text_box_port.Text;

            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            ini.WriteINI("server", "port", port_);
            // ------------------------------------------------------------------ 
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            Listen();
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            if (!socket_listener_[0].Connected)
            {
                return;
            }

            socket_listener_[0].Shutdown(SocketShutdown.Both);

            // socket_listener_[0].Disconnect(true);
            socket_listener_[0].Close();
            // socket_listener_[0].Dispose();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (socket_listener_[0].Connected)
            {
                socket_listener_Send();
            }
        }

        // 聆聽
        private void Listen()
        {
            // https://keeppracticing.pixnet.net/blog/post/40203548

            // 用 Resize 的方式動態增加 Socket 的數目
            Array.Resize(ref socket_listener_, 1);

            socket_listener_[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string local_ip = "127.0.0.1";
            socket_listener_[0].Bind(new IPEndPoint(IPAddress.Parse(local_ip), Int32.Parse(text_box_port.Text)));


            // 其中 local_ip 和 port 分別為 string 和 int 型態, 前者為 Server 端的IP, 後者為S erver 端的Port
            socket_listener_[0].Listen(10); // 進行聆聽; Listen( )為允許 Client 同時連線的最大數
            socket_listener_wait_accept();   // 另外寫一個函數用來分配 Client 端的 Socket

        }

        // 等待Client連線

        private void socket_listener_wait_accept()
        {
            // 判斷目前是否有空的 Socket 可以提供給Client端連線
            bool FlagFinded = false;

            for (int i = 1; i < socket_listener_.Length; i++)
            {
                // SckSs[i] 若不為 null 表示已被實作過, 判斷是否有 Client 端連線
                if (socket_listener_[i] != null)
                {
                    // 如果目前第 i 個 Socket 若沒有人連線, 便可提供給下一個 Client 進行連線
                    if (socket_listener_[i].Connected == false)
                    {
                        socket_listener_index_ = i;
                        FlagFinded = true;
                        break;

                    }
                }
            }

            // 如果 FlagFinded 為 false 表示目前並沒有多餘的 Socket 可供 Client 連線
            if (FlagFinded == false)
            {
                // 增加 Socket 的數目以供下一個 Client 端進行連線
                socket_listener_index_ = socket_listener_.Length;
                Array.Resize(ref socket_listener_, socket_listener_index_ + 1);

            }



            // 以下兩行為多執行緒的寫法, 因為接下來 Server 端的部份要使用 Accept() 讓 Cleint 進行連線;
            // 該執行緒有需要時再產生即可, 因此定義為區域性的 Thread. 命名為 SckSAcceptTd;
            // 在 new Thread( ) 裡為要多執行緒去執行的函數. 這裡命名為 SckSAcceptProc;

            Thread SckSAcceptTd = new Thread(socket_listener_accept_proc);
            SckSAcceptTd.Start();  // 開始執行 SckSAcceptTd 這個執行緒

            // 這裡要點出 SckSacceptTd 這個執行緒會在 Start( ) 之後開始執行 SckSAcceptProc 裡的程式碼, 同時主程式的執行緒也會繼續往下執行各做各的. 
            // 主程式不用等到 SckSAcceptProc 的程式碼執行完便會繼續往下執行.

        }

        // 接收來自Client的連線與Client傳來的資料
        private void socket_listener_accept_proc()
        {
            // 這裡加入 try 是因為 SckSs[0] 若被 Close 的話, SckSs[0].Accept() 會產生錯誤
            try
            {
                socket_listener_[socket_listener_index_] = socket_listener_[0].Accept();  // 等待Client 端連線
                // 為什麼 Accept 部份要用多執行緒, 因為 SckSs[0] 會停在這一行程式碼直到有 Client 端連上線, 並分配給 SckSs[SckCIndex] 給 Client 連線之後程式才會繼續往下, 若是將 Accept 寫在主執行緒裡, 在沒有Client連上來之前, 主程式將會被hand在這一行無法再做任何事了!!
                // 能來這表示有 Client 連上線. 記錄該 Client 對應的 SckCIndex

                int Scki = socket_listener_index_;
                // 再產生另一個執行緒等待下一個 Client 連線
                socket_listener_wait_accept();

                long IntAcceptData;
                byte[] clientData = new byte[max_length];  // 其中RDataLen為每次要接受來自 Client 傳來的資料長度

                while (true && socket_listener_[Scki].Connected)
                {
                    // 程式會被 hand 在此, 等待接收來自 Client 端傳來的資料
                    IntAcceptData = socket_listener_[Scki].Receive(clientData);

                    // 往下就自己寫接收到來自Client端的資料後要做什麼事唄~^^”
                    // 因為Client端傳ABCDE過來, 所以可以試著將Byte陣列轉成字串列印出來看看~

                    string S = Encoding.Default.GetString(clientData);
                    Console.WriteLine(S);
                    // https://tw.coderbridge.com/questions/183da228fcac4da481fb227d3c0e5b60
                    updateText(S, text_box_receive);
                }
            }
            catch
            {
                // 這裡若出錯主要是來自 SckSs[Scki] 出問題, 可能是自己 Close, 也可能是 Client 斷線, 自己加判斷吧~

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

        // Server 傳送資料給所有Client

        private void socket_listener_Send()
        {
            for (int Scki = 1; Scki < socket_listener_.Length; Scki++)
            {
                if (null != socket_listener_[Scki] && socket_listener_[Scki].Connected == true)
                {
                    try
                    {
                        string SendS = text_box_send.Text.ToString();      // SendS 在這裡為 string 型態, 為 Server 要傳給 Client 的字串, 我測試傳送 字串 "ABCDE" 給Client端
                        socket_listener_[Scki].Send(Encoding.ASCII.GetBytes(SendS));

                    }
                    catch
                    {
                        // 這裡出錯, 主要是出在 SckSs[Scki] 出問題, 自己加判斷吧~

                    }
                }
            }
        }
    }
}