using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

class Euler26
{
    public static string answer()
    {
        /*

        int sequenceLength = 0;

        for (int i = 17; i < 1000; i--)
        {
            if (sequenceLength >= i)
            {
                break;
            }

            int[] foundRemainders = new int[i];
            int value = 1;
            int position = 0;
            int quotient = 0;
            while (foundRemainders[value] == 0 && value != 0)
            {
                foundRemainders[value] = position;
                value *= 10;
                quotient = value / i;
                value = value % i; // the remainder
                position++;
            }

            if (position - foundRemainders[value] > sequenceLength)
            {
                sequenceLength = position - foundRemainders[value];
            }
        } 
       */
        /*BigNum aa = new BigNum(1);
        BigNum bb = aa.add(aa);
        BigNum bb2 = aa.divRecurring(7, 7);
        return bb2.asString();*/
 
        BigNum a = new BigNum(1);
        int max = 1;
        int maxPos = 1;
        for (int i = 999; i > 1; --i)
        {
            if (i < max)
                break;
            BigNum b = a.divRecurring(i, i);
            string str = b.asString(true);
            if (b.m_RepeatLength > max)
            {
                max = b.m_RepeatLength;
                maxPos = i;
            }
        }
        //Console.Write(maxPos);
        //Console.Write(' ');
        //Console.Write(max);
        //Console.Write(' ');
        return Convert.ToString(maxPos);
    }
}