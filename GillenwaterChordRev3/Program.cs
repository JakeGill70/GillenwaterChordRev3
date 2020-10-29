using System;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class Program
    {
        
        static async Task Main()
        {
            // Determine the port this node will operate on
            int localServerPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + localServerPort);
            LocalNode localNode = new LocalNode(localServerPort);

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

            // Communicate with the remote node
            while (true)
            {
                // Prompt the user for test data
                OutputManager.Ui.Write("What to send?");
                string data = Console.ReadLine();

                // Exit from this loop upon request from the user
                if (data.ToLower().Equals("exit")) {
                    break;
                }
                
                // Create the message
                Message msg = localNode.CreateMessage(MessageType.Testing);
                // Package the test data
                msg["TestData"] = data;
                OutputManager.Ui.Write("Port of the target?");
                msg["target"] = Console.ReadLine().Trim();
                // Send the message and get the response
                Message response = await localNode.SendMessage(msg);
                // Display the response
                OutputManager.Ui.Write("Response: " + response);
            }

            // Disconnect from the remote node before closing
            localNode.DisconnectFromNode();
        }
    }
}
