using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1.EventHandlerCore
{
    public class EventsListenerManager
    {
        private object m_locker = new object();
        private static EventsListenerManager m_managerInstance;

        private Queue<EventData> m_eventsQueue;

        public event Action NewEventAddedToQueue;

        public event Action EventWasRemovedFromQueue;

        private EventsListenerManager()
        {
            m_eventsQueue = new Queue<EventData>();
        }

        public void AddEventToQueue(EventData eventData)
        {
            lock (m_locker)
            {
                m_eventsQueue.Enqueue(eventData);
                NewEventAddedToQueue?.Invoke();
            }
        }

        public void SetEventsFilter(/*TODO: Filter filter*/)
        {
            // Filter events
        }

        public EventData DequeueElement()
        {
            lock (m_locker)
            {
                EventData result = null;

                result = m_eventsQueue.Dequeue();
                if (result != null)
                {
                    EventWasRemovedFromQueue?.Invoke();
                }

                return result;
            }
        }

        public static EventsListenerManager GetInstance()
        {
            if (m_managerInstance == null)
                m_managerInstance = new EventsListenerManager();

            return m_managerInstance;
        }

    }
}
