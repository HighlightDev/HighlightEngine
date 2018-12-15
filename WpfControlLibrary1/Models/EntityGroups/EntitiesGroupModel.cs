using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.Models.EntityGroups
{
    public class EntitiesGroupModel : IEntityGroupsModelBase
    {
        public EntitiesGroupModel() : base()
        {
            this.TemplateType = "templateEntities";
            this.LabelMain = "Entities";
        }
    }
}
