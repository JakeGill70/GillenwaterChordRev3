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
            processDispatch[MessageType.UpdatePredNodeRequest] = (async msg => await ProcUpdatePredRequest(msg));
            processDispatch[MessageType.UpdateSuccNodeRequest] = (async msg => await ProcUpdateSuccRequest(msg));
        }

        public string GetLocalResource(string resourceId)
        {
            return this.localNode.GetLocalResource(resourceId);
        }

        // Compare the resourceId to the local node's Id to determine if the node
        // is responisble for handling the resource or not
        private bool IsResponsibleForResource(string recId) {
            string myId = this.localNode.Id;
            string myPredId = this.localNode.predNode.Id;
            bool normalCase = string.Compare(myId, recId) <= 0;
            bool isLargerThanSelf = (string.Compare(myId, recId) > 0);
            bool isLargerThanPred = (string.Compare(myPredId, recId) > 0);
            bool isPredLargerThanSelf = (string.Compare(myPredId, myId) > 0);
            bool smallestNodeInRingCase = isLargerThanSelf && isLargerThanPred && isPredLargerThanSelf;
            bool isResponsible = normalCase || smallestNodeInRingCase;
            return isResponsible;
        }

        private async Task<Message> ProcResourceOwnerRequest(Message msg) {
            Messages.ResourceOwnerRequest rorMsg = new Messages.ResourceOwnerRequest(msg.ToString());
            OutputManager.Server.Write("Processing resource owner request...");
            if (IsResponsibleForResource(rorMsg.resourceId))
            {
                OutputManager.Server.Write("Is responsible for resource...");
                msg = new Messages.ResourceOwnerResponse(this.localNode, rorMsg.resourceId, this.localNode, this.localNode.predNode, this.localNode.succNode);
                msg.isProcessed = true;
            }
            else {
                OutputManager.Server.Write("Not responsible for resource...");
            }
            OutputManager.Server.Write("Resource owner request processed!");
            return msg;
        }

        private async Task<Message> ProcAddResourceRequest(Message msg)
        {
            Messages.AddResourceRequest arMsg = new Messages.AddResourceRequest(msg.ToString());
            if (IsResponsibleForResource(arMsg.resourceId))
            {
                this.SetLocalResource(arMsg.resourceId, arMsg.resourceName, arMsg.resourceContent);
                msg = new Messages.AddResourceResponse(this.localNode, arMsg.resourceId, arMsg.resourceName);
                msg.isProcessed = true;
            }
            return msg;
        }

        private async Task<Message> ProcJoinRequest(Message msg) {
            OutputManager.Server.Write("Processing join request...");
            Messages.JoinRequest jrMsg = new JoinRequest(msg.ToString());
            OutputManager.Server.Write("Finding join point...");
            Messages.ResourceOwnerRequest joinPointRequest = new Messages.ResourceOwnerRequest(this.localNode, jrMsg.senderID);
            OutputManager.Server.Write("Awaiting to process...");
            Message tmp = await ProcResourceOwnerRequest(joinPointRequest);
            OutputManager.Server.Write("process awaited");
            Messages.ResourceOwnerResponse joinPointResponse = tmp as Messages.ResourceOwnerResponse;
            OutputManager.Server.Write("Generating Pred and Succ nodes...");
            ChordNode succNode = new ChordNode(joinPointResponse.ownerIpAddress, joinPointResponse.ownerPort, joinPointResponse.ownerId);
            ChordNode predNode = new ChordNode(joinPointResponse.predIpAddress, joinPointResponse.predPort, joinPointResponse.predId);
            OutputManager.Server.Write("Generating a response...");
            Messages.JoinResponse rMsg = new Messages.JoinResponse(this.localNode, succNode, predNode);
            rMsg.isProcessed = true;
            OutputManager.Server.Write("Join request processed!");
            return rMsg;
        }

        private async Task<Message> ProcUpdatePredRequest(Message msg) {
            Messages.UpdatePredNodeRequest upnrMsg = new Messages.UpdatePredNodeRequest(msg.ToString());
            // Update local node relationship
            ChordNode predNode = new ChordNode(upnrMsg.predIpAddress, upnrMsg.predPort, upnrMsg.predId);
            this.localNode.predNode = predNode;
            // The pred communicates with the server component,
            // so there is no reason to update the client connections
            upnrMsg.isProcessed = true;
            // craft a response message
            Messages.UpdateNodeResponse response = new UpdateNodeResponse(this.localNode, true);
            response.isProcessed = true;
            return response;
        }

        private async Task<Message> ProcUpdateSuccRequest(Message msg)
        {
            Messages.UpdateSuccNodeRequest usnrMsg = new Messages.UpdateSuccNodeRequest(msg.ToString());
            // Update local node relationship
            OutputManager.Server.Write("Updating local node relationship...");
            ChordNode succNode = new ChordNode(usnrMsg.succIpAddress, usnrMsg.succPort, usnrMsg.succId);
            this.localNode.succNode = succNode;
            // Adjust connection to client node
            OutputManager.Server.Write("Disconnecting from old succ node...");
            this.localNode.DisconnectFromNode();
            OutputManager.Server.Write("Connecting to new succ node...");
            this.localNode.ConnectToNode(succNode.IpAddress, succNode.Port);
            usnrMsg.isProcessed = true;
            // craft a response message
            OutputManager.Server.Write("Generating a response...");
            Messages.UpdateNodeResponse response = new UpdateNodeResponse(this.localNode, true);
            response.isProcessed = true;
            OutputManager.Server.Write("Finished processing an update succ request");
            return response;
        }

        // Process incoming messages and craft an appropriate response
        public async Task<Message> ProcessMsgAsync(Message msg)
        {
            OutputManager.Server.Write("Processing Msg: " + msg);

            Message response;

            OutputManager.Server.Write("\tDispatching msg...");
            response = await processDispatch[msg.messageType](msg);
            OutputManager.Server.Write("\tMsg dispatched.");

            if (!response.isProcessed) {
                OutputManager.Server.Write("Could not process message. Now passing the buck...");
                // If the response was NOT processed
                // Pass the buck
                response = await this.localNode.clientComponent.SendMsgAsync(msg);
                OutputManager.Server.Write("Someone else could do it!");
            }

             return response;
        }

        public void SetLocalResource(string resourceId, string resourceName, string resourceContent)
        {
            this.localNode.SetLocalResource(resourceId, resourceName, resourceContent);
        }
    }
}
