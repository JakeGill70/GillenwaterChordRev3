namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for responding to a ResourceOwnerRequest
    /// </summary>
    public class ResourceOwnerResponse : Message
    {
        readonly string resourceId;
        readonly string ownerId;
        readonly string ownerIpAddress;
        readonly int ownerPort;

        public ResourceOwnerResponse(string messageJSON) : base(messageJSON)
        {
            resourceId = this["resourceid"];
            ownerId = this["ownerid"];
            ownerIpAddress = this["owneripaddress"];
            ownerPort = int.Parse(this["ownerport"]);
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
            this["resourceid"] = resourceId;
            this["ownerid"] = ownerId;
            this["owneripaddress"] = ownerIpAddress;
            this["ownerport"] = ownerPort.ToString();
            return base.ToString();
        }
    }
}
