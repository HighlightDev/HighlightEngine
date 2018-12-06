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

namespace HighlightEngineUI
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
            Views.EngineObjectEntryView engineView = new Views.EngineObjectEntryView();
            engineView.TestCreateEntries();

            EngineObjectListBoxControl.DataContext = engineView;
        }
    }
}
