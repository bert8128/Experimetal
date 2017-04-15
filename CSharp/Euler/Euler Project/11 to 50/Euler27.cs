using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler27
{
    public static string answer()
    {
        int upperLimit = 3000000;
        int[] primes2 = Utils.seive(upperLimit);
        bool[] primes = new bool[upperLimit];
        int k = 0;
        for (; k < upperLimit; ++k)
            primes[k] = false;
        for (k = 0; k < primes2.Length; ++k)
            primes[primes2[k]] = true;

        Int64 tests = 0;

        int upper = 1000;
        int maxN = 0;
        int maxA = 0;
        int maxB = 0;

        int a = -999;
        int b = -999;
        for (; a < upper; ++a)
        {
            b = -999;
            for (; b < upper; ++b)
            {
                int n = 0;
                while (true)
                {
                    ++tests;
                    int p = (n * n) + (a * n) + b;
                    if (p > 0 && primes[p])
                        ++n;
                    else
                        break;
                }
                if (n > 0 && n > maxN)
                {
                    maxN = n;
                    maxA = a;
                    maxB = b;
                }
            }
        }
        return Convert.ToString(maxA*maxB);
    }
}