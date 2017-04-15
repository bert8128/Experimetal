using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler41
{
    static string test(int n)
    {
        if (n > 9)
            return null;

        List<string> strings = new List<string>(Utils.factorial(n));
        List<char> chars = new List<char>(9);
        for (int i = 1; i <= n; ++i)
            chars.Add((char)(i + '0'));
        char[] digits = chars.ToArray();
        Permute.permute(digits, strings); // generate all the permutations of the digits as 7 digit strings
        int upper = strings.Count;
        int tests = 0;
        for (int i = upper - 1; i >= 2; --i)
        {
            ++tests;
            string p = strings[i];
            int pi = Convert.ToInt32(p);
            if (Utils.isPrime(pi))
                return p;
        }
        return null;
    }
    public static string answer()
    {
        /* slower
        double d = Math.Pow(10, 7);
        int upper = (int)d;
        int[] primes = Utils.seive(upper);
        upper = primes.Length;
        for (int i = upper-1; i >= 2; --i)
        {
            int p = primes[i];
            if (Utils.isPandigital(p))
                return Convert.ToString(p);
        }
         * */
        // faster
        for (int i = 7; i > 0; --i) // no need to test 8 or 9 digits, as these are always divisble by 3
        {
            string s = test(i);
            if (null != s)
                return s;
        }
        return Convert.ToString(0);
    }
}
