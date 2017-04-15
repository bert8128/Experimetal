using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler34
{
    public static string answer()
    {
        double d = Math.Pow(10, 7);
        int max = (int)d;

        int maxSum = 0;
        int[] factorials = new int[10];
        factorials[0] = 1;
        for (int k=1; k<10; ++k)
        {
            factorials[k] = Utils.factorial(k);
        }
        for (int i=3; i<max; ++i)
        {
            int sum = 0;
            int n = i;
            while (n > 0)
            {
                int digit = n % 10;
                n = n / 10;
                int f = factorials[digit];
                sum += f;
            }
            if (i == sum)
            {
                maxSum += sum;
            }
        }
        return Convert.ToString(maxSum);
    }
}

