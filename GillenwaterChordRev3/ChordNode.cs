using System.Security.Cryptography;
using System.Text;

namespace GillenwaterChordRev3
{
    public class ChordNode
    {
        // This node's id as a Hash string of its IpAddress and Port.
        public string id { get; private set; }
        public string ipAddress { get; private set; }
        public int port { get; private set; }

        public ChordNode(string ip, int port)
        {
            id = EZHash.GetHashString(ip + port);
            this.ipAddress = ip;
            this.port = port;
        }

    }
}
