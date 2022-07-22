using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMessenger;

namespace IndiGames.Network
{

    public abstract class NetworkProtocol : INetworkProtocol
    {
        protected readonly Dictionary<string, Delegate> _handlerEventDictionary =
            new Dictionary<string, Delegate>();
        public abstract Task Close();
        public abstract Task Connect();
        public abstract Task Emit(string jsonString);
        public abstract Task Emit(byte[] bytes);
        public abstract Task Emit<T>(T data);
        protected abstract void OnMessage(byte[] data);
        public virtual void RegisterEventListener<T>(Action<T> listener) where T : class, ITinyMessage
        {
            // this.RegisterEventListener(new T().EventName, listener);
        }

        public virtual void UnregisterEventListener<T>(Action<T> listener) where T : class, ITinyMessage
        {
            // this.UnregisterEventListener(new T().EventName, listener);
        }

        public virtual void RegisterEventListener<T>(string eventName, Action<T> listener) where T : class, ITinyMessage
        {
            // if (!this._handlerEventDictionary.ContainsKey(eventName))
            // {
            //     this._handlerEventDictionary[eventName] = listener as Action<NetworkArgs>;
            // }

            // this._handlerEventDictionary[eventName] = Delegate.Combine(this._handlerEventDictionary[eventName], listener as Delegate);
        }

        public virtual void UnregisterEventListener<T>(string eventName, Action<T> listener) where T : class, ITinyMessage
        {
            // if (!this._handlerEventDictionary.ContainsKey(eventName))
            //     return;

            // this._handlerEventDictionary[eventName] = Delegate.Remove(this._handlerEventDictionary[eventName], listener as Delegate);
        }
    }
}