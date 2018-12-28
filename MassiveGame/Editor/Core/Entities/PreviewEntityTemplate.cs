using MassiveGame.Editor.Core.Entities.PreviewTemplateParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Editor.Core.Entities
{
    public enum EntityType
    {
        StaticEntity,
        MovableEntity,
        SkeletalEntity,
        TerrainEntity,
        SkyboxEntity,
        DirectionalLightEntity, 
        PointLightEntity,
        SpotLightEntity,
        WaterPlaneEntity,
        SunEntity
    }
    
    public class PreviewEntityTemplate
    {
        public ITemplateParameters TemplateParameters { private set; get; } = null;

        public PreviewEntityTemplate(ITemplateParameters parameters)
        {
            TemplateParameters = parameters;
        }
    }
}
