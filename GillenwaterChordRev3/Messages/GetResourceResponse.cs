namespace GillenwaterChordRev3.Messages
{
    public class GetResourceResponse : Message
    {
        public string resourceId { get { return this["resourceid"]; } private set { this["resourceid"] = value; } }
        public string resourceName { get { return this["resourcename"]; } private set { this["resourcename"] = value; } }
        public string resourceContent { get { return this["resourcecontent"]; } private set { this["resourcecontent"] = value; } }
        public bool resourceFoundSuccessfully { get { return bool.Parse(this["resourcefoundsuccessfully"]); } private set { this["resourcefoundsuccessfully"] = value.ToString(); } }

        public GetResourceResponse(string messageJSON) : base(messageJSON)
        {

        }

        public GetResourceResponse(ChordNode senderNode, string recId, string recName, string recContent, bool recFoundSuccess=true) : base(senderNode, MessageType.GetResourceResponse)
        {
            resourceId = recId;
            resourceName = recName;
            resourceContent = recContent;
            resourceFoundSuccessfully = recFoundSuccess;
        }
    }

}
