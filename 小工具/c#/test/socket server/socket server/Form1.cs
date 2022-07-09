using System.Net.Sockets;
using System.Net;
using System.Threading;
using hahahalib;
using System;
using System.Diagnostics;

namespace hahaha
{
    public partial class Socket_Server : Form
    {
        public static Socket Socket_Server_;
        public Thread My_Thread;
        public static bool Close = true;

        public Socket_Server()
        {
            InitializeComponent();

            panel_connect.BackColor = Color.Red;

        }

        private void Socket_Server_Load(object sender, EventArgs e)
        {
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String port_ = ini.ReadINI("socket server", "port");
            String send_ = ini.ReadINI("socket server", "send");
            // ------------------------------------------------------------------ 

            port.Text = port_;
            // send.Text = send_;
            
        }

        private void Socket_Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            String port_ = port.Text;
            // String send_ = send.Text;


            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            ini.WriteINI("socket server", "port", port_);
            // ini.WriteINI("socket server", "send", send_);
            // ------------------------------------------------------------------ 
        }

        private void button_send_Click(object sender, EventArgs e)
        {

        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            // https://www.796t.com/content/1549246517.html
            //伺服器IP地址
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String port_ = ini.ReadINI("socket server", "port");
            // String send_ = ini.ReadINI("socket server", "send");
            // ------------------------------------------------------------------ 
            Socket_Server_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            Socket_Server_.Bind(new IPEndPoint(ipAddress, Convert.ToInt32(port_)));  //繫結IP地址：埠
            Socket_Server_.Listen(10);    //設定最多10個排隊連線請求
            Console.WriteLine("啟動監聽{0}成功", Socket_Server_.LocalEndPoint.ToString());
            //通過Clientsoket傳送資料
            My_Thread = new Thread(ListenClientConnect);
            
            Close = false;
            My_Thread.Start();
            panel_connect.BackColor = Color.Green;
            // Console.ReadLine();
        }

        /// <summary>
        /// 監聽客戶端連線
        /// </summary>
        private static void ListenClientConnect()
        {
            while (true && !Close)
            {
                try
                {
                    Socket clientSocket;
                    clientSocket = Socket_Server_.Accept();
                    clientSocket.Send(System.Text.Encoding.ASCII.GetBytes("Server Say Hello"));
                    Thread receiveThread = new Thread(ReceiveMessage);
                    receiveThread.Start(clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // myClientSocket.Shutdown(SocketShutdown.Both);
                    // myClientSocket.Close();
                    break;
                }
            }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        /// <param name="clientSocket"></param>
        private static void ReceiveMessage(object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            while (true)
            {
                byte[] result = new byte[1024];
                try
                {
                    
                    //通過clientSocket接收資料
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("接收客戶端{0}訊息{1}", myClientSocket.RemoteEndPoint.ToString(), System.Text.Encoding.ASCII.GetString(result, 0, receiveNumber));
                    String rrr = myClientSocket.RemoteEndPoint.ToString();
                    int rr4r = 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
                //https://iter01.com/537311.html
                try
                {
                    // byte[] result = new byte[1024];
                    //通過clientSocket接收資料
                    int receiveNumber = myClientSocket.Send(result);
                    Console.WriteLine("接收客戶端{0}訊息{1}", myClientSocket.RemoteEndPoint.ToString(), System.Text.Encoding.ASCII.GetString(result, 0, receiveNumber));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            /*
            Close = true;
 ;
            Socket_Server_.Close();

            panel_connect.BackColor = Color.Red;
            */

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//地址型別,流，協議型別
            //192.168.43.76 本機ip：127.0.0.1

            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipendpoint = new IPEndPoint(ipaddress, 8000);

            serverSocket.Bind(ipendpoint);//繫結完成

            serverSocket.Listen(10);//處理連結佇列個數 為0則為不限制


            Socket clientSocket = serverSocket.Accept();//接收一個客戶端連結

            ///向客戶端傳送一條訊息
            string msg = "Hello client!";
            byte[] date = System.Text.Encoding.UTF8.GetBytes(msg);//轉換成為bytes陣列
            clientSocket.Send(date);

            ///接收一條客戶端的訊息
            byte[] dateBuffer = new byte[1024];


            int count = clientSocket.Receive(dateBuffer);

            string msgReceive = System.Text.Encoding.UTF8.GetString(dateBuffer, 0, count);
            //Console.WriteLine(msgReceive);

            //Console.ReadKey();

            clientSocket.Close();
            serverSocket.Close();
        }
    }
}