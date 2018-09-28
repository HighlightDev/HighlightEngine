using MassiveGame.Core.ioCore;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

namespace MassiveGame.CollisionEditor.Core.SerializeAPI
{
    public class CollisionComponentSerializer : ISerializer
    {
        private BinaryFormatter serializer = null;

        public CollisionComponentSerializer()
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
            CollisionComponentsWrapper deserializedComponent = null;
            using (FileStream fileStream = new FileStream(pathToFile, FileMode.Open))
            {
                deserializedComponent = (CollisionComponentsWrapper)serializer.Deserialize(fileStream);
            }
            PostDeserializeObserver(deserializedComponent);
            return deserializedComponent;
        }

        public void PostDeserializeObserver(object deserializedObject)
        {
            CollisionComponentsWrapper wrapper = deserializedObject as CollisionComponentsWrapper;
            if (wrapper == null)
                throw new InvalidCastException();

            foreach (var component in wrapper.SerializedComponents)
            {
                component.PostDeserializeInit();
            }
        }
    }
}
