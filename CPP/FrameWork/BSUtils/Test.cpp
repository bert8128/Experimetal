#include "Association.h"

namespace BSUtilsTest
{
	class IntInt : public Tuple<int, int>
	{
	public:
		IntInt() : Tuple() {}
		IntInt(int a, int b) : Tuple<int, int>(a, b) {}
	};

	void Test()
	{
		IntInt i1;
		IntInt i2(0, 0);
		IntInt i3(1, 2);
		IntInt i4(i3);
		IntInt i5(i3);
		auto a1 = i1 == i2;
		auto a2 = i1 != i2;
		auto a3 = i1 > i2;
		auto a4 = i1 >= i2;
		auto a5 = i1 < i2;
		auto a6 = i1 <= i2;
		auto a7 = i1.compareTo(i2);
		auto a8 = i1.isEqual(i2);
		auto a9 = i1.hash();
		i1 = i2;
	}
}
