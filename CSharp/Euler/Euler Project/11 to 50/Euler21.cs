using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler21
{
    public static string answer()
    {
        int max = 10000;
        int[] factors = new int[max];
        for (int i = 1; i < max; ++i)
        {
            for (int j = i; j < max; )
            {
                if (j > i)
                    factors[j] += i;
                j += i;
            }
        }
        Int64 answer = 0;
        int count = 0;
        for (int i = 2; i < max; ++i)
        {
            if (factors[i] < max)
            {
                if (i < factors[i])
                {
                    if (i == factors[factors[i]])
                    {
                        ++count;
                        answer += i;
                        answer += factors[i];
                    }
                }
            }
        }
        return Convert.ToString(answer);
    }
}