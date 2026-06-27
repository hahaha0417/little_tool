using System;
using System.Diagnostics;
namespace hahaha
{
    public partial class monitor_form : Form
    {
        private readonly Func<Process?> processAccessor_;
        private readonly Func<ProcessPriorityClass> desiredPriorityAccessor_;
        private readonly Action<ProcessPriorityClass> desiredPrioritySetter_;
        private readonly Func<long?> desiredAffinityAccessor_;
        private readonly Action<long?> desiredAffinitySetter_;
        private Process? trackedProcess_;
        private TimeSpan previousCpuTime_ = TimeSpan.Zero;
        private DateTime previousSampleTime_ = DateTime.MinValue;
        private bool updatingAffinityUi_;
        private bool updatingPriorityUi_;
        private readonly string[] cpuLabels_;

        public monitor_form(
            Func<Process?> processAccessor,
            Func<ProcessPriorityClass> desiredPriorityAccessor,
            Action<ProcessPriorityClass> desiredPrioritySetter,
            Func<long?> desiredAffinityAccessor,
            Action<long?> desiredAffinitySetter)
        {
            processAccessor_ = processAccessor;
            desiredPriorityAccessor_ = desiredPriorityAccessor;
            desiredPrioritySetter_ = desiredPrioritySetter;
            desiredAffinityAccessor_ = desiredAffinityAccessor;
            desiredAffinitySetter_ = desiredAffinitySetter;
            cpuLabels_ = CpuTopologyDetector.GetLogicalCpuLabels();
            InitializeComponent();
            InitializePriorityOptions();
            InitializeAffinityOptions();
        }

        private void monitor_form_Load(object? sender, EventArgs e)
        {
            timer_refresh.Start();
            RefreshProcessStats();
            RefreshProcessSettings();
        }

        private void timer_refresh_Tick(object? sender, EventArgs e)
        {
            RefreshProcessStats();
        }

        private void button_refresh_Click(object? sender, EventArgs e)
        {
            RefreshFromCurrentProcess();
        }

        private void button_apply_Click(object? sender, EventArgs e)
        {
            try
            {
                ApplySelectedSettings();
                RefreshProcessStats();
                RefreshProcessSettings();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"套用失敗: {ex.Message}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void monitor_form_FormClosed(object? sender, FormClosedEventArgs e)
        {
            timer_refresh.Stop();
        }

        private void RefreshProcessStats()
        {
            Process? process = processAccessor_();
            if (process is null)
            {
                trackedProcess_ = null;
                previousCpuTime_ = TimeSpan.Zero;
                previousSampleTime_ = DateTime.MinValue;
                label_status_value.Text = "未執行";
                label_pid_value.Text = "-";
                label_cpu_value.Text = "-";
                button_apply.Enabled = true;
                return;
            }

            button_apply.Enabled = true;
            process.Refresh();

            if (trackedProcess_ is null || trackedProcess_.Id != process.Id)
            {
                trackedProcess_ = process;
                previousCpuTime_ = process.TotalProcessorTime;
                previousSampleTime_ = DateTime.UtcNow;
            }

            label_status_value.Text = process.Responding ? "執行中" : "背景執行中";
            label_pid_value.Text = process.Id.ToString();
            label_cpu_value.Text = $"{CalculateCpuUsage(process):0.0}%";
        }

        private void RefreshProcessSettings()
        {
            Process? process = processAccessor_();
            if (process is null)
            {
                SetPrioritySelection(desiredPriorityAccessor_().ToString());
                SetAffinityFromMask(desiredAffinityAccessor_() ?? 0);
                button_apply.Enabled = true;
                return;
            }

            button_apply.Enabled = true;
            process.Refresh();
            SetPrioritySelection(process.PriorityClass.ToString());
            SetAffinityFromMask((long)process.ProcessorAffinity);
        }

        private double CalculateCpuUsage(Process process)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan totalCpu = process.TotalProcessorTime;

            if (previousSampleTime_ == DateTime.MinValue)
            {
                previousSampleTime_ = now;
                previousCpuTime_ = totalCpu;
                return 0;
            }

            double elapsedMs = (now - previousSampleTime_).TotalMilliseconds;
            double cpuMs = (totalCpu - previousCpuTime_).TotalMilliseconds;

            previousSampleTime_ = now;
            previousCpuTime_ = totalCpu;

            if (elapsedMs <= 0)
            {
                return 0;
            }

            return Math.Max(0, cpuMs / (elapsedMs * Environment.ProcessorCount) * 100.0);
        }

        private void InitializePriorityOptions()
        {
            combo_priority.Items.AddRange(new object[]
            {
                ProcessPriorityClass.Idle.ToString(),
                ProcessPriorityClass.BelowNormal.ToString(),
                ProcessPriorityClass.Normal.ToString(),
                ProcessPriorityClass.AboveNormal.ToString(),
                ProcessPriorityClass.High.ToString(),
                ProcessPriorityClass.RealTime.ToString()
            });

            combo_priority.DropDownStyle = ComboBoxStyle.DropDownList;
            SetPrioritySelection(desiredPriorityAccessor_().ToString());
        }

        private void InitializeAffinityOptions()
        {
            checkedListBox_affinity.Items.Clear();
            for (int i = 0; i < cpuLabels_.Length; i++)
            {
                checkedListBox_affinity.Items.Add(cpuLabels_[i]);
            }
        }

        private void SetAffinityFromMask(long mask)
        {
            updatingAffinityUi_ = true;
            try
            {
                for (int i = 0; i < checkedListBox_affinity.Items.Count; i++)
                {
                    bool isChecked = mask != 0 && (mask & (1L << i)) != 0;
                    checkedListBox_affinity.SetItemChecked(i, isChecked);
                }
            }
            finally
            {
                updatingAffinityUi_ = false;
            }
        }

        private void SetPrioritySelection(string priorityText)
        {
            updatingPriorityUi_ = true;
            try
            {
                combo_priority.SelectedItem = priorityText;
            }
            finally
            {
                updatingPriorityUi_ = false;
            }
        }

        private long? BuildAffinityMask()
        {
            long mask = 0;
            for (int i = 0; i < checkedListBox_affinity.CheckedIndices.Count; i++)
            {
                int cpuIndex = checkedListBox_affinity.CheckedIndices[i];
                mask |= 1L << cpuIndex;
            }

            return mask == 0 ? null : mask;
        }

        private ProcessPriorityClass ParsePriority(string priorityText)
        {
            return (ProcessPriorityClass)Enum.Parse(typeof(ProcessPriorityClass), priorityText);
        }

        private void ApplySelectedSettings()
        {
            ProcessPriorityClass priorityClass = ParsePriority(combo_priority.Text);
            long? affinityMask = BuildAffinityMask();

            desiredPrioritySetter_(priorityClass);
            desiredAffinitySetter_(affinityMask);
        }

        private void RefreshFromCurrentProcess()
        {
            RefreshProcessStats();

            Process? process = processAccessor_();
            if (process is null)
            {
                RefreshProcessSettings();
                return;
            }

            process.Refresh();

            ProcessPriorityClass priorityClass = process.PriorityClass;
            long? affinityMask = (long)process.ProcessorAffinity;

            desiredPrioritySetter_(priorityClass);
            desiredAffinitySetter_(affinityMask);

            RefreshProcessSettings();
        }

        private void combo_priority_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (updatingPriorityUi_)
            {
                return;
            }
        }

        private void checkedListBox_affinity_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void checkedListBox_affinity_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (updatingAffinityUi_)
            {
                return;
            }

            // 正要取消勾選
            if (e.CurrentValue == CheckState.Checked &&
                e.NewValue == CheckState.Unchecked &&
                checkedListBox_affinity.CheckedItems.Count == 1)
            {
                // 保持勾選
                e.NewValue = CheckState.Checked;
            }
        }
    }
}
