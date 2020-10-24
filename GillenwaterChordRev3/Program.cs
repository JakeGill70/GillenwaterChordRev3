using System;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            int localServerPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + localServerPort);
            LocalNode localNode = new LocalNode(localServerPort);

            OutputManager.Ui.Write("Node to connect to: ");
            string nodeAddr = "127.0.0.1"; // Console.ReadLine();
            OutputManager.Ui.Write(nodeAddr);

            OutputManager.Ui.Write("Port to connect to: ");
            int nodePort = int.Parse(Console.ReadLine());

            OutputManager.Ui.Write("Connecting to remote node...");
            localNode.ConnectToNode(nodeAddr, nodePort);
            OutputManager.Ui.Write("Connected");

            while (true)
            {
                OutputManager.Ui.Write("What to send?");
                string data = Console.ReadLine();

                if (data.ToLower().Equals("exit")) {
                    break;
                }

                Message msg = localNode.StartMessage(MessageType.Testing);
                msg["TestData"] = data;
                Message response = await localNode.SendMessage(msg);
                OutputManager.Ui.Write("Response: " + response);
            }

            localNode.DisconnectFromNode();
        }
    }
}
