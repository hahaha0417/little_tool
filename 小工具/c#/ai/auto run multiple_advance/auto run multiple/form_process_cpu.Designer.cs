namespace hahaha
{
    partial class form_process_cpu
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            label_item_name = new Label();
            label_item_name_value = new Label();
            label_process_id = new Label();
            label_process_id_value = new Label();
            label_status = new Label();
            label_status_value = new Label();
            label_cpu = new Label();
            label_cpu_value = new Label();
            label_priority = new Label();
            combo_priority = new ComboBox();
            label_affinity = new Label();
            checked_cpu = new CheckedListBox();
            button_apply_live = new Button();
            label_apply_result = new Label();
            timer_cpu = new System.Windows.Forms.Timer(components);
            SuspendLayout();
            // 
            // label_item_name
            // 
            label_item_name.AutoSize = true;
            label_item_name.Location = new Point(18, 18);
            label_item_name.Name = "label_item_name";
            label_item_name.Size = new Size(55, 15);
            label_item_name.TabIndex = 0;
            label_item_name.Text = "項目名稱";
            // 
            // label_item_name_value
            // 
            label_item_name_value.AutoEllipsis = true;
            label_item_name_value.Location = new Point(98, 18);
            label_item_name_value.Name = "label_item_name_value";
            label_item_name_value.Size = new Size(454, 15);
            label_item_name_value.TabIndex = 1;
            label_item_name_value.Text = "-";
            // 
            // label_process_id
            // 
            label_process_id.AutoSize = true;
            label_process_id.Location = new Point(18, 48);
            label_process_id.Name = "label_process_id";
            label_process_id.Size = new Size(53, 15);
            label_process_id.TabIndex = 2;
            label_process_id.Text = "ProcessId";
            // 
            // label_process_id_value
            // 
            label_process_id_value.AutoSize = true;
            label_process_id_value.Location = new Point(98, 48);
            label_process_id_value.Name = "label_process_id_value";
            label_process_id_value.Size = new Size(12, 15);
            label_process_id_value.TabIndex = 3;
            label_process_id_value.Text = "-";
            // 
            // label_status
            // 
            label_status.AutoSize = true;
            label_status.Location = new Point(18, 78);
            label_status.Name = "label_status";
            label_status.Size = new Size(31, 15);
            label_status.TabIndex = 4;
            label_status.Text = "狀態";
            // 
            // label_status_value
            // 
            label_status_value.AutoSize = true;
            label_status_value.Location = new Point(98, 78);
            label_status_value.Name = "label_status_value";
            label_status_value.Size = new Size(12, 15);
            label_status_value.TabIndex = 5;
            label_status_value.Text = "-";
            // 
            // label_cpu
            // 
            label_cpu.AutoSize = true;
            label_cpu.Location = new Point(18, 108);
            label_cpu.Name = "label_cpu";
            label_cpu.Size = new Size(55, 15);
            label_cpu.TabIndex = 6;
            label_cpu.Text = "CPU 使用";
            // 
            // label_cpu_value
            // 
            label_cpu_value.AutoSize = true;
            label_cpu_value.Location = new Point(98, 108);
            label_cpu_value.Name = "label_cpu_value";
            label_cpu_value.Size = new Size(40, 15);
            label_cpu_value.TabIndex = 7;
            label_cpu_value.Text = "0.00 %";
            // 
            // label_priority
            // 
            label_priority.AutoSize = true;
            label_priority.Location = new Point(18, 149);
            label_priority.Name = "label_priority";
            label_priority.Size = new Size(65, 15);
            label_priority.TabIndex = 8;
            label_priority.Text = "CPU Priority";
            // 
            // combo_priority
            // 
            combo_priority.DropDownStyle = ComboBoxStyle.DropDownList;
            combo_priority.FormattingEnabled = true;
            combo_priority.Location = new Point(98, 146);
            combo_priority.Name = "combo_priority";
            combo_priority.Size = new Size(182, 23);
            combo_priority.TabIndex = 9;
            combo_priority.SelectedIndexChanged += combo_priority_SelectedIndexChanged;
            // 
            // label_affinity
            // 
            label_affinity.AutoSize = true;
            label_affinity.Location = new Point(18, 187);
            label_affinity.Name = "label_affinity";
            label_affinity.Size = new Size(72, 15);
            label_affinity.TabIndex = 10;
            label_affinity.Text = "CPU 親和性";
            // 
            // checked_cpu
            // 
            checked_cpu.CheckOnClick = true;
            checked_cpu.FormattingEnabled = true;
            checked_cpu.Location = new Point(98, 187);
            checked_cpu.MultiColumn = true;
            checked_cpu.Name = "checked_cpu";
            checked_cpu.Size = new Size(454, 184);
            checked_cpu.TabIndex = 11;
            checked_cpu.ItemCheck += checked_cpu_ItemCheck;
            // 
            // button_apply_live
            // 
            button_apply_live.Location = new Point(18, 392);
            button_apply_live.Name = "button_apply_live";
            button_apply_live.Size = new Size(114, 44);
            button_apply_live.TabIndex = 12;
            button_apply_live.Text = "套用到目前進程";
            button_apply_live.UseVisualStyleBackColor = true;
            button_apply_live.Click += button_apply_live_Click;
            // 
            // label_apply_result
            // 
            label_apply_result.AutoEllipsis = true;
            label_apply_result.Location = new Point(18, 451);
            label_apply_result.Name = "label_apply_result";
            label_apply_result.Size = new Size(534, 35);
            label_apply_result.TabIndex = 15;
            // 
            // timer_cpu
            // 
            timer_cpu.Tick += timer_cpu_Tick;
            // 
            // form_process_cpu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(571, 499);
            Controls.Add(label_apply_result);
            Controls.Add(button_apply_live);
            Controls.Add(checked_cpu);
            Controls.Add(label_affinity);
            Controls.Add(combo_priority);
            Controls.Add(label_priority);
            Controls.Add(label_cpu_value);
            Controls.Add(label_cpu);
            Controls.Add(label_status_value);
            Controls.Add(label_status);
            Controls.Add(label_process_id_value);
            Controls.Add(label_process_id);
            Controls.Add(label_item_name_value);
            Controls.Add(label_item_name);
            FormBorderStyle = FormBorderStyle.Sizable;
            Name = "form_process_cpu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CPU 監控與設定";
            FormClosed += form_process_cpu_FormClosed;
            Load += form_process_cpu_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label_item_name;
        private Label label_item_name_value;
        private Label label_process_id;
        private Label label_process_id_value;
        private Label label_status;
        private Label label_status_value;
        private Label label_cpu;
        private Label label_cpu_value;
        private Label label_priority;
        private ComboBox combo_priority;
        private Label label_affinity;
        private CheckedListBox checked_cpu;
        private Button button_apply_live;
        private Label label_apply_result;
        private System.Windows.Forms.Timer timer_cpu;
    }
}
