using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler32
{
    public static string answer()
    {
        List<string> strings = new List<string>(9 * 8 * 7 * 6 * 5 * 4 * 3 * 2 * 1);
        char[] digits = new char[] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        Permute.permute(digits, strings); // generate all the permutations of the digits as 9 digit strings
        List<int> answers = new List<int>();
        int sum = 0;
        int[] powers = new int[8]; // a cache of powers of 10 - we'll need these later
        for (int j = 0; j < 8; ++j)
            powers[j] = (int)Math.Pow(10, j);
        foreach (string s in strings)
        {
            int a = 1, b = 1, c = 7;
            // start by converting from string to int. 
            // For speed, we'll use maths rather than string manipulation to move the digits between the three substrings
            int ai = Convert.ToInt32(s.Substring(0, a));
            int bi = Convert.ToInt32(s.Substring(a, b));
            int ci = Convert.ToInt32(s.Substring(a + b, c));
            for (int i = 0; i < 28; ++i) // there are only 28 arrangements (1+2+3+4+5+6+7)
            {
                if (ai * bi == ci && !answers.Contains(ci))
                {
                    answers.Add(ci);
                    sum += ci;
                }
                if (i < 27)
                {
                    if (c > 1)
                    {
                        // move the first digit of c to the last digit of b
                        --c;
                        ++b;
                        int x = powers[c];
                        int y = ci / x;
                        bi *= 10;
                        bi += y;
                        ci -= y * x;
                    }
                    else
                    {
                        // move the first digit of b to the last digit of a
                        int x = powers[b - 1];
                        int y = bi / x;
                        ai *= 10;
                        ai += y;
                        bi -= y * x;

                        // move all but the first digit of b onto c
                        x = powers[b - 2];
                        y = bi / x;
                        ci += (bi - (y * x)) * powers[c];

                        // remove all but the first digit of b
                        bi = y;

                        ++a;
                        b = 1;
                        c = 9 - a - b;
                    }
                }
            }
        }
        return Convert.ToString(sum);
    }
}

class Euler35
{
    public static string answer()
    {
        int[] powers = new int[8]; // a cache of powers of 10 - we'll need these later
        for (int j = 0; j < 8; ++j)
            powers[j] = (int)Math.Pow(10, j);
        int length = 6;
        int digits = 1;
        int trigger = digits * 10;
        int upper = powers[length];
        int[] ps = Utils.seive(powers[length]);
        HashSet<int> primes = new HashSet<int>(ps);
        int count = 0;
        int power = powers[digits - 1];
        for (int i = 2; i < upper; ++i)
        {
            if (i == trigger)
            {
                ++digits;
                power = powers[digits - 1];
                trigger *= 10;
            }
            if (((i & 1 ) == 1) // if it is even then we're finished with this one
                && primes.Contains(i))
            {
                int num = i;
                bool prime = true;
                if (digits > 1)
                {
                    int max = digits - 1;
                    for (int j = 0; prime && j < max; ++j)
                    {
                        int pop = num % 10;
                        if ((pop & 1) == 1)  // if it is even then we're finished with this one
                        {
                            prime = false;
                        }
                        else
                        {
                            num /= 10;
                            num += (pop * power);
                            if (!primes.Contains(num))
                                prime = false;
                        }
                    }
                }
                if (prime)
                    ++count;
            }
        }
        return Convert.ToString(count);
    }
}
