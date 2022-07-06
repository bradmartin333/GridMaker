using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GridMaker
{
    public partial class IndexSelector : Form
    {
        private Size _GridSize = new Size(50, 50);
        public Size GridSize { get => _GridSize; }

        private List<Point> _SkippedIndices = new List<Point>();
        public List<Point> SkippedIndices { get => _SkippedIndices; }

        public IndexSelector(Size gridSize, List<Point> skippedIndices)
        {
            InitializeComponent();
            _GridSize = gridSize;
            _SkippedIndices = skippedIndices;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        private void DiscoForm_Load(object sender, EventArgs e)
        {
            pictureBox.BackgroundImage = new Bitmap(1000, 1000);
            pictureBox.Image = new Bitmap(1000, 1000);
            Functions.MakeGrid(this);
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Functions.ClickTile(e.Location, this, waitForExit: true);
            Functions.HighlightTile(e.Location, this);
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Functions.ClickTile(e.Location, this);
            _SkippedIndices = Functions.GetSkippedTileLocations();
        }
    }
}
