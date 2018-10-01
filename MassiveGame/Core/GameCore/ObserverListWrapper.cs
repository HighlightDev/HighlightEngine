using MassiveGame.Core.GameCore.Entities;
using System;
using System.Collections.Generic;
using System.Collections;

namespace MassiveGame.Core.GameCore
{
    public class ObserverListWrapper<T> : IEnumerable<T>
        where T : IObservable
    {
        private List<T> m_dataList;

        public ObserverListWrapper()
        {
            m_dataList = new List<T>();
        }

        public T this[Int32 index]
        {
            set { m_dataList[index] = value; }
            get { return m_dataList[index]; }
        }

        public void AddToList(T obj)
        {
            obj.NotifyAdded();
            m_dataList.Add(obj);
        }

        public void AddToList(params T[] objects)
        {
            for (Int32 i = 0; i< objects.Length; i++)
            {
                objects[i].NotifyAdded();
            }
            m_dataList.AddRange(objects);
        }

        public void AddRangeToList(List<T> objects)
        {
            objects.ForEach(input => input.NotifyAdded());
            m_dataList.AddRange(objects);
        }

        public void RemoveFromList(T obj)
        {
            obj.NotifyRemoved();
            m_dataList.Remove(obj);
        }

        public void RemoveRangeFromList(List<T> objects)
        {
            objects.ForEach(input => input.NotifyRemoved());
            for (Int32 i = 0; i < m_dataList.Count; i++)
            {
                m_dataList.Remove(m_dataList[i]);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)m_dataList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)m_dataList).GetEnumerator();
        }
    }

}
