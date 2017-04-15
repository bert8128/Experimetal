using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler44
{
    static int pentagon(int n)
    {
        return (n * (3 * n - 1) / 2);
    }
    static bool isPentagonal(int n)
    {
        //h:=(sqrt(24*x+1)+1)/6;
        int m = (int)(0.5 + Math.Sqrt(0.25 + 6*n)) / 3;
        int nn = pentagon(m);
        return (n == nn) ;
    }
    public static string answer()
    {
        Dictionary<int, int> pent = new Dictionary<int, int>();
        Dictionary<int, int> num = new Dictionary<int, int>();
        int i = 1;
        while (true)
        {
            int a, b;
            if (!pent.TryGetValue(i, out a))
            {
                a = pentagon(i);
                pent.Add(i, a);
                num.Add(a, i);
            }
            for (int j = 1; j < i; ++j)
            {
                if (!pent.TryGetValue(j, out b))
                {
                    b = pentagon(j);
                    pent.Add(j, b);
                    num.Add(b, j);
                }
                int sub = a - b;
                if (num.ContainsKey(sub))
                {
                    int sum = a + b;
                    if (isPentagonal(sum))
                        return Convert.ToString(a - b);
                }
            }
            ++i;
        }
        return Convert.ToString(i);
    }
}
