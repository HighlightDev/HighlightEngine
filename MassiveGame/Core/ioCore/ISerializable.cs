using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MassiveGame.Core.ioCore
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
