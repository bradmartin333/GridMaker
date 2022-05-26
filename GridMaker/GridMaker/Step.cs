using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Serialization;

namespace GridMaker
{
    [Serializable()]
    public class Step
    {
        [XmlElement("Array")]
        public Size Array { get; set; } = Size.Empty;
        [XmlElement("Pitch")]
        public PointF Pitch { get; set; } = PointF.Empty;
        [XmlArray("SkippedIndices")]
        public List<Point> SkippedIndices { get; set; } = new List<Point>();

        public Step() { }

        public Step(Size array, PointF pitch, List<Point> skippedIndices)
        {
            Array = array;
            Pitch = pitch;
            SkippedIndices = skippedIndices;
        }

        public Step(Size array, PointF pitch)
        {
            Array = array;
            Pitch = pitch;
        }
    }
}
