namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for discovering who is responsible for a resource
    /// </summary>
    public class ResourceOwnerRequest : Message
    {
        public readonly string resourceId;

        public ResourceOwnerRequest(string messageJSON) : base(messageJSON)
        {
            resourceId = this["resourceid"];
        }

        public ResourceOwnerRequest(ChordNode senderNode, string recId) : base(senderNode, MessageType.OwnerOfIdRequest)
        {
            resourceId = recId;
        }

        public override string ToString()
        {
            this[resourceId] = resourceId;
            return base.ToString();
        }
    }
}
