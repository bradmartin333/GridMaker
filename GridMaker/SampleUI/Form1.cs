using GridMaker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static GridMaker.Generator;

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
            DialogResult result = Composer.ShowDialog();
            if (result == DialogResult.OK)
            {
                Generator generator = new Generator(new PointF(100.0f, 0.003f), new PointF(0.001f, 0.002f), new PointF(50.0f, 50.0f));
                List<Node> nodes = generator.Generate();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < nodes.Count; i++)
                {
                    PointF pos = generator.GetStagePosition(nodes[i]);
                    sb.AppendLine($"{i + 1}\t{pos.X:f3}\t{pos.Y:f3}\t{nodes[i]}{(nodes[i].Callback ? "Callback" : "")}");
                }
                richTextBox1.Text = sb.ToString();
            }
        }
    }
}
