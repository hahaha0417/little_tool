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
            //���A��IP�a�}
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String port_ = ini.ReadINI("socket server", "port");
            // String send_ = ini.ReadINI("socket server", "send");
            // ------------------------------------------------------------------ 
            Socket_Server_ = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            Socket_Server_.Bind(new IPEndPoint(ipAddress, Convert.ToInt32(port_)));  //ô��IP�a�}�G��
            Socket_Server_.Listen(10);    //�]�w�̦h10�ӱƶ��s�u�ШD
            Console.WriteLine("�Ұʺ�ť{0}���\", Socket_Server_.LocalEndPoint.ToString());
            //�q�LClientsoket�ǰe���
            My_Thread = new Thread(ListenClientConnect);
            
            Close = false;
            My_Thread.Start();
            panel_connect.BackColor = Color.Green;
            // Console.ReadLine();
        }

        /// <summary>
        /// ��ť�Ȥ�ݳs�u
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
        /// �����T��
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
                    
                    //�q�LclientSocket�������
                    int receiveNumber = myClientSocket.Receive(result);
                    Console.WriteLine("�����Ȥ��{0}�T��{1}", myClientSocket.RemoteEndPoint.ToString(), System.Text.Encoding.ASCII.GetString(result, 0, receiveNumber));
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
                    //�q�LclientSocket�������
                    int receiveNumber = myClientSocket.Send(result);
                    Console.WriteLine("�����Ȥ��{0}�T��{1}", myClientSocket.RemoteEndPoint.ToString(), System.Text.Encoding.ASCII.GetString(result, 0, receiveNumber));
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

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//�a�}���O,�y�A��ĳ���O
            //192.168.43.76 ����ip�G127.0.0.1

            IPAddress ipaddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipendpoint = new IPEndPoint(ipaddress, 8000);

            serverSocket.Bind(ipendpoint);//ô������

            serverSocket.Listen(10);//�B�z�s����C�Ӽ� ��0�h��������


            Socket clientSocket = serverSocket.Accept();//�����@�ӫȤ�ݳs��

            ///�V�Ȥ�ݶǰe�@���T��
            string msg = "Hello client!";
            byte[] date = System.Text.Encoding.UTF8.GetBytes(msg);//�ഫ����bytes�}�C
            clientSocket.Send(date);

            ///�����@���Ȥ�ݪ��T��
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