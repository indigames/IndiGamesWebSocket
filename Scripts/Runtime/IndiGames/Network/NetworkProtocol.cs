using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TinyMessenger;
using UnityEngine;

namespace IndiGames.Network
{
    public class ErrorHandler : ISubscriberErrorHandler
    {
        public void Handle(ITinyMessage message, Exception exception)
        {
            Debug.LogWarning("ErrorHandler::" + exception.GetType().Name + ": " + exception.Message);
        }
    }

    public abstract class NetworkProtocol : INetworkProtocol
    {
        protected readonly ITinyMessengerHub MessageHub;

        protected readonly Dictionary<string, Type> HandlerEventDictionary =
            new Dictionary<string, Type>();

        public abstract Task Close();
        public abstract Task Connect();
        public abstract Task Emit<T>(T data) where T : NetworkMessage;
        public abstract Task Emit(string jsonString);
        public abstract Task Emit(byte[] bytes);

        protected NetworkProtocol()
        {
            this.MessageHub = new TinyMessengerHub(new ErrorHandler());
        }

        public TinyMessageSubscriptionToken RegisterEventListener<T>(Action<T> listener) where T : NetworkMessage, new()
        {
            var message = new T();
            var eventName = message.EventName;
            return this.RegisterEventListener(eventName, listener);
        }

        public TinyMessageSubscriptionToken RegisterEventListener<T>(string eventName, Action<T> listener)
            where T : NetworkMessage, new()
        {
            if (!this.HandlerEventDictionary.ContainsKey(eventName))
                this.HandlerEventDictionary[eventName] = typeof(T);
            return this.MessageHub.Subscribe<T>(listener);
        }

        public void UnregisterEventListener<T>(TinyMessageSubscriptionToken subscriptionToken)
            where T : NetworkMessage, new()
        {
            this.UnregisterEventListener(subscriptionToken);
        }

        public void UnregisterEventListener(TinyMessageSubscriptionToken subscriptionToken)
        {
            this.MessageHub.Unsubscribe(subscriptionToken);
        }

        protected abstract void OnMessage(byte[] data);
    }
}