using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IndiGames.Network
{

    public class NetworkProtocol : INetworkProtocol
    {
        public Dictionary<string, EventHandler<string>> _listenerMap = new();

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

        public virtual Task Emit(string jsonString)
        {
            throw new NotImplementedException();
        }

        public virtual Task Emit(byte[] bytes)
        {
            throw new NotImplementedException();
        }

        public void RegisterEvent(string eventName, EventHandler<string> listener)
        {
            if (!this._listenerMap.TryGetValue(eventName, out EventHandler<string> eventListener))
            {
                this._listenerMap.Add(eventName, eventListener);
            }
            this._listenerMap[eventName] += listener;
        }


        public void UnregisterEventListener(string eventName, EventHandler<string> listener)
        {
            if (this._listenerMap.TryGetValue(eventName, out EventHandler<string> eventListener))
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