#pragma once

namespace Hash
{
	inline unsigned int hash(         char      value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(         short     value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(         int       value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(         long      value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(         long long value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(unsigned char      value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(unsigned short     value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(unsigned int       value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(unsigned long      value) { return static_cast<unsigned int>(value); }
	inline unsigned int hash(unsigned long long value) { return static_cast<unsigned int>(value); }
	int primes[] = { 3, 5, 7, 11, 13, 17, 19, 23, 31 }; // only supports a few!
}
