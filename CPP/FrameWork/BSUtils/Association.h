#pragma once

#include <tuple>

#include "Hash.h"

template <typename ... Ts>
class Tuple
{
private:
	std::tuple<Ts...> m_Data;

	template<std::size_t I, typename T>
	void init(T& arg)
	{
		std::get<I>(m_Data) = arg;
	}
	template<std::size_t I, typename T, typename ...Ts>
	void init(T& t, Ts...ts)
	{
		init<I, T>(t);
		init<I+1, Ts...>(ts...);
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
				comp = -1;
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
		static unsigned int hash(int& primeIndex, const T& x)
		{
			primeIndex++;
			unsigned int a = tuple_ops<T, I + 1, N>::hash(primeIndex, x);
			unsigned int b = Hash::hash(std::get<I>(x)) *  Hash::primes[primeIndex];
			return a ^ b;
		}
	};
	template<typename T, std::size_t N>
	struct tuple_ops<T, N, N>
	{
		static int compareTo(const T& x, const T& y) { return 0; }
		static bool isEqual(const T& x, const T& y) { return true; }
		static unsigned int hash(int& primeIndex, const T& x) { return 0; }
	};
public:
	template<typename ... Ts>
	Tuple()
	{
	}

	template<typename ... Ts>
	Tuple(Ts...ts)
	{
		init<0, Ts...>(ts...);
	}

	template<typename ... Ts>
	Tuple(const Tuple& rhs)
		: m_Data(rhs)
	{
	}

	template<typename ... Ts>
	Tuple(const Tuple&& rhs)
		: m_Data(std::move(rhs))
	{
	}

	template<typename ... Ts>
	Tuple& operator=(const Tuple& rhs)
	{
		if (this != &rhs)
			m_Data = rhs.m_Data;
		return *this;
	}

	template<typename ... Ts>
	Tuple& operator=(const Tuple&& rhs)
	{
		if (this != &rhs)
			m_Data = satd::move(rhs.m_Data);
		return *this;
	}

	template<std::size_t I>
	using data_type = typename std::tuple_element<I, decltype(m_Data)>::type;
	template<std::size_t I>
	auto get()->data_type<I>
	{
		return std:get<I>(m_Data);
	}

	template<typename ... Ts> bool operator==(const Tuple& rhs) { return m_Data == rhs.m_Data; }
	template<typename ... Ts> bool operator!=(const Tuple& rhs) { return m_Data != rhs.m_Data; }
	template<typename ... Ts> bool operator> (const Tuple& rhs) { return m_Data >  rhs.m_Data; }
	template<typename ... Ts> bool operator>=(const Tuple& rhs) { return m_Data >= rhs.m_Data; }
	template<typename ... Ts> bool operator< (const Tuple& rhs) { return m_Data <  rhs.m_Data; }
	template<typename ... Ts> bool operator<=(const Tuple& rhs) { return m_Data <= rhs.m_Data; }

	template<typename ... Ts>
	int compareTo(const Tuple& rhs) const
	{
		return tuple_ops<decltype(m_Data), 0, sizeof...(Ts)>::compareTo(m_Data, rhs.m_Data);
	}

	template<typename ... Ts>
	bool isEqual(const Tuple& rhs) const
	{
		return tuple_ops<decltype(m_Data), 0, sizeof...(Ts)>::isEqual(m_Data, rhs.m_Data);
	}

	template<typename ... Ts>
	unsigned int hash() const
	{
		int primeIndex = 0;
		return tuple_ops<decltype(m_Data), 0, sizeof...(Ts)>::hash(primeIndex, m_Data);
	}



};
