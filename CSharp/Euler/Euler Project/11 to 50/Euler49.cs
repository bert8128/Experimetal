using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler49
{
    static bool anagramatical(int[] a, int[] b)// takes a set of digits, consumes a
    {
        if (a.Length != b.Length)
            return false;
        for (int i = 0; i < b.Length; ++i)
        {
            int c = b[i];
            int j = 0;
            for (; j < a.Length; ++j)
            {
                if (b[i] == a[j])
                {
                    a[j] = -1;
                    break;
                }
            }
            if (j == a.Length || a[j] != -1)
                return false;
        }
        return true;
    }
    static bool anagramatical(int a, int b, int c)// takes a set of digits, consumes a
    {
        int[] digits1 = new int[4];
        int[] digits2 = new int[4];
        for (int k = 0; k < 4; ++k)
        {
            digits1[k] = a % 10;
            a /= 10;
        }
        for (int k = 0; k < 4; ++k)
        {
            digits2[k] = b % 10;
            b /= 10;
        }
        if (!anagramatical(digits1, digits2))
            return false;
        for (int k = 0; k < 4; ++k)
        {
            digits1[k] = c % 10;
            c /= 10;
        }
        return anagramatical(digits1, digits2);
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
        int upper = 10000;
        int[] primes = Utils.seive(upper);
        int x = primes.Length;
        for (int i = 168; i < x; ++i)
        {
            int start = primes[i];
            if (start == 1487)
                continue;
            int n = 1;
            while ((i + (2 * n)) <= x)
            {
                int diff = primes[i + n] - start;
                if (isPrime(start + diff + diff))
                {
                    if (anagramatical(start, start + diff, start + diff + diff))
                        return Convert.ToString(start) + Convert.ToString(start + diff) + Convert.ToString(start + diff + diff);
                }
                ++n;
            }
        }
        return Convert.ToString(upper);
    }
}
