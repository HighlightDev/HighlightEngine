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
using WpfControlLibrary1.Models.ImageResources;
using WpfControlLibrary1.Models.Property;
using WpfControlLibrary1.ResourceLoader;

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for PropertyWindow.xaml
    /// </summary>
    public partial class PropertyWindow : UserControl
    {
        private IPropertyModelBase m_propertyModelBase;
        public IPropertyModelBase PropertyTemplateType
        {
            set
            {
                if (m_propertyModelBase != value)
                {
                    m_propertyModelBase = value;
                    (FindName("CustomItemsControl") as FrameworkElement).DataContext = value;
                }
            }
            get { return m_propertyModelBase; }
        }

        public PropertyWindow()
        {
            InitializeComponent();
        }

        private void scrollviewer_1_MouseEnter(object sender, MouseEventArgs e)
        {
            var scrollViewer = (sender as ScrollViewer);
            scrollViewer?.Focus();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object header = FindName("CustomItemsControl");
            FrameworkElement headerElement = header as FrameworkElement;
            headerElement.Visibility = headerElement.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PropertyContent_Loaded(object sender, RoutedEventArgs e)
        {
            //(sender as FrameworkElement).DataContext = PropertyTemplateType;
        }

        private void headerImageLoaded(object sender, RoutedEventArgs e)
        {
            string headerImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\hide.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(headerImageSrc);
        }

    }
}
