
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace IndiGames.Network.Vitalify
{
    // public abstract class VitalifyBaseArgs : NetworkArgs
    // {
    //     [JsonProperty("codeKey")]
    //     public string CodeKey;
    //     [JsonProperty("message")]
    //     public string Message = "sendmessage";
    // }

    // public class VitalifyRequestArgs : VitalifyBaseArgs
    // {
    //     [JsonProperty("requestId")]
    //     public string RequestId = Guid.NewGuid().ToString();
    // }

    public class VitalifyWSProtocol : WebSocketProtocol
    {
        public VitalifyWSProtocol(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {
        }

        protected override void OnMessage(byte[] data)
        {
            string stringtifyData = Encoding.UTF8.GetString(data);
            Debug.Log("Received OnMessage! (" + data.Length + " bytes) " + stringtifyData);
            try
            {
                // var deserializeData = JsonConvert.DeserializeObject<VitalifyBaseArgs>(stringtifyData);

                // if (this._listenerMap.TryGetValue(deserializeData.CodeKey, out EventHandler<string> eventListener))
                // {
                //     if (eventListener == null)
                //     {
                //         this._listenerMap.Add(deserializeData.CodeKey, null);
                //     }
                //     eventListener?.Invoke(this, stringtifyData);
                // }
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Fallback to base WebSocketProtocol] " + ex.GetType().Name + ": " + ex.Message);
                base.OnMessage(data);
            }
        }
    }
}