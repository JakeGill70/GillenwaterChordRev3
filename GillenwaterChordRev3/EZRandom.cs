using System;
using System.Collections.Generic;
using System.Text;

namespace GillenwaterChordRev3
{
    class EZRandom
    {
        static Random rand = new Random();

        public static int Next()
        {
            return rand.Next();
        }

        public static int Next(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
