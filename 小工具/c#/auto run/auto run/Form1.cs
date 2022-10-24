using hahahalib;
using System;
using System.Diagnostics;

namespace hahaha
{
    public partial class form_auto_run : Form
    {
        bool running_ = false;
        public form_auto_run()
        {
            InitializeComponent();

            panel_autoload.BackColor = Color.Red;
        }

        private void form_auto_run_Load(object sender, EventArgs e)
        {
            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            String command_ = ini.ReadINI("auto_run", "command");
            String parameter_ = ini.ReadINI("auto_run", "parameter");
            String check_autoload_ = ini.ReadINI("auto_run", "check_autoload");
            // ------------------------------------------------------------------ 

            command.Text = command_;
            parameter.Text = parameter_;
            check_autoload.Checked = (check_autoload_ == "true") ? true : false;

        }

        private void form_auto_run_FormClosed(object sender, FormClosedEventArgs e)
        {
            String command_ = command.Text;
            String parameter_ = parameter.Text;
            bool check_autoload_ = check_autoload.Checked;

            // https://www.delftstack.com/zh-tw/howto/csharp/how-to-get-current-folder-path-in-csharp/#c%23-%25E4%25BD%25BF%25E7%2594%25A8-currentdirectory-%25E5%25B1%25AC%25E6%2580%25A7%25E7%258D%25B2%25E5%258F%2596%25E7%2595%25B6%25E5%2589%258D%25E8%25B3%2587%25E6%2596%2599%25E5%25A4%25BE%25E8%25B7%25AF%25E5%25BE%2591
            // ------------------------------------------------------------------ 
            String path = Directory.GetCurrentDirectory();

            path += "\\option.ini";
            ReadWriteINIfile ini = new ReadWriteINIfile(path);

            ini.WriteINI("auto_run", "command", command_);
            ini.WriteINI("auto_run", "parameter", parameter_);
            ini.WriteINI("auto_run", "check_autoload", (check_autoload_ ? "true" : "false"));
            // ------------------------------------------------------------------ 

        }

        private void button_run_Click(object sender, EventArgs e)
        {
            String command_ = command.Text;
            String parameter_ = parameter.Text;
            bool check_autoload_ = check_autoload.Checked;

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command_,
                    Arguments = parameter_,
                    UseShellExecute = true,
                    RedirectStandardOutput = false,
                    CreateNoWindow = false,
                }
            };
            // https://docs.microsoft.com/zh-tw/dotnet/api/system.diagnostics.process.exited?view=net-6.0
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(myProcess_Exited);

            process.Start();
            running_ = true;
            panel_autoload.BackColor = Color.Green;
        }

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            String command_ = command.Text;
            String parameter_ = parameter.Text;
            bool check_autoload_ = check_autoload.Checked;

            panel_autoload.BackColor = Color.Red;
            if (running_ && check_autoload_)
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = command_,
                        Arguments = parameter_,
                        UseShellExecute = true,
                        RedirectStandardOutput = false,
                        CreateNoWindow = false,
                    }
                };
                // https://docs.microsoft.com/zh-tw/dotnet/api/system.diagnostics.process.exited?view=net-6.0
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(myProcess_Exited);

                process.Start();
                panel_autoload.BackColor = Color.Green;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            running_ = false;
            panel_autoload.BackColor = Color.Red;
        }
    }
}