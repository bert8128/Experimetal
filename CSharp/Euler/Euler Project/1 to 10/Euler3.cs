using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler3
{
    public static string answer()
    {
        Int64 n = 600851475143;
        //Int64[] primes = Utils.seive64((Int64)Math.Sqrt(n));
        Int64[] factors = Utils.primeFactors64(n, null);
        return Convert.ToString(factors.Last());
    }
}