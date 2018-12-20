using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfControlLibrary1.EventHandlerCore;

namespace WpfControlLibrary1
{
    public partial class PropertyDataTemplate : ResourceDictionary
    {
        private void SubHeaderCollapseFactory(FrameworkElement parent, string name)
        {
            FrameworkElement element = null;
            switch (name)
            {
                case "stackPanel_EntityMeshHeader": {
                        object o = parent.FindName("stackPanel_EntityMeshContent") as FrameworkElement;
                        element = o as FrameworkElement;
                        break;
                    }
                case "stackPanel_EntityTexturesHeader":
                    {
                        object o = parent.FindName("stackPanel_EntityTexturesContent");
                        element = o as FrameworkElement;
                        break;
                    }
            }

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
    }
}
