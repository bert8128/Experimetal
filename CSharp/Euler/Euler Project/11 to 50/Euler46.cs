using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler46
{
    static int[] primes = new int[] { 17, 13, 11, 7, 5, 3 };

    static bool match(int n, int[] primes)
    {
        int m = (int)Math.Sqrt(n / 2); // max squarable
        for (int i = 1; i <= m; ++i)
        {
            //for each m,square,double and see if there is a prime that fits
            int mm = i * i * 2;
            int nn = n - mm;
            for (int j = 0; j < nn && primes[j] <= nn; ++j)
            {
                if (nn == primes[j])
                {
                    //Console.WriteLine("{0}\t{1}\t{2}", n, nn, i);
                    return true;
                }
            }
        }
        return false;
    }
    public static string answer()
    {
        int upperLimit = 1000000;
        int[] primes = Utils.seive(upperLimit);
        int p1, p2;
        for (int i = 2; i < primes.Length - 2; ++i)
        {
            p1 = primes[i];
            p2 = primes[i + 1];
            for (int j = p1+2; j < p2;)
            {
                if (!match(j, primes))
                    return Convert.ToString(j);
                j += 2;
            }
        }

        return Convert.ToString(0);
    }
}
