using System;

namespace Euler24
{
    class Program
    {
        static void Main(string[] args)
        {
            int maxn = 20;
            int sumcubes = 0;
            
            for (int n = 1; n <= maxn; n++)
            {
                int binarynum = (1 << n + 1) - 1;
                //Console.WriteLine("N = " + n);
                //Console.WriteLine("Can't get from " + (binarynum - 1) + " (" + SparseBitcount(binarynum - 1) + " bits) to " + binarynum + " (" + SparseBitcount(binarynum) + " bits)");
                //Console.WriteLine("Lose: " + (binarynum - 1));
                int lowestlose = binarynum - 1;
                while (binarynum > 1)
                {
                    int bitsdiff = 0;
                    do
                    {
                        bitsdiff += Bitcount(--binarynum);
                        //if (bitsdiff <= n)
                            //Console.WriteLine("Win: " + binarynum + " (" + bitsdiff + " bits)");
                    } while (bitsdiff <= n);
                    //Console.WriteLine("Lose: " + (binarynum - 1) + " (" + bitsdiff + " bits)");
                    if (binarynum  > 0)
                        lowestlose = binarynum - 1;
                }
                int sv = sequenceval(lowestlose);
                Console.WriteLine("M(" + n + ") = " + sv);

                sumcubes += (sv*sv*sv);
            }
            Console.WriteLine("Sum of cubes = " + sumcubes);
            Console.WriteLine("Finished");
        }

        static int Bitcount(int n)
        {
            int count = 0;
            while (n != 0)
            {
                count++;
                n &= (n - 1);
            }
            return count;
        }

        static int sequenceval(int n)
        {
            int val = 0;
            while (n > 0)
            {
                val += Bitcount(n--);
            }
            return val;
        }
    }
}
