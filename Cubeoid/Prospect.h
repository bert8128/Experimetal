#ifndef ProspectDLX_H
#define ProspectDLX_H

#include "DLX.h"
#include <vector>
#include <iostream>
#include <stdarg.h>

using namespace::std;

#define NUM_PIECES_2 5
#define SIDE_SIZE_2 11
#define NUM_PLACES_2 121
#define ROTATIONS_2 1

class Coord2
{
public:
    Coord2 () : m_x(0),m_y(0) { }
    Coord2 (int x, int y)	: m_x(x),m_y(y) { }
    int X(void) const { return m_x; }
    int Y(void) const { return m_y; }
    void X(int v) { m_x = v; }
    void Y(int v) { m_y = v; }
    void decX(int v) { m_x -= v; }
    void decY(int v) { m_y -= v; }
    void incX(int v) { m_x += v; }
    void incY(int v) { m_y += v; }

private:
    int m_x, m_y;
};

class CoordList2
{
public:
    CoordList2() {}
    CoordList2(Coord2* p1, ...);
    vector<Coord2*> getList(void) { return m_list; };
private:
    vector<Coord2*> m_list;
};

/*class RotationMatrix
{
public:
    RotationMatrix(int* a) : m(a) {}
    const int* getM(void) const { return m; }
private:
    int* m;
};*/

class Piece2
{
public:
    vector<Coord2*> coords;

    Piece2() ;
    Piece2(vector<Coord2*> co);

    void add(Coord2* c);
    Coord2* GetCoord(int index) const;

    int maxX(void) const { return m_maxX;}
    int maxY(void) const { return m_maxY;}
    size_t NumBlocks(void) const { return coords.size();}
    int length(void) const { return m_maxX - m_minX; }
    int width (void) const { return m_maxY - m_minY; }

//    Piece2* CopyRotate(const RotationMatrix& m) const;
    Piece2* CopyRotate(int r) const;
    Piece2* CopyTranslate(int xdelta, int ydelta) const;
    void ResetOrigin(void);
    unsigned long BinaryValue(void) const;
    void Dump(void) const;

private:
    int m_minX, m_maxX;
    int m_minY, m_maxY;
};
/*
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
*/
class ProspectDLX : public DLX
{
public:
    ProspectDLX(std::vector <Piece2*>& pieces, 
              const std::vector<std::vector<std::pair<unsigned long, Piece2*>>>& AllMoves,
              unsigned int maxSols); // 0 for unlimited
    static bool contains(const vector<pair<unsigned long, Piece2*>> & list, const unsigned long b);
    static int ProspectDLX::doProspect(unsigned int maxSols);
};

#endif
