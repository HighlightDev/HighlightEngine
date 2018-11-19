using MassiveGame.Core.GameCore.Entities;
using System;
using System.Collections.Generic;
using System.Collections;
using MassiveGame.Core.ioCore;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore
{
    [Serializable]
    public class ObserverListWrapper<T> : IEnumerable<T>, ISerializable, IPostDeserializable
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
            if (objects != null)
            {
                for (Int32 i = 0; i < objects.Length; i++)
                {
                    objects[i].NotifyAdded();
                }
                m_dataList.AddRange(objects);
            }
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

        public void ClearList()
        {
            m_dataList.ForEach(input => input.NotifyRemoved());
            m_dataList.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)m_dataList).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)m_dataList).GetEnumerator();
        }

        #region Serialization

        public void PostDeserializeInit()
        {
            for (Int32 i = 0; i < m_dataList.Count; i++)
            {
                m_dataList[i].NotifyAdded();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(string.Format("data{0}", ToString()), m_dataList, typeof(List<T>));
        }

        protected ObserverListWrapper(SerializationInfo info, StreamingContext context)
        {
            m_dataList = (List<T>)info.GetValue(string.Format("data{0}", ToString()), typeof(List<T>));
        }

        #endregion
    }

}
