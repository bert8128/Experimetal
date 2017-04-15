using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;
using System.IO;

class Euler92
{
    static int[] squares = new int[] { 0, 1, 4, 9, 16, 25, 36, 49, 64, 81 };
    
    static int squareAndAdd(int n)
    {
        int r = n % 10;
        int result = 0;
        do
        {
            result += r * r;
            n /= 10;
            r = n % 10;
        }
        while (n != 0);
        return result;
    }
    public static string answer()
    {
        int[] arr = new int[10000000];
        int count = 0;
        for (int n = 1; n < 10000000; ++n)
        {
            int nn = n;
            while (true)
            {
                if (1 == nn || arr[nn] == 1)
                {
                    arr[n] = nn;
                    break;
                }
                else if (89 == nn || arr[nn] == 89)
                {
                    arr[n] = nn;
                    ++count;
                    break;
                }
                else
                {
                    nn = squareAndAdd(nn);
                }
            }
        }
        return Convert.ToString(count);
    }
}
