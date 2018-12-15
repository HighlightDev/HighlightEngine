using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfControlLibrary1.Models;
using WpfControlLibrary1.Models.EntityGroups;

namespace WpfControlLibrary1.Views
{
    public class EntityGroupsView
    {
        public ObservableCollection<IEntityGroupsModelBase> EntityGroups { set; get; }

        public void FillEntityGroupsList()
        {
            string currentDir = Environment.CurrentDirectory;
            Int32 index = currentDir.IndexOf("HighlightEngine");
            string newPath = currentDir.Substring(0, index);

            var entries = new ObservableCollection<IEntityGroupsModelBase>
            {
                 new EntitiesGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
               , new TerrainGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
               , new SkyboxGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
               , new WaterPlanesGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
               , new SunMeshGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
               , new LightSourcesGroupModel {
                    IconMain = newPath + "\\HighlightEngine\\MassiveGame\\Textures\\EditorTextures\\icon2.png" }
            };

            EntityGroups = entries;
        }

    }
}
