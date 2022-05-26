using System;
using System.Xml.Serialization;

namespace GridMaker
{
    [Serializable()]
    public class Grid
    {
        [XmlElement("Name")]
        public string Name { get; set; } = "Name";
        [XmlElement("StepA")]
        public Step StepA { get; set; } = new Step();
        [XmlElement("StepB")]
        public Step StepB { get; set; } = new Step();
        [XmlElement("StepC")]
        public Step StepC { get; set; } = new Step();

        public Grid() { }
    }
}
