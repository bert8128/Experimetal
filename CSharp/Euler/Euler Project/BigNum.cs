using System;
using System.Collections;
using System.Collections.Generic;
using EulerUtils;

namespace bignum
{
    class BigNum
    {
        public Int64[] m_Value;
        public static int m_BasePower = 10;
        public static Int64 m_Base = (Int64)Math.Pow(10, m_BasePower);
        public int m_Radix;
        public int m_RepeatLength;
        public BigNum()
        {
            //m_Radix = 0;
            m_RepeatLength = -1;
        }
        public BigNum(Int64[] rhs, int radix = 0, int repeatlength=0)
        {
            m_Value = rhs;
            m_Radix = radix;
            m_RepeatLength = repeatlength;
        }
        public BigNum(BigNum rhs)
        {
            m_Value = rhs.m_Value;
            m_Radix = rhs.m_Radix;
            m_RepeatLength = rhs.m_RepeatLength;
        }
        public BigNum(Int64 n)
        {
            m_Value = new Int64[1];
            m_Value[0] = n;
            //m_Radix = 0;
            m_RepeatLength = -1;
        }
        public BigNum(string str2)
        {
            if (str2.Length == 0)
                return;
            if (str2.Length == 1 && str2[0] == '.')
                return;
            if (str2.Length == 2 && str2[0] == '0' && str2[1] == '.')
                return;
            while (str2.Length > 1 && str2[0] == '0' && str2[1] != '.')
            {
                str2 = str2.Remove(0, 1);
            }
            if (!str2.Contains("."))
            {
                m_Radix = 0;
            }
            else
            {
                if (str2.Length > 2 && str2[0] == '0' && str2[1] == '.')
                {
                    // the whole thing is less than 0;
                    str2 = str2.Remove(0, 2);
                    while (str2[0] == '0')
                    {
                        str2 = str2.Remove(0, 1);
                        ++m_Radix;
                    }
                    m_Radix += str2.Length;
                }
                else
                {
                    int posn = str2.IndexOf('.');
                    m_Radix = str2.Length - posn - 1;
                    str2 = str2.Remove(posn, 1);
                }
            }

            Int64[] ii = new Int64[str2.Length/m_BasePower + 1];
            string str = Utils.Reverse(str2);
            int i = 0;
            int digit = 0;
            while (i < str.Length)
            {
                int remaining = str.Length - i;
                int length;
                if (remaining > m_BasePower)
                    length = m_BasePower;
                else
                    length = remaining;
                char[] substr = new char[length];
                for (int j = length-1; j >=0; --j)
                {
                    substr[j] = str[i];
                    ++i;
                }
                string s = new String(substr);
                ii[digit] = Convert.ToInt64(s);
                ++digit;
            }
            m_Value = ii;
        }
        public Int64 countBinaryOnes()
        {
            Int64 sum = 0;
            for (int i = m_Value.Length - 1; i >= 0; --i)
            {
                UInt64 x = (UInt64)m_Value[i];
                while (x != 0)
                {
                    ++sum;
                    x &= (x - 1);
                }
            }
            return sum;
        }
        public string asString(bool includedp=true)
        {
            squash();
            string str = null;
            int i = m_Value.Length-1;
            for (; i>0 ; --i)
            {
                string thisOne = Convert.ToString(m_Value[i]);
                while (thisOne.Length < m_BasePower)
                    thisOne = '0' + thisOne;
                if (str == null)
                    str = thisOne;
                else
                    str = str + thisOne;
            }
            string thisOne2 = Convert.ToString(m_Value[i]);
            while (thisOne2.Length < m_BasePower)
                thisOne2 = '0' + thisOne2;
            str = str + thisOne2;
            if (includedp && m_Radix != 0)
            {
                if (str.Length > m_Radix*m_BasePower)
                    str = str.Substring(0, str.Length - m_Radix*m_BasePower) 
                          + '.'
                          + str.Substring(str.Length - m_Radix*m_BasePower, m_Radix*m_BasePower);
                else
                {
                    while (str.Length < m_Radix)
                        str = '0' + str;
                    str = "0." + str;
                }
            }
            if (str.Length > 0 && str[0] == '.')
                str = '0' + str;
            return str;
        }
        public string getFirstNDigits(int n)
        {
            string str = asString(false);
            if (n > str.Length)
                return str;
            else
                return str.Substring(0, n);
        }
        public Int64 sumDigits()
        {
            string str = asString(false);
            Int64 count = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                count += Utils.getIntForChar(str[i]);
            }
            return count;
        }
        public Int64 numDigits()
        {
            Int64 count = 0;
            int i = 0;
            for (; i < m_Value.Length - 1; ++i)
            {
                count += m_BasePower;
            }
            string str = Convert.ToString(m_Value[i]);
            count += str.Length;
            return count;
        }
        public void squash()
        {
            Int64 max = m_Base-1;
            for (int i = 0; i < m_Value.Length; ++i)
            {
                Int64 v = m_Value[i];
                while (m_Value[i] > max)
                {
                    m_Value[i] -= m_Base;
                    if (i == m_Value.Length-1)
                    {
                        //add another element
                        Int64[] value = new Int64[m_Value.Length + 1];
                        for (int j = 0; j < m_Value.Length; ++j)
                            value[j] = m_Value[j];
                        value[m_Value.Length] = 0;
                        m_Value = value;
                    }
                    m_Value[i + 1] += 1;
                }
            }
            Int64 v2 = m_Value[m_Value.Length - 1];
            if (m_Value[m_Value.Length - 1] == 0)
            {
                int remove = 0;
                for (int i = m_Value.Length-1; i >= 1; --i)
                {
                    Int64 v1 = m_Value[i];
                    if (v1 == 0)
                        ++remove;
                    else
                        break;
                }
                if (remove > 0)
                {
                    Int64[] value = new Int64[m_Value.Length - remove];
                    for (int j = 0; j < m_Value.Length - remove; ++j)
                        value[j] = m_Value[j];
                    m_Value = value;
                }
                if (m_BasePower > 1)
                {
                    Int64 mbase = m_Base/10;
                    for (int i = 0; i < m_Value.Length-2; ++i)
                    {
                        Int64 v1 = m_Value[i];
                        if (v1 < mbase)
                            break;
                    }
                }
            }
        }
        public BigNum add(BigNum rhs)
        {
            BigNum a = this;
            BigNum b = rhs;
            if (m_Value == null)
                return rhs;
            if (rhs.m_Value == null)
                return this;
            if (m_Value.Length < rhs.m_Value.Length)
            {
                a = rhs; // a is always as long or longer than b
                b = this;
            }
            Int64[] result = new Int64[a.m_Value.Length + 2];
            for (int i = 0; i < result.Length; ++i)
                result[i] = 0;
            for (int i = 0; i < a.m_Value.Length; ++i )
            {
                Int64 ia = 0;
                Int64 ib = 0;
                if (i < a.m_Value.Length)
                    ia = a.m_Value[i];
                if (i < b.m_Value.Length)
                    ib = b.m_Value[i];
                Int64 ir = ia + ib;
                result[i] += ir;
            }
            BigNum r = new BigNum(result);
            r.squash();
            return r;
        }
        public static BigNum factorial(int n)
        {
            if (n < 3)
                return new BigNum(n);
            BigNum fact = new BigNum("1");
            for (int i = 2; i <= n; ++i)
            {
                BigNum ii = new BigNum(i);
                fact = fact.mult(ii);
            }
            return fact;
        }
        public BigNum mult(BigNum rhs)
        {
            BigNum a = this;
            BigNum b = rhs;
            if (m_Value == null)
                return rhs;
            if (rhs.m_Value == null)
                return this;
            if (m_Value.Length > rhs.m_Value.Length)
            {
                a = rhs;
                b = this;
            }
            Int64[] result = new Int64[a.m_Value.Length + b.m_Value.Length];
            for (int i = 0; i > result.Length; ++i)
                result[i] = 0;
            for (int i = 0; i < b.m_Value.Length; ++i)
            {
                Int64 ir = 0;
                Int64 multiplier2 = 1;
                Int64 bb = b.m_Value[i];
                for (int j = 0; j < a.m_Value.Length; ++j)
                {
                    ir += bb * a.m_Value[j] * multiplier2;
                    multiplier2 *= m_Base;
                }
                result[i] += ir;
            }
            BigNum r = new BigNum(result);
            r.squash();
            return r;
        }
        public BigNum mult(Int64 rhs)
        {
            if (m_Value == null)
                return new BigNum(rhs);
            Int64[] result = new Int64[m_Value.Length + 1];
            for (int i = 0; i < m_Value.Length; ++i)
                result[i] = rhs * m_Value[i];
            BigNum r = new BigNum(result);
            r.squash();
            return r;
        }
        public BigNum divRecurring(Int64 rhs, int maxDigits)
        {
            if (m_Value == null)
                return new BigNum(rhs);

            List<Int64> result = new List<Int64>(m_Value.Length);
            Int64 quotient = 0;
            int radix = 0;
            int i = m_Value.Length - 1;
            Int64 remainder = 0;
            for (; i >= 1; --i)
            {
                quotient = (m_Value[i] + remainder*m_Base) / rhs;
                remainder = (m_Value[i] + remainder * m_Base) - (quotient * rhs);
                result.Add(quotient);
            }

            List<Int64> remainders = new List<Int64>();

            remainder *= m_Base;
            remainder += m_Value[i];
            quotient = remainder / rhs;
            remainder = remainder % rhs;
            result.Add(quotient);
            if (0 == quotient && remainder != 0)
                ++radix;
            remainders.Add(remainder);

            int length = -1;
            while (remainder != 0)
            {
                remainder *= m_Base;
                quotient = remainder / rhs;
                remainder = remainder % rhs;
                result.Add(quotient);
                ++radix;
                if (length < 0)
                {
                    int pos = remainders.IndexOf(remainder);
                    if (-1 != pos)
                        length = result.Count - pos - 1;
                }
                remainders.Add(remainder);
                if (result.Count > maxDigits)
                    break;
            }
            result.Reverse();
            return new BigNum(result.ToArray(), radix, length);
        }
        public static void test()
        {
            BigNum a = new BigNum("100000");
            BigNum b = new BigNum("1000");
            BigNum c = a.divRecurring(1, 10);
            /*Int64 count = a.countBinaryOnes();
            for (int i = 0; i < 10; ++i)
            {
                a = a.add(b);
                count = a.countBinaryOnes();
            }*/
        }
    }
    class BigNum2
    {
        public Int64[] m_Value;
        public static int m_BasePower = 3;
        public static Int64 m_Base = (Int64)Math.Pow(10, m_BasePower);
        public int m_Exp;
        //public int m_RepeatLength;
        public BigNum2()
        {
            m_Exp = 0;
            //m_RepeatLength = -1;
        }
        public BigNum2(Int64[] rhs, int exp = 0)//, int repeatlength = 0)
        {
            m_Value = rhs;
            m_Exp = exp;
            //m_RepeatLength = repeatlength;
        }
        public BigNum2(BigNum2 rhs)
        {
            m_Value = rhs.m_Value;
            m_Exp = rhs.m_Exp;
            //m_RepeatLength = rhs.m_RepeatLength;
        }
        public BigNum2(Int64 n)
        {
            string thisOne = Convert.ToString(n);
            BigNum2 a = new BigNum2(thisOne);
            m_Value = a.m_Value;
            m_Exp = a.m_Exp;
        }
        public BigNum2(string str2)
        {
            if (str2.Length == 0)
                return;
            if (str2.Length == 1 && str2[0] == '.')
                return;
            if (str2.Length == 2 && str2[0] == '0' && str2[1] == '.')
                return;
            while (str2.Length > 1 && str2[0] == '0' && str2[1] != '.')
            {
                str2 = str2.Remove(0, 1);
            }
            if (!str2.Contains("."))
            {
                while (str2.Length > 1 && str2[str2.Length - 1] == '0')
                {
                    str2 = str2.Remove(str2.Length - 1);
                    ++m_Exp;
                }
                m_Exp += str2.Length - 1;
            }
            else
            {
                if (str2[0] == '.')
                    str2 = '0' + str2; 
                if (str2.Length > 2 && str2[0] == '0' && str2[1] == '.')
                {
                    // the whole thing is less than 0;
                    str2 = str2.Remove(0, 2);
                    m_Exp = -1;
                    while (str2[0] == '0')
                    {
                        str2 = str2.Remove(0, 1);
                        --m_Exp;
                    }
                }
                else
                {
                    int posn = str2.IndexOf('.');
                    m_Exp = posn - 1;
                    str2 = str2.Remove(posn, 1);
                }
            }

            Int64[] ii = new Int64[str2.Length / m_BasePower + 1];
            string str = Utils.Reverse(str2);
            int i = 0;
            int digit = 0;
            while (i < str.Length)
            {
                int remaining = str.Length - i;
                int length;
                if (remaining > m_BasePower)
                    length = m_BasePower;
                else
                    length = remaining;
                char[] substr = new char[length];
                for (int j = length - 1; j >= 0; --j)
                {
                    substr[j] = str[i];
                    ++i;
                }
                string s = new String(substr);
                ii[digit] = Convert.ToInt64(s);
                ++digit;
            }
            m_Value = ii;
        }
        public Int64 countBinaryOnes()
        {
            Int64 sum = 0;
            for (int i = m_Value.Length - 1; i >= 0; --i)
            {
                UInt64 x = (UInt64)m_Value[i];
                while (x != 0)
                {
                    ++sum;
                    x &= (x - 1);
                }
            }
            return sum;
        }
        public string asString()
        {
            squash();
            string str = null;
            for (int i=0; i<m_Value.Length; ++i)
            {
                string thisOne = Convert.ToString(m_Value[i]);
                if (i < m_Value.Length - 1)
                {
                    while (thisOne.Length < m_BasePower)
                        thisOne = '0' + thisOne;
                }
                if (str == null)
                    str = thisOne;
                else
                    str = thisOne + str;
            }

            int pos = 1 + m_Exp; // position of the decimal point
            if (pos > str.Length)
            {
                while (pos > str.Length)
                    str = str + '0';
            }
            else if (pos < str.Length)
            {
                if (pos > 0)
                {
                    str = str.Substring(0, pos) + '.' + str.Substring(pos);
                }
                else if (pos < 0)
                {
                    while (pos++ < 0)
                        str = '0' + str;
                    str = "0." + str;
                }
                else
                {
                    str = "0." + str;
                }
            }
            else
            {
            }
            {
                if (str.Length > 0 && str[0] == '.')
                    str = '0' + str;
            }
            return str;
        }
        public string getFirstNDigits(int n)
        {
            string str = asString();
            if (n > str.Length)
                return str;
            else
                return str.Substring(0, n);
        }
        public Int64 sumDigits()
        {
            string str = asString();
            Int64 count = 0;
            for (int i = 0; i < str.Length; ++i)
            {
                count += Utils.getIntForChar(str[i]);
            }
            return count;
        }
        public Int64 numDigits()
        {
            Int64 count = 0;
            int i = 0;
            for (; i < m_Value.Length - 1; ++i)
            {
                count += m_BasePower;
            }
            string str = Convert.ToString(m_Value[i]);
            count += str.Length;
            return count;
        }
        public void squash()
        {
            Int64 max = m_Base - 1;
            for (int i = 0; i < m_Value.Length; ++i)
            {
                Int64 v = m_Value[i];
                while (m_Value[i] > max)
                {
                    m_Value[i] -= m_Base;
                    if (i == m_Value.Length - 1)
                    {
                        //add another element
                        Int64[] value = new Int64[m_Value.Length + 1];
                        for (int j = 0; j < m_Value.Length; ++j)
                            value[j] = m_Value[j];
                        value[m_Value.Length] = 0;
                        m_Value = value;
                    }
                    m_Value[i + 1] += 1;
                }
            }
            Int64 v2 = m_Value[m_Value.Length - 1];
            if (m_Value[m_Value.Length - 1] == 0)
            {
                int remove = 0;
                for (int i = m_Value.Length - 1; i >= 1; --i)
                {
                    Int64 v1 = m_Value[i];
                    if (v1 == 0)
                        ++remove;
                    else
                        break;
                }
                if (remove > 0)
                {
                    Int64[] value = new Int64[m_Value.Length - remove];
                    for (int j = 0; j < m_Value.Length - remove; ++j)
                        value[j] = m_Value[j];
                    m_Value = value;
                }
                if (m_BasePower > 1)
                {
                    Int64 mbase = m_Base / 10;
                    for (int i = 0; i < m_Value.Length - 2; ++i)
                    {
                        Int64 v1 = m_Value[i];
                        if (v1 < mbase)
                            break;
                    }
                }
            }
        }
        public BigNum2 add2(BigNum2 rhs)
        {
            if (m_Value == null)
                return rhs;
            if (rhs.m_Value == null)
                return this;
            BigNum2 a = this;
            BigNum2 b = rhs;
            if (m_Exp < rhs.m_Exp)
            {
                a = rhs;
                b = this;
            }
            Int64[] result = new Int64[a.m_Value.Length + 2];
            for (int i = 0; i < result.Length; ++i)
                result[i] = 0;
            for (int i = 0; i < a.m_Value.Length; ++i)
            {
                Int64 ia = 0;
                Int64 ib = 0;
                if (i < a.m_Value.Length)
                    ia = a.m_Value[i];
                if (i < b.m_Value.Length)
                    ib = b.m_Value[i];
                Int64 ir = ia + ib;
                result[i] += ir;
            }
            BigNum2 r = new BigNum2(result);
            r.squash();
            return r;
        }
        public BigNum2 add(BigNum2 rhs)
        {
            if (m_Value == null)
                return rhs;
            if (rhs.m_Value == null)
                return this;
            BigNum2 a = this;
            BigNum2 b = rhs;
            if (m_Exp < rhs.m_Exp)
            {
                a = rhs; // a is always as big or bigger than b, exponent-wise
                b = this;
            }
            //int smallestDigit_a = a.m_Exp + 1 - 
            int diff = a.m_Exp - b.m_Exp;
            for (int i=0; i<diff; ++i)//while (a.m_Exp > b.m_Exp)
            {
                //int exp = a.m_Exp;
                a = a.mult2(10);
                //a.m_Exp = exp - 1;
                //b = b.mult2(10);
            }
            Int64[] result = new Int64[a.m_Value.Length + 2];
            BigNum2 r = a.add2(b);
            r.m_Exp = -diff;
            r.squash();
            return r;
        }
        public static BigNum2 factorial(int n)
        {
            if (n < 3)
                return new BigNum2(n);
            BigNum2 fact = new BigNum2("1");
            for (int i = 2; i <= n; ++i)
            {
                BigNum2 ii = new BigNum2(i);
                fact = fact.mult(ii);
            }
            return fact;
        }
        public BigNum2 mult(BigNum2 rhs)
        {
            BigNum2 a = new BigNum2(this.mult2(rhs));
            a.m_Exp = m_Exp + rhs.m_Exp;
            return a;
        }
        public BigNum2 mult(Int64 rhs)
        {
            BigNum2 a = new BigNum2(this.mult2(rhs));
//            a.m_Exp = m_Exp;
            string thisOne = Convert.ToString(rhs);
            a.m_Exp = m_Exp + thisOne.Length-1;
            return a;
        }
        public BigNum2 mult2(BigNum2 rhs)
        {
            BigNum2 a = this;
            BigNum2 b = rhs;
            if (m_Value == null)
                return rhs;
            if (rhs.m_Value == null)
                return this;
            if (m_Value.Length > rhs.m_Value.Length)
            {
                a = rhs;
                b = this;
            }
            Int64[] result = new Int64[a.m_Value.Length + b.m_Value.Length];
            for (int i = 0; i > result.Length; ++i)
                result[i] = 0;
            for (int i = 0; i < b.m_Value.Length; ++i)
            {
                Int64 ir = 0;
                Int64 multiplier2 = 1;
                Int64 bb = b.m_Value[i];
                for (int j = 0; j < a.m_Value.Length; ++j)
                {
                    ir += bb * a.m_Value[j] * multiplier2;
                    multiplier2 *= m_Base;
                }
                result[i] += ir;
            }
            BigNum2 r = new BigNum2(result);
            r.squash();
            return r;
        }
        public BigNum2 mult2(Int64 rhs)
        {
            if (m_Value == null)
                return new BigNum2(rhs);
            Int64[] result = new Int64[m_Value.Length + 1];
            for (int i = 0; i < m_Value.Length; ++i)
                result[i] = rhs * m_Value[i];
            BigNum2 r = new BigNum2(result);
            r.squash();
            return r;
        }
        public BigNum2 divRecurring(Int64 rhs, int maxDigits)
        {
            if (m_Value == null)
                return new BigNum2(rhs);

            List<Int64> result = new List<Int64>(m_Value.Length);
            Int64 quotient = 0;
            int radix = 0;
            int i = m_Value.Length - 1;
            Int64 remainder = 0;
            for (; i >= 1; --i)
            {
                quotient = (m_Value[i] + remainder * m_Base) / rhs;
                remainder = (m_Value[i] + remainder * m_Base) - (quotient * rhs);
                result.Add(quotient);
            }

            List<Int64> remainders = new List<Int64>();

            remainder *= m_Base;
            remainder += m_Value[i];
            quotient = remainder / rhs;
            remainder = remainder % rhs;
            result.Add(quotient);
            if (0 == quotient && remainder != 0)
                ++radix;
            remainders.Add(remainder);

            int length = -1;
            while (remainder != 0)
            {
                remainder *= m_Base;
                quotient = remainder / rhs;
                remainder = remainder % rhs;
                result.Add(quotient);
                ++radix;
                if (length < 0)
                {
                    int pos = remainders.IndexOf(remainder);
                    if (-1 != pos)
                        length = result.Count - pos - 1;
                }
                remainders.Add(remainder);
                if (result.Count > maxDigits)
                    break;
            }
            result.Reverse();
            return new BigNum2(result.ToArray(), radix);
        }
        public static void test()
        {
            BigNum2 a1 = new BigNum2("1");
            BigNum2 a2 = new BigNum2("12");
            BigNum2 a3 = a1.add(a2);
            //BigNum2 a3 = a1.mult(a2);

            Console.WriteLine(a1.asString());
            Console.WriteLine(a2.asString());
            Console.WriteLine(a3.asString());
            Console.ReadLine();

            /*Int64 count = a.countBinaryOnes();
            for (int i = 0; i < 10; ++i)
            {
                a = a.add(b);
                count = a.countBinaryOnes();
            }*/
        }
    }
}

