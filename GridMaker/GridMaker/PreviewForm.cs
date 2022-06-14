using System.Linq;
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
            Generator generator = new Generator(Point.Empty, Point.Empty, Point.Empty);
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
            sb.AppendLine("#\tRR\tRC\tR\tC\tSR\tSC\tCallback");
            for (int i = 0; i < nodes.Count; i++)
            {
                sb.AppendLine($"{i + 1}\t{nodes[i]}{nodes[i].Callback}");
                PreviewChart.Series[0].Points.AddXY(nodes[i].Location.X, nodes[i].Location.Y);
            }
            RTB.Text = sb.ToString();
        }
    }
}
