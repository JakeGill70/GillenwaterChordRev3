namespace GillenwaterChordRev3.Messages
{
    public class AddResourceRequest : Message {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }
        public string resourceName { get { return this["resourcename"]; } private set { this["resourcename"] = value; } }
        public string resourceContent { get { return this["resourcecontent"]; } private set { this["resourcecontent"] = value; } }
        public bool resourceAddedSuccessfully { get { return bool.Parse(this["resourceaddedsuccessfully"]); } private set { this["resourceaddedsuccessfully"] = value.ToString(); } }

        public AddResourceRequest(ChordNode senderNode, string recId, string recName, string recContent) : base(senderNode, MessageType.AddResourceRequest)
        {
            resourceId = recId;
            resourceName = recName;
            resourceContent = recContent;
            resourceAddedSuccessfully = false;
        }
    }
}
