namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for responding to a ResourceOwnerRequest
    /// </summary>
    public class ResourceOwnerResponse : Message
    {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }

        public string ownerId { get { return this["ownerid"]; } private set { this["ownerid"] = value; } }
        public string ownerIpAddress { get { return this["owneripaddress"]; } private set { this["owneripaddress"] = value; } }
        public int ownerPort { get { return int.Parse(this["ownerport"]); } private set { this["ownerport"] = value.ToString(); } }

        public string succId { get { return this["succid"]; } private set { this["succid"] = value; } }
        public string succIpAddress { get { return this["succipaddress"]; } private set { this["succipaddress"] = value; } }
        public int succPort { get { return int.Parse(this["succport"]); } private set { this["succport"] = value.ToString(); } }

        public string predId { get { return this["predid"]; } private set { this["predid"] = value; } }
        public string predIpAddress { get { return this["predipaddress"]; } private set { this["predipaddress"] = value; } }
        public int predPort { get { return int.Parse(this["predport"]); } private set { this["predport"] = value.ToString(); } }

        public ResourceOwnerResponse(string messageJSON) : base(messageJSON)
        {
        }

        public ResourceOwnerResponse(ChordNode senderNode, string recId, ChordNode ownerNode, ChordNode predNode, ChordNode succNode) : base(senderNode, MessageType.OwnerOfIdResponse)
        {
            resourceId = recId;
            // Owner Info
            ownerId = ownerNode.Id;
            ownerIpAddress = ownerNode.IpAddress;
            ownerPort =  ownerNode.Port;
            // Predecessor Info
            predId = predNode.Id;
            predIpAddress = predNode.IpAddress;
            predPort = predNode.Port;
            // Successor Info
            succId = succNode.Id;
            succIpAddress = succNode.IpAddress;
            succPort = succNode.Port;
        }
    }
}
