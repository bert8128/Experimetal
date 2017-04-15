// This is the main DLL file.

#include "BigNum.h"
#include <memory.h>

#define _BASE 10
static Int64 power(int a, int b)
{
	int result = a;
	for (int i=0; i<b; ++i)
		result *= a;
	return result;
}
const int BigNum::m_BasePower = _BASE;
Int64 BigNum::m_Base = power(10, _BASE);

BigNum::BigNum() :
	m_Radix(0)
	//,m_RepeatLength(-1)
{
	//for (size_t i=0; i<_BIGNUMSIZE; ++i)
	//	m_Value[i] = 0;
	memset(&m_Value, 0, sizeof(m_Value));
}
/*BigNum::BigNum(Int64* rhs, int radix, int repeatlength) :
    m_Value(rhs)
    ,m_Radix(radix)
    //,m_RepeatLength(repeatlength)
{
}*/
/*BigNum::BigNum(const BigNum& rhs) :
    m_Value(rhs.m_Value),
    m_Radix(rhs.m_Radix),
    m_RepeatLength(rhs.m_RepeatLength)
{
}*/
/*BigNum::BigNum(Int64 n) : 
    m_Radix(0),
    m_RepeatLength(-1)
{
    m_Value = new Int64[1];
    m_Value[0] = n;
}*/
BigNum::BigNum(long n)// : 
//    m_Radix(0)
//    ,m_RepeatLength(-1)
{
	BigNum::BigNum();
    m_Value[0] = n;
}
/*BigNum::BigNum(string str2) :
    m_Radix(0),
    m_RepeatLength(-1)
{
    if (str2.length() == 0)
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
}*/
/*Int64 BigNum::countBinaryOnes() const
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
}*/
/*string BigNum::asString(bool includedp) const
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
}*/
/*string BigNum::getFirstNDigits(int n) const
{
    string str = asString(false);
    if (n > str.Length)
        return str;
    else
        return str.Substring(0, n);
}*/
/*Int64 BigNum::sumDigits() const
{
    string str = asString(false);
    Int64 count = 0;
    for (int i = 0; i < str.Length; ++i)
    {
        count += Utils.getIntForChar(str[i]);
    }
    return count;
}*/
/*Int64 BigNum::numDigits() const
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
}*/
/*void BigNum::squash()
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
}*/
/*BigNum BigNum::add(const BigNum& rhs) const
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
}*/
/*BigNum BigNum::factorial(int n)
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
}*/
/*BigNum BigNum::mult(const BigNum& rhs) const
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
}*/
/*BigNum BigNum::mult(Int64 rhs) const
{
    if (m_Value == null)
        return new BigNum(rhs);
    Int64[] result = new Int64[m_Value.Length + 1];
    for (int i = 0; i < m_Value.Length; ++i)
        result[i] = rhs * m_Value[i];
    BigNum r = new BigNum(result);
    r.squash();
    return r;
}*/
/*BigNum BigNum::divRecurring(Int64 rhs, int maxDigits) const
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
}*/
