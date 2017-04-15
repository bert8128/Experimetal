using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler393
{
    static int dim = 4;
    static int dimsq = dim * dim;
    static int max = 2 * ((dim * dim) - dim);
    static int hmax = max/2;

    static bool testOne(int thisLine, int[] points)
    {
        if (thisLine < hmax)
        {
            int row = thisLine / (dim - 1);
            int col = thisLine % (dim - 1);
            if (points[(row * dim) + col] > 1)
                return false;
            ++points[(row * dim) + col];
            if (points[(row * dim) + col + 1] > 1)
                return false;
            ++points[(row * dim) + col + 1];
        }
        else
        {
            int col = (thisLine - hmax) / (dim - 1);
            int row = (thisLine - hmax) % (dim - 1);
            if (points[(row * dim) + col] > 1)
                return false;
            ++points[(row * dim) + col];
            if (points[((row + 1) * dim) + col] > 1)
                return false;
            ++points[((row + 1) * dim) + col];
        }
        return true;
    }
/*    static bool test(int pos, bool[] lines)
    {
        int[] points = new int[dimsq];
        for (int j = 0; j < dimsq; ++j)
            points[j] = 0;

        for (int i = 0; i < max; ++i)
        {
            if (lines[i])
            {
                if (!testOne(i, points))
                    return false;
            }
        }
        for (int j = 0; j < dimsq; ++j)
            if (points[j] != 2)
                return false;

        return true;
    }
 * */
    static bool test2(int[] points)
    {
        for (int j = 0; j < dimsq; ++j)
            if (points[j] != 2)
                return false;

        return true;
    }
    static int numSols(int pos, bool[] lines, int numLines, int[] points)
    {
        if (pos == max)
        {
            if (numLines != dimsq)
                return 0;
            else if (test2(points))
                return 1;
            else
                return 0;
        }
        int sols = 0;
        bool[] lines2 = (bool[])lines.Clone();
        int[] points2 = (int[])points.Clone();
        //lines2[pos] = false;
        if (dimsq-numLines < max-pos) // check to see if we can have enough lines - we need exactly dimsq lines
            sols += numSols(pos + 1, lines2, numLines, points2);
        lines2[pos] = true;
        ++numLines;
        if (numLines <= dimsq) // since we need exactly dimsq lines, there isn't any point in testing with more
        {
            if (testOne(pos, points2))
                sols += numSols(pos + 1, lines2, numLines, points2);
        }
        return sols;
    }
    public static string answer()
    {
        bool[] lines = new bool[max];
        for (int i = 0; i < max; ++i)
            lines[i] = false;

        int[] points = new int[dimsq];
        for (int j = 0; j < dimsq; ++j)
            points[j] = 0;
        
        int sols = numSols(0, lines, 0, points);

        return Convert.ToString(sols);
    }
}
