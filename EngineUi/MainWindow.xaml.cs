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
using WpfControlLibrary1;
using WpfControlLibrary1.Models.Property;
using WpfControlLibrary1.Views;

namespace EngineUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void EngineObjectListBoxControl_Loaded(object sender, RoutedEventArgs e)
        {
            EngineObjectEntryView engineView = new EngineObjectEntryView();
            engineView.TestCreateEntries();
            (sender as EngineEntityListBox).DataContext = engineView;
        }

        private void list_createEntity_Loaded(object sender, RoutedEventArgs e)
        {
            EntityGroupsView entityGroupsView = new EntityGroupsView();
            entityGroupsView.FillEntityGroupsList();
            (sender as EngineListCreateEntity).DataContext = entityGroupsView;
        }

        private void property_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as PropertyWindow).PropertyTemplateType = new LightPropertyModel();
        }
    }
}

