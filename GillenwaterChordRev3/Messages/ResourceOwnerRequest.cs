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

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
