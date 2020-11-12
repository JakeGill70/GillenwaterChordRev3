namespace GillenwaterChordRev3.Messages
{
    /// <summary>
    /// Specialized message class for requesting to join a ring
    /// </summary>
    public class JoinRequest : Message
    {

        public JoinRequest(string messageJSON) : base(messageJSON)
        {
        }

        public JoinRequest(ChordNode senderNode) : base(senderNode, MessageType.JoinRequest)
        {
        }
    }

}
