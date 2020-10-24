using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace GillenwaterChordRev3
{
    // Offers a simplified interface for creating a hash string based on a string input.
    public class EZHash
    {
        // Get the hash of a string as a string
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
}
