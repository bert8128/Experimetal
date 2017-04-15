using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

class Euler25
{
    public static string answer()
    {
        BigNum first = new BigNum(1);
        BigNum second = new BigNum(1);
        BigNum third = new BigNum(1);
        int term = 2;
        while (third.numDigits() < 1000)
        {
            ++term;
            third = first.add(second);
            first = second;
            second = third;
        }
        return Convert.ToString(term);
    }
}