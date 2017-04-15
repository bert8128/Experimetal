using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler243
{
    public static string answer()
    {
 //       int lower = 9699690; // I know it's more than this
        int upper = 223092870;// I know that this fits the bill - just have to find the smallest now
        //892371480
        //223092828
        double num = 15499;
        double denom = 94744;
        double limit = num / denom;
        double cand = 0;
        double best = 1;
        double bestN = 1;

        int[] primes = Utils.seive((int)Math.Sqrt(upper));

        Int64 max = 0;
        for (int i = 1; i < 11; ++i)
        {
            Int64 n = 1;
            int[] factors = new int[i];
            for (int j = 0; j < i; ++j)
            {
                factors[j] = primes[j];
                n = n * primes[j];
            }
            Int64 m = Utils.EulersTotient2(n, factors);
            double dn = n-1;
            double m3 = m / dn;
            if (m3 < limit)
            {
                max = i - 1;
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", i, n, m, m3);
                break;
            }
        }
        {
            Int64 n = 1;
            int[] factors = new int[max];
            for (int j = 0; j < max; ++j)
            {
                factors[j] = primes[j];
                n = n * primes[j];
            }
            max = n;

            {
                // check 2
                n = max;
                for (int i = 1; i < 100; ++i)
                {
                    n *= 2;
                    Int64 m = Utils.EulersTotient2(n, factors);
                    double dn = n - 1;
                    double m3 = m / dn;
                    if (m3 < limit)
                    {
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}", i, n, m, m3);
                        bestN = n;
                        best = n;
                        break;
                    }
                }
            }
            {
                // check 3
                n = max;
                for (int i = 1; i < 100; ++i)
                {
                    n *= 3;
                    Int64 m = Utils.EulersTotient2(n, factors);
                    double dn = n - 1;
                    double m3 = m / dn;
                    if (m3 < limit & bestN > n)
                    {
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}", i, n, m, m3);
                        bestN = n;
                        best = n;
                        break;
                    }
                }
            }
            {
                // check 5
                n = max;
                for (int i = 1; i < 100; ++i)
                {
                    n *= 5;
                    Int64 m = Utils.EulersTotient2(n, factors);
                    double dn = n - 1;
                    double m3 = m / dn;
                    if (m3 < limit & bestN > n)
                    {
                        Console.WriteLine("{0}\t{1}\t{2}\t{3}", i, n, m, m3);
                        bestN = n;
                        best = n;
                        break;
                    }
                }
            }
        }


        //        while (best > limit && n <= upper)
        /*for (int i = primes.Length - 1; i >= 0; --i)
        {
            int n = primes[i] + i;
            int m = Utils.EulersTotient(n, primes);
            cand = m / (double)(n - 1);
            if (cand < best)
                best = cand;
            if (i % 100000 == 0)
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", n, m, best, cand);
            ++n;
        }*/
        return Convert.ToString(best);
    }
}
