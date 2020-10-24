using System.Security.Cryptography;
using System.Text;

namespace GillenwaterChordRev3
{
    public class ChordNode
    {
        public string id { get; private set; }
        public string ipAddress { get; private set; }
        public int port { get; private set; }

        public ChordNode(string ip, int port)
        {
            id = EZHash.GetHashString(ip + port);
            this.ipAddress = ip;
            this.port = port;
        }
    public class LocalNode : ChordNode
    {
        public LocalNode(string ip, int port) : base(ip, port)
        {

        }
    }
}
