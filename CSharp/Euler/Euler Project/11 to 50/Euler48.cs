using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler48
{
    public static string answer()
    {
        Int64 max = 10000000000;
        Int64 upper = 1000;
        Int64 count = 0;
        for (int i = 1; i <= upper; ++i )
        {
            Int64 thisCount = i;
            for (Int64 j = 1; j < i; ++j)
            {
                thisCount *= i;
                thisCount = thisCount % max;
            }
            count += thisCount;
            count = count % max;
        }
        return Convert.ToString(count);
    }
}
