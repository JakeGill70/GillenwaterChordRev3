using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    // Abstracts the Chord Node on this machine
    // Simplifies the management of server and client components
    public class LocalNode : ChordNode
    {
        // Component responsible for receiving messages from remote nodes
        readonly AsynchronousServer serverComponent;

        // Component responsible for sending messages to remote nodes
        readonly AsynchronousClient clientComponent;

        // string localIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()
        public LocalNode(int port) : base(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), port)
        {
            serverComponent = new AsynchronousServer(port);
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
            return new Message(this.id, this.ipAddress, this.port, type.ToString());
        }

        // Send a Message to a remote node
        public async Task<Message> SendMessage(Message msg) {
            string responseData = await clientComponent.SendMsgAsync(msg.ToString());
            Message responseMsg = new Message(responseData);
            return responseMsg;
        }
    }
}
