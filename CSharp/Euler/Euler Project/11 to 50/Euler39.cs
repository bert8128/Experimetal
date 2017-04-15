using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler37
{
    static bool isRightTruncatable(int n, HashSet<int> primes)
    {
        int pow = 10;
        while (pow < n)
        {
            int m = n / pow;
            if (!primes.Contains(m))
                return false;
            pow *= 10;
        }
        return true;
    }

    public static string answer()
    {
        int upper = 1000000;
        int[] pr = Utils.seive(upper);
        HashSet<int> primes = new HashSet<int>(pr);
        List<int> these = new List<int>();
        these.Add(3);
        these.Add(5);
        these.Add(7);
        int length = these.Count;
        int power = 10;
        int sum = 0;
        while (0 < these.Count)
        {
            List<int> those = new List<int>();
            for (int i = 0; i < these.Count; ++i)
            {
                int num = these[i];
                for (int j = 1; j < 10; ++j)
                {
                    if (primes.Contains(j * power + num))
                        those.Add(j * power + num);
                }
                if (num != 3 && num != 5 && num != 7 && isRightTruncatable(num, primes))
                    sum += num;
            }
            power *= 10;
            these = those; // start again with the new, longer set
        }
        return Convert.ToString(sum);
    }
}
