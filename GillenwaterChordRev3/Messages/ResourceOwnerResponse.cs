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

        public ResourceOwnerResponse(string messageJSON) : base(messageJSON)
        {
        }

        public ResourceOwnerResponse(ChordNode senderNode, string recId, ChordNode ownerNode) : base(senderNode, MessageType.OwnerOfIdResponse)
        {
            resourceId = recId;
            ownerId = ownerNode.Id;
            ownerIpAddress = ownerNode.IpAddress;
            ownerPort = ownerNode.Port;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
