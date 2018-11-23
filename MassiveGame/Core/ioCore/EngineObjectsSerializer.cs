using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MassiveGame.Core.ioCore
{
    public class EngineObjectsSerializer : ICustomSerializer
    {
        private BinaryFormatter serializer = null;

        public EngineObjectsSerializer()
        {
            serializer = new BinaryFormatter();
        }

        public void Serialize(object serializableObject, string pathToFile)
        {
            using (FileStream fileStream = new FileStream(pathToFile, FileMode.OpenOrCreate))
            {
                serializer.Serialize(fileStream, serializableObject);
            }
        }

        public object Deserialize(string pathToFile)
        {
            object deserializedComponent = null;
            using (FileStream fileStream = new FileStream(pathToFile, FileMode.Open))
            {
                deserializedComponent = serializer.Deserialize(fileStream);
            }
            PostDeserializeObserver(deserializedComponent);
            return deserializedComponent;
        }

        public void PostDeserializeObserver(object deserializedObject)
        {
            IPostDeserializable obj = deserializedObject as IPostDeserializable;
            obj?.PostDeserializeInit();
        }
    }
}
