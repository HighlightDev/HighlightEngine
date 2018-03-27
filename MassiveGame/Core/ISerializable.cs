using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Core
{
    public interface ISerializable
    {
        void Serialize(BinaryFormatter formatter, FileStream fileStream);

        //public void SerializeComponent(string FileName, Type type)
        //{
        //    using (FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate))
        //    {
        //        serializer.Serialize(fileStream, componentContainer);
        //    }
        //}

        //public SerializedComponentsContainer DeserializerComponent(string PathToFile)
        //{
        //    SerializedComponentsContainer deserializedComponent = null;
        //    using (FileStream fileStream = new FileStream(PathToFile, FileMode.Open))
        //    {
        //        deserializedComponent = (SerializedComponentsContainer)serializer.Deserialize(fileStream);
        //    }
        //    return deserializedComponent;
        //}
    }
}
