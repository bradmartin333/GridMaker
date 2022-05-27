using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace GridMaker
{
    /// <summary>
    /// Class for turning a Grid into a list of Nodes
    /// </summary>
    public class Generator
    {
        private enum WarningState { None, Active, Ignore, }
        private readonly double CauseWarning = 1e6;
        private readonly PointF SW, SE, Origin;
        private PointF CenterOfRotation;

        /// <summary>
        /// Storage for Grid points
        /// </summary>
        public struct Node
        {
            /// <summary>
            /// The raw location of the node
            /// </summary>
            public PointF Location;
            /// <summary>
            /// The A index of the node
            /// </summary>
            public Point A;
            /// <summary>
            /// The B index of the node
            /// </summary>
            public Point B;
            /// <summary>
            /// The C index of the node
            /// </summary>
            public Point C;
            /// <summary>
            /// Whether or not this node should cause a callback
            /// </summary>
            public bool Callback;
            /// <summary>
            /// Get TSV string representation of the Node
            /// </summary>
            /// <returns>
            /// One-indexed node location string
            /// </returns>
            public override string ToString() => $"{A.Y + 1}\t{A.X + 1}\t{B.Y + 1}\t{B.X + 1}\t{C.Y + 1}\t{C.X + 1}\t";
        }

        /// <summary>
        /// Initialize the Grid Generator
        /// </summary>
        /// <param name="sw">
        /// Stage coordinates in mm of SW corner of substrate
        /// </param>
        /// <param name="se">
        /// Stage coordinates in mmof SE corner of substrate
        /// </param>
        /// <param name="origin">
        /// Location of SW corner of ROI
        /// </param>
        public Generator(PointF sw, PointF se, PointF origin)
        {
            SW = sw;
            SE = se;
            Origin = origin;
            UpdateCenterOfRotation();
        }

        /// <summary>
        /// Get Node position rotated to the substrate
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Stage coordinates of aligned node in mm
        /// in reference to the origin
        /// </returns>
        public PointF GetStagePosition(Node node)
        {
            PointF globalPos = new PointF(Origin.X - node.Location.X, Origin.Y - node.Location.Y);
            RotatePoint(ref globalPos);
            return globalPos;
        }

        private void RotatePoint(ref PointF point, bool getCOR = false)
        {
            if (point == SW && !getCOR)
                return;
            double angle = -1 * Math.Atan((SE.Y - SW.Y) / Math.Abs(SE.X - SW.X));
            float x = (float)(point.X * Math.Cos(angle) - point.Y * Math.Sin(angle));
            float y = (float)(point.X * Math.Sin(angle) + point.Y * Math.Cos(angle));
            point = new PointF(x + CenterOfRotation.X, y + CenterOfRotation.Y);
        }

        private void UpdateCenterOfRotation()
        {
            CenterOfRotation = new PointF(0, 0);
            PointF SWcopy = SW;
            RotatePoint(ref SWcopy, true);
            CenterOfRotation = new PointF(SW.X - SWcopy.X, SW.Y - SWcopy.Y);
        }

        /// <summary>
        /// Generate the Grid node
        /// </summary>
        /// <returns>
        /// List of nodes that make up the grid in a serpentine order
        /// </returns>
        public List<Node> Generate()
        {
            WarningState warning = WarningState.None;
            List<Node> points = new List<Node>();
            for (int i = 0; i < Composer.Grid.StepA.Array.Width; i++)
                for (int j = 0; j < Composer.Grid.StepA.Array.Height; j++)
                {
                    if (warning == WarningState.Active) return new List<Node>();
                    Point point = new Point(i, j);
                    if (Composer.Grid.StepA.SkippedIndices.Contains(point)) continue;
                    points.Add(new Node()
                    {
                        Location = new PointF(
                            i * Composer.Grid.StepA.Pitch.X,
                            j * Composer.Grid.StepA.Pitch.Y),
                        A = point,
                        B = Point.Empty,
                        C = Point.Empty,
                        Callback = Composer.Grid.StepA.Callback,
                    });
                    if (warning != WarningState.Ignore) warning = CheckWarning(points.Count);
                }

            Node[] basePoints = (Node[])points.ToArray().Clone();
            for (int k = 0; k < basePoints.Count(); k++)
                for (int i = 0; i < Composer.Grid.StepB.Array.Width; i++)
                    for (int j = 0; j < Composer.Grid.StepB.Array.Height; j++)
                    {
                        if (warning == WarningState.Active) return new List<Node>();
                        Point point = new Point(i, j);
                        if (point == Point.Empty) continue;
                        if (Composer.Grid.StepB.SkippedIndices.Contains(point)) continue;
                        points.Add(new Node
                        {
                            Location = new PointF(
                                basePoints[k].Location.X + i * Composer.Grid.StepB.Pitch.X,
                                basePoints[k].Location.Y + j * Composer.Grid.StepB.Pitch.Y),
                            A = basePoints[k].A,
                            B = point,
                            C = Point.Empty,
                            Callback = Composer.Grid.StepB.Callback,
                        });
                        if (warning != WarningState.Ignore) warning = CheckWarning(points.Count);
                    }

            basePoints = (Node[])points.ToArray().Clone();
            for (int k = 0; k < basePoints.Count(); k++)
                for (int i = 0; i < Composer.Grid.StepC.Array.Width; i++)
                    for (int j = 0; j < Composer.Grid.StepC.Array.Height; j++)
                    {
                        if (warning == WarningState.Active) return new List<Node>();
                        Point point = new Point(i, j);
                        if (point == Point.Empty) continue;
                        if (Composer.Grid.StepC.SkippedIndices.Contains(point)) continue;
                        points.Add(new Node
                        {
                            Location = new PointF(
                                basePoints[k].Location.X + i * Composer.Grid.StepC.Pitch.X,
                                basePoints[k].Location.Y + j * Composer.Grid.StepC.Pitch.Y),
                            A = basePoints[k].A,
                            B = basePoints[k].B,
                            C = point,
                            Callback = Composer.Grid.StepC.Callback,
                        });
                        if (warning != WarningState.Ignore) warning = CheckWarning(points.Count);
                    }

            return Serpentize(points);
        }

        private WarningState CheckWarning(int count)
        {
            if (count > CauseWarning)
            {
                DialogResult result = MessageBox.Show(
                    $"There are more than {CauseWarning:#E+0} points, do you want to continue at risk of causing a program stall?", 
                    "Grid Maker", MessageBoxButtons.YesNo);
                return result == DialogResult.No ? WarningState.Active : WarningState.Ignore;
            }
            return WarningState.None;
        }

        private List<Node> Serpentize(List<Node> points)
        {
            List<Node> output = new List<Node>();
            var groups = points.OrderBy(x => x.Location.X).OrderBy(x => x.Location.Y).GroupBy(x => x.Location.X);
            int idx = 0;
            foreach (IGrouping<float, Node> group in groups)
            {
                if (idx % 2 != 0) // Odd groups
                    output.AddRange(group.ToArray().Reverse());
                else
                    output.AddRange(group.ToArray());
                idx++;
            }
            return output;
        }
    }
}
