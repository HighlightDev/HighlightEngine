using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfControlLibrary1.EventHandlerCore;
using WpfControlLibrary1.Models.ImageResources;
using WpfControlLibrary1.ResourceLoader;

namespace WpfControlLibrary1
{
    public partial class PropertyDataTemplate : ResourceDictionary
    {
        private void SubHeaderCollapseFactory(FrameworkElement parent, string name)
        {
            FrameworkElement element = null;
            string contentElement = string.Empty;
            switch (name)
            {
                case "stackPanel_EntityMeshHeader": {
                        contentElement = "stackPanel_EntityMeshContent";
                        break;
                    }
                case "stackPanel_EntityTexturesHeader":
                    {
                        contentElement = "stackPanel_EntityTexturesContent";
                        break;
                    }
                case "stackPanel_SkyboxCubemapHeader":
                    {
                        contentElement = "stackPanel_SkyboxCubemapContent";
                        break;
                    }
                case "stackPanel_TerrainHeightMapHeader":
                    {
                        contentElement = "stackPanel_TerrainHeightMapContent";
                        break;
                    }
                case "stackPanel_TerrainMultitexturingHeader":
                    {
                        contentElement = "stackPanel_TerrainMultitexturingContent";
                        break;
                    }
                case "stackPanel_LightColorHeader":
                    {
                        contentElement = "stackPanel_LightColorContent";
                        break;
                    }
                case "stackPanel_WaterNormalMapHeader":
                    {
                        contentElement = "stackPanel_WaterNormalMapContent";
                        break;
                    }
                case "stackPanel_WaterDistortionHeader":
                    {
                        contentElement = "stackPanel_WaterDistortionContent";
                        break;
                    }
            }

            element = parent.FindName(contentElement) as FrameworkElement;
            if (element != null)
                element.Visibility = element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SubHeader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement eventOwner = sender as FrameworkElement;
            string name = eventOwner.Name;
            FrameworkElement parent = (FrameworkElement)((DockPanel)sender).Parent;
            SubHeaderCollapseFactory(parent, name);
        }

        private void TextBlockSelector_MouseDown(object sender, MouseButtonEventArgs e)
        {
            EventsListenerManager.GetInstance().AddEventToQueue(new EventData() { Sender = sender, Args = e });
        }

        private void subheaderImageLoaded(object sender, RoutedEventArgs e)
        {
            string subheaderImageSrc = ResourceIO.GetInstance().GetTexturePath() + "editor\\submenu_indicator.png";
            (sender as FrameworkElement).DataContext = new DefaultImageModel(subheaderImageSrc);
        }
    }
}
