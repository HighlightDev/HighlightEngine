using MassiveGame.API.Mesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextureLoader;

namespace MassiveGame.Editor.Core.Entities.PreviewTemplateParameters
{
    public class StaticMeshTemplateParameters : ITemplateParameters
    {
        private ITexture m_albedo = null, 
                         m_normalMap = null,
                         m_specularMap = null;

        private Skin m_mesh = null;

        public void SetAlbedo(string albedoPath)
        {

        }

        public void SetNormalMap(string nmPath)
        {

        }

        public void SetSpecularMap(string smPath)
        {

        }

        public void SetMesh(string meshPath)
        {

        }

        public StaticMeshTemplateParameters()
        {

        }

        public EntityType GetPreviewEntityType()
        {
            return EntityType.StaticEntity;
        }
    }
}
