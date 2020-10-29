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
    class AsynchronousServer
    {
        
        public readonly int port;

        private readonly IPHostEntry ipHostInfo;
        private readonly IPAddress ipAddress;
        private readonly IPEndPoint localEndPoint;

        private readonly Socket listener;

        private bool isServerRunning;

        public AsynchronousServer(int port) {
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StartServerAsync() {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            listener.Bind(localEndPoint);
            listener.Listen(10);

            isServerRunning = true;

            while (isServerRunning)
            {
                OutputManager.Server.Write("Waiting for a connection...");
                // Program is suspended while waiting for an incoming connection.  
                Socket handler = listener.Accept();
                OutputManager.Server.Write("New Handler at: " + handler.RemoteEndPoint.ToString());

                _ = ConnectionHandlerAsync(handler);
            }
        }

        private async Task ConnectionHandlerAsync(Socket handler) {
            while (true)
            {
                Message request = await ReadDataAsync(handler);
                OutputManager.Server.Write("Request: " + request);
                Message response = await ProcessMsgAsync(request);
                await SendMsgAsync(handler, response);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<Message> ProcessMsgAsync(Message msg) {
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            msg["Processed"] = true.ToString();
            return msg;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task SendMsgAsync(Socket handler, Message msg)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            string data = msg.ToString();
            data += "<EOF>";

            // Echo the data back to the client.  
            byte[] msgBuffer = Encoding.ASCII.GetBytes(data);

            handler.Send(msgBuffer);
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<Message> ReadDataAsync(Socket handler)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
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

            Message msg = new Message(data);

            return msg;
        }
    }
}
