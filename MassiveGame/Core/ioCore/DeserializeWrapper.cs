using MassiveGame.CollisionEditor.Core.SerializeAPI;

namespace MassiveGame.Core.ioCore
{
    public class DeserializeWrapper
    {
        ICustomSerializer GetDeserializer(string extension)
        {
            ICustomSerializer serializer = null;
            switch (extension)
            {
                case "cl":
                    serializer = new CollisionComponentSerializer(); break;
                default:
                    serializer = new EngineObjectsSerializer(); break;
            }

            return serializer;
        }

        public ReturnType Deserialize<ReturnType>(string pathToFile) where ReturnType : class
        {
            string extension = pathToFile.Split('.')[1];
            ICustomSerializer deserializer = GetDeserializer(extension);
            object result = deserializer.Deserialize(pathToFile);
            return (result as ReturnType);
        }

    }
}
