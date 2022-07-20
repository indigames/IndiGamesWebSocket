using System;
using System.Threading.Tasks;

namespace IndiGames.Network
{
    public interface INetworkProtocol
    {
        public void RegisterEvent<T>(string eventName, EventHandler<T> listener) where T : EventArgs;
        public void UnregisterEventListener<T>(string eventName, EventHandler<T> listener) where T : EventArgs;
        public Task Emit<T>(string eventName, T data) where T : EventArgs;
        public Task Emit(EventArgs data);
        public Task Emit(string jsonString);
        public Task Emit(Byte[] bytes);
        public Task Connect();
        public Task Close();
        public void DispatchMessageQueue();
    }
}