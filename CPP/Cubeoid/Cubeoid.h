#ifndef CuboidDLX_H
#define CuboidDLX_H

#include "DLX.h"
#include <vector>
#include <iostream>
#include <stdarg.h>

using namespace::std;

static const size_t ROTATIONS = 24;

class Coord
{
public:
    Coord () : m_x(0),m_y(0),m_z(0) { }
    Coord (int x, int y, int z)	: m_x(x),m_y(y),m_z(z) { }
    int X(void) const { return m_x; }
    int Y(void) const { return m_y; }
    int Z(void) const { return m_z; }
    void X(int v) { m_x = v; }
    void Y(int v) { m_y = v; }
    void Z(int v) { m_z = v; }
    void decX(int v) { m_x -= v; }
    void decY(int v) { m_y -= v; }
    void decZ(int v) { m_z -= v; }
    void incX(int v) { m_x += v; }
    void incY(int v) { m_y += v; }
    void incZ(int v) { m_z += v; }

private:
    int m_x, m_y, m_z;
};

class CoordList
{
public:
    CoordList() {}
    CoordList(Coord* p1, ...);
    vector<Coord*> getList(void) { return m_list; };
private:
    vector<Coord*> m_list;
};

class RotationMatrix
{
public:
    RotationMatrix(int* a) : m(a) {}
    const int* getM(void) const { return m; }
private:
    int* m;
};

class Piece
{
public:
    vector<Coord*> coords;

    Piece() ;
    Piece(vector<Coord*> co);

    void add(Coord* c);
    Coord* GetCoord(int index) const;

    int maxX(void) const { return m_maxX;}
    int maxY(void) const { return m_maxY;}
    int maxZ(void) const { return m_maxZ;}
    size_t NumBlocks(void) const { return coords.size();}

    Piece* CopyRotate(const RotationMatrix& m) const;
    Piece* CopyTranslate(int xdelta, int ydelta, int zdelta) const;
    void ResetOrigin(void);
    unsigned long long BinaryValue(void) const;
    void Dump(void) const;

private:
    int m_minX, m_maxX;
    int m_minY, m_maxY;
    int m_minZ, m_maxZ;
};

class RotationMatrixes
{
public:

    static int getV(int x, int i, int j);
    static int Length (void) { return ROTATIONS; }
    static const RotationMatrix& matrix(int index);

private:
    static int rm_01[];
    static int rm_02[];
    static int rm_03[];
    static int rm_04[];
    static int rm_05[];
    static int rm_06[];
    static int rm_07[];
    static int rm_08[];
    static int rm_09[];
    static int rm_10[];
    static int rm_11[];
    static int rm_12[];
    static int rm_13[];
    static int rm_14[];
    static int rm_15[];
    static int rm_16[];
    static int rm_17[];
    static int rm_18[];
    static int rm_19[];
    static int rm_20[];
    static int rm_21[];
    static int rm_22[];
    static int rm_23[];
    static int rm_24[];

    static RotationMatrix m_matrix[ROTATIONS];
    static int arr[];
};

class Cubeoid
{
public:
	virtual const Piece* getPieces() const = 0;
	virtual int getNumPieces() const = 0;
	virtual int getX() const = 0;
	virtual int getY() const = 0;
	virtual int getZ() const = 0;
	int getNumPlaces() const { return getX()*getY()*getZ(); }
};

class CuboidDLX : public DLX
{
public:
    CuboidDLX(Cubeoid& rCubeoid,
              const std::vector<std::vector<std::pair<unsigned long long, Piece*>>>& AllMoves,
              unsigned int maxSols); // 0 for unlimited
    static bool contains(const vector<pair<unsigned long long, Piece*>> & list, unsigned long long b);
    static int CuboidDLX::doBedlam(unsigned int maxSols);
private:
	Cubeoid& m_rCubeoid;
};

#endif
