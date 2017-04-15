// BigNum.h

#pragma once

//using namespace System;

#define Int64 __int64
#define _BIGNUMSIZE 3

/*class BigNum
{
	public: 
		BigNum();
	private:
		Int64 m_num[_BIGNUMSIZE];
		short m_Radix;
};*/
class BigNum
{
public:
    BigNum();
    //BigNum(Int64[_BIGNUMSIZE] rhs, int radix = 0, int repeatlength=0); 
    //BigNum(const BigNum& rhs);
    BigNum(long n);
    //BigNum(Int64 n);
    //BigNum(string str2);
    //Int64 countBinaryOnes() const;
    //string asString(bool includedp=true) const;
    //string getFirstNDigits(int n) const;
    //Int64 sumDigits() const;
    //Int64 numDigits() const;
    //void squash();
    //BigNum add(const BigNum& rhs) const;
    //static BigNum factorial(int n);
    //BigNum mult(const BigNum& rhs) const;
    //BigNum mult(Int64 rhs) const;
    //BigNum divRecurring(Int64 rhs, int maxDigits) const;
private:
    static const int m_BasePower;
    static Int64 m_Base;
    //int m_RepeatLength;
	Int64 m_Value[_BIGNUMSIZE];
	short m_Radix;
};
