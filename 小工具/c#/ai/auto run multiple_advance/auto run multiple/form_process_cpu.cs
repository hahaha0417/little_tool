using System.Diagnostics;

namespace hahaha
{
    public partial class form_process_cpu : Form
    {
        private hahaha_setting_system_item Item_;
        private int Last_Process_Id_ = -1;
        private TimeSpan Last_Total_Processor_Time_ = TimeSpan.Zero;
        private DateTime Last_Sample_Time_ = DateTime.MinValue;
        private bool Is_Loading_ = true;

        public form_process_cpu(hahaha_setting_system_item item)
        {
            Item_ = item;
            InitializeComponent();
        }

        private void form_process_cpu_Load(object sender, EventArgs e)
        {
            combo_priority.Items.AddRange(hahaha_process_cpu.Priority_Options);
            Build_Affinity_List();
            Reload_Current_Item();
            timer_cpu.Interval = 1000;
            timer_cpu.Start();
            Refresh_Process_State();
            Is_Loading_ = false;
        }

        private void form_process_cpu_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer_cpu.Stop();
            Update_Current_Setting();
        }

        private void Build_Affinity_List()
        {
            checked_cpu.Items.Clear();
            for (int i = 0; i < hahaha_process_cpu.Max_Affinity_Bits; i++)
            {
                checked_cpu.Items.Add($"CPU {i}");
            }
        }

        public void Set_Item(hahaha_setting_system_item item)
        {
            Item_ = item;
            Reload_Current_Item();
            Refresh_Process_State();
        }

        private void Reload_Current_Item()
        {
            bool previousLoading = Is_Loading_;
            Is_Loading_ = true;
            Load_Item_Setting_Into_UI();
            Reset_Sample();
            Is_Loading_ = previousLoading;
        }

        private void Load_Item_Setting_Into_UI()
        {
            label_item_name_value.Text = string.IsNullOrWhiteSpace(Item_.Name) ? "(未命名)" : Item_.Name;

            string priority = Item_.Cpu_Priority;
            if (!combo_priority.Items.Contains(priority))
            {
                priority = nameof(ProcessPriorityClass.Normal);
            }
            combo_priority.SelectedItem = priority;

            Apply_Mask_To_Checked_List(hahaha_process_cpu.Normalize_Affinity_Mask(Item_.Cpu_Affinity_Mask));
        }

        private void Apply_Mask_To_Checked_List(long mask)
        {
            for (int i = 0; i < checked_cpu.Items.Count; i++)
            {
                bool enabled = (mask & (1L << i)) != 0;
                checked_cpu.SetItemChecked(i, enabled);
            }
        }

        private long Build_Selected_Mask()
        {
            long mask = 0;
            foreach (int index in checked_cpu.CheckedIndices)
            {
                mask |= 1L << index;
            }

            return hahaha_process_cpu.Normalize_Affinity_Mask(mask);
        }

        private Process? Get_Live_Process()
        {
            if (Item_.Process == null)
            {
                return null;
            }

            try
            {
                if (Item_.Process.HasExited)
                {
                    return null;
                }

                Item_.Process.Refresh();
                return Item_.Process;
            }
            catch
            {
                return null;
            }
        }

        private void Reset_Sample()
        {
            Last_Process_Id_ = -1;
            Last_Total_Processor_Time_ = TimeSpan.Zero;
            Last_Sample_Time_ = DateTime.MinValue;
        }

        private void Refresh_Process_State()
        {
            var process = Get_Live_Process();
            if (process == null)
            {
                label_process_id_value.Text = "-";
                label_status_value.Text = Item_.Running ? "等待啟動 / 已停止" : "未執行";
                label_cpu_value.Text = "0.00 %";
                button_apply_live.Enabled = false;
                Reset_Sample();
                return;
            }

            label_process_id_value.Text = process.Id.ToString();
            label_status_value.Text = "執行中";
            button_apply_live.Enabled = true;

            try
            {
                string currentPriority = process.PriorityClass.ToString();
                if (!Is_Loading_ && combo_priority.SelectedItem == null)
                {
                    combo_priority.SelectedItem = currentPriority;
                }

                long currentMask = (long)process.ProcessorAffinity;
                if (!Is_Loading_ && checked_cpu.CheckedIndices.Count == 0)
                {
                    Apply_Mask_To_Checked_List(hahaha_process_cpu.Normalize_Affinity_Mask(currentMask));
                }
            }
            catch
            {
            }

            Update_Cpu_Usage(process);
        }

        private void Update_Cpu_Usage(Process process)
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan total = process.TotalProcessorTime;

            if (Last_Process_Id_ != process.Id || Last_Sample_Time_ == DateTime.MinValue)
            {
                Last_Process_Id_ = process.Id;
                Last_Total_Processor_Time_ = total;
                Last_Sample_Time_ = now;
                label_cpu_value.Text = "計算中...";
                return;
            }

            double elapsedMs = (now - Last_Sample_Time_).TotalMilliseconds;
            double cpuMs = (total - Last_Total_Processor_Time_).TotalMilliseconds;

            Last_Total_Processor_Time_ = total;
            Last_Sample_Time_ = now;

            if (elapsedMs <= 0)
            {
                label_cpu_value.Text = "0.00 %";
                return;
            }

            double usage = cpuMs / (elapsedMs * Environment.ProcessorCount) * 100.0;
            if (usage < 0)
            {
                usage = 0;
            }

            label_cpu_value.Text = $"{usage:F2} %";
        }

        private void timer_cpu_Tick(object sender, EventArgs e)
        {
            Refresh_Process_State();
        }

        private void button_apply_live_Click(object sender, EventArgs e)
        {
            Update_Current_Setting();
            int result = hahaha_process_cpu.Apply_Process_Settings(Item_);
            label_apply_result.Text = result == 0 ? "已先寫回此 item 的 CPU 設定，並套用到執行中 process" : "已先寫回此 item 的 CPU 設定，但無法套用到執行中 process";
            Refresh_Process_State();
        }

        private void Update_Current_Setting()
        {
            Item_.Cpu_Priority = combo_priority.SelectedItem?.ToString() ?? nameof(ProcessPriorityClass.Normal);
            Item_.Cpu_Affinity_Mask = Build_Selected_Mask();
        }

        private void combo_priority_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Is_Loading_)
            {
                return;
            }

            Update_Current_Setting();
            label_apply_result.Text = "此 item 的 CPU 設定已更新，關閉程式時才會寫入 setting";
        }

        private void checked_cpu_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (Is_Loading_)
            {
                return;
            }

            BeginInvoke(new Action(() =>
            {
                Update_Current_Setting();
                label_apply_result.Text = "此 item 的 CPU 設定已更新，關閉程式時才會寫入 setting";
            }));
        }
    }
}
