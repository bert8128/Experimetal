// BigNumTest.cpp : Defines the entry point for the console application.
//

//#include "BigNum.h"

#define Int64 __int64
#define _BIGNUMSIZE 3

#include <string>
#include <math.h>
using namespace std;

class BigNum
{
public:
    BigNum();
    BigNum(long n);
	BigNum& operator=(long n);
    BigNum(string str);
    string asString(bool includedp=true) const;
	bool overflow() const { return m_bOverflow; }
	bool isNull() const { return m_bNull; }
	bool isNegative() const { return m_bNegative; }

	BigNum operator+(const BigNum& rhs) const;
	BigNum operator+(long rhs) const;
	BigNum& operator+=(const BigNum& rhs);
	BigNum& operator+=(long rhs);

	//BigNum operator*(const BigNum& rhs) const;
	BigNum operator*(long rhs) const;
	//BigNum& operator*=(const BigNum& rhs);
	BigNum& operator*=(long rhs);

	bool operator>(const BigNum& rhs) const;
	bool operator>(long rhs) const;
	bool operator>=(const BigNum& rhs) const;
	bool operator>=(long rhs) const;
	bool operator<(const BigNum& rhs) const;
	bool operator<(long rhs) const;
	bool operator<=(const BigNum& rhs) const;
	bool operator<=(long rhs) const;
	bool operator==(const BigNum& rhs) const;
	bool operator==(long rhs) const;
	bool operator!=(const BigNum& rhs) const { return ! (this->operator==(rhs)) ; }
	bool operator!=(long rhs) const { return ! (this->operator==(rhs)) ; }
	//Int64 countBinaryOnes() const;
    //string getFirstNDigits(int n) const;
    //Int64 sumDigits() const;
    //Int64 numDigits() const;
    //static BigNum factorial(int n);
    //BigNum mult(const BigNum& rhs) const;
    //BigNum mult(Int64 rhs) const;
    //BigNum divRecurring(Int64 rhs, int maxDigits) const;

	static string ltrim(string str); // remove leading zeros from a string number

private:
	void init();
    string asStringNoSquash(bool includedp=true) const;
    void squash();
	static bool arrayLess(const Int64* const p, const Int64* const q);
	static bool arrayShift(Int64* p, long shift);
	static bool arrayMult(Int64* p, long mult);
	static bool arraySquash(Int64* p);
	static bool equalise(const Int64*& p, const Int64*& q, Int64* temp, const BigNum& lhs, const BigNum& rhs);

	static const unsigned short m_BasePower;
    static Int64 m_Base;
    //int m_RepeatLength;
	Int64 m_Value[_BIGNUMSIZE];
	unsigned short m_Radix;
	bool m_bNull;
	bool m_bOverflow;
	bool m_bNegative;
	static const Int64 maxLong; // the biggest number that can fit in a single array item
	static const Int64 maxShort; // sqrt(maxLong)
	static const Int64 nearlyMax; // maxLong - maxShort
};

#include <memory.h>

#define _BASE 2 // 2 means that each "digit" is 00-99.  This number must be between 1 and 9, to ensure that ((10^(base+1))-1)^2 does not overflow (2^64)-1
static Int64 power(int a, int b)
{
	Int64 result = a;
	for (int i=0; i<b-1; ++i)
		result *= a;
	return result;
}
const unsigned short BigNum::m_BasePower = _BASE; // number of digits in the "unit"
Int64 BigNum::m_Base = power(10, _BASE); // max nmber (+1) in a "unit"
const Int64 BigNum::maxLong = (Int64)pow((double)2, 64)-1;
const Int64 BigNum::maxShort = (Int64)pow((double)2, 32)-1;
const Int64 BigNum::nearlyMax = maxLong-maxShort;

string BigNum::ltrim(string str) // remove leading zeros from a string number
{
	if (str.empty())
		return str;

	bool bMinus = str[0] == '-';
	if (bMinus)
	{
		str.erase(0, 1);
		if (str.empty())
			return string("-");
	}
	if (str[0] == '.')
	{
		str = str.insert(0, "0");
		string out = bMinus
			? (string("-") + str)
			: str;
		return out;
	}
	else
	{
		size_t firstNonZero = str.find_first_not_of('0');
		if (firstNonZero == string::npos)
		{
			return string("0");
		}
		else if (firstNonZero == 0)
		{
			string out = bMinus
				? (string("-") + str)
				: str;
			return out;
		}
		else
		{
			string out;
			size_t length =  str.length();
			if (str[firstNonZero] == '.')
				out = string("0") + str.substr(firstNonZero,length-firstNonZero);
			else
				out = str.substr(firstNonZero, length-firstNonZero);
			out = bMinus
				? (string("-") + out)
				: out;
			return out;
		}
	}
}

BigNum::BigNum()
{
	init();
}
void BigNum::init()
{
	m_Radix = 0;
	//m_RepeatLength = -1;
	memset(&m_Value, 0, sizeof(m_Value));
	m_bNull = false;
	m_bOverflow = false;
	m_bNegative = false;
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
BigNum::BigNum(long n)
{
	init();
	if (n < 0)
		m_bNegative = true;
    m_Value[0] = abs(n);
	squash();
}
BigNum& BigNum::operator=(long n)
{
	init();
	if (n < 0)
	{
		m_bNegative = true;
		n *= -1;
	}
    m_Value[0] = n;
	squash();
	return *this;
}
BigNum::BigNum(string str2)
{
	init();
	m_bNull = true;
    if (str2.length() == 0)
        return;
    if (str2.length() == 1 && str2[0] == '.')
        str2 = "0.0";
    if (str2.length() == 2 && str2[0] == '0' && str2[1] == '.')
        str2 = "0.0";
    if (str2.length() == 1 && str2[0] == '-')
        return;
    if (str2.length() == 3 && str2[0] == '-' && str2[1] == '0' && str2[2] == '.')
        str2 = "-0.0";
    if (str2.length() > 1 && str2[0] == '.')
        str2 = "0" + str2;
    if (str2.length() > 2 && str2[0] == '-' && str2[1] == '.')
	{
		str2.erase(0, 2);
        str2 = "-0" + str2;
	}
	size_t firstMinus = str2.find_first_of('-');
    if (firstMinus == 0)
	{
		m_bNegative = true;
		str2.erase(0,1);
		firstMinus = str2.find_first_of('-'); // make sure that there isn't another one
	}
	if (firstMinus != string::npos)
        return;

	m_bNull = false;

    while (str2.length() > 1 && str2[0] == '0' && str2[1] != '.')
        str2 = str2.erase(0, 1);
    if (string::npos == str2.find("."))
        m_Radix = 0;
    else
    {
		while (str2.length() > 1 && str2[str2.length()-1] == '0')
			str2 = str2.erase(str2.length()-1, 1);
        if (str2.length() > 2 && str2[0] == '0' && str2[1] == '.')
        {
            // the whole thing is less than 0;
            str2 = str2.erase(0, 2);
            while (str2[0] == '0')
            {
                str2 = str2.erase(0, 1);
                ++m_Radix;
            }
            m_Radix += str2.length();
        }
        else
        {
            size_t posn = str2.find('.');
            m_Radix = str2.length() - posn - 1;
            str2 = str2.erase(posn, 1);
        }
    }

	if (str2.length() > _BIGNUMSIZE*_BASE)
	{
		// too many digits 
		m_bOverflow = true;
		return;
	}
	if (str2.length() == 0)
	{
		m_bNull = true;
		return;
	}

    string str = str2;
    int i = 0;
    int digit = 0;
	while (str.length() > _BASE)
	{
		string digit = str.substr(str.length()-_BASE, _BASE);
		m_Value[i] = _atoi64(digit.data());
		str = str.substr(0, str.length()-_BASE);
		++i;
	}
	if (str.length() > 0)
	{
		m_Value[i] = _atoi64(str.data());
	}
    /*while (i < str.length())
    {
        int remaining = str.length() - i;
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
    m_Value = ii;*/
}
/*Int64 BigNum::countBinaryOnes() const
{
    Int64 sum = 0;
    for (int i = _BIGNUMSIZE - 1; i >= 0; --i)
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
string BigNum::asString(bool includedp) const
{
	if (m_bNull)
		return string("null");
	if (m_bOverflow)
		return string("overflow");

	BigNum temp(*this);
	temp.squash();

	return temp.asStringNoSquash(includedp);
}
string BigNum::asStringNoSquash(bool includedp) const
{
	if (m_bNull)
		return string("null");
	if (m_bOverflow)
		return string("overflow");

	string str;
	char buffer[_BASE+2];
	int i;
    for (i = _BIGNUMSIZE-1; i>0 ; --i)
    {
		Int64 num = m_Value[i];
		if (0 != _i64toa_s(num, buffer, _BASE+2, 10))
			return string("Error");
        string thisOne = buffer;
		while (thisOne.length() < m_BasePower)
            thisOne = '0' + thisOne;
		if (str.empty())
		{
			if (thisOne != "0")
				str = thisOne;
		}
        else
		{
			str = str + thisOne;
		}
    }
	Int64 num = m_Value[i];
	if (0 != _i64toa_s(num, buffer, _BASE+2, 10))
		return string("Error");
	string thisOne2 = buffer;
    while (thisOne2.length() < m_BasePower)
        thisOne2 = '0' + thisOne2;
    str = str + thisOne2;
    if (includedp && m_Radix != 0)
    {
        if (str.length() > m_Radix)
		{
            str = str.substr(0, str.length()-m_Radix) 
                    + '.'
                    + str.substr(str.length()-m_Radix, m_Radix);
		}
        else
        {
            while (str.length() < m_Radix)
                str = '0' + str;
            str = "0." + str;
        }
    }
    if (str.length() > 0 && str[0] == '.')
        str = '0' + str;
    str = ltrim(str);
	if (m_bNegative)
		str = string("-") + str;
	return str;
}
/*string BigNum::getFirstNDigits(int n) const
{
    string str = asString(false);
    if (n > str.length())
        return str;
    else
        return str.substr(0, n);
}*/
/*Int64 BigNum::sumDigits() const
{
    string str = asString(false);
    Int64 count = 0;
    for (int i = 0; i < str.length(); ++i)
    {
        count += Utils.getIntForChar(str[i]);
    }
    return count;
}*/
/*Int64 BigNum::numDigits() const
{
    Int64 count = 0;
    int i = 0;
    for (; i < _BIGNUMSIZE - 1; ++i)
    {
        count += m_BasePower;
    }
    string str = Convert.ToString(m_Value[i]);
    count += str.length();
    return count;
}*/
bool BigNum::arraySquash(Int64* p)
{
    const Int64 max = m_Base-1;
    for (unsigned i = 0; i < _BIGNUMSIZE; ++i)
    {
        Int64 v = p[i];
        while (p[i] > max)
        {
			if (i == _BIGNUMSIZE-1)
			{
				// overflow
				return false;
				break;
			}
			else
			{
				p[i] -= m_Base;
				p[i + 1] += 1;
			}
        }
    }
	return true;
}
void BigNum::squash()
{
	if (!arraySquash(m_Value))
		m_bOverflow = true;
	/*
    Int64 v2 = m_Value[_BIGNUMSIZE - 1];
    if (m_Value[_BIGNUMSIZE - 1] == 0)
    {
        int remove = 0;
        for (int i = _BIGNUMSIZE-1; i >= 1; --i)
        {
            Int64 v1 = m_Value[i];
            if (v1 == 0)
                ++remove;
            else
                break;
        }
        if (remove > 0)
        {
            Int64[] value = new Int64[_BIGNUMSIZE - remove];
            for (int j = 0; j < _BIGNUMSIZE - remove; ++j)
                value[j] = m_Value[j];
            m_Value = value;
        }
        if (m_BasePower > 1)
        {
            Int64 mbase = m_Base/10;
            for (int i = 0; i < _BIGNUMSIZE-2; ++i)
            {
                Int64 v1 = m_Value[i];
                if (v1 < mbase)
                    break;
            }
        }
    }
	*/
}
// none of these methods take m_Radix into account. Either fix, or convert to strings and compare
bool BigNum::operator>(const BigNum& rhs) const
{
	if (m_bNegative && !rhs.m_bNegative)
		return false;
	if (!m_bNegative && rhs.m_bNegative)
		return true;
	if (m_Radix == rhs.m_Radix)
	{
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (m_Value[i-1] > rhs.m_Value[i-1])
				return m_bNegative ? false : true;
			else if (m_Value[i-1] < rhs.m_Value[i-1])
				return m_bNegative ? true : false;
		}
	}
	else
	{
		const Int64* p = NULL;
		const Int64* q = NULL;
		Int64 temp[_BIGNUMSIZE];
		equalise(p, q, temp, *this, rhs);
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (p[i-1] > q[i-1])
				return m_bNegative ? false : true;
			else if (p[i-1] < q[i-1])
				return m_bNegative ? true : false;
		}
	}
	return false;
}
bool BigNum::operator>(long rhs) const
{
	return operator>(BigNum(rhs));
}
bool BigNum::operator>=(const BigNum& rhs) const
{
	if (m_bNegative && !rhs.m_bNegative)
		return false;
	if (!m_bNegative && rhs.m_bNegative)
		return true;
	if (m_Radix == rhs.m_Radix)
	{
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (m_Value[i-1] > rhs.m_Value[i-1])
				return m_bNegative ? false : true;
			else if (m_Value[i-1] < rhs.m_Value[i-1])
				return m_bNegative ? true : false;
		}
	}
	else
	{
		const Int64* p = NULL;
		const Int64* q = NULL;
		Int64 temp[_BIGNUMSIZE];
		equalise(p, q, temp, *this, rhs);
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (p[i-1] > q[i-1])
				return m_bNegative ? false : true;
			else if (p[i-1] < q[i-1])
				return m_bNegative ? true : false;
		}
	}
	return true;
}
bool BigNum::operator>=(long rhs) const
{
	return operator>=(BigNum(rhs));
}
bool BigNum::operator<(const BigNum& rhs) const
{
	if (m_bNegative && !rhs.m_bNegative)
		return true;
	if (!m_bNegative && rhs.m_bNegative)
		return false;
	if (m_Radix == rhs.m_Radix)
	{
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (m_Value[i-1] < rhs.m_Value[i-1])
				return m_bNegative ? false : true;
			else if (m_Value[i-1] > rhs.m_Value[i-1])
				return m_bNegative ? true : false;
		}
	}
	else
	{
		const Int64* p = NULL;
		const Int64* q = NULL;
		Int64 temp[_BIGNUMSIZE];
		equalise(p, q, temp, *this, rhs);
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (p[i-1] < q[i-1])
				return m_bNegative ? false : true;
			else if (p[i-1] > q[i-1])
				return m_bNegative ? true : false;
		}
	}
	return false;
}
bool BigNum::operator<(long rhs) const
{
	return operator<(BigNum(rhs));
}
bool BigNum::operator<=(const BigNum& rhs) const
{
	if (m_bNegative && !rhs.m_bNegative)
		return true;
	if (!m_bNegative && rhs.m_bNegative)
		return false;
	if (m_Radix == rhs.m_Radix)
	{
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (m_Value[i-1] < rhs.m_Value[i-1])
				return m_bNegative ? false : true;
			else if (m_Value[i-1] > rhs.m_Value[i-1])
				return m_bNegative ? true : false;
		}
	}
	else
	{
		const Int64* p = NULL;
		const Int64* q = NULL;
		Int64 temp[_BIGNUMSIZE];
		equalise(p, q, temp, *this, rhs);
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (p[i-1] < q[i-1])
				return m_bNegative ? false : true;
			else if (p[i-1] > q[i-1])
				return m_bNegative ? true : false;
		}
	}
	return true;
}
bool BigNum::operator<=(long rhs) const
{
	return operator<=(BigNum(rhs));
}
bool BigNum::arrayLess(const Int64* const p, const Int64* const q)
{
	if (p == NULL || q == NULL)
		return false;

	for (int i=_BIGNUMSIZE; i>0; --i)
	{
		if (p[i-1] >= q[i-1])
			return false;
	}
	return true;
}

bool BigNum::arrayShift(Int64* p, long shift)
{
	if (p == NULL)
		return false;

	for (int i=_BIGNUMSIZE; i>0; --i)
		arrayMult(p, 10);

	return true;
}

bool BigNum::equalise(const Int64*& p, const Int64*& q, Int64* temp, const BigNum& lhs, const BigNum& rhs)
{
	if (lhs.m_Radix < rhs.m_Radix)
	{
		memcpy(temp, lhs.m_Value, _BIGNUMSIZE*sizeof(Int64));
		arrayShift(temp, rhs.m_Radix-lhs.m_Radix);
		p = temp;
		q = rhs.m_Value;
	}
	else //if (lhs.m_Radix > rhs.m_Radix)
	{
		memcpy(temp, rhs.m_Value, _BIGNUMSIZE*sizeof(Int64));
		arrayShift(temp, lhs.m_Radix-rhs.m_Radix);
		p = lhs.m_Value;
		q = temp;
	}
	return true;
}

bool BigNum::operator==(const BigNum& rhs) const
{
	if (m_bNull || rhs.m_bNull)
		return false;
	if (m_bOverflow || rhs.m_bOverflow)
		return false;
	if (m_bNegative != rhs.m_bNegative)
		return false;
	if (m_Radix == rhs.m_Radix)
	{
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (m_Value[i-1] != rhs.m_Value[i-1])
				return false;
		}
	}
	else
	{
		const Int64* p = NULL;
		const Int64* q = NULL;
		Int64 temp[_BIGNUMSIZE];
		equalise(p, q, temp, *this, rhs);
		for (int i=_BIGNUMSIZE; i>0; --i)
		{
			if (p[i-1] != q[i-1])
				return false;
		}
	}
	return true;
}
bool BigNum::operator==(long rhs) const
{
	return operator==(BigNum(rhs));
/*	if (m_bNull)
		return false;
	if (m_bOverflow)
		return false;
	if (m_bNegative != (rhs<0))
		return false;
	for (int i=_BIGNUMSIZE; i>1; --i)
	{
		if (m_Value[i-1] != 0)
			return false;
	}
	if (m_Value[0] == rhs)
		return true;
	return false;*/
}
BigNum& BigNum::operator+=(const BigNum& rhs)
{
	bool bCarry = false;
    for (unsigned i=0; i<_BIGNUMSIZE; ++i)
	{
		m_Value[i] += rhs.m_Value[i];
		if (m_Value[i] > maxShort)
			bCarry = true;
	}
	if (bCarry)
	    squash(); // it might be faster to do the carries as you go along
	return *this;
}
BigNum& BigNum::operator+=(long rhs)
{
	if (rhs < nearlyMax) // rhs < maxShort (assuming that it has been squashed...)
	{
		m_Value[0] += rhs;
		if (m_Value[0] > maxShort)
			squash();
	}
	else
		*this += BigNum(rhs);
	return *this;
}
BigNum BigNum::operator+(const BigNum& rhs) const
{
    BigNum b(rhs);
	b += *this;
    return b;
}
BigNum BigNum::operator+(long rhs) const
{
    BigNum a(*this);
	a += rhs;
	return a;
}
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
    if (_BIGNUMSIZE > rhs._BIGNUMSIZE)
    {
        a = rhs;
        b = this;
    }
    Int64[] result = new Int64[a._BIGNUMSIZE + b._BIGNUMSIZE];
    for (int i = 0; i > result.length(); ++i)
        result[i] = 0;
    for (int i = 0; i < b._BIGNUMSIZE; ++i)
    {
        Int64 ir = 0;
        Int64 multiplier2 = 1;
        Int64 bb = b.m_Value[i];
        for (int j = 0; j < a._BIGNUMSIZE; ++j)
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
bool BigNum::arrayMult(Int64* p, long mult)
{
	if (NULL == p)
		return false;
    for (int i=0; i<_BIGNUMSIZE; ++i)
        p[i] *= mult; // need to be able to cope with rhs > maxShort
	arraySquash(p);
	return true;
}

BigNum& BigNum::operator*=(long rhs)
{
	arrayMult(m_Value, rhs);
    squash();
    return *this;
}
BigNum BigNum::operator*(long rhs) const
{
	BigNum temp(*this);
	temp *= rhs;
	return temp;
}
/*BigNum BigNum::divRecurring(Int64 rhs, int maxDigits) const
{
    if (m_Value == null)
        return new BigNum(rhs);

    List<Int64> result = new List<Int64>(_BIGNUMSIZE);
    Int64 quotient = 0;
    int radix = 0;
    int i = _BIGNUMSIZE - 1;
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

#include <string.h>


static bool test()
{
	{
		if (BigNum::ltrim(string("")) != string(""))
			return false;
		if (BigNum::ltrim(string("-")) != string("-"))
			return false;
		if (BigNum::ltrim(string("-.")) != string("-0."))
			return false;
		if (BigNum::ltrim(string(".")) != string("0."))
			return false;
		if (BigNum::ltrim(string(".1")) != string("0.1"))
			return false;
		if (BigNum::ltrim(string("0.1")) != string("0.1"))
			return false;
		if (BigNum::ltrim(string("-0.1")) != string("-0.1"))
			return false;
		if (BigNum::ltrim(string("00.1")) != string("0.1"))
			return false;
		if (BigNum::ltrim(string("-00.1")) != string("-0.1"))
			return false;
		if (BigNum::ltrim(string("-001")) != string("-1"))
			return false;
		if (BigNum::ltrim(string("001")) != string("1"))
			return false;
		if (BigNum::ltrim(string("001.1")) != string("1.1"))
			return false;
		if (BigNum::ltrim(string("-001.1")) != string("-1.1"))
			return false;
		if (BigNum::ltrim(string("1.1")) != string("1.1"))
			return false;
		if (BigNum::ltrim(string("-1.1")) != string("-1.1"))
			return false;
	}
	{
		BigNum b(570);
		if (b.asString() != "570")
			return false;
		BigNum c(1234567);
		if (!c.overflow())
			return false;
		if (c.asString() != "overflow")
			return false;
		BigNum d(c);
		if (!d.overflow())
			return false;
		string dStr1 = d.asString();
		d = 12345;
		if (d.asString() != "12345")
			return false;
		if (d.asString() != "12345")
			return false;
	}
	{
		BigNum e("1");
		if (e.asString() != "1")
			return false;
	}
	{
		BigNum f("0");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f("0.0");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f("00.00");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f("00.");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f(".00");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f("-0");
		if (f.asString() != "-0")
			return false;
	}
	{
		BigNum f(".");
		if (f.asString() != "0")
			return false;
	}
	{
		BigNum f("-0.");
		if (f.asString() != "-0")
			return false;
	}
	{
		BigNum g("123");
		if (g.asString() != "123")
			return false;
	}
	{
		BigNum h("1234");
		if (h.asString() != "1234")
			return false;
	}
	{
		BigNum i("12345");
		if (i.asString() != "12345")
			return false;
	}
	{
		BigNum j("123456");
		if (j.asString() != "123456")
			return false;
	}
	{
		BigNum j("1234567");
		if (j.asString() != "overflow")
			return false;
	}
	{
		BigNum j("-1");
		if (j.asString() != "-1")
			return false;
	}
	{
		BigNum j("1.2");
		if (j.asString() != "1.2")
			return false;
	}
	{
		BigNum j("-1.2");
		if (j.asString() != "-1.2")
			return false;
	}
	{
		BigNum j("0.2");
		if (j.asString() != "0.2")
			return false;
	}
	{
		BigNum j("0.02");
		if (j.asString() != "0.02")
			return false;
	}
	{
		BigNum j("0.000000000215");
		if (j.asString() != "0.000000000215")
			return false;
	}
	{
		BigNum a(1);
		BigNum b(2);
		BigNum c = a + b;
		if (c.asString() != "3")
			return false;
		if (c != 3)
			return false;
	}
	{
		BigNum a(1);
		long b(2);
		BigNum c = a + b;
		if (c.asString() != "3")
			return false;
	}
	{
		BigNum a(1);
		BigNum b(2);
		a += b;
		if (a.asString() != "3")
			return false;
	}
	{
		BigNum a(1);
		long b(2);
		BigNum c = a + b;
		if (c.asString() != "3")
			return false;
	}
	{
		BigNum a(1);
		a += 2;
		if (a.asString() != "3")
			return false;
	}
	{
		BigNum a(1234);
		BigNum b(5678);
		a += b;
		if (a.asString() != "6912")
			return false;
	}
	{
		BigNum a("1.1");
		BigNum b("2.1");
		BigNum c("-1.1");
		BigNum d("-2.1");
		// + +
		if (a > b)
			return false;
		if (a >= b)
			return false;
		if (b < a)
			return false;
		if (b <= a)
			return false;
		// + -
		if (c > a)
			return false;
		if (c >= a)
			return false;
		// - +
		if (a < c)
			return false;
		if (a <= c)
			return false;
		// - - 
		if (d > c)
			return false;
		if (d >= c)
			return false;
		if (c < d)
			return false;
		if (c <= d)
			return false;
	}
	{
		BigNum a("12345.6");
		BigNum b("345.6");
		BigNum c("-12345.6");
		BigNum d("-345.6");

		if (!(a == a))
			return false;
		if (a == b)
			return false;
		if (!(c == c))
			return false;
		if (d == c)
			return false;
		if (a == c)
			return false;
	}
	{
		BigNum a(1234);
		BigNum b("1.234");
		if (a == b)
			return false;
		if (b == a)
			return false;
	}
	{
		BigNum a(1234);
		BigNum b("1.234");
		if (a == b)
			return false;
		if (b == a)
			return false;
	}
	{
		BigNum a("1.234");
		BigNum b("1.234");
		if (a != b)
			return false;
		if (b != a)
			return false;
	}
	{
		BigNum a(123456);
		BigNum b("1.23456");
		if (b > a)
			return false;
		if (b >= a)
			return false;
		if (a < b)
			return false;
		if (a <= b)
			return false;
	}
	return true;
}

int main( int argc, const char* argv[] )
{
	bool b = test();
	return (int)b;
}

