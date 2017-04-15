using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler38
{
    public static string answer()
    {
        double d = Math.Pow(10, 5);
        Int64 upper = (Int64)d;
        Int64 max = 0;
        Int64 winner = 0;

        for (Int64 i = 1; i < upper; ++i)
        {
            Int64 sum = 0;
            Int64 prev = 0;
            for (int n = 1; n <= 9; ++n)
            {
                Int64 thisProd = n * i;
                int numDigits = (int)Math.Log10(thisProd)+1;
                sum *= (Int64)Math.Pow(10, numDigits);
                sum += thisProd;
                if (987654321 < sum)
                    break;
                prev = sum;
            }
            if (Utils.isPandigital((int)prev))
            {
                if (prev > max)
                {
                    max = prev;
                    winner = i;
                }
            }
        }
        return Convert.ToString(max);
    }
}

