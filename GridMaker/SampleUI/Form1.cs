using GridMaker;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static GridMaker.Generator;

namespace SampleUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (Composer composer = new Composer())
            {
                composer.ShowDialog();
            }
            Generator generator = new Generator(new PointF(100.0f, 0.003f), new PointF(0.001f, 0.002f));
            List<Node> nodes = generator.Generate();
            richTextBox1.Text = "";
            for (int i = 0; i < nodes.Count; i++)
            {
                PointF pos = generator.GetStagePosition(nodes[i]);
                richTextBox1.Text += $"{i + 1}\t{pos.X:f3}\t{pos.Y:f3}\t{nodes[i]}\n";
            }
        }
    }
}
