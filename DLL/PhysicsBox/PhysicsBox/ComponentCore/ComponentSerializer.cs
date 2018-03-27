using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml.Serialization;
using System.Xml;

using OpenTK;
using System.Runtime.Serialization.Formatters.Binary;

namespace PhysicsBox.ComponentCore
{
    public class ComponentSerializer
    {
        private BinaryFormatter serializer = null;

        public ComponentSerializer()
        {
            serializer = new BinaryFormatter();
        }

        public void SerializeComponents(SerializedComponentsContainer componentContainer, string FileName)
        {
            using (FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fileStream, componentContainer);
            }
        }

        public SerializedComponentsContainer DeserializeComponents(string PathToFile)
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
