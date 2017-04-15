using System;
using System.Collections;
using System.Collections.Generic;

namespace EulerUtils
{
    class Utils
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
        public static Int64 EulersTotient2(Int64 n, int[] factors)
        {
            Int64 sum = n;
            for (int i = 0; i < factors.Length; ++i)
            {
                if (0 == i || factors[i] != factors[i - 1])
                {
                    sum *= (factors[i] - 1);
                    sum /= factors[i];
                }
            }
            return sum;
        }
        public static int EulersTotient(int n, int[] primes)
        {
            if (primes == null || primes.Length == 0)
                primes = seive(n);
            int[] factors = primeFactors(n, primes);
            return (int)EulersTotient2(n, factors);
        }
        public static Int64 EulersTotient64(Int64 n, int[] primes)
        {
            if (primes == null || primes.Length == 0)
                primes = seive((int)Math.Sqrt(n));

            Int64 sum = n;
            Int64[] factors = primeFactors64(n, primes);
            if (factors.Length == 1)
                return n - 1;
            for (int i = 0; i < factors.Length; ++i)
            {
                if (0 == i || factors[i] != factors[i - 1])
                {
                    sum *= (factors[i] - 1);
                    sum /= factors[i];
                }
            }
            return (Int64)sum;
        }
        public static double resiliance(int n)
        {
            return (double)EulersTotient(n, null) / (double)(n - 1);
        }
        public static Int64 GetGCDByModulus(Int64 value1, Int64 value2)
        {
            while (value1 != 0 && value2 != 0)
            {
                if (value1 > value2)
                    value1 %= value2;
                else
                    value2 %= value1;
            }
            return Math.Max(value1, value2);
        }
        public static bool Coprime(Int64 value1, Int64 value2)
        {
            return (1 == GetGCDByModulus(value1, value2));
        }
        static public int getDigit(int n, int digitToGet, int numberDigitsInN)
        {
            int thisDigit = n;
            for (int j = 0; j < ((numberDigitsInN) - (digitToGet + 1)); ++j)
            {
                thisDigit = thisDigit / 10;
            }
            thisDigit = thisDigit % 10;
            return thisDigit;
        }
        public static int wordScore(string s)
        {
            int sum = 0;
            foreach (char c in s)
            {
                sum += c - 'A' + 1;
            }
            return sum;
        }
        public static int factorial(int n)
        {
            int ret = 1;
            for (int i = 2; i <= n; ++i)
                ret *= i;
            return ret;
        }
        public static Int64 factorial64(int n)
        {
            Int64 ret = 1;
            for (int i = 2; i <= n; ++i)
                ret *= i;
            return ret;
        }
        public static bool isPrime(int n)
        {
            int upper = (int)Math.Sqrt(n);
            for (int i = 2; i < upper; ++i)
            {
                if ((n % i) == 0)
                    return false;
            }
            return true;
        }
        public static bool hasRepeatingDigits(int n)
        {
            int matches = 0;

            while (n > 0)
            {
                int remainder = n % 10;
                int pos = 1 << remainder;
                if ((matches & pos) != 0)
                    return true;
                else
                    matches |= pos;
                n /= 10;
            }
            return false;
        }
        public static bool isTenDigitPandigital64(Int64 n) // all 10 digits
        {
            if (n > 9876543210) return false;
            if (n < 1234567890) return false;

            int allOnes = 1023;
            int matches = 0;

            while (n > 0)
            {
                Int64 digit = n / 10;
                int remainder = (int)(n - (digit * 10));
                n /= 10;
                matches |= (1 << (remainder));
            }
            return matches == allOnes;
        }
        public static bool isPandigital(int n) //  1 -> numDigits (no 0)
        {
            if (n > 999999999) return false;

            int allOnes = 1;
            if      (n > 99999999) allOnes = 511;
            else if (n > 9999999) allOnes = 255;
            else if (n > 999999) allOnes = 127;
            else if (n > 99999) allOnes = 63;
            else if (n > 9999) allOnes = 31;
            else if (n > 999) allOnes = 15;
            else if (n > 99) allOnes = 7;
            else if (n > 9) allOnes = 3;

            int matches = 0;

            while (n > 0)
            {
                int digit = n % 10;
                n = n / 10;
                matches |= (1 << (digit - 1));
            }
            return matches == allOnes;
        }
        public static bool isPandigital(string s)
        {
            if (s.Length > 9) return false;
            int numDigits = s.Length;
            int allOnes = 1;
            switch (numDigits)
            {
                case 9: allOnes = 511; break;
                case 8: allOnes = 255; break;
                case 7: allOnes = 127; break;
                case 6: allOnes =  63; break;
                case 5: allOnes =  31; break;
                case 4: allOnes =  15; break;
                case 3: allOnes =   7; break;
                case 2: allOnes =   3; break;
            }

            int matches = 0;

            foreach (char c in s)
            {
                int digit = getIntForChar(c);
                matches |= (1 << (digit - 1));
            }
            return matches == allOnes;
        }
        public static string Reverse(string text)
        {
            char[] cArray = text.ToCharArray();
            string reverse = String.Empty;
            for (int i = cArray.Length - 1; i > -1; i--)
            {
                reverse += cArray[i];
            }
            return reverse;
        }

        public static char getIntForChar(char ch)
        {
            switch (ch)
            {
                case '0':
                    return (char)0;
                case '1':
                    return (char)1;
                case '2':
                    return (char)2;
                case '3':
                    return (char)3;
                case '4':
                    return (char)4;
                case '5':
                    return (char)5;
                case '6':
                    return (char)6;
                case '7':
                    return (char)7;
                case '8':
                    return (char)8;
            }
            return (char)9;
        }
        public static char getCharForInt(char i)
        {
            switch (i)
            {
                case (char)0:
                    return '0';
                case (char)1:
                    return '1';
                case (char)2:
                    return '2';
                case (char)3:
                    return '3';
                case (char)4:
                    return '4';
                case (char)5:
                    return '5';
                case (char)6:
                    return '6';
                case (char)7:
                    return '7';
                case (char)8:
                    return '8';
            }
            return '9';
        }
        public static int[] seive(int upperLimit)
        {
            if (upperLimit < 3)
                return new int[] { };
            int sieveBound = (int)(upperLimit - 1) / 2;
            int upperSqrt = ((int)Math.Sqrt(upperLimit) - 1) / 2;

            BitArray PrimeBits = new BitArray(sieveBound + 1, true);

            int j = 0;
            for (int i = 1; i <= upperSqrt; ++i)
            {
                if (PrimeBits.Get(i))
                {
                    int twoip1 = (i << 1) + 1;
                    for (j = (i << 1) * (i + 1);
                        j <= sieveBound;
                        j += twoip1)
                    {
                        PrimeBits.Set(j, false);
                    }
                }
            }
            List<int> numbers = new List<int>((int)(upperLimit / (Math.Log(upperLimit) - 1.08366)));
            numbers.Add(2);

            for (int i = 1; i <= sieveBound; ++i)
                if (PrimeBits.Get(i))
                    numbers.Add(2 * i + 1);

            return numbers.ToArray();
        }
        public static int[] seive2(int upperLimit)
        {
            if (upperLimit < 3)
                return new int[] { };
            int sieveBound = (int)(upperLimit - 1) / 2;
            int upperSqrt = ((int)Math.Sqrt(upperLimit) - 1) / 2;

            //BitArray PrimeBits = new BitArray(sieveBound + 1, true);
            bool[] PrimeBits = new bool[sieveBound + 1];
            //for (int k = 0; k < PrimeBits.Length; ++k) opposite of true for speed
            //    PrimeBits[k] = true;

            int j = 0;
            for (int i = 1; i <= upperSqrt; ++i)
            {
                if (!PrimeBits[i])//.Get(i))
                {
                    int twoip1 = (i << 1) + 1;
                    for (j = (i << 1) * (i + 1);
                        j <= sieveBound;
                        j += twoip1)
                    {
                        PrimeBits[j] = true;// false;//.Set(j, false);
                    }
                }
            }
            List<int> numbers = new List<int>((int)(upperLimit / (Math.Log(upperLimit) - 1.08366)));
            numbers.Add(2);

            for (int i = 1; i <= sieveBound; ++i)
                if (!PrimeBits[i])//.Get(i))
                    numbers.Add(2 * i + 1);

            return numbers.ToArray();
        }
        public static Int64[] seive64(Int64 upperLimit)
        {
            if (upperLimit < 3)
                return new Int64[] { };
            Int64 sieveBound = (Int64)(upperLimit - 1) / 2;
            Int64 upperSqrt = ((Int64)Math.Sqrt(upperLimit) - 1) / 2;
 
            //BitArray PrimeBits = new BitArray(sieveBound + 1, true);
            bool[] PrimeBits = new bool[sieveBound + 1];
            //for (Int64 k = 0; k < PrimeBits.Length; ++k) opposite of true for speed
            //    PrimeBits[k] = true;

            Int64 j = 0;
            for (Int64 i = 1; i <= upperSqrt; ++i)
            {
                if (!PrimeBits[i])//.Get(i))
                {
                    Int64 twoip1 = (i << 1) + 1;
                    for (j = (i<<1) * (i + 1);
                        j <= sieveBound;
                        j += twoip1)
                    {
                        PrimeBits[j] = true;// false;//.Set(j, false);
                    }
                }
            }

            Int64 logg = (Int64)Math.Log(upperLimit);
            List<Int64> numbers = new List<Int64>((int)(upperLimit / (logg - 1.08366)));
            numbers.Add(2);
 
            for (int i = 1; i <= sieveBound; ++i)
                if (!PrimeBits[i])//.Get(i))
                    numbers.Add(2 * i + 1);
 
            return numbers.ToArray();
        }
        public static int[] primeFactors(int n, int[] primes)
        {
            if (1 == n)
                return new int[] { 1 };
            if (2 == n)
                return new int[] { 2 };
            int orig = n;

            if (primes == null || primes.Length == 0)
                primes = seive(n);
            List<int> prime_factors = new List<int>((int)Math.Sqrt(n) + 1);

            for (int i = 0; i < primes.Length && primes[i] * primes[i] <= n; ++i)
            {
                while (n % primes[i] == 0)
                {
                    prime_factors.Add(primes[i]);
                    n /= primes[i];
                }
            }
            if (n > 1 && (n != orig || prime_factors.Count == 0))
                prime_factors.Add(n);

            return prime_factors.ToArray();
        }
        public static Int64[] primeFactors(Int64 n, int[] primes)
        {
            if (1 == n)
                return new Int64[] { 1 };
            if (2 == n)
                return new Int64[] { 2 };
            Int64 orig = n;

            if (primes == null || primes.Length == 0)
                primes = seive((int)Math.Sqrt(n));
            List<Int64> prime_factors = new List<Int64>((int)Math.Sqrt(n) + 1);

            for (int i = 0; i < primes.Length && primes[i] * primes[i] <= n; ++i)
            {
                while (n % primes[i] == 0)
                {
                    prime_factors.Add(primes[i]);
                    n /= primes[i];
                }
            }
            if (n > 1 && (n != orig || prime_factors.Count == 0))
                prime_factors.Add(n);

            return prime_factors.ToArray();
        }
        public static Int64[] primeFactors64(Int64 n, int[] primes)
        {
            if (1 == n)
                return new Int64[] { 1 };
            if (2 == n)
                return new Int64[] { 2 };
            Int64 orig = n;

            if (primes == null || primes.Length == 0)
                primes = seive((int)Math.Sqrt(n));
            int sqrtn = (int)Math.Sqrt(n);
            List<Int64> prime_factors = new List<Int64>();

            for (Int64 i = 0; i < primes.Length && primes[i] * primes[i] <= n; ++i)
            {
                while (n % primes[i] == 0)
                {
                    prime_factors.Add(primes[i]);
                    n /= primes[i];
                }
            }
            if (n > 1 && (n != orig || prime_factors.Count == 0))
                prime_factors.Add(n);

            return prime_factors.ToArray();
        }
        public static void testPFs(int upper, int[] primes)
        {
            for (int j = 0; j < 1; ++j)
            {
                for (int i = upper - 1; i < upper; ++i)
                {
                    int[] pfs = primeFactors(i, primes);
                }
            }
        }
    }

    class Permute
    {
        // starts with the lowest and goes to the hiest. could do with one which starts at the highest
        static void permute(char[] v, int start, int n, List<string> strings)
        {
            strings.Add(new string(v));
            if (start < n)
            {
                int j;
                for (int i = n - 2; i >= start; --i)
                {
                    for (j = i + 1; j < n; ++j)
                    {
                        {
                            //swap(v, i, j);
                            char t = v[i];
                            v[i] = v[j];
                            v[j] = t;
                        }
                        permute(v, i + 1, n, strings);
                    }
                    {
                        //rotateLeft(v, i, n);
                        char tmp = v[i];
                        for (int k = i; k < n - 1; ++k)
                            v[k] = v[k + 1];
                        v[n - 1] = tmp;
                    }
                }
            }
        }
        public static void permute(char[] v, List<string> strings)
        {
            permute(v, 0, v.Length, strings);
        }
    }
    class IntTree
    {
        public int m_prime;
        public Dictionary<int, IntTree> m_lst;
        bool m_bTerminator;
        public IntTree()
        {
            m_lst = new Dictionary<int, IntTree>();
            m_bTerminator = false;
        }
        public void insert(SortedDictionary<int, int> number)
        {
            IntTree next = this;
            IntTree prev = null;
            IntTree temp;
            foreach (KeyValuePair<int, int> kvp in number)
            {
                int prime = kvp.Key;
                int exponent = kvp.Value;
                next.m_prime = prime;
                next.m_lst.TryGetValue(exponent, out temp);
                if (null == temp)
                    next.m_lst.Add(exponent, new IntTree());
                next.m_lst.TryGetValue(exponent, out prev);
                //prev = next;
                next = next.m_lst[exponent];
            }
            if (null != prev)
                prev.m_bTerminator = true;
        }
        List<string> asStrings(string str1)
        {
            List<string> strs1 = new List<string>();
            char tab = ' ';
            foreach (KeyValuePair<int, IntTree> kvp in m_lst)
            {
                string str = new string(str1.ToCharArray());
                str = str + tab + Convert.ToString(m_prime);
                str = str + tab + Convert.ToString(kvp.Key);
                //str = str + "\r\n";
                if (m_bTerminator)
                {
                    str = str + "xx";
                    strs1.Add(str);
                }
                List<string> strs = kvp.Value.asStrings(str);
                foreach (string s in strs)
                    strs1.Add(s);
            }
            return strs1;
        }
        public List<string> asStrings()
        {
            string str = string.Empty;
            return asStrings(str);
        }
        int entries(int i)
        {
            foreach (KeyValuePair<int, IntTree> kvp in m_lst)
            {
                if (m_bTerminator)
                    ++i;
                int j = kvp.Value.entries(i);
                i += j;
            }
            return i;
        }
        public int entries()
        {
            int i = 0;
            return entries(i);
        }
    }
}

