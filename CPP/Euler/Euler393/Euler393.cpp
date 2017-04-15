// Euler393.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <iostream>
#include <vector>
using namespace std;

#include <windows.h>

class MTimer
{
        /* All returned times are in milliseconds */
public:
        MTimer(void);
        void start(void) ; //< Start the timer
        double stop(void); //< Returns the time since start
        double reset(void); //< Stops the timer and resets and returns the cummulative time
        double pause(void); //< Pauses the timer and returns the cumulative time 
        double resume(void); //< Resumes the timer and returns the cumulative time 
        double getCumulative(void) const;
private:
        LONGLONG getFrequency(void) const;
        LARGE_INTEGER s;// start
        LARGE_INTEGER f; // finish
        LONGLONG c; // cumulative
        LONGLONG frequency;
};
MTimer::MTimer(void)
  : s(),
        f(),
        c(0),
        frequency(0)
{
        frequency = getFrequency();
}
LONGLONG MTimer::getFrequency(void) const
{
        LARGE_INTEGER proc_freq;
        ::QueryPerformanceFrequency(&proc_freq);
        return proc_freq.QuadPart;
}
void MTimer::start(void)
{
        DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
        ::QueryPerformanceCounter(&s);
        ::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
}
double MTimer::stop(void)
{
        DWORD_PTR oldmask = ::SetThreadAffinityMask(::GetCurrentThread(), 0);
        ::QueryPerformanceCounter(&f);
        ::SetThreadAffinityMask(::GetCurrentThread(), oldmask);
        LONGLONG l = f.QuadPart - s.QuadPart;
        c += l;
        return l / (double)frequency;
}
double MTimer::reset(void)
{
        stop();
        LONGLONG l = c;
        c = 0;
        return l / (double)frequency;
}
double MTimer::pause(void)
{
        stop();
        return getCumulative();
}
double MTimer::resume(void)
{
        start();
        return getCumulative();
}
double MTimer::getCumulative(void) const
{
        return c / (double)frequency;
}

class e393
{
#define dim		4
#define dimM1		(dim-1)
#define dimsq	(dim*dim)
#define ___max		(2 * ((dim * dim) - dim))
#define hmax	(___max/2)

	static const int _dim = dim;
	static const int _dimM1 = dimM1;
	static const int _dimsq = dimsq;
	static const int _max = ___max;
	static const int _hmax = hmax;
	static const int lenBool = _max*sizeof(bool);
	static const int lenInt = _dimsq*sizeof(int);
    
	enum Direction { N = 0, S, E, W, undef };

	class Path
	{
	public:
		static char getX(char point)
		{
			return point % dim;
		}
		static char getY(char point)
		{
			return point / dim;
		}
		char* m_Points;//[_dimsq+1];
		Path()
		{
			m_Points = new char[_dimsq+1];
			memset(m_Points, -1, _dimsq);
		}
		bool contains(char point)
		{
			char* p = m_Points;
			while (*p != -1)
				if (*p == point)
					return true;
			return false;
		}
		char length(void) const
		{
			char len = 0;
			char* p = m_Points;
			while (*p != (char)(-1))
				++len;
			return len;
		}
		char first(void) const
		{
			return m_Points[0];
		}
		char last(void) const
		{
			int len = length();
			if (len == 0)
				return -1;
			return m_Points[len-1];
		}
		char nextToLast(void) const
		{
			int len = length();
			if (len < 2)
				return -1;
			return m_Points[len-2];
		}
		Path addPoint(char p) const
		{
			Path path = *this;
			path.m_Points[length()] = p;
			return path;
		}
		static bool isValidDirection(char point, Direction d)
		{
			switch (d)
			{
			case N:
				return (getY(point) < _dim-1);
			case S:
				return (getY(point) > 1);
			case E:
				return (getX(point) < _dim-1);
			case W:
				return (getX(point) > 1);
			}
			return false;
		}
		static char calcPoint(char point, Direction d)
		{
			switch (d)
			{
			case N:
				return point + _dim;
			case S:
				return point - _dim;
			case E:
				return point+1;
			case W:
				return point-1;
			}
			return -1;
		}
		static Direction calcDirectionToPoint(char from, char to)
		{
			char fromX = getX(from);
			char toX = getX(to);
			if (fromX < toX)
				return E;
			else if (fromX > toX)
				return W;
			else
			{
				char fromY = getY(from);
				char toY = getY(to);
				if (fromY < toY)
					return N;
				else if (fromY > toY)
					return S;
			}
			return undef;
		}
	};
	vector<Path> getListOfPaths(Path pathSoFar)
	{
		vector<Path> list;
		char here = pathSoFar.last();
		if (pathSoFar.length() > 1 && here == pathSoFar.first())
		{
			list.push_back(pathSoFar);
			return list;
		}
		Direction in = Path::calcDirectionToPoint(pathSoFar.nextToLast(), here);
		for (char i=0; i<4; ++i)
		{
			Direction out = static_cast<Direction>(i);
			if (out != in && Path::isValidDirection(here, out))
			{
				char there = Path::calcPoint(here, out);
				if (here == pathSoFar.first())
				{
					list.push_back(pathSoFar.addPoint(out));
				}
				else if (!pathSoFar.contains(there))
				{
					vector<Path> these = getListOfPaths(pathSoFar.addPoint(out));
					for (size_t i=0; i<these.size(); ++i)
						list.push_back(these[i]);
				}
			}
		}
		return list;
	}

	bool testOne(int thisLine, int* points)
    {
        if (thisLine < _hmax)
        {
            int row = thisLine / _dimM1;
            int col = thisLine % _dimM1;
            if (points[(row * _dim) + col] > 1)
                return false;
            if (points[(row * _dim) + col + 1] > 1)
                return false;
            ++points[(row * _dim) + col];
            ++points[(row * _dim) + col + 1];
        }
        else
        {
            int col = (thisLine - _hmax) / _dimM1;
            int row = (thisLine - _hmax) % _dimM1;
            if (points[(row * _dim) + col] > 1)
                return false;
            if (points[((row + 1) * _dim) + col] > 1)
                return false;
            ++points[(row * _dim) + col];
            ++points[((row + 1) * _dim) + col];
        }
        return true;
    }
    bool test2(int* points)
    {
        for (int j = 0; j < _dimsq; ++j)
            if (points[j] != 2)
                return false;

        return true;
    }
    int numSols(int pos, bool* lines, int numLines, int* points, int dummy)
    {
		++dummy;
        if (pos == _max)
        {
            if (numLines != _dimsq)
                return 0;
            else if (test2(points))
                return 1;
            else
                return 0;
        }
        int sols = 0;
		bool lines2[_max];
		memcpy(lines2, lines, lenBool);
		int points2[dimsq];
		memcpy(points2, points, lenInt);

		lines2[pos] = false;
        if (_dimsq-numLines < _max-pos) // check to see if we can have enough lines - we need exactly dimsq lines
            sols += numSols(pos + 1, lines2, numLines, points2, dummy);

		lines2[pos] = true;
        ++numLines;
        if (numLines <= _dimsq) // since we need exactly dimsq lines, there isn't any point in testing with more
        {
            if (testOne(pos, points2))
                sols += numSols(pos + 1, lines2, numLines, points2, dummy);
        }
		return sols;
    }
public:
    int answer()
    {
/*        bool lines[_max];
        for (int i = 0; i < _max; ++i)
            lines[i] = false;
		 
        int points[dimsq];
        for (int j = 0; j < dimsq; ++j)
            points[j] = 0;
    
		int sols=0;
		MTimer t;
		MTimer t2;
		t.start();
		{
			for (int i=0;i<100;++i)
			{
				t2.resume();
				sols += numSols(0, lines, 0, points,i);
				t2.pause();
			}
		}
		cout << t.stop() << endl;
		cout << t2.getCumulative() << endl;
		*/

		vector<Path> list;
		Path path;
		path.addPoint(0);
		char here = 0;
		for (char i=0; i<4; ++i)
		{
			Direction out = static_cast<Direction>(i);
			if (Path::isValidDirection(here, out))
			{

				vector<Path> these = getListOfPaths(path.addPoint(out));
				for (size_t i=0; i<these.size(); ++i)
					list.push_back(these[i]);
			}
		}
		return list.size();
    }
};

int _tmain(int argc, _TCHAR* argv[])
{
	e393 x;
	cout << x.answer() << endl;
	char ch;
	cin >> ch;
	return 0;
}
