using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using GillenwaterChordRev3.Messages;

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
        public async Task<Message> SendMsgAsync(Message msg)
        {
            try
            {
                string data = msg.ToString();
                data += "<EOF>";

                // Encode the data string into a byte array.  
                byte[] msgBuffer = Encoding.ASCII.GetBytes(data);

                // Send the data through the socket.  
                int bytesSent = sender.Send(msgBuffer);

                Message response = await ReadMsgAsync();

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
            return null;
        }

        // Read a response message from the remote node
        public async Task<Message> ReadMsgAsync()
        {
            try
            {
                // Data buffer for incoming data.  
                byte[] bytes = new byte[1024];

                // Receive the response from the remote device.  
                int bytesRec = sender.Receive(bytes);
                string msgData = Encoding.ASCII.GetString(bytes, 0, bytesRec);

                // Remove <EOF>
                msgData = msgData.Substring(0, msgData.Length - "<EOF>".Length);

                OutputManager.Client.Write("Received: " + msgData);

                Message msg = new Message(msgData);

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
            return null;
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
