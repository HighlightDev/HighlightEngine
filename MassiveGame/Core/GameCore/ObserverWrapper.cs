using MassiveGame.Core.GameCore.Entities;
using MassiveGame.Core.ioCore;
using System;
using System.Runtime.Serialization;

namespace MassiveGame.Core.GameCore
{
    [Serializable]
    public class ObserverWrapper<T> : ISerializable, IPostDeserializable
         where T : class, IObservable
    {
        private T m_data;

        public ObserverWrapper()
        {
        }

        public T GetData()
        {
            return m_data;
        }

        public void AddToWrapper(T obj)
        {
            obj.NotifyAdded();
            m_data = obj;
        }
       
        public void ClearWrapper()
        {
            m_data?.NotifyRemoved();
            m_data = null;
        }
     
        #region Serialization

        public void PostDeserializeInit()
        {
            m_data.NotifyAdded();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(string.Format("data{0}", ToString()), m_data, typeof(T));
        }

        protected ObserverWrapper(SerializationInfo info, StreamingContext context)
        {
            m_data = (T)info.GetValue(string.Format("data{0}", ToString()), typeof(T));
        }

        #endregion
    }
}
