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
        //�إߤ@�Ӻ�ť��Socket
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

        // ��ť
        private void Listen()
        {
            // https://keeppracticing.pixnet.net/blog/post/40203548

            // �� Resize ���覡�ʺA�W�[ Socket ���ƥ�
            Array.Resize(ref socket_listener_, 1);

            socket_listener_[0] = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            string local_ip = "127.0.0.1";
            socket_listener_[0].Bind(new IPEndPoint(IPAddress.Parse(local_ip), Int32.Parse(text_box_port.Text)));


            // �䤤 local_ip �M port ���O�� string �M int ���A, �e�̬� Server �ݪ�IP, ��̬�S erver �ݪ�Port
            socket_listener_[0].Listen(10); // �i���ť; Listen( )�����\ Client �P�ɳs�u���̤j��
            socket_listener_wait_accept();   // �t�~�g�@�Ө�ƥΨӤ��t Client �ݪ� Socket

        }

        // ����Client�s�u

        private void socket_listener_wait_accept()
        {
            // �P�_�ثe�O�_���Ū� Socket �i�H���ѵ�Client�ݳs�u
            bool FlagFinded = false;

            for (int i = 1; i < socket_listener_.Length; i++)
            {
                // SckSs[i] �Y���� null ��ܤw�Q��@�L, �P�_�O�_�� Client �ݳs�u
                if (socket_listener_[i] != null)
                {
                    // �p�G�ثe�� i �� Socket �Y�S���H�s�u, �K�i���ѵ��U�@�� Client �i��s�u
                    if (socket_listener_[i].Connected == false)
                    {
                        socket_listener_index_ = i;
                        FlagFinded = true;
                        break;

                    }
                }
            }

            // �p�G FlagFinded �� false ��ܥثe�èS���h�l�� Socket �i�� Client �s�u
            if (FlagFinded == false)
            {
                // �W�[ Socket ���ƥإH�ѤU�@�� Client �ݶi��s�u
                socket_listener_index_ = socket_listener_.Length;
                Array.Resize(ref socket_listener_, socket_listener_index_ + 1);

            }



            // �H�U��欰�h��������g�k, �]�����U�� Server �ݪ������n�ϥ� Accept() �� Cleint �i��s�u;
            // �Ӱ�������ݭn�ɦA���ͧY�i, �]���w�q���ϰ�ʪ� Thread. �R�W�� SckSAcceptTd;
            // �b new Thread( ) �̬��n�h������h���檺���. �o�̩R�W�� SckSAcceptProc;

            Thread SckSAcceptTd = new Thread(socket_listener_accept_proc);
            SckSAcceptTd.Start();  // �}�l���� SckSAcceptTd �o�Ӱ����

            // �o�̭n�I�X SckSacceptTd �o�Ӱ�����|�b Start( ) ����}�l���� SckSAcceptProc �̪��{���X, �P�ɥD�{����������]�|�~�򩹤U����U���U��. 
            // �D�{�����ε��� SckSAcceptProc ���{���X���槹�K�|�~�򩹤U����.

        }

        // �����Ӧ�Client���s�u�PClient�ǨӪ����
        private void socket_listener_accept_proc()
        {
            // �o�̥[�J try �O�]�� SckSs[0] �Y�Q Close ����, SckSs[0].Accept() �|���Ϳ��~
            try
            {
                socket_listener_[socket_listener_index_] = socket_listener_[0].Accept();  // ����Client �ݳs�u
                // ������ Accept �����n�Φh�����, �]�� SckSs[0] �|���b�o�@��{���X���즳 Client �ݳs�W�u, �ä��t�� SckSs[SckCIndex] �� Client �s�u����{���~�|�~�򩹤U, �Y�O�N Accept �g�b�D�������, �b�S��Client�s�W�Ӥ��e, �D�{���N�|�Qhand�b�o�@��L�k�A������ƤF!!
                // ��ӳo��ܦ� Client �s�W�u. �O���� Client ������ SckCIndex

                int Scki = socket_listener_index_;
                // �A���ͥt�@�Ӱ�������ݤU�@�� Client �s�u
                socket_listener_wait_accept();

                long IntAcceptData;
                byte[] clientData = new byte[max_length];  // �䤤RDataLen���C���n�����Ӧ� Client �ǨӪ���ƪ���

                while (true && socket_listener_[Scki].Connected)
                {
                    // �{���|�Q hand �b��, ���ݱ����Ӧ� Client �ݶǨӪ����
                    IntAcceptData = socket_listener_[Scki].Receive(clientData);

                    // ���U�N�ۤv�g������Ӧ�Client�ݪ���ƫ�n���������~^^��
                    // �]��Client�ݶ�ABCDE�L��, �ҥH�i�H�յ۱NByte�}�C�ন�r��C�L�X�Ӭݬ�~

                    string S = Encoding.Default.GetString(clientData);
                    Console.WriteLine(S);
                    // https://tw.coderbridge.com/questions/183da228fcac4da481fb227d3c0e5b60
                    updateText(S, text_box_receive);
                }
            }
            catch
            {
                // �o�̭Y�X���D�n�O�Ӧ� SckSs[Scki] �X���D, �i��O�ۤv Close, �]�i��O Client �_�u, �ۤv�[�P�_�a~

            }

        }

        private delegate void UpdateUI(String str, Control ctl); //�ŧi�e��

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

        // Server �ǰe��Ƶ��Ҧ�Client

        private void socket_listener_Send()
        {
            for (int Scki = 1; Scki < socket_listener_.Length; Scki++)
            {
                if (null != socket_listener_[Scki] && socket_listener_[Scki].Connected == true)
                {
                    try
                    {
                        string SendS = text_box_send.Text.ToString();      // SendS �b�o�̬� string ���A, �� Server �n�ǵ� Client ���r��, �ڴ��նǰe �r�� "ABCDE" ��Client��
                        socket_listener_[Scki].Send(Encoding.ASCII.GetBytes(SendS));

                    }
                    catch
                    {
                        // �o�̥X��, �D�n�O�X�b SckSs[Scki] �X���D, �ۤv�[�P�_�a~

                    }
                }
            }
        }
    }
}