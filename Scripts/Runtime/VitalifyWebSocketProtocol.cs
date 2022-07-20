
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;

namespace IndiGames.Network.Vitalify
{
    public class VitalifyBaseArgs : EventArgs
    {
        [JsonProperty("codeKey")]
        public string CodeKey;

        [JsonProperty("data")]
        public EventArgs Data;
    }

    public class VitalifyWSProtocol : WebSocketProtocol
    {
        public VitalifyWSProtocol(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {
        }

        public override void OnMessage(byte[] data)
        {
            string stringtifyData = Encoding.UTF8.GetString(data);
            try
            {
                var deserializeData = JsonConvert.DeserializeObject<VitalifyBaseArgs>(stringtifyData);

                if (this._listenerMap.TryGetValue(deserializeData.CodeKey, out EventHandler<string> eventListener))
                {
                    if (eventListener == null)
                    {
                        this._listenerMap.Add(deserializeData.CodeKey, null);
                    }
                    eventListener?.Invoke(this, stringtifyData);
                }
            }
            catch (ArgumentNullException e)
            {
                Debug.Log(e.ToString());
                base.OnMessage(data);
            }
        }
    }
}