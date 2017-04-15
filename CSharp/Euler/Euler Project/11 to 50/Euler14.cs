using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler14
{
    public static int seqLength(Int64 i, int[] soFar)
    {
        int length = 0;
        Int64 orig = i;
        while (1 != i)
        {
            if ((i & 1) == 0) 
                i = i >> 1;
            else
            {
                i += i<<1;
                ++i;
            }
            if (i < orig)
            {
                length += soFar[i];
                break;
            }
            ++length;
        }
        soFar[orig] = length;
        return length;
    }
 
    public static int answer()
    {
        int max = 1000000;
        int highest = 1;
        int maxLength = 1;
        int length = 0;
        int[] soFar=new int[max+1];
        soFar[0] = 0;
        soFar[1] = 1;
        for (int j = 2; j < max; ++j)
        {
            length = seqLength(j, soFar);
            if (length > maxLength)
            {
                maxLength = length;
                highest = j;
            }
        }
        return highest;
    }
}