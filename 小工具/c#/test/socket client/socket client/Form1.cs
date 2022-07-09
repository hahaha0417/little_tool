using System.Net.Sockets;
using System.Net;
using System.Threading;
using hahahalib;
using System;
using System.Diagnostics;

namespace hahaha
{
    public partial class Socket_Client : Form
    {

        static Socket sender_;
        static IPEndPoint remoteEP;
        public Socket_Client()
        {
            InitializeComponent();

            panel_connect.BackColor = Color.Red;
        }

        private void Socket_Server_Load(object sender, EventArgs e)
        {
            
        }

        private void Socket_Server_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }

        private void Socket_Client_Load(object sender, EventArgs e)
        {
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String ip_ = ini.ReadINI("socket client", "ip");
            String port_ = ini.ReadINI("socket client", "port");
            String send_ = ini.ReadINI("socket client", "send");
            // ------------------------------------------------------------------ 

            ip.Text = ip_;
            port.Text = port_;
            send.Text = send_;
        }

        private void Socket_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            String ip_ = ip.Text;
            String port_ = port.Text;
            String send_ = send.Text;

            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            ini.WriteINI("socket client", "ip", ip_);
            ini.WriteINI("socket client", "port", port_);
            ini.WriteINI("socket client", "send", send_);
            // ------------------------------------------------------------------ 
            panel_connect.BackColor = Color.Red;
        }

        private void button_connect_Click(object sender, EventArgs e)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

            // Connect to a remote device.  
            try
            {
                String ip_ = ip.Text;
                String port_ = port.Text;
                String send_ = send.Text;

                // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
                // ------------------------------------------------------------------ 
                String path = Directory.GetCurrentDirectory();

                path += "\\option.ini";
                ReadWriteINIfile ini = new ReadWriteINIfile(path);

                ini.WriteINI("socket client", "ip", ip_);
                ini.WriteINI("socket client", "port", port_);
                ini.WriteINI("socket client", "send", send_);
                // ------------------------------------------------------------------ 
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
                remoteEP = new IPEndPoint(ipAddress, Convert.ToInt32(port_));

                // Create a TCP/IP  socket.  
                sender_ = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

   
                IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                // IPAddress ipAddress = IPAddress.Parse(ip_);
                sender_.Connect(ipAddress, Convert.ToInt32(port_));

                Console.WriteLine("Socket connected to {0}",
                    sender_.RemoteEndPoint.ToString());

                panel_connect.BackColor = Color.Green;

            }            
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }
            
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            /*
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
            

                // Release the socket.  
                sender_.Shutdown(SocketShutdown.Both);
                sender_.Close();
                

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            
            catch (Exception ee)
            {
                Console.WriteLine("Unexpected exception : {0}", ee.ToString());
            }
            panel_connect.BackColor = Color.Red;
            */
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000));

            byte[] date = new byte[1024];
            int count = clientSocket.Receive(date);
            string msg = System.Text.Encoding.UTF8.GetString(date, 0, count);
            Console.WriteLine(msg);

            string s = Console.ReadLine();

            clientSocket.Send(System.Text.Encoding.UTF8.GetBytes("fffffffdsdfs"));

            // Console.ReadKey();
            clientSocket.Close();
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];
            // Connect the socket to the remote endpoint. Catch any errors.  
            try
            {
                

                // Encode the data string into a byte array.  
                byte[] msg = System.Text.Encoding.ASCII.GetBytes("This is a test<EOF>");

                // Send the data through the socket.  
                int bytesSent = sender_.Send(msg);

                // Receive the response from the remote device.  
                int bytesRec = sender_.Receive(bytes);
                Console.WriteLine("Echoed test = {0}",
                    System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRec));

      

            }
            catch (ArgumentNullException ane)
            {
                Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException : {0}", se.ToString());
            }
            /*
            catch (Exception e)
            {
                Console.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            */
        }

        public void StartClient()
        {
            
        }
    }
}