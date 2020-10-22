using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GillenwaterChordRev3
{
    public class SynchronousClient
    {
        IPHostEntry ipHostInfo;
        IPAddress ipAddress;
        IPEndPoint remoteEP;
        Socket sender;

        public void StartClient(string serverIP, int serverPort)
        {
            // Data buffer for incoming data.  
            byte[] bytes = new byte[1024];

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

        public string sendMsg(string msg)
        {
            try
            {
                msg += "<EOF>";

                // Encode the data string into a byte array.  
                byte[] msgBuffer = Encoding.ASCII.GetBytes(msg);

                // Send the data through the socket.  
                int bytesSent = sender.Send(msgBuffer);

                string response = this.ReadMsg();

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
            return String.Empty;
        }

        public string ReadMsg()
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

        public void Disconnect()
        {
            // Release the socket.  
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

    }
}
