using GillenwaterChordRev3.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class LocalMessageProcessor : IMessageProcessor
    {
        LocalNode localNode;
        Dictionary<MessageType, Func<Message,Task<Message>>> processDispatch;

        public LocalMessageProcessor(LocalNode localNode) {
            this.localNode = localNode;

            // Initialize dispatch table
            processDispatch = new Dictionary<MessageType, Func<Message, Task<Message>>>();
            processDispatch[MessageType.Testing] = (async msg => await ProcTestMessage(msg));
            processDispatch[MessageType.OwnerOfIdRequest] = (async msg => await ProcResourceOwnerRequest(msg));
            processDispatch[MessageType.AddResourceRequest] = (async msg => await ProcTestMessage(msg));
        }

        public string GetLocalResource(string resourceId)
        {
            return this.localNode.GetLocalResource(resourceId);
        }

        // Compare the resourceId to the local node's Id to determine if the node
        // is responisble for handling the resource or not
        private bool IsResponsibleForResource(string recId) {
            bool normalCase = string.Compare(this.localNode.Id, recId) <= 0;
            bool isLargerThanSelf = (string.Compare(this.localNode.Id, recId) > 0);
            bool isLargerThanPred = (string.Compare(this.localNode.predNode.Id, recId) > 0);
            bool isPredLargerThanSelf = (string.Compare(this.localNode.predNode.Id, this.localNode.Id) > 0);
            bool smallestNodeInRingCase = isLargerThanSelf && isLargerThanPred && isPredLargerThanSelf;
            return (normalCase || smallestNodeInRingCase);
        }

        private async Task<Message> ProcTestMessage(Message msg) {
            // If the "target" parameter exists - used for testing purposes
            if (msg["target"].Equals(string.Empty))
            {
                if (msg.senderID.Equals(localNode.Id))
                {
                    OutputManager.Ui.Write("Request could not find target.");
                    msg["processed"] = false.ToString();
                }
                else if (msg["target"].Equals(localNode.Port.ToString()))
                {
                    OutputManager.Server.Write("New msg for me from node " + msg.senderID);
                    OutputManager.Server.Write("Msg says: " + msg["TestData"]);
                    msg["processed"] = true.ToString();
                }
                else
                {
                    msg = await this.localNode.clientComponent.SendMsgAsync(msg);
                }
            }
            return msg;
        }

        private async Task<Message> ProcResourceOwnerRequest(Message msg) {
            Messages.ResourceOwnerRequest rorMsg = (msg as Messages.ResourceOwnerRequest);
            if (string.Compare(this.localNode.Id, rorMsg.resourceId) < 0)
            {
                msg = new Messages.ResourceOwnerResponse(this.localNode, rorMsg.resourceId, this.localNode);
            }
            return msg;
        }

        private async Task<Message> ProcAddResourceRequest(Message msg)
        {
            Messages.AddResourceRequest arMsg = (msg as Messages.AddResourceRequest);
            if (string.Compare(this.localNode.Id, arMsg.resourceId) < 0)
            {
                this.SetLocalResource(arMsg.resourceId, arMsg.resourceName, arMsg.resourceContent);
                msg = new Messages.AddResourceResponse(this.localNode, arMsg.resourceId, arMsg.resourceName);
            }
            return msg;
        }

        // Process incoming messages and craft an appropriate response
        public async Task<Message> ProcessMsgAsync(Message msg)
        {
            Message response;

            response = await processDispatch[msg.messageType](msg);

            return response;
        }

        public void SetLocalResource(string resourceId, string resourceName, string resourceContent)
        {
            this.localNode.SetLocalResource(resourceId, resourceName, resourceContent);
        }
    }
}
