using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CoatingSupport.Entities
{
    [XmlRoot("configuration")]
    public class SystemSetting
    {
        [XmlElement(ElementName = "PortName")]
        public string portName { get; set; }
        [XmlElement(ElementName = "BaudRate")]
        public int baudRate { get; set; }
        [XmlElement(ElementName = "DataBits")]
        public int dataBits { get; set; }
        [XmlElement(ElementName = "Parity")]
        public string parity { get; set; }
        [XmlElement(ElementName = "StopBits")]
        public string stopBits { get; set; }
        public static int ReadXML<Type>(out Type pClass, string pPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Type));
            using (FileStream stream = new FileStream(pPath, FileMode.Open))
            {
                pClass = (Type)serializer.Deserialize(stream);
            }
            return 0;
        }
        public static int WriteXML<Type>(Type pClass, string pPath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Type));
            using (FileStream stream = new FileStream(pPath, FileMode.Create))
            {
                serializer.Serialize((Stream)stream, pClass);
            }
            return 0;
        }
    }
}
