namespace GillenwaterChordRev3.Messages
{
    public class AddResourceResponse : Message
    {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }
        public string resourceName { get { return this["resourcename"]; } private set { this["resourcename"] = value; } }
        public bool resourceAddedSuccessfully { get { return bool.Parse(this["resourceaddedsuccessfully"]); } private set { this["resourceaddedsuccessfully"] = value.ToString(); } }

        public AddResourceResponse(string messageJSON) : base(messageJSON)
        {
        }

        public AddResourceResponse(ChordNode senderNode, string recId, string recName, bool recSuccess=true) : base(senderNode, MessageType.AddResourceResponse)
        {
            resourceId = recId;
            resourceName = recName;
            resourceAddedSuccessfully = recSuccess;
        }
    }
}
