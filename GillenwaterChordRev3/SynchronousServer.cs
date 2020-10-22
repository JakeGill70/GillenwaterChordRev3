using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    class SynchronousServer
    {
        
        public readonly int port;

        private IPHostEntry ipHostInfo;
        private IPAddress ipAddress;
        private IPEndPoint localEndPoint;

        private Socket listener;

        private bool isServerRunning;

        public SynchronousServer(int port) {
            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, port);
            this.port = port;
            this.isServerRunning = false;

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void StopServer() {
            isServerRunning = false;
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
        }

        public void StartServer() {
            listener.Bind(localEndPoint);
            listener.Listen(10);

            isServerRunning = true;

            while (isServerRunning)
            {
                OutputManager.Server.Write("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                OutputManager.Server.Write("New Hander at: " + handler.RemoteEndPoint.ToString());

                string received = readData(handler);

                // Show the data on the console.  
                OutputManager.Server.Write($"Text received : {rece}");

                string response = ProcessMsg(received);

                sendMsg(handler, response);
            }
        }

        private string ProcessMsg(string msg) {
            return msg;
        }

        private void sendMsg(Socket handler, string msg)
        {
            msg += "<EOF>";

            // Echo the data back to the client.  
            byte[] msgBuffer = Encoding.ASCII.GetBytes(msg);

            handler.Send(msgBuffer);
        }

        private string readData(Socket handler)
        {

            // Data buffer for incoming data.  
            byte[] bytes = new Byte[1024];
            // Message
            string data = string.Empty;

            // An incoming connection needs to be processed.  
            while (true)
            {
                int bytesRec = handler.Receive(bytes);
                data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                if (data.IndexOf("<EOF>") > -1)
                {
                    break;
                }
            }

            // Remove <EOF>
            data = data.Substring(0, data.Length - "<EOF>".Length);

            return data;
        }
    }
}
