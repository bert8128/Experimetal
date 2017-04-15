using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

class Euler56
{
    public static string answer()
    {
        int max = 100;
        long result = 0;
        int ra = 0;
        int rb = 0;
        for (int i = 2; i <= max; ++i)
        {
            BigNum num = new BigNum(i);
            for (int j = 1; j <= max; ++j)
            {
                num = num.mult(i);
                long sum = num.sumDigits();
                if (sum > result)
                {
                    result = sum;
                    ra = i;
                    rb = j;
                }
            }
        }
        return Convert.ToString(result) + " " + Convert.ToString(ra) + " " + Convert.ToString(rb) + " ";
    }
}