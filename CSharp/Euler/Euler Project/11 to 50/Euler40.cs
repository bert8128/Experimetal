using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler40
{
    static int check(int whereAmI, int number, int numberDigits, int location)
    {
        if (whereAmI == location || (whereAmI < location && whereAmI + numberDigits-1 >= location))
        {
            int digitToGet = location - whereAmI;
            return Utils.getDigit(number, digitToGet, numberDigits);
        }
        else
            return -1;
    }
    public static string answer()
    {
        int product = 1;
        int lower = 1;
        int upper = 1;
        int number = 1;
        int numberDigits = 1;
        int whereAmI = 1;
        int location = 10;
        while (location < 1000001)
        {
            lower = upper;
            upper *= 10;
            for (int i = lower; i < upper; ++i)
            {
                int thisDigit = check(whereAmI, number, numberDigits, location);
                if (thisDigit >= 0)
                {
                    product *= thisDigit;
                    location *= 10;
                }
                whereAmI += numberDigits;
                ++number;
            }
            ++numberDigits;
        }
        return Convert.ToString(product);
    }
}
