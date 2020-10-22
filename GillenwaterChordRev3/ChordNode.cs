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
            id = GetHashString(ip + port);
            this.ipAddress = ip;
            this.port = port;
        }

        public static string GetHashString(string inputString)
        {
            // Get the hash of the string
            // using SHA-256
            using HashAlgorithm hasher = SHA256.Create();
            byte[] hashBytes = hasher.ComputeHash(Encoding.UTF8.GetBytes(inputString));

            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
                sb.Append(b.ToString("X2"));

            return sb.ToString();
        }
    }

    public class LocalNode : ChordNode
    {
        public LocalNode(string ip, int port) : base(ip, port)
        {

        }
    }
}
