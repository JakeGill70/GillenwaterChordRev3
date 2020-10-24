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
    public class LocalNode : ChordNode
    {
        AsynchronousServer serverComponent;
        AsynchronousClient clientComponent;

        // string localIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()
        public LocalNode(int port) : base(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), port)
        {
            serverComponent = new AsynchronousServer(port);
            clientComponent = new AsynchronousClient();
            var serverTask = Task.Run(() => serverComponent.StartServerAsync());
        }

        public void ConnectToNode(string targetIP, int targetPort) {
            clientComponent.StartClient(targetIP, targetPort);
        }

        public void DisconnectFromNode() {
            clientComponent.Disconnect();
        }

        public Message StartMessage(MessageType type) {
            return new Message(this.id, this.ipAddress, this.port, type.ToString());
        }

        public async Task<Message> SendMessage(Message msg) {
            string responseData = await clientComponent.sendMsgAsync(msg.ToString());
            Message responseMsg = new Message(responseData);
            return responseMsg;
        }
    }
}
