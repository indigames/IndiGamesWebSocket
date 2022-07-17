
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NativeWebSocket;
using Newtonsoft.Json;

namespace IndiGames.Network
{
    public class VitalifyWSProtocol : WebSocketProtocol
    {
        public VitalifyWSProtocol(string url, Dictionary<string, string> headers = null) : base(url, headers)
        {
        }
    }
}