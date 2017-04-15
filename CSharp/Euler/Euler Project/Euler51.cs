using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler51
{
    static bool anagramatical(int[] a, int[] b)// takes a set of digits, consumes a
    {
        return true;
    }
    static bool isPrime(int n)
    {
        int max = (int)Math.Sqrt(n);
        for (int i = 2; i <= max; ++i)
            if ((n % i) == 0)
                return false;
        return true;
    }
    public static string answer()
    {
        int upper = 1000000000; // 1bn
        int[] primes = Utils.seive(upper);
        int x = primes.Length; // about 50,847,534 of them
        int lastP = primes[primes.Length - 1];
        int max = 0;
        int maxStart = 0;
        int limit = 1000000;
        int maxLength = 0;
        for (int i = 0; i < x; ++i )
        {
            if (x - i < maxLength)
                break;
            if (primes[i] >= limit)
                break;
            int sum = primes[i];
            int length = 1;
            for (int j = i+1; j < x; ++j)
            {
                if (primes[j] >= limit)
                    break;
                sum += primes[j];
                if (sum >= limit)
                    break;
                ++length;
                if (length > maxLength)
                {
                    if (isPrime(sum))
                    {
                        max = sum;
                        maxStart = primes[i];
                        maxLength = length;
                    }
                }
            }
        }
        return Convert.ToString(maxStart) + " " + Convert.ToString(maxLength) + " " + Convert.ToString(max);
    }
}
