using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Xml;
using PhysicsBox.ComponentCore;
using OpenTK;
using OTKWinForm.Core;
using System.Runtime.Serialization.Formatters.Binary;

namespace OTKWinForm.IOCore
{
    public class Serializer
    {
        private BinaryFormatter serializer = null;

        public Serializer()
        {
            serializer = new BinaryFormatter();
        }

        public void SerializeComponent(SerializedComponentsContainer componentContainer, string FileName)
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fileStream, componentContainer);
            }
        }

        public SerializedComponentsContainer DeserializerComponent(string PathToFile)
        {
            SerializedComponentsContainer deserializedComponent = null;
            using (FileStream fileStream = new FileStream(PathToFile, FileMode.Open))
            {
                deserializedComponent = (SerializedComponentsContainer)serializer.Deserialize(fileStream);
            }
            return deserializedComponent;
        }
    }
}
