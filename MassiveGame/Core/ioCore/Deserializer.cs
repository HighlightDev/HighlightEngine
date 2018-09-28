using MassiveGame.CollisionEditor.Core.SerializeAPI;

namespace MassiveGame.Core.ioCore
{
    public class Deserializer
    {
        ISerializer GetDeserializer(string extension)
        {
            ISerializer serializer = null;
            switch (extension)
            {
                case "cl":
                    serializer = new CollisionComponentSerializer(); break;
                default: break;
            }

            return serializer;
        }

        public ReturnType Deserialize<ReturnType>(string pathToFile) where ReturnType : class, new()
        {
            string extension = pathToFile.Split('.')[1];
            ISerializer deserializer = GetDeserializer(extension);
            object result = deserializer.Deserialize(pathToFile);
            return (result as ReturnType);
        }

    }
}
