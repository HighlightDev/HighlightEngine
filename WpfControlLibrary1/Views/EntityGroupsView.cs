using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary1.Models;
using WpfControlLibrary1.Models.EntityGroups;
using WpfControlLibrary1.ResourceLoader;

namespace WpfControlLibrary1.Views
{
    public class EntityGroupsView
    {
        public ObservableCollection<IEntityGroupsModelBase> EntityGroups { set; get; }

        public void FillEntityGroupsList()
        {
            var entries = new ObservableCollection<IEntityGroupsModelBase>
            {
                 new EntitiesGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
               , new TerrainGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
               , new SkyboxGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
               , new WaterPlanesGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
               , new SunMeshGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
               , new LightSourcesGroupModel {
                    IconMain = ResourceIO.GetInstance().GetTexturePath() + "editor\\icon2.png" }
            };

            EntityGroups = entries;
        }

    }
}
