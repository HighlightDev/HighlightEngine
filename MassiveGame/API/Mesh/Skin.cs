using VBO;

namespace MassiveGame.API.Mesh
{
    public class Skin
    {
        public VertexArrayObject Buffer { private set; get; }

        public Skin(VertexArrayObject buffer)
        {
            Buffer = buffer;
        }

        public virtual void CleanUp()
        {
            Buffer.CleanUp();
        }
    }
}
