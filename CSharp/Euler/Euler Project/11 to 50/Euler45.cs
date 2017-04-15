using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler45
{
    static Int64 pentagon(Int64 n)
    {
        return (n * (3 * n - 1) / 2);
    }
    static bool isPentagonal(Int64 n)
    {
        Int64 m = (Int64)(0.5 + Math.Sqrt(0.25 + 6 * n)) / 3;
        Int64 nn = pentagon(m);
        return (n == nn);
    }
    static Int64 triangle(Int64 n)
    {
        return (n * (n + 1) / 2);
    }
    static bool isTriangular(Int64 n)
    {
        Int64 m = (Int64)(Math.Sqrt(0.25 + 2 * n) - 0.5);
        Int64 nn = triangle(m);
        return (n == nn);
    }
    static Int64 hexagon(Int64 n)
    {
        return (n * (2 * n - 1));
    }
    static bool isHexagonal(Int64 n)
    {
        Int64 m = (Int64)(1 + Math.Sqrt(1 + 8 * n)) / 4;
        Int64 nn = hexagon(m);
        return (n == nn);
    }
    public static string answer()
    {
        Int64 t = 1;
        Int64 inc = 5;
        while (true)
        {
            if (t != 1 && t != 40755)
                if (isPentagonal(t))
                    break;
            t += inc;
            inc += 4;
        }
        return Convert.ToString(t);
    }
}
