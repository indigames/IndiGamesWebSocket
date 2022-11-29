using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using TinyMessenger;
using UnityEngine;

namespace IndiGames.Network
{
    public class WebSocketProtocol : NetworkProtocol
    {
        public class WebSocketArgs : NetworkMessage
        {
            public override string EventName { get; set; } = "DefaultEvent";
        }

        public class WebSocketOpenArgs : WebSocketArgs
        {
            public override string EventName => "OnOpen";
        }

        public class WebSocketErrorArgs : WebSocketArgs
        {
            public override string EventName => "OnError";
            [JsonProperty("message")] public string Message;
        }

        public class WebSocketCloseArgs : WebSocketArgs
        {
            [JsonProperty("eventName")] public override string EventName => "OnClose";
            [JsonProperty("code")] public WebSocketCloseCode Code;
        }

        private WebSocket _webSocketInstance;

        public WebSocketProtocol(string url, Dictionary<string, string> headers = null)
        {
            this._webSocketInstance = new WebSocket(url, headers);

            this._webSocketInstance.OnOpen += this.OnOpen;
            this._webSocketInstance.OnError += this.OnError;
            this._webSocketInstance.OnClose += this.OnClose;
            this._webSocketInstance.OnMessage += this.OnMessage;
        }

        public virtual void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            this._webSocketInstance.DispatchMessageQueue();
#endif
        }

        public override async Task Emit<T>(T data)
        {
            if (this._webSocketInstance.State == WebSocketState.Open)
                await this._webSocketInstance.SendText(JsonConvert.SerializeObject(data));
        }

        public override async Task Emit(string simpleString)
        {
            if (this._webSocketInstance.State == WebSocketState.Open)
                await this._webSocketInstance.SendText(simpleString);
        }

        public override async Task Emit(byte[] bytes)
        {
            if (this._webSocketInstance.State == WebSocketState.Open)
                await this._webSocketInstance.Send(bytes);
        }

        public override async Task Connect()
        {
            await this._webSocketInstance.Connect();
        }

        public override async Task Close()
        {
            await this._webSocketInstance.Close();
        }

        protected override void OnMessage(byte[] data)
        {
            string stringtifyData = Encoding.UTF8.GetString(data);
            Debug.Log("Received OnMessage! (" + data.Length + " bytes) " + stringtifyData);
            var deserializeData = JsonConvert.DeserializeObject<WebSocketArgs>(stringtifyData);
            var eventName = deserializeData.EventName;

            if (eventName == null)
            {
                Debug.LogWarning($"Websocket cannot handle event of null\n{stringtifyData}");
                return;
            }

            if (this.HandlerEventDictionary.TryGetValue(eventName, out Type typeToCast))
            {
                this.MessageHub.Publish(Activator.CreateInstance(typeToCast) as NetworkMessage);
            }
        }

        public void EmitDebug(NetworkMessage message)
        {
#if UNITY_EDITOR
            this.OnMessage((Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message))));
#endif
        }

        private void OnClose(WebSocketCloseCode closeCode)
        {
            var args = new WebSocketCloseArgs()
            {
                Code = closeCode
            };
            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnError(string errorMsg)
        {
            var args = new WebSocketErrorArgs()
            {
                Message = errorMsg
            };
            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnOpen()
        {
            var args = new WebSocketOpenArgs();
            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }
    }
}