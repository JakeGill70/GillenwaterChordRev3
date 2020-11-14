namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for discovering who is responsible for a resource
    /// </summary>
    public class ResourceOwnerRequest : Message
    {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }

        public ResourceOwnerRequest(string messageJSON) : base(messageJSON)
        {
        }

        public ResourceOwnerRequest(ChordNode senderNode, string recId) : base(senderNode, MessageType.OwnerOfIdRequest)
        {
            resourceId = recId;
        }
    }

    /// <summary>
    /// Specialized message class for having a target node update their relation to other nodes (their succ and pred nodes) 
    /// </summary>
    public class UpdateRelationshipRequest : Message
    {
        public string succId { get { return this["succid"]; } private set { this["succid"] = value; } }
        public string succIpAddress { get { return this["succipaddress"]; } private set { this["succipaddress"] = value; } }
        public int succPort { get { return int.Parse(this["succport"]); } private set { this["succport"] = value.ToString(); } }

        public string predId { get { return this["predid"]; } private set { this["predid"] = value; } }
        public string predIpAddress { get { return this["predipaddress"]; } private set { this["predipaddress"] = value; } }
        public int predPort { get { return int.Parse(this["predport"]); } private set { this["predport"] = value.ToString(); } }

        public UpdateRelationshipRequest(string messageJSON) : base(messageJSON)
        {
        }

        public UpdateRelationshipRequest(ChordNode senderNode) : base(senderNode, MessageType.OwnerOfIdRequest)
        {
        }
    }

    /// <summary>
    /// Specialized message class for responding to a UpdateRelationshipRequest
    /// </summary>
    public class UpdateRelationshipResponse : Message
    {

        public UpdateRelationshipResponse(string messageJSON) : base(messageJSON)
        {
        }

        public UpdateRelationshipResponse(ChordNode senderNode) : base(senderNode, MessageType.OwnerOfIdRequest)
        {
        }
    }

}
