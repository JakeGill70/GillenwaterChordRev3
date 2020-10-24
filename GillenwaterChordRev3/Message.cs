using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GillenwaterChordRev3
{
    public class Message
    {
        public readonly string senderID;
        public readonly string senderIpAddress;
        public readonly int senderPort;
        public readonly string messageType;
        private Dictionary<string, string> content;

        public string this[string key] {
            get {
                // Force the key to lower case to help prevent simply typos
                key = key.ToLower();
                // Try to get the value at the key
                string output;
                content.TryGetValue(key, out output);
                // Make the value empty if the lookup failed
                output = String.IsNullOrEmpty(output) ? string.Empty : output;
                // Return the value
                return output;
            }
            set {
                key = key.ToLower();
                content[key] = value;
            }
        }

        public Message(string messageJSON) {
            this.content = JsonSerializer.Deserialize<Dictionary<string, string>>(messageJSON);
            this.senderID = content["senderid"];
            this.senderIpAddress = content["senderipaddress"];
            this.senderPort = int.Parse(content["senderport"]);
            this.messageType = content["messagetype"];
        }

        public Message(string id, string ipAddr, int port, string msgType) {
            this.content = new Dictionary<string, string>();
            this.senderID = id;
            this.senderIpAddress = ipAddr;
            this.senderPort = port;
            this.messageType = msgType;
        }

        public override string ToString()
        {
            content["senderid"] = this.senderID;
            content["senderipaddress"] = this.senderIpAddress;
            content["senderport"] = this.senderPort.ToString();
            content["messagetype"] = this.messageType;
            return JsonSerializer.Serialize<Dictionary<string, string>>(content);
        }
    }
}
