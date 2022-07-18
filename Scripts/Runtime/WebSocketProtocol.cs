using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;
using UnityEngine;

namespace IndiGames.Network
{
    public class WebSocketCloseEventArgs : EventArgs
    {
        public WebSocketCloseCode CloseCode;
    }

    public class WebSocketErrorEventArgs : EventArgs
    {
        public string ErrorMessage;
    }

    public class WebSocketArgs : EventArgs
    {
        public string EventName;
        public EventArgs Data;
    }

    public class WebSocketEvent
    {
        public static string OnOpen { get { return "OnOpen"; } }
        public static string OnClose { get { return "OnClose"; } }
        public static string OnEror { get { return "OnError"; } }
    }
    public class WebSocketProtocol : NetworkProtocol
    {
        private WebSocket _webSocketInstance;
        protected WebSocket WebSocketInstance
        {
            get;
        }

        public WebSocketProtocol(string url, Dictionary<string, string> headers = null)
        {
            this._webSocketInstance = new WebSocket(url, headers);

            this._webSocketInstance.OnOpen += this.OnOpen;
            this._webSocketInstance.OnError += this.OnError;
            this._webSocketInstance.OnClose += this.OnClose;
            this._webSocketInstance.OnMessage += this.OnMessage;
        }

        public override void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            this._webSocketInstance.DispatchMessageQueue();
#endif
        }

        public override Task Emit(string eventName, EventArgs data)
        {
            if (this._webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;

            var requestArgs = new WebSocketArgs()
            {
                EventName = eventName,
                Data = data
            };


            string message = JsonConvert.SerializeObject(requestArgs);
            Debug.Log("Emitting (" + message + ")");
            return this._webSocketInstance.SendText(message);
        }

        public override Task Emit(string simpleString)
        {
            if (this._webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;
            return this._webSocketInstance.SendText(simpleString);
        }

        public override Task Emit(byte[] bytes)
        {
            if (this._webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;
            return this._webSocketInstance.Send(bytes);
        }

        public override Task Connect()
        {
            return this._webSocketInstance.Connect();
        }

        public override Task Close()
        {
            return this._webSocketInstance.Close();
        }

        protected override void OnMessage(byte[] data)
        {
            string stringtifyData = Encoding.UTF8.GetString(data);
            Debug.Log("Received OnMessage! (" + data.Length + " bytes) " + stringtifyData);
            var deserializeData = JsonConvert.DeserializeObject<WebSocketArgs>(stringtifyData);

            if (this._listenerMap.TryGetValue(deserializeData.EventName, out EventHandler<string> eventListener))
            {
                if (eventListener == null)
                {
                    this._listenerMap.Add(deserializeData.EventName, null);
                }
                eventListener?.Invoke(this, stringtifyData);
            }
        }

        private void OnClose(WebSocketCloseCode closeCode)
        {
            var eventCloseArgs = new WebSocketCloseEventArgs()
            {
                CloseCode = closeCode
            };

            var args = new WebSocketArgs()
            {
                EventName = WebSocketEvent.OnClose,
                Data = eventCloseArgs
            };

            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnError(string errorMsg)
        {
            var eventErrorArgs = new WebSocketErrorEventArgs()
            {
                ErrorMessage = errorMsg
            };

            var args = new WebSocketArgs()
            {
                EventName = WebSocketEvent.OnEror,
                Data = eventErrorArgs
            };

            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }

        private void OnOpen()
        {
            var args = new WebSocketArgs()
            {
                EventName = WebSocketEvent.OnOpen
            };
            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }
    }
}