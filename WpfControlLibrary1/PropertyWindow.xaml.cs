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
        public IPropertyModelBase PropertyTemplateType { set; get; }

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

        private void stackPanel_TransformationHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            object content = FindName("stackPanel_TransformationContent");
            FrameworkElement contentElement = content as FrameworkElement;
            contentElement.Visibility = contentElement.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void PropertyContent_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as FrameworkElement).DataContext = PropertyTemplateType;
        }

        private void headerImageLoaded(object sender, RoutedEventArgs e)
        {
            string headerImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\hide.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(headerImageSrc);
        }

        private void subheaderImageLoaded(object sender, RoutedEventArgs e)
        {
            string subheaderImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\submenu_indicator.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(subheaderImageSrc);
        }

        private void ImageLeftArrowLoaded(object sender, RoutedEventArgs e)
        {
            string leftArrowImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\arrow_left.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(leftArrowImageSrc);
        }

        private void ImageRightArrowLoaded(object sender, RoutedEventArgs e)
        {
            string rightArrowImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\arrow_right.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(rightArrowImageSrc);
        }
    }
}
