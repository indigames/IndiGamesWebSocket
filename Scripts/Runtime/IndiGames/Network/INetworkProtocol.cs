using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyMessenger;

namespace IndiGames.Network
{
    public abstract class NetworkMessage : ITinyMessage
    {
        [JsonProperty("eventName")] public abstract string EventName { get; set; }
        public object Sender { get; } = null;
    }

    public interface INetworkProtocol
    {
        public TinyMessageSubscriptionToken RegisterEventListener<T>(Action<T> listener)
            where T : NetworkMessage, new();

        public TinyMessageSubscriptionToken RegisterEventListener<T>(string eventName, Action<T> listener)
            where T : NetworkMessage, new();

        public void UnregisterEventListener<T>(TinyMessageSubscriptionToken subscriptionToken)
            where T : NetworkMessage, new();

        public void UnregisterEventListener(TinyMessageSubscriptionToken subscriptionToken);
        public Task Emit<T>(T data) where T : NetworkMessage;
        public Task Emit(string jsonString);
        public Task Emit(Byte[] bytes);
        public Task Connect();
        public Task Close();
    }
}