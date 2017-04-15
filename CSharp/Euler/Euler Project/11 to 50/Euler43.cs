using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler43
{
    static int[] primes = new int[] { 17, 13, 11, 7, 5, 3 };

    static bool match(int m, int minTests, int maxTests)
    {
        int start1 = 1;
        for (int j = 0; j < maxTests && j < 6; ++j)
        {
            if (j >= minTests && j != 0 && j != 3)
            {
                int x = m / start1;
                x = x % 1000;
                if (x % primes[j] != 0)
                    return false;
            }
            start1 *= 10;
        }
        return true;
    }
    public static string answer()
    {
        Int64 sum = 0;
        int p3 = 1000;
        int p6 = 1000000;
        Int64 p9 = 1000000000;
        int tests = 0;

        for (int l = 0; l < p3; l += 17)
        {
            if (!Utils.hasRepeatingDigits(l))
            {
                for (int k = 0; k < p3; k += 7)
                {
                    int kk = k * p3 + l;
                    if (match(kk, 0, 4) && !Utils.hasRepeatingDigits(kk))
                    {
                        for (int j = 0; j < p3; j += 2)
                        {
                            int jj = j * p6 + kk;
                            if (match(jj, 4, 6) && !Utils.hasRepeatingDigits(jj))
                            {
                                for (int i = 1; i <= 9; ++i)
                                {
                                    ++tests;
                                    Int64 x = jj + i * p9;
                                    if (Utils.isTenDigitPandigital64(x))
                                        sum += x;
                                }
                            }
                        }
                    }
                }
            }
        }
        return Convert.ToString(sum);
    }
}
