using System;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

namespace GillenwaterChordRev3
{
    class Program
    {
        LocalNode localNode;

        static async Task Main()
        {
            Program app = new Program();

            // Determine the port this node will operate on
            int localServerPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + localServerPort);
            app.localNode = new LocalNode(localServerPort);

            bool appIsRunning = true;

            // Communicate with the remote node
            while (appIsRunning)
            {
                // Prompt the user for test data
                OutputManager.Ui.Write("What to do?");
                string cmd = Console.ReadLine();

                switch (cmd.ToLower()) {
                    case "exit":
                        appIsRunning = false;
                        break;
                    case "info":
                        app.ProcInfo();
                        break;
                    case "connect":
                        app.ProcConnect();
                        break;
                    case "join":
                        await app.ProcJoin();
                        break;
                }
            }

            // Disconnect from the remote node before closing
            app.localNode.DisconnectFromNode();
        }

        void ProcInfo() {
            OutputManager.Ui.Write("Pred port: " + localNode.predNode.Port);
            OutputManager.Ui.Write("Succ port: " + localNode.succNode.Port);
        }

        void ProcConnect() {
            // Determine where this node should connect to
            OutputManager.Ui.Write("Node to connect to: ");
            string nodeAddr = "127.0.0.1";
            OutputManager.Ui.Write(nodeAddr);

            // Determine what port this node should connect to
            OutputManager.Ui.Write("Port to connect to: ");
            int nodePort = int.Parse(Console.ReadLine());

            // Connect to the remote node
            OutputManager.Ui.Write("Connecting to remote node...");
            localNode.ConnectToNode(nodeAddr, nodePort);
            OutputManager.Ui.Write("Connected");
        }

        async Task ProcJoin() {
            // Connect to arbitrary node in the ring
            ProcConnect();
            // Make a join request
            OutputManager.Ui.Write("Join Request...");
            JoinRequest joinRequest = new JoinRequest(this.localNode);
            Message joinResponseTmp = await localNode.SendMessage(joinRequest);
            JoinResponse joinResponse = new JoinResponse(joinResponseTmp.ToString());
            // Disconnect from the arbitary node in the ring
            OutputManager.Ui.Write("Disconnecting from node...");
            localNode.DisconnectFromNode();
            // Update local node connection info
            OutputManager.Ui.Write("Updating local info...");
            localNode.predNode = new ChordNode(joinResponse.predIpAddress, joinResponse.predPort, joinResponse.predId);
            localNode.succNode = new ChordNode(joinResponse.succIpAddress, joinResponse.succPort, joinResponse.succId);
            // Connect to pred
            OutputManager.Ui.Write("Connecting to pred...");
            localNode.ConnectToNode(localNode.predNode.IpAddress, localNode.predNode.Port);
            // Have pred update their succ
            OutputManager.Ui.Write("Having pred update their succ...");
            UpdateSuccNodeRequest uSuccRequest = new UpdateSuccNodeRequest(this.localNode, this.localNode);
            Message uSuccResponseTmp = await localNode.SendMessage(uSuccRequest);
            UpdateNodeResponse uSuccResponse = new UpdateNodeResponse(uSuccResponseTmp.ToString());
            // TODO: Ensure that update was successful
            // Disconnect from pred
            OutputManager.Ui.Write("Disconnecting from pred...");
            localNode.DisconnectFromNode();
            // Connect to succ
            OutputManager.Ui.Write("Connecting to succ...");
            localNode.ConnectToNode(localNode.succNode.IpAddress, localNode.succNode.Port);
            // Have succ update their pred
            OutputManager.Ui.Write("Having succ update their pred...");
            UpdatePredNodeRequest uPredRequest = new UpdatePredNodeRequest(this.localNode, this.localNode);
            UpdateNodeResponse uPredResponse = (await localNode.SendMessage(uPredRequest)) as UpdateNodeResponse;
            // TODO: Ensure that update was successful
            OutputManager.Ui.Write("Successfully joined to ring!");
        }
    }
}
