using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler69
{
    public static string answer()
    {
        int n=2;
        double num = 15499;
        double denom = 94744;
        double limit = num / denom;
        double cand = 0;
        double best = 1;
        int upper = 1000000;
        int[] primes = Utils.seive((int)Math.Sqrt(upper));
        int bestN = 0;
        while (n<=upper)
        {
            int m = Utils.EulersTotient(n, primes);
            cand = n/(double)m;
            if (cand > best)
            {
                best = cand;
                bestN = n;
            }
            if (n % 1000 == 0)
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", n, m, best, cand); 
            ++n;
        }
        return Convert.ToString(bestN);
    }
}
