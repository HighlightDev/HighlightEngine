using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
    public class TerrainGroupModel : IEntityGroupsModelBase
    {
        public TerrainGroupModel() : base()
        {
            this.TemplateType = "templateTerrain";
            this.LabelMain = "Terrain";
        }
    }
}
