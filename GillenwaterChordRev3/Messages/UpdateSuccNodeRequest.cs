namespace GillenwaterChordRev3.Messages
{
    public class UpdateSuccNodeRequest : Message
    {

        public string succId { get { return this["succid"]; } private set { this["succid"] = value; } }
        public string succIpAddress { get { return this["succipaddress"]; } private set { this["succipaddress"] = value; } }
        public int succPort { get { return int.Parse(this["succport"]); } private set { this["succport"] = value.ToString(); } }

        public UpdateSuccNodeRequest(string messageJSON) : base(messageJSON)
        {
        }

        public UpdateSuccNodeRequest(ChordNode senderNode, ChordNode succNode) : base(senderNode, MessageType.UpdateSuccNodeRequest) { 
            succId = succNode.Id;
            succIpAddress = succNode.IpAddress;
            succPort = succNode.Port;
        }
    }
}
