using hahahalib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace hahaha
{
    public partial class form_auto_run_multiple : Form
    {
        public bool Is_Update_UI = false;
        private form_process_cpu? Form_Process_Cpu_;

        public form_auto_run_multiple()
        {
            InitializeComponent();

            panel_is_run.BackColor = Color.Red;
            Is_Update_UI = true;
            Update_UI();
            Is_Update_UI = false;

            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage4);
        }

        private hahaha_setting_system_item? Get_Selected_Item()
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx < 0 || itemIdx < 0)
            {
                return null;
            }

            return hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
        }

        private IEnumerable<hahaha_setting_system_item> Get_Selected_Items_For_Batch()
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    yield return items[itemIdx];
                    yield break;
                }

                foreach (var item in items)
                {
                    yield return item;
                }
                yield break;
            }

            foreach (var cls in hahaha.Setting_Box_!.Setting.Items)
            {
                foreach (var item in cls.Items)
                {
                    yield return item;
                }
            }
        }

        private static ProcessStartInfo Create_Process_Start_Info(hahaha_setting_system_item item)
        {
            if (item.Use_Shell_Excute)
            {
                return new ProcessStartInfo
                {
                    FileName = item.Command,
                    Arguments = item.Parameter,
                    UseShellExecute = true,
                    CreateNoWindow = item.Create_No_Window,
                };
            }

            return new ProcessStartInfo
            {
                FileName = item.Command,
                Arguments = item.Parameter,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = item.Create_No_Window,
            };
        }

        private bool Start_Item_Process(hahaha_setting_system_item item)
        {
            if (string.IsNullOrWhiteSpace(item.Command))
            {
                return false;
            }

            if (item.Process != null)
            {
                item.Process.Dispose();
            }

            var process = new Process
            {
                StartInfo = Create_Process_Start_Info(item),
                EnableRaisingEvents = true,
            };
            process.Exited += myProcess_Exited;
            process.Start();

            item.Process = process;
            item.Running = true;
            hahaha_process_cpu.Apply_Process_Settings(item);
            return true;
        }

        private void Stop_Item_Process(hahaha_setting_system_item item)
        {
            item.Running = false;
        }

        private void Kill_Item_Process(hahaha_setting_system_item item)
        {
            if (item.Process == null)
            {
                return;
            }

            try
            {
                item.Process.Kill();
            }
            catch
            {
            }
        }

        private void Open_Process_Cpu_Form(hahaha_setting_system_item item)
        {
            if (Form_Process_Cpu_ == null || Form_Process_Cpu_.IsDisposed)
            {
                Form_Process_Cpu_ = new form_process_cpu(item);
                Form_Process_Cpu_.FormClosed += (_, _) => Form_Process_Cpu_ = null;
                Form_Process_Cpu_.Show(this);
                return;
            }

            Form_Process_Cpu_.Set_Item(item);
            if (!Form_Process_Cpu_.Visible)
            {
                Form_Process_Cpu_.Show(this);
            }
            Form_Process_Cpu_.BringToFront();
            Form_Process_Cpu_.Activate();
        }

        private void Sync_Process_Cpu_Form_With_Selected_Item()
        {
            if (Form_Process_Cpu_ == null || Form_Process_Cpu_.IsDisposed)
            {
                return;
            }

            var item = Get_Selected_Item();
            if (item != null)
            {
                Form_Process_Cpu_.Set_Item(item);
            }
        }

        private void form_auto_run_Load(object sender, EventArgs e)
        {
        }

        private void form_auto_run_FormClosed(object sender, FormClosedEventArgs e)
        {
            hahaha.Setting_Box_?.Save_All();
            hahaha.Close();
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null && !item.Running && Start_Item_Process(item))
            {
                panel_is_run.BackColor = Color.Green;
            }
        }

        public void myProcess_Exited(object? sender, EventArgs e)
        {
            var process = sender as Process;
            hahaha_setting_system_item? foundItem = null;

            foreach (var cls in hahaha.Setting_Box_!.Setting.Items)
            {
                foundItem = cls.Items.FirstOrDefault(item => item.Process == process);
                if (foundItem != null)
                {
                    break;
                }
            }

            if (foundItem == null)
            {
                return;
            }

            if (foundItem.Running && foundItem.Auto_Reload)
            {
                if (Start_Item_Process(foundItem))
                {
                    panel_is_run.BackColor = Color.Green;
                }
            }
            else
            {
                foundItem.Running = false;
                panel_is_run.BackColor = Color.Red;
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                Stop_Item_Process(item);
                panel_is_run.BackColor = Color.Red;
            }
        }

        private void button_add_class_Click(object sender, EventArgs e)
        {
            string newName = name_class.Text.Trim();
            if (!string.IsNullOrEmpty(newName))
            {
                var newClass = new hahaha_setting_system_class { Name = newName };
                hahaha.Setting_Box_!.Setting.Items.Add(newClass);
                box_class.Items.Add(newName);
                name_class.Clear();
            }
        }

        private void button_rename_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            string newName = name_class.Text.Trim();
            if (idx >= 0 && !string.IsNullOrEmpty(newName))
            {
                hahaha.Setting_Box_!.Setting.Items[idx].Name = newName;
                box_class.Items[idx] = newName;
                name_class.Clear();
            }
        }

        private void button_delete_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            if (idx >= 0)
            {
                hahaha.Setting_Box_!.Setting.Items.RemoveAt(idx);
                box_class.Items.RemoveAt(idx);
                name_class.Clear();
            }
        }

        private void button_up_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            if (idx > 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items;
                (items[idx - 1], items[idx]) = (items[idx], items[idx - 1]);
                (box_class.Items[idx - 1], box_class.Items[idx]) = (box_class.Items[idx], box_class.Items[idx - 1]);
                box_class.SelectedIndex = idx - 1;
            }
        }

        private void button_down_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            var items = hahaha.Setting_Box_!.Setting.Items;
            if (idx >= 0 && idx < items.Count - 1)
            {
                (items[idx + 1], items[idx]) = (items[idx], items[idx + 1]);
                (box_class.Items[idx + 1], box_class.Items[idx]) = (box_class.Items[idx], box_class.Items[idx + 1]);
                box_class.SelectedIndex = idx + 1;
            }
        }

        private void button_copy_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            if (idx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items;
                var original = items[idx];
                var copy = new hahaha_setting_system_class
                {
                    Name = original.Name + "_copy",
                    Items = original.Items.Select(item => new hahaha_setting_system_item
                    {
                        Name = item.Name,
                        Auto_Reload = item.Auto_Reload,
                        Use_Shell_Excute = item.Use_Shell_Excute,
                        Create_No_Window = item.Create_No_Window,
                        Cpu_Affinity_Mask = item.Cpu_Affinity_Mask,
                        Cpu_Priority = item.Cpu_Priority,
                        Command = item.Command,
                        Parameter = item.Parameter,
                    }).ToList()
                };
                items.Insert(idx + 1, copy);
                box_class.Items.Insert(idx + 1, copy.Name);
                box_class.SelectedIndex = idx + 1;
            }
        }

        private void box_class_SelectedIndexChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            box_item.Items.Clear();

            if (classIdx >= 0)
            {
                if (!Is_Update_UI)
                {
                    name_class.Text = hahaha.Setting_Box_!.Setting.Items[classIdx].Name;
                }

                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                foreach (var item in items)
                {
                    box_item.Items.Add(item.Name);
                }

                int itemIdx = box_item.SelectedIndex;
                if (itemIdx >= 0)
                {
                    var item = items[itemIdx];
                    command.Text = item.Command;
                    parameter.Text = item.Parameter;
                    check_box_auto_reload.Checked = item.Auto_Reload;
                    check_box_use_shell_excute.Checked = item.Use_Shell_Excute;
                    check_box_create_no_window.Checked = item.Create_No_Window;
                    name_item.Text = item.Name;
                }
                else
                {
                    command.Clear();
                    parameter.Clear();
                    check_box_auto_reload.Checked = false;
                    check_box_use_shell_excute.Checked = false;
                    check_box_create_no_window.Checked = false;
                    name_item.Clear();
                }
            }
            else
            {
                name_class.Clear();
                command.Clear();
                parameter.Clear();
                check_box_auto_reload.Checked = false;
                check_box_use_shell_excute.Checked = false;
                check_box_create_no_window.Checked = false;
                name_item.Clear();
            }
            panel_is_run.BackColor = Color.Red;
        }

        private void button_add_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            string newName = name_item.Text.Trim();
            if (classIdx >= 0 && !string.IsNullOrEmpty(newName))
            {
                var newItem = new hahaha_setting_system_item { Name = newName };
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                items.Add(newItem);
                box_item.Items.Add(newName);
                name_item.Clear();
            }
        }

        private void button_rename_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            string newName = name_item.Text.Trim();
            if (classIdx >= 0 && itemIdx >= 0 && !string.IsNullOrEmpty(newName))
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                items[itemIdx].Name = newName;
                box_item.Items[itemIdx] = newName;
                name_item.Clear();
            }
        }

        private void button_delete_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                items.RemoveAt(itemIdx);
                box_item.Items.RemoveAt(itemIdx);
                name_item.Clear();
            }
        }

        private void button_up_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx > 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                (items[itemIdx - 1], items[itemIdx]) = (items[itemIdx], items[itemIdx - 1]);
                (box_item.Items[itemIdx - 1], box_item.Items[itemIdx]) = (box_item.Items[itemIdx], box_item.Items[itemIdx - 1]);
                box_item.SelectedIndex = itemIdx - 1;
            }
        }

        private void button_down_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
            if (classIdx >= 0 && itemIdx >= 0 && itemIdx < items.Count - 1)
            {
                (items[itemIdx + 1], items[itemIdx]) = (items[itemIdx], items[itemIdx + 1]);
                (box_item.Items[itemIdx + 1], box_item.Items[itemIdx]) = (box_item.Items[itemIdx], box_item.Items[itemIdx + 1]);
                box_item.SelectedIndex = itemIdx + 1;
            }
        }

        private void button_copy_item_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                var original = items[itemIdx];
                var copy = new hahaha_setting_system_item
                {
                    Name = original.Name + "_copy",
                    Auto_Reload = original.Auto_Reload,
                    Use_Shell_Excute = original.Use_Shell_Excute,
                    Create_No_Window = original.Create_No_Window,
                    Cpu_Affinity_Mask = original.Cpu_Affinity_Mask,
                    Cpu_Priority = original.Cpu_Priority,
                    Command = original.Command,
                    Parameter = original.Parameter,
                };
                items.Insert(itemIdx + 1, copy);
                box_item.Items.Insert(itemIdx + 1, copy.Name);
                box_item.SelectedIndex = itemIdx + 1;
            }
        }

        private void box_item_SelectedIndexChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                if (!Is_Update_UI)
                {
                    name_item.Text = item.Name;
                }

                command.Text = item.Command;
                parameter.Text = item.Parameter;
                check_box_auto_reload.Checked = item.Auto_Reload;
                check_box_use_shell_excute.Checked = item.Use_Shell_Excute;
                check_box_create_no_window.Checked = item.Create_No_Window;
                panel_is_run.BackColor = item.Running ? Color.Green : Color.Red;
                Sync_Process_Cpu_Form_With_Selected_Item();
            }
            else
            {
                name_item.Clear();
                command.Clear();
                parameter.Clear();
                check_box_auto_reload.Checked = false;
                check_box_use_shell_excute.Checked = false;
                check_box_create_no_window.Checked = false;
                panel_is_run.BackColor = Color.Red;
            }
        }

        private void command_TextChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                item.Command = command.Text;
            }
        }

        private void parameter_TextChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                item.Parameter = parameter.Text;
            }
        }

        private void check_box_auto_reload_CheckedChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                item.Auto_Reload = check_box_auto_reload.Checked;
            }
        }

        private void check_box_auto_reload_select_CheckedChanged(object sender, EventArgs e)
        {
            bool value = check_box_auto_reload_select.Checked;
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                item.Auto_Reload = value;
            }
        }

        private void button_load_all_Click(object sender, EventArgs e)
        {
            hahaha.Setting_Box_!.Load_All();
            Is_Update_UI = true;
            Update_UI();
            Is_Update_UI = false;
        }

        private void button_save_all_Click(object sender, EventArgs e)
        {
            hahaha.Setting_Box_!.Save_All();
        }

        private void button_reset_all_Click(object sender, EventArgs e)
        {
            hahaha.Setting_Box_!.Setting.Items.Clear();
            Update_UI();
        }

        public void Update_UI()
        {
            Update_UI_System();
            Update_UI_Box_Class();
            Update_UI_Box_Item();
            Update_UI_Other();
        }

        public void Update_UI_System()
        {
            var sys = hahaha.Setting_Box_!.System;
            count_line.Text = sys.Count_Line.ToString();
            time_display.Text = sys.Time_Display.ToString();
        }

        public void Update_UI_Box_Class()
        {
            int selectedIdx = box_class.SelectedIndex;
            box_class.Items.Clear();
            foreach (var cls in hahaha.Setting_Box_!.Setting.Items)
            {
                box_class.Items.Add(cls.Name);
            }

            if (selectedIdx >= 0 && selectedIdx < box_class.Items.Count)
            {
                box_class.SelectedIndex = selectedIdx;
            }
            else if (box_class.Items.Count > 0)
            {
                box_class.SelectedIndex = 0;
            }
        }

        public void Update_UI_Box_Item()
        {
            int classIdx = box_class.SelectedIndex;
            int selectedIdx = box_item.SelectedIndex;
            box_item.Items.Clear();
            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                foreach (var item in items)
                {
                    box_item.Items.Add(item.Name);
                }

                if (selectedIdx >= 0 && selectedIdx < box_item.Items.Count)
                {
                    box_item.SelectedIndex = selectedIdx;
                }
                else if (box_item.Items.Count > 0)
                {
                    box_item.SelectedIndex = 0;
                }
            }
        }

        public void Update_UI_Other()
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                if (!Is_Update_UI)
                {
                    name_class.Text = hahaha.Setting_Box_!.Setting.Items[classIdx].Name;
                    name_item.Text = item.Name;
                }

                command.Text = item.Command;
                parameter.Text = item.Parameter;
                check_box_auto_reload.Checked = item.Auto_Reload;
                check_box_use_shell_excute.Checked = item.Use_Shell_Excute;
                check_box_create_no_window.Checked = item.Create_No_Window;
            }
            else
            {
                name_class.Clear();
                name_item.Clear();
                command.Clear();
                parameter.Clear();
                check_box_auto_reload.Checked = false;
                check_box_use_shell_excute.Checked = false;
                check_box_create_no_window.Checked = false;
            }
        }

        private void count_line_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(count_line.Text, out int value))
            {
                hahaha.Setting_Box_!.System.Count_Line = value;
            }
        }

        private void time_display_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(time_display.Text, out int value))
            {
                hahaha.Setting_Box_!.System.Time_Display = value;
            }
        }

        private void button_un_select_Click(object sender, EventArgs e)
        {
            box_class.SelectedIndex = -1;
            box_item.SelectedIndex = -1;
        }

        private void button_run_select_Click(object sender, EventArgs e)
        {
            bool startedAny = false;
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                if (!item.Running && Start_Item_Process(item))
                {
                    startedAny = true;
                }
            }

            if (startedAny)
            {
                panel_is_run.BackColor = Color.Green;
            }
        }

        private void button_close_select_Click(object sender, EventArgs e)
        {
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                Stop_Item_Process(item);
            }
            panel_is_run.BackColor = Color.Red;
        }

        private void button_kill_process_Click(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                Kill_Item_Process(item);
            }
        }

        private void button_kill_process_select_Click(object sender, EventArgs e)
        {
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                Kill_Item_Process(item);
            }
        }

        private void button_process_cpu_Click(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item == null)
            {
                MessageBox.Show(this, "請先選擇一個 item。", "CPU 設定", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Open_Process_Cpu_Form(item);
        }

        private void check_box_use_shell_excute_CheckedChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                item.Use_Shell_Excute = check_box_use_shell_excute.Checked;
            }
        }

        private void check_box_create_no_window_CheckedChanged(object sender, EventArgs e)
        {
            var item = Get_Selected_Item();
            if (item != null)
            {
                item.Create_No_Window = check_box_create_no_window.Checked;
            }
        }

        private void check_box_use_shell_excute_select_CheckedChanged(object sender, EventArgs e)
        {
            bool value = check_box_use_shell_excute_select.Checked;
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                item.Use_Shell_Excute = value;
            }
        }

        private void check_box_create_no_window_select_CheckedChanged(object sender, EventArgs e)
        {
            bool value = check_box_create_no_window_select.Checked;
            foreach (var item in Get_Selected_Items_For_Batch())
            {
                item.Create_No_Window = value;
            }
        }
    }
}
