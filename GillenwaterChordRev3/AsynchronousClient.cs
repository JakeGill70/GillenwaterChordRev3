using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GillenwaterChordRev3
{
    public class AsynchronousClient
    {
        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint remoteEP;
        Socket sender;

        // Connect to Asynchronous Server component on remote node
        public void StartClient(string serverIP, int serverPort)
        {

            // Connect to a remote device.  
            try
            {
                // Establish the remote endpoint for the socket.  
                // This example uses port 11000 on the local computer.  
                ipHostInfo = Dns.GetHostEntry(serverIP);
                ipAddress = ipHostInfo.AddressList[0];
                remoteEP = new IPEndPoint(ipAddress, serverPort);

                // Create a TCP/IP  socket.  
                sender = new Socket(ipAddress.AddressFamily,
                    SocketType.Stream, ProtocolType.Tcp);

                // Connect the socket to the remote endpoint. Catch any errors.  
                try
                {
                    sender.Connect(remoteEP);

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        // Send a message to the remote node
        public async Task<string> SendMsgAsync(string msg)
        {
            try
            {
                msg += "<EOF>";

                // Encode the data string into a byte array.  
                byte[] msgBuffer = Encoding.ASCII.GetBytes(msg);

                // Send the data through the socket.  
                int bytesSent = sender.Send(msgBuffer);

                string response = await ReadMsgAsync();

                return response;
            }
            catch (ArgumentNullException ane)
            {
                Console.Error.WriteLine("ArgumentNullException : {0}", ane.ToString());
            }
            catch (SocketException se)
            {
                Console.Error.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            return string.Empty;
        }

        // Read a response message from the remote node
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string> ReadMsgAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            try
            {
                // Data buffer for incoming data.  
                byte[] bytes = new byte[1024];

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                string msg = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Remove <EOF>
                msg = msg.Substring(0, msg.Length - "<EOF>".Length);

                OutputManager.Client.Write("Received: " + msg);

                return msg;

            }
            catch (SocketException se)
            {
                Console.Error.WriteLine("SocketException : {0}", se.ToString());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Unexpected exception : {0}", e.ToString());
            }
            return String.Empty;
        }

        // Disconnect from the remote node
        public void Disconnect()
        {
            // Release the socket. 
            if (sender.Connected)
            {
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }
        }

    }
}
