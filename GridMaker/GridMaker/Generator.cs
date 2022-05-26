using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GridMaker
{
    public class Generator
    {
        private readonly PointF SW, SE;
        private PointF CenterOfRotation;

        /// <summary>
        /// Storage for Grid points
        /// </summary>
        public struct Node
        {
            public PointF Location;
            public Point A;
            public Point B;
            public Point C;
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
        /// Stage coordinates in mm of SW corner of ROI
        /// </param>
        /// <param name="se">
        /// Stage coordinates in mmof SE corner of ROI
        /// </param>
        public Generator(PointF sw, PointF se)
        {
            SW = sw;
            SE = se;
            UpdateCenterOfRotation();
        }

        /// <summary>
        /// Get Node position rotated to ROI
        /// </summary>
        /// <param name="node"></param>
        /// <returns>
        /// Stage coordinates of aligned node in mm
        /// </returns>
        public PointF GetStagePosition(Node node)
        {
            PointF globalPos = new PointF(SW.X - node.Location.X, SW.Y - node.Location.Y);
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
            List<Node> points = new List<Node>();
            for (int i = 0; i < Composer.Grid.StepA.Array.Width; i++)
            {
                for (int j = 0; j < Composer.Grid.StepA.Array.Height; j++)
                {
                    Point point = new Point(i, j);
                    if (Composer.Grid.StepA.SkippedIndices.Contains(point)) continue;
                    points.Add(new Node()
                    {
                        Location = new PointF(
                            i * Composer.Grid.StepA.Pitch.X,
                            j * Composer.Grid.StepA.Pitch.Y),
                        A = point,
                        B = Point.Empty,
                        C = Point.Empty
                    });
                }
            }

            Node[] basePoints = (Node[])points.ToArray().Clone();
            for (int k = 0; k < basePoints.Count(); k++)
            {
                for (int i = 0; i < Composer.Grid.StepB.Array.Width; i++)
                {
                    for (int j = 0; j < Composer.Grid.StepB.Array.Height; j++)
                    {
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
                            C = Point.Empty
                        });
                    }
                }
            }

            basePoints = (Node[])points.ToArray().Clone();
            for (int k = 0; k < basePoints.Count(); k++)
            {
                for (int i = 0; i < Composer.Grid.StepC.Array.Width; i++)
                {
                    for (int j = 0; j < Composer.Grid.StepC.Array.Height; j++)
                    {
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
                            C = point
                        });
                    }
                }
            }

            return Serpentize(points);
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
