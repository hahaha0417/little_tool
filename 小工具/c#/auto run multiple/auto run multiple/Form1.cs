using hahahalib;
using System;
using System.Diagnostics;
using System.Linq;

namespace hahaha
{
    public partial class form_auto_run_multiple : Form
    {
        public bool Is_Update_UI = false;
        public form_auto_run_multiple()
        {
            InitializeComponent();

            panel_is_run.BackColor = Color.Red;
            Is_Update_UI = true;
            Update_UI();
            Is_Update_UI = false;

            // ------------------------------------------------- 
            tabControl1.TabPages.Remove(tabPage2);
            tabControl1.TabPages.Remove(tabPage4);
    
        }

        private void form_auto_run_Load(object sender, EventArgs e)
        {
            

        }

        private void form_auto_run_FormClosed(object sender, FormClosedEventArgs e)
        {


        }

        private void button_run_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                if (!string.IsNullOrWhiteSpace(item.Command))
                {
                    if (item.Process != null)
                    {
                        item.Process.Dispose();
                    }
                    if (!item.Running)
                    {
                        item.Process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = item.Command,
                                Arguments = item.Parameter,
                                UseShellExecute = item.Use_Shell_Excute,
                                RedirectStandardOutput = true,
                                RedirectStandardError = true,
                                CreateNoWindow = item.Create_No_Window,

                            }
                        };

                        item.Process.EnableRaisingEvents = true;
                        item.Process.Exited += new EventHandler(myProcess_Exited);
                        item.Process.Start();
                        item.Running = true;
                        // 若有 panel_autoload 可加上狀態顯示
                        panel_is_run.BackColor = Color.Green;
                    }

                }
            }
        }

        private void myProcess_Exited(object sender, System.EventArgs e)
        {
            var process = sender as Process;
            hahaha_setting_system_item foundItem = null!;

            // 遍歷所有 class 和 item，找到對應的 item
            foreach (var cls in hahaha.Setting_Box_!.Setting.Items)
            {
                foreach (var item in cls.Items)
                {
                    if (item.Process == process)
                    {
                        foundItem = item;
                        break;
                    }
                }
                if (foundItem != null) break;
            }

            // 若找到對應 item
            if (foundItem != null)
            {


                // 檢查是否需要自動重載
                if (foundItem.Running && foundItem.Auto_Reload)
                {
                    foundItem.Process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = foundItem.Command,
                            Arguments = foundItem.Parameter,
                            UseShellExecute = foundItem.Use_Shell_Excute,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = foundItem.Create_No_Window,
                        }
                    };
                    foundItem.Process.EnableRaisingEvents = true;
                    foundItem.Process.Exited += new EventHandler(myProcess_Exited);
                    foundItem.Process.Start();

                    panel_is_run.BackColor = Color.Green;
                }
                else
                {
                    foundItem.Running = false;
                    panel_is_run.BackColor = Color.Red;
                }
            }
        }

        private void button_close_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Running = false;
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
                var temp = items[idx];
                items[idx] = items[idx - 1];
                items[idx - 1] = temp;

                var name = box_class.Items[idx];
                box_class.Items[idx] = box_class.Items[idx - 1];
                box_class.Items[idx - 1] = name;

                box_class.SelectedIndex = idx - 1;
            }
        }

        private void button_down_class_Click(object sender, EventArgs e)
        {
            int idx = box_class.SelectedIndex;
            var items = hahaha.Setting_Box_!.Setting.Items;
            if (idx >= 0 && idx < items.Count - 1)
            {
                var temp = items[idx];
                items[idx] = items[idx + 1];
                items[idx + 1] = temp;

                var name = box_class.Items[idx];
                box_class.Items[idx] = box_class.Items[idx + 1];
                box_class.Items[idx + 1] = name;

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
                    Items = original.Items
                        .Select(item => new hahaha_setting_system_item
                        {
                            Name = item.Name,
                            Auto_Reload = item.Auto_Reload,
                            Use_Shell_Excute = item.Use_Shell_Excute,
                            Create_No_Window = item.Create_No_Window,
                            Command = item.Command,
                            Parameter = item.Parameter
                            // Status 不複製
                        })
                        .ToList()
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
                

                // 重新載入該 class 的所有 items
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                foreach (var item in items)
                {
                    box_item.Items.Add(item.Name);
                }

                // 若有選取 item，則同步顯示 item 屬性
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
                var temp = items[itemIdx];
                items[itemIdx] = items[itemIdx - 1];
                items[itemIdx - 1] = temp;

                var name = box_item.Items[itemIdx];
                box_item.Items[itemIdx] = box_item.Items[itemIdx - 1];
                box_item.Items[itemIdx - 1] = name;

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
                var temp = items[itemIdx];
                items[itemIdx] = items[itemIdx + 1];
                items[itemIdx + 1] = temp;

                var name = box_item.Items[itemIdx];
                box_item.Items[itemIdx] = box_item.Items[itemIdx + 1];
                box_item.Items[itemIdx + 1] = name;

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
                    Command = original.Command,
                    Parameter = original.Parameter
                    // Status 不複製
                };
                items.Insert(itemIdx + 1, copy);
                box_item.Items.Insert(itemIdx + 1, copy.Name);
                box_item.SelectedIndex = itemIdx + 1;
            }
        }

        private void box_item_SelectedIndexChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                if (!Is_Update_UI)
                {
                    name_item.Text = item.Name;
                }
                
                command.Text = item.Command;
                parameter.Text = item.Parameter;
                check_box_auto_reload.Checked = item.Auto_Reload;
                check_box_use_shell_excute.Checked = item.Use_Shell_Excute;
                check_box_create_no_window.Checked = item.Create_No_Window;
                panel_is_run.BackColor = (item.Running ? Color.Green : Color.Red);
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
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Command = command.Text;
            }
        }

        private void parameter_TextChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Parameter = parameter.Text;
            }
        }

        private void check_box_auto_reload_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Auto_Reload = check_box_auto_reload.Checked;
            }
        }

        private void check_box_auto_reload_select_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            bool value = check_box_auto_reload_select.Checked;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只修改選中的 item
                    items[itemIdx].Auto_Reload = value;
                }
                else
                {
                    // 修改該 class 下所有 items
                    foreach (var item in items)
                    {
                        item.Auto_Reload = value;
                    }
                }
            }
            else
            {
                var items = hahaha.Setting_Box_!.Setting.Items;
                // 修改所有 class 下的所有 items
                foreach (var cls in items)
                {
                    foreach (var item in cls.Items)
                    {
                        item.Auto_Reload = value;
                    }
                }
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
            // 保持原選取
            if (selectedIdx >= 0 && selectedIdx < box_class.Items.Count)
                box_class.SelectedIndex = selectedIdx;
            else if (box_class.Items.Count > 0)
                box_class.SelectedIndex = 0;
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
                // 保持原選取
                if (selectedIdx >= 0 && selectedIdx < box_item.Items.Count)
                    box_item.SelectedIndex = selectedIdx;
                else if (box_item.Items.Count > 0)
                    box_item.SelectedIndex = 0;
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
                }
                if (!Is_Update_UI)
                {
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
            // 若格式錯誤可選擇提示或忽略
        }

        private void time_display_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(time_display.Text, out int value))
            {
                hahaha.Setting_Box_!.System.Time_Display = value;
            }
            // 若格式錯誤可選擇提示或忽略
        }

        private void button_un_select_Click(object sender, EventArgs e)
        {
            box_class.SelectedIndex = -1;
            box_item.SelectedIndex = -1;

        }

        private void button_run_select_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只執行選中的 item
                    var item = items[itemIdx];
                    if (!string.IsNullOrWhiteSpace(item.Command))
                    {
                        if (item.Process != null)
                        {
                            item.Process.Dispose();
                        }
                        if (!item.Running)
                        {
                            item.Process = new Process
                            {
                                StartInfo = new ProcessStartInfo
                                {
                                    FileName = item.Command,
                                    Arguments = item.Parameter,
                                    UseShellExecute = false,
                                    RedirectStandardOutput = true,
                                    RedirectStandardError = true,
                                    CreateNoWindow = false,
                                }
                            };
                            item.Process.EnableRaisingEvents = true;
                            item.Process.Exited += new EventHandler(myProcess_Exited);
                            item.Process.Start();
                            item.Running = true;
                            panel_is_run.BackColor = Color.Green;
                        }
                    }
                }
                else
                {
                    // 執行該 class 下所有 items
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Command))
                        {
                            if (item.Process != null)
                            {
                                item.Process.Dispose();
                            }
                            if (!item.Running)
                            {
                                item.Process = new Process
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        FileName = item.Command,
                                        Arguments = item.Parameter,
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = false,
                                    }
                                };
                                item.Process.EnableRaisingEvents = true;
                                item.Process.Exited += new EventHandler(myProcess_Exited);
                                item.Process.Start();
                                item.Running = true;
                            }
                        }
                    }
                    panel_is_run.BackColor = Color.Green;
                }
            }
            else
            {
                // 執行所有 class 下的所有 items
                var classes = hahaha.Setting_Box_!.Setting.Items;
                foreach (var cls in classes)
                {
                    foreach (var item in cls.Items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.Command))
                        {
                            if (item.Process != null)
                            {
                                item.Process.Dispose();
                            }
                            if (!item.Running)
                            {
                                item.Process = new Process
                                {
                                    StartInfo = new ProcessStartInfo
                                    {
                                        FileName = item.Command,
                                        Arguments = item.Parameter,
                                        UseShellExecute = false,
                                        RedirectStandardOutput = true,
                                        RedirectStandardError = true,
                                        CreateNoWindow = false,
                                    }
                                };
                                item.Process.EnableRaisingEvents = true;
                                item.Process.Exited += new EventHandler(myProcess_Exited);
                                item.Process.Start();
                                item.Running = true;
                            }
                        }
                    }
                }
                panel_is_run.BackColor = Color.Green;
            }
        }

        private void button_close_select_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只關閉選中的 item
                    var item = items[itemIdx];
                    item.Running = false;
                    if (item.Process != null)
                    {
                        item.Running = false;
                    }
                    panel_is_run.BackColor = Color.Red;
                }
                else
                {
                    // 關閉該 class 下所有 items
                    foreach (var item in items)
                    {
                        item.Running = false;
                        if (item.Process != null)
                        {
                            item.Running = false;
                        }
                    }
                    panel_is_run.BackColor = Color.Red;
                }
            }
            else
            {
                // 關閉所有 class 下的所有 items
                var classes = hahaha.Setting_Box_!.Setting.Items;
                foreach (var cls in classes)
                {
                    foreach (var item in cls.Items)
                    {
                        item.Running = false;
                        if (item.Process != null)
                        {
                            item.Running = false;
                        }
                    }
                }
                panel_is_run.BackColor = Color.Red;
            }
        }

        private void button_kill_process_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                if (item.Process != null)
                {
                    try { item.Process.Kill(); } catch { }

                }

            }
        }

        private void button_kill_process_select_Click(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只關閉選中的 item
                    var item = items[itemIdx];
                    if (item.Process != null)
                    {
                        try { item.Process.Kill(); } catch { }
                    }
                }
                else
                {
                    // 關閉該 class 下所有 items
                    foreach (var item in items)
                    {
                        if (item.Process != null)
                        {
                            try { item.Process.Kill(); } catch { }
                        }
                    }
                }
            }
            else
            {
                // 關閉所有 class 下的所有 items
                var classes = hahaha.Setting_Box_!.Setting.Items;
                foreach (var cls in classes)
                {
                    foreach (var item in cls.Items)
                    {
                        if (item.Process != null)
                        {
                            try { item.Process.Kill(); } catch { }
                        }
                    }
                }
            }
        }

        private void check_box_use_shell_excute_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Use_Shell_Excute = check_box_use_shell_excute_select.Checked;
            }
        }

        private void check_box_create_no_window_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            if (classIdx >= 0 && itemIdx >= 0)
            {
                var item = hahaha.Setting_Box_!.Setting.Items[classIdx].Items[itemIdx];
                item.Create_No_Window = check_box_create_no_window.Checked;
            }
        }

        private void check_box_use_shell_excute_select_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            bool value = check_box_use_shell_excute_select.Checked;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只修改選中的 item
                    items[itemIdx].Use_Shell_Excute = value;
                }
                else
                {
                    // 修改該 class 下所有 items
                    foreach (var item in items)
                    {
                        item.Use_Shell_Excute = value;
                    }
                }
            }
            else
            {
                var items = hahaha.Setting_Box_!.Setting.Items;
                // 修改所有 class 下的所有 items
                foreach (var cls in items)
                {
                    foreach (var item in cls.Items)
                    {
                        item.Use_Shell_Excute = value;
                    }
                }
            }
        }

        private void check_box_create_no_window_select_CheckedChanged(object sender, EventArgs e)
        {
            int classIdx = box_class.SelectedIndex;
            int itemIdx = box_item.SelectedIndex;
            bool value = check_box_create_no_window_select.Checked;

            if (classIdx >= 0)
            {
                var items = hahaha.Setting_Box_!.Setting.Items[classIdx].Items;
                if (itemIdx >= 0)
                {
                    // 只修改選中的 item
                    items[itemIdx].Create_No_Window = value;
                }
                else
                {
                    // 修改該 class 下所有 items
                    foreach (var item in items)
                    {
                        item.Create_No_Window = value;
                    }
                }
            }
            else
            {
                var items = hahaha.Setting_Box_!.Setting.Items;
                // 修改所有 class 下的所有 items
                foreach (var cls in items)
                {
                    foreach (var item in cls.Items)
                    {
                        item.Create_No_Window = value;
                    }
                }
            }
        }
    }
}