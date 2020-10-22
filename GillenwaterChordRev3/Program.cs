using System;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class Program
    {
        
        static void Main(string[] args)
        {
            int serverPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + serverPort);
            SynchronousServer serverComponent = new SynchronousServer(serverPort);
            SynchronousClient clientComponent = new SynchronousClient();

            OutputManager.Ui.Write("Starting server...");
            Task serverTask = Task.Run(() => serverComponent.StartServer());
            OutputManager.Ui.Write("Server started.");

            OutputManager.Ui.Write("Node to connect to: ");
            string nodeAddr = "127.0.0.1"; // Console.ReadLine();
            OutputManager.Ui.Write(nodeAddr);

            OutputManager.Ui.Write("Port to connect to: ");
            int nodePort = int.Parse(Console.ReadLine());

            OutputManager.Ui.Write("Starting client...");
            clientComponent.StartClient(nodeAddr, nodePort);
            OutputManager.Ui.Write("Client started.");

            OutputManager.Ui.Write("What to send?");
            string data = Console.ReadLine();
            string response = clientComponent.sendMsg(data);
            OutputManager.Ui.Write("Response: " + response);

            Console.ReadLine();
            clientComponent.Disconnect();
            serverComponent.StopServer(); // Hos to stop server???
        }
    }
}
