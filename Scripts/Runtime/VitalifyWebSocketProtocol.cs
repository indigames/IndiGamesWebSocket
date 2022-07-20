
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace IndiGames.Network.Vitalify
{
    public class VitalifyBaseArgs : WebSocketArgs
    {
        [JsonProperty("codeKey")]
        public string CodeKey;

        [JsonProperty("data")]
        public new EventArgs Data;
    }

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
                var deserializeData = JsonConvert.DeserializeObject<VitalifyBaseArgs>(stringtifyData);

                if (this._listenerMap.TryGetValue(deserializeData.CodeKey, out EventHandler<EventArgs> eventListener))
                {
                    if (eventListener == null)
                    {
                        this._listenerMap.Add(deserializeData.CodeKey, null);
                    }
                    eventListener?.Invoke(this, deserializeData);
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