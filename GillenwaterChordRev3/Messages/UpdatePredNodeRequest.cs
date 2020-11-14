namespace GillenwaterChordRev3.Messages
{
    public class UpdatePredNodeRequest : Message {

        public string predId { get { return this["predid"]; } private set { this["predid"] = value; } }
        public string predIpAddress { get { return this["predipaddress"]; } private set { this["predipaddress"] = value; } }
        public int predPort { get { return int.Parse(this["predport"]); } private set { this["predport"] = value.ToString(); } }

        public UpdatePredNodeRequest(string messageJSON) : base(messageJSON)
        {
        }

        public UpdatePredNodeRequest(ChordNode senderNode, ChordNode predNode) : base(senderNode, MessageType.UpdatePredNodeRequest)
        {
            predId = predNode.Id;
            predIpAddress = predNode.IpAddress;
            predPort = predNode.Port;
        }
    }
}
