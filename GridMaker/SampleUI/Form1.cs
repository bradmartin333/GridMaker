using GridMaker;
using System;
using System.Text;
using System.Windows.Forms;

namespace SampleUI
{
    public partial class Form1 : Form
    {
        private readonly Composer Composer = new Composer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _ = Composer.ShowDialog();
            Generator generator = new Generator(
                new System.Drawing.PointF(0, 50),
                new System.Drawing.PointF(50, 51),
                new Generator.Node());
            var points = generator.Generate();
            StringBuilder sb = new StringBuilder();
            foreach (var item in points)
            {
                var point = generator.GetStagePosition(item);
                sb.AppendLine($"{point.X}\t{point.Y}\t{item}");
            }
            if (!string.IsNullOrEmpty(sb.ToString())) Clipboard.SetText(sb.ToString());
        }
    }
}
