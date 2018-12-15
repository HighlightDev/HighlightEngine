using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
    public class WaterPlanesGroupModel : IEntityGroupsModelBase
    {
        public WaterPlanesGroupModel() : base()
        {
            this.TemplateType = "templateWaterPlanes";
            this.LabelMain = "Water Planes";
        }
    }
}
