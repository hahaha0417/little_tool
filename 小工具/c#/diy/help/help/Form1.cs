namespace empty
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel_main.Dock = DockStyle.Fill;
            panel_sub.Dock = DockStyle.Fill;
            panel_third.Dock = DockStyle.Fill;
            panel_other.Dock = DockStyle.Fill;

            button_main_Click(sender, e);
        }

        private void button_main_Click(object sender, EventArgs e)
        {
            panel_main.Visible = true;
            panel_sub.Visible = false;
            panel_third.Visible = false;
            panel_other.Visible = false;

        }

        private void button_sub_Click(object sender, EventArgs e)
        {
            panel_main.Visible = false;
            panel_sub.Visible = true;
            panel_third.Visible = false;
            panel_other.Visible = false;

        }

        private void button_third_Click(object sender, EventArgs e)
        {
            panel_main.Visible = false;
            panel_sub.Visible = false;
            panel_third.Visible = true;
            panel_other.Visible = false;

        }

        private void button_other_Click(object sender, EventArgs e)
        {
            panel_main.Visible = false;
            panel_sub.Visible = false;
            panel_third.Visible = false;
            panel_other.Visible = true;

        }
    }
}