using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace MySite.Models
{
    public class Product
    {          
        [XmlAttribute]
        public int Id { get; set; }
        [XmlElement]
        public string Name { get; set; }
        [XmlElement]
        public bool State { get; set; }
    }
}