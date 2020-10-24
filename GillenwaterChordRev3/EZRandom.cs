using System;
using System.Collections.Generic;
using System.Text;

namespace GillenwaterChordRev3
{
    // Offers a simplified interface for getting random numbers
    class EZRandom
    {
        static Random rand = new Random();

        // Get a random integer from a given range.
        // min is inclusive and max is exclusive.
        public static int Next(int min, int max)
        {
            return rand.Next(min, max);
        }
    }
}
