using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;
using System.Diagnostics;

class Euler391
{
    static Int64 Bitcount(Int64 n)
    {
        Int64 count = 0;
        while (n != 0)
        {
            ++count;
            n &= (n - 1);
        }
        return count;
    }

    static Int64 sequenceval(Int64 n)
    {
        Int64 val = 0;
        while (n > 0)
        {
            val += Bitcount(n--);
        }
        return val;
/*
 *      BigNum a = new BigNum("0");
        BigNum b = new BigNum("1");
        Int64 count = a.countBinaryOnes();
        Int64 sum = 0;
        for (int i = 1; i <= n; ++i)
        {
            a = a.add(b);
            count = a.countBinaryOnes();
            sum += count;
        }
        return sum;
 * */
    }

    public static string answer()
    {
        List<int> numbers = new List<int>();
        BigNum a = new BigNum("0");
        BigNum b = new BigNum("1");
        int max = 10;
        int upper = (int)Math.Pow(2, max);
        Int64 count = a.countBinaryOnes();
        for (int i = 1; i <= upper; ++i)
        {
            a = a.add(b);
            count = a.countBinaryOnes();
            numbers.Add((int)count);
        }
        List<int> sequence = new List<int>(numbers.Count);
        sequence.Add(1);
        Console.WriteLine("{0}\t{1}", 0, 0);
        Console.WriteLine("{0}\t{1}", 1, 1);
        for (int i = 1; i < numbers.Count; ++i)
        {
            int prev = sequence[i - 1];
            int ones = numbers[i];
            sequence.Add(prev+ones);
            Console.WriteLine("{0}\t{1}", i+1, prev+ones);
        }
        Console.WriteLine("n, first point where you can't jump 2, first point whre you can't jump n");
        for (int i = 3; i <= max; ++i)
        {
            int point = 0;
            // find first point where you can't jump two
            for (int j = 0; j < sequence.Count - 2; ++j)
            {
                if (sequence[j + 2] - sequence[j] > i)
                {
                    point = j;
                    break;
                }
            }
            Console.WriteLine("{0}\t{1}\t{2}", i, point + 2, (int)Math.Pow(2, i) - 1);
        }
        Int64 sumcubes = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        int minn = 0;
        int maxn = 20;
        for (int n = minn; n <= maxn; ++n)
        {
            Int64 binarynum = 0;
            if (n <= 7)
            {
                binarynum = (1 << n + 1) - 1;
            }
            else if ((n&1) == 0)
            {
                binarynum = (Int64)Math.Pow(2, (1 + ((n + 1) / 2))) - 3;
            }
            else
            {
                binarynum = (Int64)Math.Pow(2, (1 + (n / 2))) - 2;
            }

            //Console.WriteLine("N = " + n);
            //Console.WriteLine("Can't get from " + (binarynum - 1) + " (" + SparseBitcount(binarynum - 1) + " bits) to " + binarynum + " (" + SparseBitcount(binarynum) + " bits)");
            //Console.WriteLine("Lose: " + (binarynum - 1));
            Int64 lowestlose = binarynum - 1;
            while (binarynum > 1)
            {
                Int64 bitsdiff = 0;
                do
                {
                    bitsdiff += Bitcount(--binarynum);
                    //if (bitsdiff <= n)
                    //Console.WriteLine("Win: " + binarynum + " (" + bitsdiff + " bits)");
                } while (bitsdiff <= n);
                //Console.WriteLine("Lose: " + (binarynum - 1) + " (" + bitsdiff + " bits)");
                if (binarynum > 0)
                    lowestlose = binarynum - 1;
            }
            Int64 sv = sequenceval(lowestlose);
            Console.WriteLine("{0}\tM(" + n + ") = {1}", sw.Elapsed, sv);

            sumcubes += (sv * sv * sv);
        }
        return Convert.ToString(sumcubes);
    }
}
