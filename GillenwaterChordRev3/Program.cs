using System;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class Program
    {
        
        static async Task Main(string[] args)
        {
            int serverPort = 5000 + EZRandom.Next(0, 999);
            OutputManager.Ui.Write("Node Server Port: " + serverPort);
            AsynchronousServer serverComponent = new AsynchronousServer(serverPort);
            AsynchronousClient clientComponent = new AsynchronousClient();

            OutputManager.Ui.Write("Starting server...");
            var serverTask = Task.Run(() => serverComponent.StartServerAsync());
            OutputManager.Ui.Write("Server started.");

            OutputManager.Ui.Write("Node to connect to: ");
            string nodeAddr = "127.0.0.1"; // Console.ReadLine();
            OutputManager.Ui.Write(nodeAddr);

            OutputManager.Ui.Write("Port to connect to: ");
            int nodePort = int.Parse(Console.ReadLine());

            OutputManager.Ui.Write("Starting client...");
            clientComponent.StartClient(nodeAddr, nodePort);
            OutputManager.Ui.Write("Client started.");

            while (true)
            {
                OutputManager.Ui.Write("What to send?");
                string data = Console.ReadLine();

                if (data.ToLower().Equals("exit")) {
                    break;
                }

                string response = await clientComponent.sendMsgAsync(data);
                OutputManager.Ui.Write("Response: " + response);
            }

            Console.ReadLine();
            clientComponent.Disconnect();
            serverComponent.StopServer(); // Hos to stop server???
        }
    }
}
