using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

class Euler16
{
    public static Int64 answer()
    {
        BigNum num = new BigNum("2");
        BigNum two = new BigNum("2");
        for (int j = 1; j < 1000; ++j)
        {
            num = num.mult(two);
        }
        return num.sumDigits();
    }
}