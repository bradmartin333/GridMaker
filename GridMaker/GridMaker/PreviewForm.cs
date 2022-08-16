using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GridMaker
{
    public partial class PreviewForm : Form
    {
        public PreviewForm()
        {
            InitializeComponent();
            Generator generator = new Generator(Point.Empty, Point.Empty, new Generator.Node());
            List<Generator.Node> nodes = generator.Generate();
            if (nodes.Count > 10e3)
            {
                DialogResult result = MessageBox.Show(
                    "There are more than 10K points, which might cause a software freeze. Continue?",
                    "Grid Maker Preview", MessageBoxButtons.YesNo);
                if (result != DialogResult.Yes)
                {
                    Close();
                    return;
                }  
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("#\tType\tRR\tRC\tR\tC\tSR\tSC\tCallback");
            for (int i = 0; i < nodes.Count; i++)
            {
                sb.AppendLine($"{i + 1}\t{nodes[i].NodeType}\t{nodes[i]}{nodes[i].Callback}");
                PreviewChart.Series[0].Points.AddXY(nodes[i].Location.X, nodes[i].Location.Y);
            }
            RTB.Text = sb.ToString();
            FormClosing += PreviewForm_FormClosing;
        }

        private void PreviewForm_Load(object sender, System.EventArgs e)
        {
            if (Properties.Settings.Default.PreviewMaximized)
            {
                Location = Properties.Settings.Default.PreviewLocation;
                WindowState = FormWindowState.Maximized;
                Size = Properties.Settings.Default.PreviewSize;
            }
            else if (Properties.Settings.Default.PreviewMinimized)
            {
                Location = Properties.Settings.Default.PreviewLocation;
                WindowState = FormWindowState.Minimized;
                Size = Properties.Settings.Default.PreviewSize;
            }
            else
            {
                Location = Properties.Settings.Default.PreviewLocation;
                Size = Properties.Settings.Default.PreviewSize;
            }
        }

        private void PreviewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.PreviewLocation = RestoreBounds.Location;
                Properties.Settings.Default.PreviewSize = RestoreBounds.Size;
                Properties.Settings.Default.PreviewMaximized = true;
                Properties.Settings.Default.PreviewMinimized = false;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.PreviewLocation = Location;
                Properties.Settings.Default.PreviewSize = Size;
                Properties.Settings.Default.PreviewMaximized = false;
                Properties.Settings.Default.PreviewMinimized = false;
            }
            else
            {
                Properties.Settings.Default.PreviewLocation = RestoreBounds.Location;
                Properties.Settings.Default.PreviewSize = RestoreBounds.Size;
                Properties.Settings.Default.PreviewMaximized = false;
                Properties.Settings.Default.PreviewMinimized = true;
            }
            Properties.Settings.Default.Save();
        }
    }
}
