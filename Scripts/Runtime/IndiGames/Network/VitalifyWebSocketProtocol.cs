using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace IndiGames.Network
{
    public class VitalifyBaseArgs : NetworkMessage
    {
        [JsonProperty("codeKey")] public string CodeKey;
        [JsonProperty("message")] public string Message = "sendmessage";

        public override string EventName
        {
            get => this.CodeKey;
            set { }
        }
    }

    public class VitalifyWsProtocol : WebSocketProtocol
    {
        public VitalifyWsProtocol(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {
        }

        protected override void OnMessage(byte[] data)
        {
            string stringtifyData = Encoding.UTF8.GetString(data);
            Debug.Log("Received OnMessage! (" + data.Length + " bytes) " + stringtifyData);
            try
            {
                var deserializeData = JsonConvert.DeserializeObject<VitalifyBaseArgs>(stringtifyData);
                var eventName = deserializeData.CodeKey;

                if (this.HandlerEventDictionary.TryGetValue(eventName, out Type typeToCast))
                {
                    var publishClientData = JsonConvert.DeserializeObject(stringtifyData, typeToCast) as NetworkMessage;
                    this.MessageHub.Publish(publishClientData);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Fallback to base WebSocketProtocol] " + ex.GetType().Name + ": " + ex.Message);
                base.OnMessage(data);
            }
        }
    }
}