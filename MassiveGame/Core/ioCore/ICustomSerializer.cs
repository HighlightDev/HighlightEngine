namespace MassiveGame.Core.ioCore
{
    public interface ICustomSerializer
    {
        void Serialize(object serializableObject, string pathToFile);
        object Deserialize(string pathToFile);
        void PostDeserializeObserver(object deserializedObject);
    }
}
