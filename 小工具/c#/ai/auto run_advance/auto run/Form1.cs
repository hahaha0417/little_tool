using hahahalib;
using System;
using System.Diagnostics;

namespace hahaha
{
    public partial class form_auto_run : Form
    {
        private bool running_ = false;
        private Process? currentProcess_;
        private monitor_form? monitorForm_;
        private ProcessPriorityClass desiredPriorityClass_ = ProcessPriorityClass.Normal;
        private long? desiredAffinityMask_;

        public form_auto_run()
        {
            InitializeComponent();

            panel_autoload.BackColor = Color.Red;
            button_run.Enabled = true;
            button_close.Enabled = false;
        }

        private void form_auto_run_Load(object? sender, EventArgs e)
        {
            ReadWriteINIfile ini = OpenIniFile();

            string command_ = ini.ReadINI("auto_run", "command");
            string parameter_ = ini.ReadINI("auto_run", "parameter");
            string check_autoload_ = ini.ReadINI("auto_run", "check_autoload");
            string priority_ = ini.ReadINI("auto_run", "priority");
            string affinity_ = ini.ReadINI("auto_run", "affinity");

            command.Text = command_;
            parameter.Text = parameter_;
            check_autoload.Checked = check_autoload_ == "true";
            desiredPriorityClass_ = ParsePriorityOrDefault(priority_);
            desiredAffinityMask_ = ParseAffinityOrNull(affinity_);
        }

        private void form_auto_run_FormClosed(object? sender, FormClosedEventArgs e)
        {
            string command_ = command.Text;
            string parameter_ = parameter.Text;
            bool check_autoload_ = check_autoload.Checked;
            ReadWriteINIfile ini = OpenIniFile();

            ini.WriteINI("auto_run", "command", command_);
            ini.WriteINI("auto_run", "parameter", parameter_);
            ini.WriteINI("auto_run", "check_autoload", check_autoload_ ? "true" : "false");
            ini.WriteINI("auto_run", "priority", desiredPriorityClass_.ToString());
            ini.WriteINI("auto_run", "affinity", FormatAffinity(desiredAffinityMask_));
        }

        private void button_run_Click(object? sender, EventArgs e)
        {
            try
            {
                Process process = StartManagedProcess(command.Text, parameter.Text);
                running_ = true;
                SetRunningState(true);
                button_run.Enabled = false;
                button_close.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"啟動失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void myProcess_Exited(object? sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => myProcess_Exited(sender, e)));
                return;
            }

            SetRunningState(false);

            if (sender is Process exitedProcess && currentProcess_?.Id == exitedProcess.Id)
            {
                currentProcess_ = null;
            }

            if (running_ && check_autoload.Checked)
            {
                try
                {
                    Process process = StartManagedProcess(command.Text, parameter.Text);
                    SetRunningState(true);
                }
                catch (Exception ex)
                {
                    running_ = false;
                    MessageBox.Show($"自動重啟失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button_close_Click(object? sender, EventArgs e)
        {
            running_ = false;
            SetRunningState(false);
            button_run.Enabled = true;
            button_close.Enabled = false;
        }

        private void button_monitor_Click(object? sender, EventArgs e)
        {
            if (monitorForm_ is not null && !monitorForm_.IsDisposed)
            {
                monitorForm_.BringToFront();
                monitorForm_.Focus();
                return;
            }

            monitorForm_ = new monitor_form(
                GetCurrentProcess,
                GetDesiredPriorityClass,
                SetDesiredPriorityClass,
                GetDesiredAffinityMask,
                SetDesiredAffinityMask);
            monitorForm_.Show(this);
        }

        private Process? GetCurrentProcess()
        {
            try
            {
                if (currentProcess_ is null)
                {
                    return null;
                }

                currentProcess_.Refresh();
                return currentProcess_.HasExited ? null : currentProcess_;
            }
            catch
            {
                return null;
            }
        }

        private Process CreateProcess(string commandText, string parameterText)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = commandText,
                    Arguments = parameterText,
                    UseShellExecute = true,
                    CreateNoWindow = false,
                },
                EnableRaisingEvents = true,
            };

            process.Exited += myProcess_Exited;
            return process;
        }

        private Process StartManagedProcess(string commandText, string parameterText)
        {
            Process process = CreateProcess(commandText, parameterText);

            process.Start();
            currentProcess_ = process;
            ApplyDesiredSettings(process);
            return process;
        }

        private void ApplyDesiredSettings(Process process)
        {
            process.PriorityClass = desiredPriorityClass_;

            if (desiredAffinityMask_ is not null)
            {
                process.ProcessorAffinity = (IntPtr)desiredAffinityMask_.Value;
            }

            process.Refresh();
        }

        private ProcessPriorityClass GetDesiredPriorityClass()
        {
            return desiredPriorityClass_;
        }

        private void SetDesiredPriorityClass(ProcessPriorityClass priorityClass)
        {
            desiredPriorityClass_ = priorityClass;

            Process? process = GetCurrentProcess();
            if (process is null)
            {
                return;
            }

            process.PriorityClass = priorityClass;
            process.Refresh();
        }

        private long? GetDesiredAffinityMask()
        {
            return desiredAffinityMask_;
        }

        private void SetDesiredAffinityMask(long? affinityMask)
        {
            desiredAffinityMask_ = affinityMask;

            Process? process = GetCurrentProcess();
            if (process is null || affinityMask is null)
            {
                return;
            }

            process.ProcessorAffinity = (IntPtr)affinityMask.Value;
            process.Refresh();
        }

        private void SetRunningState(bool isRunning)
        {
            panel_autoload.BackColor = isRunning ? Color.Green : Color.Red;
        }

        private ReadWriteINIfile OpenIniFile()
        {
            // Keep compatibility with the project's existing current-directory-based option.ini behavior.
            string path = Path.Combine(Directory.GetCurrentDirectory(), "option.ini");
            return new ReadWriteINIfile(path);
        }

        private ProcessPriorityClass ParsePriorityOrDefault(string priorityText)
        {
            return Enum.TryParse(priorityText, out ProcessPriorityClass priorityClass)
                ? priorityClass
                : ProcessPriorityClass.Normal;
        }

        private long? ParseAffinityOrNull(string affinityText)
        {
            if (string.IsNullOrWhiteSpace(affinityText))
            {
                return null;
            }

            return long.TryParse(affinityText, out long affinityMask) && affinityMask > 0
                ? affinityMask
                : null;
        }

        private string FormatAffinity(long? affinityMask)
        {
            return affinityMask?.ToString() ?? string.Empty;
        }
    }
}
