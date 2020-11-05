using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace GillenwaterChordRev3.Messages
{
    // Represents a message sent between nodes
    public class Message
    {
        public string senderID { get { return this["senderid"]; } private set { this["senderid"] = value; } }
        public string senderIpAddress { get { return this["senderipaddress"]; } private set { this["senderipaddress"] = value; } }
        public int senderPort { get { return int.Parse(this["senderport"]); } private set { this["senderport"] = value.ToString(); } }
        public MessageType messageType { get { return (MessageType)Enum.Parse(typeof(MessageType), this["senderid"]); } private set { this["senderid"] = value.ToString(); } }
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
        }

        public Message(ChordNode senderNode, MessageType msgType)
        {
            this.content = new Dictionary<string, string>();
            this.senderID = senderNode.Id;
            this.senderIpAddress = senderNode.IpAddress;
            this.senderPort = senderNode.Port;
            this.messageType = msgType;
        }

        public Message(string id, string ipAddr, int port, MessageType msgType) {
            this.content = new Dictionary<string, string>();
            this.senderID = id;
            this.senderIpAddress = ipAddr;
            this.senderPort = port;
            this.messageType = msgType;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize<Dictionary<string, string>>(content);
        }
    }
}
