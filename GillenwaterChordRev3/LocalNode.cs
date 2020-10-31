using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

namespace GillenwaterChordRev3
{
    // Abstracts the Chord Node on this machine
    // Simplifies the management of server and client components
    public class LocalNode : ChordNode, IMessageProcessor
    {
        // Component responsible for receiving messages from remote nodes
        readonly AsynchronousServer serverComponent;

        // Component responsible for sending messages to remote nodes
        readonly AsynchronousClient clientComponent;

        // string localIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()
        public LocalNode(int port) : base(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), port)
        {
            serverComponent = new AsynchronousServer(port, this);
            clientComponent = new AsynchronousClient();
            var serverTask = Task.Run(() => serverComponent.StartServerAsync());
        }

        // Connect remote node to send messages to
        public void ConnectToNode(string targetIP, int targetPort) {
            clientComponent.StartClient(targetIP, targetPort);
        }

        // Disconnect from the remote node
        public void DisconnectFromNode() {
            clientComponent.Disconnect();
        }

        // Creates a new Message object with some necessary fields
        // automatically populated
        public Message CreateMessage(MessageType type) {
            return new Message(this.Id, this.IpAddress, this.Port, type);
        }

        // Send a Message to a remote node
        public async Task<Message> SendMessage(Message msg) {
            Message responseMsg = await clientComponent.SendMsgAsync(msg);
            return responseMsg;
        }

        // Process incoming messages and craft an appropriate response
        public async Task<Message> ProcessMsgAsync(Message msg)
        {
            // If the "target" parameter exists - used for testing purposes
            if (msg["target"].Equals(string.Empty))
            {
                if (msg.senderID.Equals(this.Id))
                {
                    OutputManager.Ui.Write("Request could not find target.");
                    msg["processed"] = false.ToString();
                }
                else if (msg["target"].Equals(this.Port.ToString()))
                {
                    msg["processed"] = true.ToString();
                }
                else
                {
                    msg = await this.clientComponent.SendMsgAsync(msg);
                }
            }

            if (msg.messageType == MessageType.AddResourceRequest) {
                Messages.ResourceOwnerRequest rorMsg = (msg as Messages.ResourceOwnerRequest);
                if (string.Compare(this.Id, rorMsg.resourceId) < 0)
                {
                    msg = new Messages.ResourceOwnerResponse(this, rorMsg.resourceId, this);
                }
            }

            return msg;
        }
    }
}
