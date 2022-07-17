using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndiGames.Network
{
    public interface INetworkProtocol
    {
        public void RegisterEvent(string eventName, EventHandler<EventArgs> listener);
        public void UnregisterEventListener(string eventName, EventHandler<EventArgs> listener);
        public Task Emit(string eventName, EventArgs data);
        public Task Connect();
        public Task Close();
        public void DispatchMessageQueue();
    }

    public class NetworkProtocol : INetworkProtocol
    {
        public Dictionary<string, EventHandler<EventArgs>> _listenerMap = new();

        public virtual Task Close()
        {
            throw new NotImplementedException();
        }

        public virtual Task Connect()
        {
            throw new NotImplementedException();
        }

        public virtual void DispatchMessageQueue()
        {
            throw new NotImplementedException();
        }

        public virtual Task Emit(string eventName, EventArgs data)
        {
            throw new NotImplementedException();
        }

        public void RegisterEvent(string eventName, EventHandler<EventArgs> listener)
        {
            if (!this._listenerMap.TryGetValue(eventName, out EventHandler<EventArgs> eventListener))
            {
                this._listenerMap.Add(eventName, eventListener);
            }
            this._listenerMap[eventName] += listener;
        }


        public void UnregisterEventListener(string eventName, EventHandler<EventArgs> listener)
        {
            if (this._listenerMap.TryGetValue(eventName, out EventHandler<EventArgs> eventListener))
            {
                eventListener -= listener;
                this._listenerMap[eventName] = eventListener;
            }
        }

        protected virtual void OnMessage(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}