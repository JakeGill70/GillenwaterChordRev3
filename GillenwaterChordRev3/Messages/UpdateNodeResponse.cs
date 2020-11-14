namespace GillenwaterChordRev3.Messages
{
    public class UpdateNodeResponse : Message
    {

        public bool nodeUpdatedSucccessfully { get { return bool.Parse(this["nodeupdatedsuccessfully"]); } private set { this["nodeupdatedsuccessfully"] = value.ToString(); } }


        public UpdateNodeResponse(string messageJSON) : base(messageJSON)
        {
        }

        public UpdateNodeResponse(ChordNode senderNode, bool wasUpdateSuccessful) : base(senderNode, MessageType.UpdateNodeResponse)
        {
            nodeUpdatedSucccessfully = wasUpdateSuccessful;
        }
    }
}
