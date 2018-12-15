using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfControlLibrary1.EventHandlerCore;

namespace WpfControlLibrary1
{
    public partial class EntityGroupsDataTemplateResources : ResourceDictionary
    {
        private void EntryStackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Add event to pool
            EventsListenerManager.GetInstance().AddEventToQueue(new EventData() { Sender = sender, Args = e });
        }
    }
}
