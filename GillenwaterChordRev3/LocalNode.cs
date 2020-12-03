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
    public class LocalNode : ChordNode
    {
        // Component responsible for receiving messages from remote nodes
        private readonly AsynchronousServer serverComponent;

        // Component responsible for sending messages to remote nodes
        public readonly AsynchronousClient clientComponent;

        // Component responsible for processing messages from remote nodes
        public readonly IMessageProcessor msgProccessor;

        // Used for storing local resources
        private readonly Dictionary<string, Tuple<string,string>> localResources;

        // Predecessor Node
        public ChordNode predNode;

        // Successor Node
        public ChordNode succNode;

        // string localIpAddress = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString()
        public LocalNode(int port) : base(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString(), port)
        {
            localResources = new Dictionary<string, Tuple<string, string>>();
            msgProccessor = new LocalMessageProcessor(this);
            serverComponent = new AsynchronousServer(port, msgProccessor);
            clientComponent = new AsynchronousClient();

            predNode = this;
            succNode = this;

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
        public Message SendMessage(Message msg) {
            Message responseMsg; // Use to send the response
            if (msg is AddResourceRequest && this.succNode.Id == this.Id)
            {
                AddResourceRequest arm = msg as AddResourceRequest;
                SetLocalResource(arm.resourceId, arm.resourceName, arm.resourceContent);
                responseMsg = new AddResourceResponse(this, arm.resourceId, arm.resourceName);
            }
            else if (msg is GetResourceRequest && this.succNode.Id == this.Id) {
                GetResourceRequest grm = msg as GetResourceRequest;
                string content = GetLocalResource(grm.resourceId);
                responseMsg = new GetResourceResponse(this, grm.resourceId, "LOCALNODE ERR: RESOURCE NAME UNKNOWN!", content);
            }
            else
            {
                responseMsg = clientComponent.SendMsgAsync(msg);
            }
            return responseMsg;
        }

        // Retrive a local resource by Id
        public string GetLocalResource(string resourceId)
        {
            Tuple<string,string> resource = null;
            localResources.TryGetValue(resourceId, out resource);
            return resource.Item2; // Item 2 is the resource content, item 1 is the resource name
        }

        // Assigns a local resource
        public void SetLocalResource(string resourceId, string resourceName, string resourceContent)
        {
            localResources.Add(resourceId, new Tuple<string, string>(resourceName, resourceContent));
        }

        public string[] GetLocalResourceNames() {
            List<string> recNames = new List<string>();
            foreach (var rec in localResources) {
                recNames.Add(rec.Value.Item1); // item 1 is the resource name
            }
            recNames.Sort();
            return recNames.ToArray();
        }
    }
}
