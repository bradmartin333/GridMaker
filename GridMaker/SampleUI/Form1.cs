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
            _ = Composer.ShowDialog();
        }
    }
}
