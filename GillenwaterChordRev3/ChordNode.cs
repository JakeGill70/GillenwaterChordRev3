using System.Security.Cryptography;
using System.Text;

namespace GillenwaterChordRev3
{
    public class ChordNode
    {
        // This node's id as a Hash string of its IpAddress and Port.
        public string Id { get; private set; }
        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public ChordNode(string ip, int port)
        {
            Id = EZHash.GetHashString(ip + port);
            this.IpAddress = ip;
            this.Port = port;
        }

    }
}
