using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GridMaker
{
    public partial class Composer : Form
    {
        private readonly string GridPath = $"{Path.GetTempPath()}GridMaker.xml";
        public static Grid Grid = new Grid();

        /// <summary>
        /// Open a Grid Maker Composer window
        /// and load the last used Grid from the
        /// temp directory
        /// </summary>
        public Composer()
        {
            InitializeComponent();
            if (File.Exists(GridPath)) LoadGrid();
        }

        private void BtnValidateAndSave_Click(object sender, EventArgs e)
        {
            SaveGrid();
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
                rtb.Text += $"{point.X}, {point.Y}\n";
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
                    bool validX = int.TryParse(cols[0].Replace(" ", ""), out int x);
                    bool validY = int.TryParse(cols[1].Replace(" ", ""), out int y);
                    if (validX && validY) points.Add(new Point(x, y));
                }
            }
            step.SkippedIndices = points;
            return true;
        }

        private void SaveGrid(string path = null)
        {
            if (ValidateGrid())
            {
                using (StreamWriter stream = new StreamWriter(string.IsNullOrEmpty(path) ? GridPath : path))
                {
                    XmlSerializer x = new XmlSerializer(typeof(Grid));
                    x.Serialize(stream, Grid);
                }
                RepopulateUI();
            } 
        }

        private void LoadGrid(string path = null)
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
                             "Enter skipped indices within the array as a zero-indexed CSV and a new line between each entry.\n" +
                             "\tEx: 0, 1\n" +
                             "Enabling callback will trigger a software response by the host program.";
            MessageBox.Show(helpStr, "Grid Maker Help");
        }

        private void BtnDone_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
