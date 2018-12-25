using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.Property
{
    public class EmptyPropertyModel : IPropertyModelBase
    {
        public EmptyPropertyModel() : base()
        {
            this.TemplateType = "emptyProperty";
        }
    }
}
