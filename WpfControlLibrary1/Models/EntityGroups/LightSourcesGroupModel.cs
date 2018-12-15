using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
    public class LightSourcesGroupModel : IEntityGroupsModelBase
    {
        public LightSourcesGroupModel() : base()
        {
            this.TemplateType = "templateLightSources";
            this.LabelMain = "LightSources";
        }
    }
}
