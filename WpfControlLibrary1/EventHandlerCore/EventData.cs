using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.EventHandlerCore
{
    public class EventData
    {
        public object Sender { set; get; }
        public EventArgs Args { set; get; }
    }
}
