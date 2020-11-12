namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for respond to a request to join a ring
    /// </summary>
    public class JoinResponse : Message
    {
        public string succId { get { return this["succid"]; } private set { this["succid"] = value; } }
        public string succIpAddress { get { return this["succipaddress"]; } private set { this["succipaddress"] = value; } }
        public int succPort { get { return int.Parse(this["succport"]); } private set { this["succport"] = value.ToString(); } }

        public string predId { get { return this["predid"]; } private set { this["predid"] = value; } }
        public string predIpAddress { get { return this["predipaddress"]; } private set { this["predipaddress"] = value; } }
        public int predPort { get { return int.Parse(this["predport"]); } private set { this["predport"] = value.ToString(); } }

        public JoinResponse(string messageJSON) : base(messageJSON)
        {
        }

        public JoinResponse(ChordNode senderNode, ChordNode succNode, ChordNode predNode) : base(senderNode, MessageType.JoinResponse)
        {
            // Successor Info
            succId = succNode.Id;
            succIpAddress = succNode.IpAddress;
            succPort = succNode.Port;
            // Predecessor Info
            predId = predNode.Id;
            predIpAddress = predNode.IpAddress;
            predPort = predNode.Port;
        }
    }

}
