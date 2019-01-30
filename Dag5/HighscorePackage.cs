using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Dag5
{
    [Serializable] //Vi skal definere at klassen kan serializes
   public class HighscorePackage
    {
        //Vi skal definere tags for xml dokumentet
        [XmlAttribute("Message")]
        public string Message { get; set; }

        [XmlAttribute("Number")]
        public int Number { get; set; }
    }
}
