using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace GridMaker
{
    /// <summary>
    /// Class for turning a Grid into a list of Nodes
    /// </summary>
    public class Generator
    {
        private readonly PointF SW, SE, Origin;
        private PointF CenterOfRotation;

        /// <summary>
        /// A B or C Node
        /// </summary>
        public enum NodeType
        {
            A, B, C
        }

        /// <summary>
        /// Storage for Grid points
        /// </summary>
        public struct Node
        {
            /// <summary>
            /// Whether the Node is on the A, B or C grid
            /// </summary>
            public NodeType NodeType;
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
        /// Stage coordinates in mm of SE corner of substrate
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
            PointF copy = Origin;
            RotatePoint(ref copy, true);
            CenterOfRotation = new PointF(Origin.X - copy.X, Origin.Y - copy.Y);
        }

        /// <summary>
        /// Generate the Grid node
        /// </summary>
        /// <returns>
        /// List of nodes that make up the grid in a serpentine order
        /// </returns>
        public List<Node> Generate()
        {
            List<Node> basePoints = new List<Node>();
            for (int i = 0; i < Composer.Grid.StepA.Array.Width; i++)
                for (int j = 0; j < Composer.Grid.StepA.Array.Height; j++)
                {
                    Point point = new Point(i, j);
                    if (Composer.Grid.StepA.SkippedIndices.Contains(point)) continue;
                    basePoints.Add(new Node()
                    {
                        NodeType = NodeType.A,
                        Location = new PointF(
                            i * Composer.Grid.StepA.Pitch.X,
                            j * Composer.Grid.StepA.Pitch.Y),
                        A = point,
                        B = Point.Empty,
                        C = Point.Empty,
                        Callback = Composer.Grid.StepA.Callback,
                    });
                }

            List<Node> points = new List<Node>();
            for (int b = 0; b < basePoints.Count(); b++)
            {
                List<Node> regionPoints = new List<Node>() { basePoints[b] };
                for (int i = 0; i < Composer.Grid.StepB.Array.Width; i++)
                    for (int j = 0; j < Composer.Grid.StepB.Array.Height; j++)
                    {
                        Point pointB = new Point(i, j);
                        if (Composer.Grid.StepB.SkippedIndices.Contains(pointB)) continue;
                        Node nodeB = new Node
                        {
                            NodeType = NodeType.B,
                            Location = new PointF(
                                basePoints[b].Location.X + i * Composer.Grid.StepB.Pitch.X,
                                basePoints[b].Location.Y + j * Composer.Grid.StepB.Pitch.Y),
                            A = basePoints[b].A,
                            B = pointB,
                            C = Point.Empty,
                            Callback = Composer.Grid.StepB.Callback,
                        };
                        if (pointB != Point.Empty) regionPoints.Add(nodeB);
                        for (int k = 0; k < Composer.Grid.StepC.Array.Width; k++)
                            for (int l = 0; l < Composer.Grid.StepC.Array.Height; l++)
                            {
                                Point pointC = new Point(k, l);
                                if (pointC == Point.Empty) continue;
                                if (Composer.Grid.StepC.SkippedIndices.Contains(pointC)) continue;
                                Node nodeC = new Node
                                {
                                    NodeType = NodeType.C,
                                    Location = new PointF(
                                        nodeB.Location.X + k * Composer.Grid.StepC.Pitch.X,
                                        nodeB.Location.Y + l * Composer.Grid.StepC.Pitch.Y),
                                    A = nodeB.A,
                                    B = nodeB.B,
                                    C = pointC,
                                    Callback = Composer.Grid.StepC.Callback,
                                };
                                if (pointC != Point.Empty) regionPoints.Add(nodeC);
                            }
                    }
                points.AddRange(Serpentize(regionPoints));
            }

            return points;
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
