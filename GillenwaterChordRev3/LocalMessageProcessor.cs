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
            processDispatch[MessageType.OwnerOfIdRequest] = (async msg => await ProcResourceOwnerRequest(msg));
            processDispatch[MessageType.AddResourceRequest] = (async msg => await ProcAddResourceRequest(msg));
            processDispatch[MessageType.JoinRequest] = (async msg => await ProcJoinRequest(msg));
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

        private async Task<Message> ProcResourceOwnerRequest(Message msg) {
            Messages.ResourceOwnerRequest rorMsg = (msg as Messages.ResourceOwnerRequest);
            if (IsResponsibleForResource(rorMsg.resourceId))
            {
                msg = new Messages.ResourceOwnerResponse(this.localNode, rorMsg.resourceId, this.localNode, this.localNode.predNode, this.localNode.succNode);
                msg.isProcessed = true;
            }
            return msg;
        }

        private async Task<Message> ProcAddResourceRequest(Message msg)
        {
            Messages.AddResourceRequest arMsg = (msg as Messages.AddResourceRequest);
            if (IsResponsibleForResource(arMsg.resourceId))
            {
                this.SetLocalResource(arMsg.resourceId, arMsg.resourceName, arMsg.resourceContent);
                msg = new Messages.AddResourceResponse(this.localNode, arMsg.resourceId, arMsg.resourceName);
                msg.isProcessed = true;
            }
            return msg;
        }

        private async Task<Message> ProcJoinRequest(Message msg) {
            Messages.JoinRequest jrMsg = (msg as Messages.JoinRequest);
            Messages.ResourceOwnerRequest joinPointRequest = new Messages.ResourceOwnerRequest(this.localNode, jrMsg.senderID);
            Messages.ResourceOwnerResponse joinPointResponse = (await ProcessMsgAsync(joinPointRequest)) as Messages.ResourceOwnerResponse;
            ChordNode succNode = new ChordNode(joinPointResponse.ownerIpAddress, joinPointResponse.ownerPort, joinPointResponse.ownerId);
            ChordNode predNode = new ChordNode(joinPointResponse.predId, joinPointResponse.predPort, joinPointResponse.predId);
            Messages.JoinResponse rMsg = new Messages.JoinResponse(this.localNode, succNode, predNode);
            rMsg.isProcessed = true;
            return rMsg;
        }

        // Process incoming messages and craft an appropriate response
        public async Task<Message> ProcessMsgAsync(Message msg)
        {
            Message response;

            response = await processDispatch[msg.messageType](msg);

            if (!response.isProcessed) {
                // If the response was NOT processed
                // Pass the buck
                response = await this.localNode.clientComponent.SendMsgAsync(msg);
            }

            return response;
        }

        public void SetLocalResource(string resourceId, string resourceName, string resourceContent)
        {
            this.localNode.SetLocalResource(resourceId, resourceName, resourceContent);
        }
    }
}
