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

        private readonly IMessageProcessor msgProcessor;

        private bool isServerRunning;

        public AsynchronousServer(int port, IMessageProcessor msgProcessor) {
            // Establish the local endpoint for the socket.  
            // Dns.GetHostName returns the name of the
            // host running the application.  
            ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            ipAddress = ipHostInfo.AddressList[0];
            localEndPoint = new IPEndPoint(ipAddress, port);
            this.port = port;
            this.isServerRunning = false;
            this.msgProcessor = msgProcessor;

            // Create a TCP/IP socket.  
            listener = new Socket(ipAddress.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
        }

        public void StopServer() {
            isServerRunning = false;
            listener.Shutdown(SocketShutdown.Both);
            listener.Close();
        }

        public async Task StartServerAsync() {
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
                Message response = await this.msgProcessor.ProcessMsgAsync(request);
                await SendMsgAsync(handler, response);
            }
        }

        private async Task SendMsgAsync(Socket handler, Message msg)
        {
            string data = msg.ToString();
            data += "<EOF>";

            // Echo the data back to the client.  
            byte[] msgBuffer = Encoding.ASCII.GetBytes(data);

            handler.Send(msgBuffer);
        }

        private async Task<Message> ReadDataAsync(Socket handler)
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
