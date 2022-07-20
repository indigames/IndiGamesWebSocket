using System;
using System.Threading.Tasks;

namespace IndiGames.Network
{
    public interface INetworkProtocol
    {
        public void RegisterEvent(string eventName, EventHandler<string> listener);
        public void UnregisterEventListener(string eventName, EventHandler<string> listener);
        public Task Emit(string eventName, EventArgs data);
        public Task Emit(string jsonString);
        public Task Emit(Byte[] bytes);
        public Task Connect();
        public Task Close();
        public void DispatchMessageQueue();
        public void OnMessage(Byte[] bytes);
    }
}