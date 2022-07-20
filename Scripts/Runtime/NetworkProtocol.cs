using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace IndiGames.Network
{

    public abstract class NetworkProtocol : INetworkProtocol
    {
        protected Dictionary<string, EventHandler<EventArgs>> _listenerMap = new Dictionary<string, EventHandler<EventArgs>>();
        public abstract Task Close();
        public abstract Task Connect();
        public abstract void DispatchMessageQueue();
        public abstract Task Emit<T>(string eventName, T data) where T : EventArgs;
        public abstract Task Emit(string jsonString);
        public abstract Task Emit(byte[] bytes);
        public abstract Task Emit(EventArgs data);

        public void RegisterEvent<T>(string eventName, EventHandler<T> listener) where T : EventArgs
        {
            if (!this._listenerMap.TryGetValue(eventName, out EventHandler<EventArgs> eventListener))
            {
                this._listenerMap.Add(eventName, eventListener);
            }
            this._listenerMap[eventName] += listener;
        }

        public void UnregisterEventListener<T>(string eventName, EventHandler<T> listener) where T : EventArgs
        {
            if (this._listenerMap.TryGetValue(eventName, out EventHandler<EventArgs> eventListener))
            {
                eventListener -= listener;
                this._listenerMap[eventName] = eventListener;
            }
        }

        protected abstract void OnMessage(byte[] data);
    }
}