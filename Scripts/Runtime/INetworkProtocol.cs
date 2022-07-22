using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TinyMessenger;

namespace IndiGames.Network
{
    public interface INetworkProtocol
    {
        public void RegisterEventListener<T>(Action<T> listener) where T : class, ITinyMessage;
        public void RegisterEventListener<T>(string eventName, Action<T> listener) where T : class, ITinyMessage;
        public void UnregisterEventListener<T>(Action<T> listener) where T : class, ITinyMessage;
        public void UnregisterEventListener<T>(string eventName, Action<T> listener) where T : class, ITinyMessage;
        public Task Emit<T>(T data);
        public Task Emit(string jsonString);
        public Task Emit(Byte[] bytes);
        public Task Connect();
        public Task Close();
    }
}