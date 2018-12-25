using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfControlLibrary1.Models.Property;
using WpfControlLibrary1.Views;

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for EngineToolbar_Right.xaml
    /// </summary>
    public partial class EngineToolbar_Right : UserControl
    {
        public Action DelayedInitializeDone { set; get; }

        public EngineToolbar_Right()
        {
            InitializeComponent();
        }

        public EngineListCreateEntity CreateEntityList { set; get; }
        public PropertyWindow PropertyWindow { set; get; }

        private void InitEmpty()
        {
            propertiesList.PropertyTemplateType = new EmptyPropertyModel();
            EntityGroupsView entityGroupsView = new EntityGroupsView();
            entityGroupsView.FillEntityGroupsList();
            createEntityList.DataContext = entityGroupsView;
        }

        private void RightToolbarLoaded(object sender, RoutedEventArgs e)
        {
            CreateEntityList = createEntityList;
            PropertyWindow = propertiesList;
            InitEmpty();

            DelayedInitializeDone?.Invoke();
        }
    }
}
