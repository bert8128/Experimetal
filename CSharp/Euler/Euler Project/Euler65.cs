using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

class Euler65
{
    public static string answerInt64()
    {
        // answer = vd/vn
        double value = 1;
        for (Int64 i = 2; i < 11; ++i)
        {
            Int64 vd = 1;
            Int64 vn = 1;
            Int64 counter = i;
            if (1 == counter)
                value = vd = 2;
            else if ((counter % 3) == 0)
                value = vd = counter * 2 / 3;
            else
                value = vd = 1;
            while (counter > 1)
            {
                --counter;
                Int64 x = 0;
                if (1 == counter)
                    x = 2;
                else if ((counter % 3) == 0)
                    x = counter * 2 / 3; // an Int64eger, no rounding
                else
                    x = 1;
                value = x + 1.0 / value;
                Int64 a = x * vd + vn;
                Int64 b = vd;
                vd = a;
                vn = b;
            }
            Console.WriteLine("{0}\t{1}\t\t{2}\t{3}", i, 2.7, vd, vn);
        }
        return Convert.ToString(value);
    }
    public static string answer()
    {
        // 100x faster
        // answer = vd/vn
        BigNum vd = new BigNum(1);
        BigNum vn = new BigNum(1);
        vd = new BigNum(1);
        vn = new BigNum(1);
        int counter = 100;
        if (1 == counter)
            vd = new BigNum(2);
        else if ((counter % 3) == 0)
            vd = new BigNum(counter * 2 / 3);
        while (counter-- > 1)
        {
            int x = 1;
            if (1 == counter)
                x = 2;
            else if ((counter % 3) == 0)
                x = counter * 2 / 3; // an Int64eger, no rounding
            BigNum b = new BigNum(vd);
            vd = vd.mult(x);
            vd = vd.add(vn);
            vn = b;
//          Console.WriteLine("{0}\t{1}\t{2}", i, vd.asString(), vn.asString());
        }
        //Console.WriteLine("{0}\t{1}", vd.asString(), vn.asString());
        return Convert.ToString(vd.sumDigits());
    }
    public static string answer2()
    {
        // does it top down,so you can see how the raction is progressing
        uint limit = 100;
        BigNum lnum = new BigNum(0);
        BigNum lden = new BigNum(1);
        BigNum rnum = new BigNum(1);
        BigNum rden = new BigNum(0);
        bool goleft = true;
        for (int term = 1; term <= limit; ++term) 
        {
            int factor = 1;
            if (term % 3 == 0) 
                factor = term * 2 / 3;
            else if (term == 1) 
                factor = 2;
            while (factor-- > 0)
            {
                if (goleft)
                {
                    lnum = lnum.add(rnum);
                    lden = lden.add(rden);
                }
                else
                {
                    rnum = rnum.add(lnum);
                    rden = rden.add(lden);
                }
            }
            goleft = !goleft;
        }
        //Console.WriteLine("{0}\t{1}", rnum.asString(), rden.asString());
        return Convert.ToString(rnum.sumDigits());
    }
}



