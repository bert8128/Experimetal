#pragma once

#include <tuple>

#include "Hash.h"

template <typename ... Ts>
class Tuple
{
private:
	std::tuple m_Data;

	template<std::size_t I, typename T>
	void int(T& arg)
	{
		std::get<I>(m_Data) = arg;
	}
	template<std::size_t I, typename T, typename Ts>
	void int(T& t, Ts ts)
	{
		init<I, T>(t);
		init<I+1, Ts...>(ts);
	}
	template<typename T, std::size_t I, std::size_t N>
	struct tuple_ops
	{
		static int compareTo(const T& x, const T& y)
		{
			int comp = 0;
			const auto& xx = std::get<I>(x);
			const auto& yy = std::get<I>(y);
			if (xx < yy)
				comp = 1;
			else if (xx > yy)
				comp = 1;
			else
				comp = tuple_ops<T, I + 1, N>::compareTo(x, y);
			return comp;
		}
		static bool isEqual(const T& x, const T& y)
		{
			if (!(std::get<I>(x) == std::get<I>(y)))
				return false;
			else
				return tuple_ops<T, I + 1, N>::isEqual(x, y);
		}
		/*
		?
		static unsigned int hash(int& primeIndex, const T& x)
		{
			return 0;
			primeIndex++;
			return
				(
					Hash::hash(std::get<I>(x))
					* 
					Hash::primes[primeIndex]
				)
				^
				tuple_ops<T, I + 1, N>::hash(primeIndex, x);
		}
		*/
	};
	template<typename T, std::size_t N>
	struct tuple_ops<T, N, N>
	{
		static int compareTo(const T& x, const T& y) { return 0; }
		static bool isEqual(const T& x, const T& y) { return true; }
		//static unsigned int hash(int& primeIndex, const T& x) { return 0; }
	};
public:
	template<typename ... Ts>
	Tuple()
	{
	}
	template<typename ... Ts>
	Tuple(Ts ..ts)
	{
		init<0, Ts...>(ts);
	}

};
