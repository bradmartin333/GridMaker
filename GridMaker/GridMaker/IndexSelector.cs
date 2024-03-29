﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GridMaker
{
    public partial class IndexSelector : Form
    {
        public enum DragType
        {
            Skip,
            Include,
            Unknown,
            Null
        }
        private DragType CurrentDragType = DragType.Unknown;

        private Size _GridSize = new Size(50, 50);
        public Size GridSize { get => _GridSize; }

        private List<Point> _SkippedIndices = new List<Point>();
        public List<Point> SkippedIndices { get => _SkippedIndices; }

        private readonly bool UseRC = false;

        public IndexSelector(Size gridSize, List<Point> skippedIndices, bool useRC)
        {
            InitializeComponent();
            _GridSize = gridSize;
            _SkippedIndices = skippedIndices;
            pictureBox.MouseDown += PictureBox_MouseDown;
            pictureBox.MouseUp += PictureBox_MouseUp;
            pictureBox.MouseMove += PictureBox_MouseMove;
            pictureBox.MouseEnter += PictureBox_MouseEnter;
            pictureBox.MouseLeave += PictureBox_MouseLeave;
            FormClosing += IndexSelector_FormClosing;
            UseRC = useRC;
        }

        private void BtnSkipAll_Click(object sender, EventArgs e)
        {
            Functions.SetAllTiles(false, this);
        }

        private void BtnIncludeAll_Click(object sender, EventArgs e)
        {
            Functions.SetAllTiles(true, this);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            _SkippedIndices = Functions.GetSkippedTileLocations();
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        private void IndexSelector_Load(object sender, EventArgs e)
        {
            pictureBox.BackgroundImage = new Bitmap(1000, 1000);
            pictureBox.Image = new Bitmap(1000, 1000);
            Functions.MakeGrid(this);

            if (Properties.Settings.Default.SelectorMaximized)
            {
                Location = Properties.Settings.Default.SelectorLocation;
                WindowState = FormWindowState.Maximized;
                Size = Properties.Settings.Default.SelectorSize;
            }
            else if (Properties.Settings.Default.SelectorMinimized)
            {
                Location = Properties.Settings.Default.SelectorLocation;
                WindowState = FormWindowState.Minimized;
                Size = Properties.Settings.Default.SelectorSize;
            }
            else
            {
                Location = Properties.Settings.Default.SelectorLocation;
                Size = Properties.Settings.Default.SelectorSize;
            }
        }

        private void IndexSelector_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.SelectorLocation = RestoreBounds.Location;
                Properties.Settings.Default.SelectorSize = RestoreBounds.Size;
                Properties.Settings.Default.SelectorMaximized = true;
                Properties.Settings.Default.SelectorMinimized = false;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.SelectorLocation = Location;
                Properties.Settings.Default.SelectorSize = Size;
                Properties.Settings.Default.SelectorMaximized = false;
                Properties.Settings.Default.SelectorMinimized = false;
            }
            else
            {
                Properties.Settings.Default.SelectorLocation = RestoreBounds.Location;
                Properties.Settings.Default.SelectorSize = RestoreBounds.Size;
                Properties.Settings.Default.SelectorMaximized = false;
                Properties.Settings.Default.SelectorMinimized = true;
            }
            Properties.Settings.Default.Save();
        }

        private void PictureBox_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            CurrentDragType = DragType.Null;
            Text = $"Index Selector";
        }

        private void PictureBox_MouseEnter(object sender, EventArgs e)
        {
            Cursor = Cursors.Cross;
            CurrentDragType = DragType.Unknown;
            Text = $"Index Selector";
        }

        private void PictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Functions.ClickTile(e.Location, ref CurrentDragType, this, waitForExit: true);
            Functions.HighlightTile(e.Location, this);
            Text = $"Index Selector     ({(UseRC ? Functions.HoverLocation.Y : Functions.HoverLocation.X)}, {(UseRC ? Functions.HoverLocation.X : Functions.HoverLocation.Y)})";
        }

        private void PictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Functions.ClickTile(e.Location, ref CurrentDragType, this);
        }

        private void PictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            CurrentDragType = DragType.Unknown;
        }
    }
}
