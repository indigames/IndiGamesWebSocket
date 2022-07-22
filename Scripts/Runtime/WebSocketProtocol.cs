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
        public class WebSocketArgs : ITinyMessage
        {
            [JsonProperty("data")]
            public EventArgs Data;

            public object Sender => throw new NotImplementedException();
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
            if (this._webSocketInstance.State != WebSocketState.Open)
                return;

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
            try
            {
                // var deserializeData = JsonConvert.DeserializeObject<WebSocketArgs>(stringtifyData);

                // string eventName = deserializeData.EventName;
                // if (!this._handlerEventDictionary.ContainsKey(eventName))
                //     return;

                // var actionDel = this._handlerEventDictionary[eventName];
                // (actionDel as Action<T>)?.Invoke(JsonConvert.DeserializeObject<T>(stringtifyData));
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex.GetType().Name + ": " + ex.Message);
            }
        }

        private void OnClose(WebSocketCloseCode closeCode)
        {
            // var args = new WebSocketCloseArgs();
            // args.Data.Code = closeCode;

            // this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnError(string errorMsg)
        {
            // var args = new WebSocketErrorArgs();
            // args.Data.ErrorMessage = errorMsg;

            // this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnOpen()
        {
            // var args = new WebSocketOpenArgs();
            // this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }
    }
}