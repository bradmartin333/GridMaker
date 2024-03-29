﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GridMaker
{
    /// <summary>
    /// UI for creating/loading/saving a Grid
    /// </summary>
    public partial class Composer : Form
    {
        /// <summary>
        /// The Grid currently active in GridMaker
        /// </summary>
        public static Grid Grid = new Grid();

        private readonly bool UseRC = true;
        private readonly string GridPath = $"{Path.GetTempPath()}GridMaker.xml";

        /// <summary>
        /// Open a Grid Maker Composer window
        /// and load the last used Grid from the
        /// temp directory
        /// </summary>
        /// <param name="useRCnotation">
        /// Flips X and Y order in UI and adjusts labels
        /// </param>
        public Composer(bool useRCnotation = true)
        {
            InitializeComponent();
            UseRC = useRCnotation;
            if (UseRC)
            {
                SwitchToRC();
                InitializeTabOrderRC();
            }
            else
                InitializeTabOrderXY();

            foreach (Control item in TabOrder)
            {
                item.GotFocus += Item_GotFocus;
                item.Click += Item_Click;
            }

            Shown += Composer_Shown;
            FormClosing += Composer_FormClosing;

            if (File.Exists(GridPath))
                LoadGrid();
            else
                TxtName.Text = Grid.Name;
        }

        #region Tab Order

        private Control[] TabOrder;
        private int TabOrderIndex = 0;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Tab:
                    TabOrderIndex++;
                    if (TabOrderIndex > TabOrder.Length - 1) TabOrderIndex = 0;
                    TabOrder[TabOrderIndex].Focus();
                    return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void InitializeTabOrderXY()
        {
            TabOrder = new Control[]
            {
                NumStepACountX, NumStepACountY, NumStepAPitchX, NumStepAPitchY,
                NumStepBCountX, NumStepBCountY, NumStepBPitchX, NumStepBPitchY,
                NumStepCCountX, NumStepCCountY, NumStepCPitchX, NumStepCPitchY,
            };
        }

        private void InitializeTabOrderRC()
        {
            TabOrder = new Control[]
            {
                NumStepACountY, NumStepACountX, NumStepAPitchY, NumStepAPitchX,
                NumStepBCountY, NumStepBCountX, NumStepBPitchY, NumStepBPitchX,
                NumStepCCountY, NumStepCCountX, NumStepCPitchY, NumStepCPitchX,
            };
        }

        private void Item_Click(object sender, EventArgs e)
        {
            SelectNum(sender);
        }

        private void Item_GotFocus(object sender, EventArgs e)
        {
            SelectNum(sender);
        }

        private void SelectNum(object sender)
        {
            NumericUpDown num = (NumericUpDown)sender;
            num.Select(0, num.Text.Length);
            TabOrderIndex = TabOrder.ToList().IndexOf((Control)sender);
        }

        private void Composer_Shown(object sender, EventArgs e)
        {
            TabOrder[0].Focus();
        }

        #endregion

        private void Composer_Load(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.ComposerMaximized)
            {
                Location = Properties.Settings.Default.ComposerLocation;
                WindowState = FormWindowState.Maximized;
                Size = Properties.Settings.Default.ComposerSize;
            }
            else if (Properties.Settings.Default.ComposerMinimized)
            {
                Location = Properties.Settings.Default.ComposerLocation;
                WindowState = FormWindowState.Minimized;
                Size = Properties.Settings.Default.ComposerSize;
            }
            else
            {
                Location = Properties.Settings.Default.ComposerLocation;
                Size = Properties.Settings.Default.ComposerSize;
            }
        }

        private void Composer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                Properties.Settings.Default.ComposerLocation = RestoreBounds.Location;
                Properties.Settings.Default.ComposerSize = RestoreBounds.Size;
                Properties.Settings.Default.ComposerMaximized = true;
                Properties.Settings.Default.ComposerMinimized = false;
            }
            else if (WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.ComposerLocation = Location;
                Properties.Settings.Default.ComposerSize = Size;
                Properties.Settings.Default.ComposerMaximized = false;
                Properties.Settings.Default.ComposerMinimized = false;
            }
            else
            {
                Properties.Settings.Default.ComposerLocation = RestoreBounds.Location;
                Properties.Settings.Default.ComposerSize = RestoreBounds.Size;
                Properties.Settings.Default.ComposerMaximized = false;
                Properties.Settings.Default.ComposerMinimized = true;
            }
            Properties.Settings.Default.Save();

            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void SwitchToRC()
        {
            foreach (FlowLayoutPanel flow in TLP.Controls.OfType<FlowLayoutPanel>())
            {
                foreach (Label lbl in flow.Controls.OfType<Label>().Where(x => x.Tag != null))
                    lbl.Text = lbl.Tag.ToString();
                switch (flow.Tag.ToString())
                {
                    case "X":
                        TLP.SetColumn(flow, TLP.GetColumn(flow) + 1);
                        break;
                    case "Y":
                        TLP.SetColumn(flow, TLP.GetColumn(flow) - 1);
                        break;
                }
            }
                
        }

        private void IndexEditA_Click(object sender, EventArgs e)
        {
            using (IndexSelector indexSelector = new IndexSelector(new Size((int)NumStepACountX.Value, (int)NumStepACountY.Value), Grid.StepA.SkippedIndices, UseRC))
            {
                DialogResult result = indexSelector.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Grid.StepA.SkippedIndices = indexSelector.SkippedIndices;
                    PopulateRTB(Grid.StepA, RTBA);
                }
            }
        }

        private void IndexEditB_Click(object sender, EventArgs e)
        {
            using (IndexSelector indexSelector = new IndexSelector(new Size((int)NumStepBCountX.Value, (int)NumStepBCountY.Value), Grid.StepB.SkippedIndices, UseRC))
            {
                DialogResult result = indexSelector.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Grid.StepB.SkippedIndices = indexSelector.SkippedIndices;
                    PopulateRTB(Grid.StepB, RTBB);
                }
            }
        }

        private void IndexEditC_Click(object sender, EventArgs e)
        {
            using (IndexSelector indexSelector = new IndexSelector(new Size((int)NumStepCCountX.Value, (int)NumStepCCountY.Value), Grid.StepC.SkippedIndices, UseRC))
            {
                DialogResult result = indexSelector.ShowDialog();
                if (result == DialogResult.OK)
                {
                    Grid.StepC.SkippedIndices = indexSelector.SkippedIndices;
                    PopulateRTB(Grid.StepC, RTBC);
                }
            }
        }

        private bool ValidateGrid()
        {
            Grid = new Grid()
            {
                Name = TxtName.Text,
                StepA = new Step(
                    new Size((int)NumStepACountX.Value, (int)NumStepACountY.Value), 
                    new PointF((float)NumStepAPitchX.Value, (float)NumStepAPitchY.Value),
                    CBXA.Checked),
                StepB = new Step(
                    new Size((int)NumStepBCountX.Value, (int)NumStepBCountY.Value), 
                    new PointF((float)NumStepBPitchX.Value, (float)NumStepBPitchY.Value),
                    CBXB.Checked),
                StepC = new Step(
                    new Size((int)NumStepCCountX.Value, (int)NumStepCCountY.Value), 
                    new PointF((float)NumStepCPitchX.Value, (float)NumStepCPitchY.Value),
                    CBXC.Checked),
            };
            bool validA = MakeSkippedIndices(Grid.StepA, RTBA.Text);
            bool validB = MakeSkippedIndices(Grid.StepB, RTBB.Text);
            bool validC = MakeSkippedIndices(Grid.StepC, RTBC.Text);
            return validA && validB && validC;
        }

        private void RepopulateUI()
        {
            TxtName.Text = Grid.Name;
            NumStepACountX.Value = Grid.StepA.Array.Width;
            NumStepACountY.Value = Grid.StepA.Array.Height;
            NumStepBCountX.Value = Grid.StepB.Array.Width;
            NumStepBCountY.Value = Grid.StepB.Array.Height;
            NumStepCCountX.Value = Grid.StepC.Array.Width;
            NumStepCCountY.Value = Grid.StepC.Array.Height;
            NumStepAPitchX.Value = (decimal)Grid.StepA.Pitch.X;
            NumStepAPitchY.Value = (decimal)Grid.StepA.Pitch.Y;
            NumStepBPitchX.Value = (decimal)Grid.StepB.Pitch.X;
            NumStepBPitchY.Value = (decimal)Grid.StepB.Pitch.Y;
            NumStepCPitchX.Value = (decimal)Grid.StepC.Pitch.X;
            NumStepCPitchY.Value = (decimal)Grid.StepC.Pitch.Y;
            PopulateRTB(Grid.StepA, RTBA);
            PopulateRTB(Grid.StepB, RTBB);
            PopulateRTB(Grid.StepC, RTBC);
            CBXA.Checked = Grid.StepA.Callback;
            CBXB.Checked = Grid.StepB.Callback;
            CBXC.Checked = Grid.StepC.Callback;
        }

        private void PopulateRTB(Step step, RichTextBox rtb)
        {
            rtb.Text = "";
            foreach (Point point in step.SkippedIndices)
                rtb.Text += $"{(UseRC ? point.Y : point.X) + 1}, {(UseRC ? point.X : point.Y) + 1}\n";
        }

        private bool MakeSkippedIndices(Step step, string text)
        {
            string[] rows = text.Split('\n');
            List<Point> points = new List<Point>();
            foreach (string row in rows)
            {
                string[] cols = row.Split(',');
                if (cols.Length == 2)
                {
                    bool validX = int.TryParse(cols[UseRC ? 1 : 0].Replace(" ", ""), out int x);
                    bool validY = int.TryParse(cols[UseRC ? 0 : 1].Replace(" ", ""), out int y);
                    if (validX && validY && x > 0 && y > 0) points.Add(new Point(x - 1, y - 1));
                }
            }
            step.SkippedIndices = points;
            return true;
        }

        private bool SaveGrid(string path = null)
        {
            if (ValidateGrid())
            {
                using (StreamWriter stream = new StreamWriter(string.IsNullOrEmpty(path) ? GridPath : path))
                {
                    XmlSerializer x = new XmlSerializer(typeof(Grid));
                    x.Serialize(stream, Grid);
                }
                RepopulateUI();
                return true;
            } 
            else
            {
                MessageBox.Show("Invalid Grid", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        /// <summary>
        /// Initialize GridMaker with a saved .XML Grid
        /// </summary>
        /// <param name="path"></param>
        public void LoadGrid(string path = null)
        {
            try
            {
                using (StreamReader stream = new StreamReader(string.IsNullOrEmpty(path) ? GridPath : path))
                {
                    XmlSerializer x = new XmlSerializer(typeof(Grid));
                    Grid = (Grid)x.Deserialize(stream);
                }
                RepopulateUI();
            }
            catch (Exception) { }
        }

        private void NewToolStripButton_Click(object sender, EventArgs e)
        {
            Grid = new Grid();
            RepopulateUI();
        }

        private void OpenToolStripButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.RestoreDirectory = true;
                ofd.Title = "Open Grid";
                ofd.Filter = "XML file(*.xml)| *.xml";
                if (ofd.ShowDialog() == DialogResult.OK)
                    LoadGrid(ofd.FileName);
            }
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "XML file(*.xml)| *.xml";
                sfd.Title = "Save Grid";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    if (File.Exists(sfd.FileName)) File.Delete(sfd.FileName);
                    SaveGrid(sfd.FileName);
                }
            }
        }

        private void HelpToolStripButton_Click(object sender, EventArgs e)
        {
            string helpStr = "Step A should have the largest pitches and Step C should have the smallest pitches.\n" +
                             "The Steps are nested to allow for greater customization.\n" +
                             "Enter skipped indices within the array as a one-indexed CSV and a new line between each entry.\n" +
                             "\tEx: 0, 1\n" +
                             "Enabling callback will trigger a software response by the host program.";
            MessageBox.Show(helpStr, "Grid Maker Help");
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            DialogResult = SaveGrid() ? DialogResult.OK : DialogResult.Cancel;
            Hide();
        }

        private void ToolStripButtonViewIndices_Click(object sender, EventArgs e)
        {
            if (SaveGrid())
            {
                PreviewForm previewForm = new PreviewForm();
                if (!previewForm.IsDisposed) previewForm.ShowDialog();
            }
        }

        private void ToolStripButtonResetLayout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
        }
    }
}
