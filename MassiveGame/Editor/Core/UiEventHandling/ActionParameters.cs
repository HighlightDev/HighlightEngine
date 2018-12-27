using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MassiveGame.Editor.Core.UiEventHandling
{
    public enum ActionType
    {
        Load
    }

    public enum ParameterType
    {
        Entity,
        Skybox,
        Terrain,
        LightSource,
        WaterPlane,
        SunMesh
    }

    // this is base class
    public abstract class ActionParameters
    {
        public ActionType Type { private set; get; }

        public ActionParameters(ActionType type)
        {
            Type = type;
        }

        public abstract ParameterType GetParameterType();
    }

    public class EntityActionParameters : ActionParameters
    {
       


        public EntityActionParameters(ActionType type) : base(type)
        {

        }

        public override ParameterType GetParameterType()
        {
            return ParameterType.Entity;
        }
    }
}
