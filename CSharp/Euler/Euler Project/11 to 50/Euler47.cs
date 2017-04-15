using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler47
{
    static int distinct(int[] factors)
    {
        if (factors.Length < 2)
            return factors.Length;
        int count = 1;
        for (int i = 0; i < factors.Length-1; ++i)
        {
            if (factors[i] != factors[i + 1])
                ++count;
        }
        return count;
    }
    public static string answer()
    {
        int upper = 1000000;
        int[] primes = Utils.seive(upper);
        int lookingFor = 4;
        int i = 3;
        for (; i<upper;++i)
        {
            int[] pfs = Utils.primeFactors(i, primes);
            if (distinct(pfs) == lookingFor)
            {
                bool more = true;
                for (int j = 1; j < lookingFor; ++j)
                {
                    pfs = Utils.primeFactors(i+j, primes);
                    if (distinct(pfs) != lookingFor)
                    {
                        more = false;
                        i += j;
                    }
                }
                if (more)
                    break;
            }
        }
        return Convert.ToString(i);
    }
}
