using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
    public class SunMeshGroupModel : IEntityGroupsModelBase
    {
        public SunMeshGroupModel() : base()
        {
            this.TemplateType = "templateSunMesh";
            this.LabelMain = "Sun Mesh";
        }
    }
}
