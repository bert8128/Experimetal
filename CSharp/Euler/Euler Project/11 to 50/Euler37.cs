using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler39
{
    public static string answer()
    {
        int max = 0;
        int maxp = 0;
        for (int p = 1; p <= 1000; ++p)
        {
            if ((p & 1) == 0)
            {
                int count = 0;
                int pover2 = p / 2;
                for (int a = 1; a < pover2; ++a)
                {
                    int asq = a * a;
                    for (int b = a + 1; b < pover2; ++b)
                    {
                        double c = Math.Sqrt(asq + b * b);
                        if (a + b + c == p)
                            ++count;
                    }
                }
                if (count > max)
                {
                    max = count;
                    maxp = p;
                }
            }
        }
        /*
        int[] results = new int[1001];
        for (int i=0; i<1001; ++i)
        {
            results[i] = 0;
        }
        int a, b, c;

        for (a = 1; a < 1000; a++)
            for (b = 1; b < 1000; b++)
                for (c = b; c < 1000; c++)
                    if (a+b+c <= 1000)
                        if (a*a + b*b == c*c)
                            results[a+b+c]++;

        for (a = 0; a < 1000; a++)
            if (results[a] > maxp)
            {
                max = results[a];
                maxp = a;
            }
        */

        return Convert.ToString(maxp);
    }
}
