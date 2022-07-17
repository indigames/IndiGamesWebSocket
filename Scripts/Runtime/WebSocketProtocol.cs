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
        protected WebSocket webSocketInstance;

        public event WebSocketMessageEventHandler OnMessageEvent;
        public event WebSocketOpenEventHandler OnOpenEvent;
        public event WebSocketErrorEventHandler OnErrorEvent;
        public event WebSocketCloseEventHandler OnCloseEvent;
        public WebSocketProtocol(string url, Dictionary<string, string> headers = null)
        {
            this.webSocketInstance = new WebSocket(url, headers);

            this.webSocketInstance.OnOpen += this.OnOpen;
            this.webSocketInstance.OnError += this.OnError;
            this.webSocketInstance.OnClose += this.OnClose;
            this.webSocketInstance.OnMessage += this.OnMessage;
        }

        public override void DispatchMessageQueue()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            this.webSocketInstance.DispatchMessageQueue();
#endif
        }

        public override Task Emit(string eventName, EventArgs data)
        {
            if (this.webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;

            var requestArgs = new WebSocketArgs()
            {
                EventName = eventName,
                Data = data
            };


            string message = JsonConvert.SerializeObject(requestArgs);
            Debug.Log("Emitting (" + message + ")");
            return this.webSocketInstance.SendText(message);
        }

        public Task Emit(string simpleString)
        {
            if (this.webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;
            return this.webSocketInstance.SendText(simpleString);
        }

        public Task Emit(byte[] bytes)
        {
            if (this.webSocketInstance.State != WebSocketState.Open)
                return Task.CompletedTask;
            return this.webSocketInstance.Send(bytes);
        }

        public override Task Connect()
        {
            return this.webSocketInstance.Connect();
        }

        public override Task Close()
        {
            return this.webSocketInstance.Close();
        }

        protected override void OnMessage(byte[] data)
        {
            this.OnMessageEvent?.Invoke(data);

            string value = Encoding.UTF8.GetString(data);
            Debug.Log("Received OnMessage! (" + data.Length + " bytes) " + value);
            var deserializeData = JsonConvert.DeserializeObject<WebSocketArgs>(value);

            if (this._listenerMap.TryGetValue(deserializeData.EventName, out EventHandler<EventArgs> eventListener))
            {
                if (eventListener == null)
                {
                    this._listenerMap.Add(deserializeData.EventName, null);
                }
                eventListener?.Invoke(this, deserializeData.Data);
            }
        }

        private void OnClose(WebSocketCloseCode closeCode)
        {
            this.OnCloseEvent?.Invoke(closeCode);
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
            this.OnErrorEvent?.Invoke(errorMsg);

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
            this.OnOpenEvent?.Invoke();

            var args = new WebSocketArgs()
            {
                EventName = WebSocketEvent.OnOpen
            };
            this.OnMessage(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(args)));
        }
    }
}