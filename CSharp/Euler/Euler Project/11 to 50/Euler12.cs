using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler12
{
    static public int PrimeFactorisationNoD(int number, int[] primelist)
    {
        int nod = 1;
        int exponent;
        int remain = number;

        for (int i = 0; i < primelist.Length; i++)
        {
            // In case there is a remainder this is a prime factor as well
            // The exponent of that factor is 1
            int prime = primelist[i];
            if (prime * prime > number)
            {
                return nod * 2;
            }

            exponent = 1;
            while (remain % prime == 0)
            {
                exponent++;
                remain = remain / prime;
            }
            nod *= exponent;

            //If there is no remainder, return the count
            if (remain == 1)
            {
                return nod;
            }
        }
        return nod;
    }
    public static void getPrimeFactors(int[] primes, IList<int> factors, int num)
    {
        //get first prime factor
        int p = -1;
        int totP = primes.Length;
        for (int j = 0; j < totP && primes[j] < (1 + num / 2); ++j)
        {
            if ((num % primes[j]) == 0)
            {
                factors.Add(primes[j]);
                p = primes[j];
                break;
            }
        }
        if (p < 0)
        {
            factors.Add(num);
            return;
        }
        if (p == num)
            return;
        num = num / p;
        getPrimeFactors(primes, factors, num);
    }

    public static int divisorFunc(int[] primes, int max)
    {
        var pfs  = new List<int>(501);
        getPrimeFactors(primes, pfs, max);
        int last = -1;
        int count = 1;
        int prod = 1;
        for (int j = 0; j < pfs.Count(); ++j)
        {
            int thisOne = pfs[j];
            if (last == thisOne)
            {
                ++count;
            }
            else
            {
                if (count > 0)
                    prod *= count+1;
                count = 1;
                last = thisOne;
            }
        }
        return prod;
    }
    public static int answer(int[] primes)
    {
        int max = 0;
        int inc = 2;
        int seived = primes[primes.Length-1];// max * 8;
        //var primes = Utils.seive(seived);
        int maxMax = -1;
        int maxDiv = -1;
        int Dn1 = 2;
        int Dn = 2;
        int prod = 1;
        while (prod < 500)
        {
            if (seived < max)
            {
                seived *= 8;
                primes = Utils.seive(seived);
            }
            /*var pfs  = new List<int>(501);
            getPrimeFactors(primes, pfs, max);
            int last = -1;
            int count = 1;
            int prod = 1;
            for (int j = 0; j < pfs.Count(); ++j)
            {
                int thisOne = pfs[j];
                if (last == thisOne)
                {
                    ++count;
                }
                else
                {
                    if (count > 0)
                        prod *= count+1;
                    count = 1;
                    last = thisOne;
                }
            }
            */
            //0
            //prod = divisorFunc(primes, max);
            //1
            //prod = PrimeFactorisationNoD(max, primes);
            //2
            if (inc % 2 == 0)
            {
                Dn = PrimeFactorisationNoD(inc + 1, primes);
                prod = Dn * Dn1;
            }
            else
            {
                Dn1 = PrimeFactorisationNoD((inc + 1) / 2, primes);
                prod = Dn * Dn1;
            }
            /*
            if (inc % 2 == 0)
            {
                Dn = divisorFunc(primes, inc + 1);
                prod = Dn * Dn1;
            }
            else
            {
                Dn1 = divisorFunc(primes, (inc + 1) / 2);
                prod = Dn * Dn1;
            }*/
            //if (prod <= 500)
            {
                max += inc;
                inc++;
            }
        }
        return inc * (inc - 1) / 2;
        //return max;
    }
}