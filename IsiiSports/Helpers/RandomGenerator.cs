using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IsiiSports.Helpers
{
    public sealed class RandomGenerator
    {
        private int latestNumber = 0;
        private Random r;

        private static readonly Lazy<RandomGenerator> lazy = new Lazy<RandomGenerator>(() => new RandomGenerator());

        public static RandomGenerator Instance => lazy.Value;

        private RandomGenerator()
        {
            r = new Random();
        }

        public int GetNext(int lowerBound, int upperBound)
        {
            var rnd = r.Next(lowerBound, upperBound);
            if (latestNumber == rnd)
                if (rnd + 1 > upperBound)
                    rnd = lowerBound;
                else
                    rnd++;
            latestNumber = rnd;
            return rnd;
        }
    }
}
