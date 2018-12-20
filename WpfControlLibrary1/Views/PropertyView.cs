using System.Collections.ObjectModel;
using WpfControlLibrary1.Models.Property;

namespace WpfControlLibrary1.Views
{
    public class PropertyView
    {
        public ObservableCollection<IPropertyModelBase> PropertyEntries { set; get; }

        public PropertyView()
        {

        }

        public void FillPropertyEntries()
        {
            var entries = new ObservableCollection<IPropertyModelBase>
            {
                 new EntitiesPropertyModel()
               , new TerrainPropertyModel()
               , new SkyboxPropertyModel()
               , new WaterPlanesPropertyModel()
               , new SunMeshPropertyModel()
               , new LightPropertyModel()
            };

            PropertyEntries = entries;
        }
    }
}
