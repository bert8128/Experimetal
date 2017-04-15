using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler36
{
    static bool isPalindromic(int n, int b)
    {
        if (b == 2)
        {
            uint m = (uint)n;
            uint start = 1 << 30;
            uint length = 30;
            while ((start & m) == 0)
            {
                start = start >> 1;
                --length;
            }
            uint end = 1;
            uint upper = length / 2;
            for (uint i = 0; i <= upper; ++i)
            {
                if (((start & m) == 0) != ((end & m) == 0))
                    return false;
                start = start >> 1;
                end = end << 1;
            }
        }
        else
        {
            List<int> numbers = new List<int>();
            int remainder = 0;
            do
            {
                remainder = n % b;
                n = n / b;
                numbers.Add(remainder);
            }
            while (n > 0);
            int j = 0;
            int length = numbers.Count;
            int upper = length / 2;
            while (j <= upper)
            {
                if (numbers[j] != numbers[length - j - 1])
                    return false;
                j += 1;
            }
        }
        return true;
    }
    public static string answer()
    {
        int upper = 1000000;
        int sum = 0;
        for (int n = 1; n < upper; )
        {
            if (isPalindromic(n, 2))
            {
                if (isPalindromic(n, 10))
                    sum += n;
            }
            n += 2;
        }
        return Convert.ToString(sum);
    }
}
