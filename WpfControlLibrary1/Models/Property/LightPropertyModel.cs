using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.Property
{
    public class LightPropertyModel : IPropertyModelBase
    {
        public LightPropertyModel() : base()
        {
            this.TemplateType = "templateLightSources";
        }
    }
}
