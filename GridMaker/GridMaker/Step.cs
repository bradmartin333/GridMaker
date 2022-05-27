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
        [XmlElement("Callback")]
        public bool Callback { get; set; } = false;

        public Step() { }

        public Step(Size array, PointF pitch, List<Point> skippedIndices, bool callback)
        {
            Array = array;
            Pitch = pitch;
            SkippedIndices = skippedIndices;
            Callback = callback;
        }

        public Step(Size array, PointF pitch, bool callback)
        {
            Array = array;
            Pitch = pitch;
            Callback = callback;
        }
    }
}
