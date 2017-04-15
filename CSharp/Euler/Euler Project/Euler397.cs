using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler397
{
    public static double cos45 = Math.Cos(45 * Math.PI / 180);
    public static bool is45(Int64 a, Int64 b, Int64 c, double asqrt, double bsqrt)
    {
        double n = (a + b - c);
        if (n != 0)
        {
            double d = (2 * asqrt * bsqrt);
            double cosC = n / d;
            return (cosC > 0.70710678118 &&
                      cosC < 0.70710678119);
        }
        return false;
    }
    public static double angle(double a, double b, double c)
    {
        double n = a * a + b * b - c * c;
        double d = 2 * a * b;
        double cosC = (n) / (d);
        return cosC;
    }
    public static double angle2(double a, double b, double c)
    {
        double n = a + b - c;
        double d = 2 * Math.Sqrt(a) * Math.Sqrt(b);
        double cosC = (n) / (d);
        return cosC;
    }
    public static Int64 distance(Int64 a, Int64 b, Int64 k)
    {
        Int64 diffX = b-a;
        Int64 diffY = b*b/k - a*a/k;
        return diffX * diffX + diffY * diffY;
    }
    public static Int64 f(Int64 K, Int64 X)
    {
//        double fortyFour = 0.707;//Math.Cos(44 / (180 / Math.PI));
//        double fortyFive = Math.Cos(45 / (180 / Math.PI));
//        double fortySix = 0.708;//Math.Cos(46 / (180 / Math.PI));
        Int64 solns = 0;
        Int64 solns2 = 0;
        Int64 k = K;
        //for (Int64 k = 1; k <= K; ++k)
        {
            for (Int64 a = -X; a <= 0; ++a)
            {
                double maxb = Math.Atan(X + a - 1);
                //Console.WriteLine("a {0} maxb {1}", a, maxb);
                for (Int64 b = a+1; b <= X; ++b)
                {
                    bool bMult = false;
                    Int64 n = (b * b - a * a);
                    Int64 d = (b - a);
                    double m = 0;
                    double intersect = 0;
                    if (n % d == 0)
                    {
                        m = (double)(-1 - b - a) / (double)(1 - b - a);
                        intersect = b*b - m*b;
                        double dx = 0;
                        bMult = true;
                    }
                    bool bFoundOne = false;
                    if (bMult && b < maxb)
                    {
                        //Console.WriteLine("Perhaps {0} {1} {2}", a, b, (n/d));
                        ++solns2;
                    }
                    for (Int64 c = b+1; c <= X; ++c)
                    {
                        if (a == b || a == c || b == c)
                            continue;
                        Int64 A = distance(b, c, k);
                        Int64 B = distance(a, c, k);
                        Int64 C = distance(a, b, k);
                        double asqrt = Math.Sqrt(A);
                        double bsqrt = Math.Sqrt(B);
                        bool bFound = false;
                        char which = ' ';
                        if (is45(A, B, C, asqrt, bsqrt))
                        {
                            which = 'c';
                            bFound = true;
                            ++solns;
                            //if (b != 0)
                            //    ++solns;
                        }
                        else
                        {
                            double csqrt = Math.Sqrt(C);
                            if (is45(C, A, B, csqrt, asqrt))
                            {
                                which = 'b';
                                bFound = true;
                                ++solns;
                            }
                            else if (is45(B, C, A, bsqrt, csqrt))
                            {
                                which = 'a';
                                bFound = true;
                                ++solns;
                            }
                        }
                        if (bFound)
                        {
                            bFoundOne = true;
                            Console.WriteLine("{0} {1} {2} {3}", a, b, c, which);
                        }
                       // if (bFound && !bMult)
                            //Console.WriteLine("not mult");
                    }
                    //if (bMult && !bFoundOne)
                      //  Console.WriteLine("not found, {0} {1}", a, b);
                }
            }
        }
        Console.WriteLine("solns2 {0}", solns2);
        return solns;
    }
    public static string answer()
    {
        Int64 sols1_10 = f(1, 10);//41
        Console.WriteLine("{0} {1}", 1, sols1_10);
        Int64 sols10_100 = 0;
       /* for (Int64 i = 1; i <= 10; ++i)
        {
            sols10_100 += f(i, 100);//12492
            Console.WriteLine("{0} {1}", i, sols10_100);
        }*/

        return Convert.ToString(sols10_100);
    }
}
