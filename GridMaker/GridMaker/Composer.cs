using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace GridMaker
{
    internal partial class Composer : Form
    {
        private readonly string GridPath = $"{Path.GetTempPath()}GridMaker.xml";
        private Grid Grid = new Grid();

        public Composer()
        {
            InitializeComponent();
            if (File.Exists(GridPath))
            {
                LoadGrid();
                RepopulateUI();
            }
        }

        private void BtnValidateAndSave_Click(object sender, EventArgs e)
        {
            Grid = new Grid()
            {
                Name = TxtName.Text,
                StepA = new Step(new Size((int)NumStepACountX.Value, (int)NumStepACountY.Value), new PointF((int)NumStepAPitchX.Value, (int)NumStepAPitchY.Value)),
                StepB = new Step(new Size((int)NumStepBCountX.Value, (int)NumStepBCountY.Value), new PointF((int)NumStepBPitchX.Value, (int)NumStepBPitchY.Value)),
                StepC = new Step(new Size((int)NumStepCCountX.Value, (int)NumStepCCountY.Value), new PointF((int)NumStepCPitchX.Value, (int)NumStepCPitchY.Value)),
            };
            bool validA = MakeSkippedIndices(Grid.StepA, RTBA.Text);
            bool validB = MakeSkippedIndices(Grid.StepB, RTBB.Text);
            bool validC = MakeSkippedIndices(Grid.StepC, RTBC.Text);
            if (validA && validB && validC)
            {
                RepopulateUI();
                SaveGrid();
            }
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
            RTBA.Text = "";
            foreach (Point point in Grid.StepA.SkippedIndices)
                RTBA.Text += $"{point.X}, {point.Y}\n";
            RTBB.Text = "";
            foreach (Point point in Grid.StepB.SkippedIndices)
                RTBB.Text += $"{point.X}, {point.Y}\n";
            RTBC.Text = "";
            foreach (Point point in Grid.StepC.SkippedIndices)
                RTBC.Text += $"{point.X}, {point.Y}\n";
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
            using (StreamWriter stream = new StreamWriter(string.IsNullOrEmpty(path) ? GridPath : path))
            {
                XmlSerializer x = new XmlSerializer(typeof(Grid));
                x.Serialize(stream, Grid);
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
            }
            catch (Exception) { }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {

        }
    }
}
