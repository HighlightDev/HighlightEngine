using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
   public class SkyboxGroupModel : IEntityGroupsModelBase
    {
        public SkyboxGroupModel() : base()
        {
            this.TemplateType = "templateSkybox";
            this.LabelMain = "Skybox";
        }
    }
}
