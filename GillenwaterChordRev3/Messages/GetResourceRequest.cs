namespace GillenwaterChordRev3.Messages
{
    public class GetResourceRequest : Message
    {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }

        public GetResourceRequest(string messageJSON) : base(messageJSON)
        {

        }

        public GetResourceRequest(ChordNode senderNode, string recId) : base(senderNode, MessageType.GetResourceRequest)
        {
            resourceId = recId;
        }
    }
}
