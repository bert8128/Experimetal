using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler23
{
    public static string answer()
    {
        int max = 28123;
        int[] factors = new int[max];
        bool[] sums = new bool[max];
        for (int i = 0; i < max; ++i)
        {
            factors[i] = 0;
            sums[i] = false;
        }
        for (int i = 1; i < max; ++i)
        {
            for (int j = i; j < max; )
            {
                if (j > i)
                    factors[j] += i;
                j += i;
            }
        }

        List<int> abs = new List<int>(7000);
        for (int i = 12; i < max; ++i)
        {
            if (i < factors[i])
            {
                int count = abs.Count();
                for (int j = 0; j < count; ++j)
                {
                    int aj = i + abs[j];
                    if (aj < max && !sums[aj])
                        sums[aj] = true;
                }
                int ai = i << 1;
                if (ai < max && !sums[ai])
                    sums[ai] = true;
                abs.Add(i);
            }
        }
        Int64 answer = 0;
        for (int j = 0; j < sums.Length; ++j)
        {
            if (!sums[j])
                answer += j;
        }
        return Convert.ToString(answer);
    }
}