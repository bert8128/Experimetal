#include "Cubeoid.h"

int RotationMatrixes::getV(int x, int i, int j)
{
    return m_matrix[x].getM()[i*j];
}

const RotationMatrix& RotationMatrixes::matrix(int index)
{
    return m_matrix[index];
}

int RotationMatrixes::arr[] = 
{
    1, 0, 0,  0, 1, 0,  0, 0, 1 ,
    1, 0, 0,  0, 1, 0,  0, 0, 1 
};
int RotationMatrixes::rm_01[] = { 1, 0, 0,  0, 1, 0,  0, 0, 1};
int RotationMatrixes::rm_02[] = { 1, 0, 0,  0, 0,-1,  0, 1, 0};
int RotationMatrixes::rm_03[] = { 1, 0, 0,  0,-1, 0,  0, 0,-1};
int RotationMatrixes::rm_04[] = { 1, 0, 0,  0, 0, 1,  0,-1, 0};
int RotationMatrixes::rm_05[] = { 0,-1, 0,  1, 0, 0,  0, 0, 1};
int RotationMatrixes::rm_06[] = { 0, 0, 1,  1, 0, 0,  0, 1, 0};
int RotationMatrixes::rm_07[] = { 0, 1, 0,  1, 0, 0,  0, 0,-1};
int RotationMatrixes::rm_08[] = { 0, 0,-1,  1, 0, 0,  0,-1, 0};
int RotationMatrixes::rm_09[] = {-1, 0, 0,  0,-1, 0,  0, 0, 1};
int RotationMatrixes::rm_10[] = {-1, 0, 0,  0, 0,-1,  0,-1, 0};
int RotationMatrixes::rm_11[] = {-1, 0, 0,  0, 1, 0,  0, 0,-1};
int RotationMatrixes::rm_12[] = {-1, 0, 0,  0, 0, 1,  0, 1, 0};
int RotationMatrixes::rm_13[] = { 0, 1, 0, -1, 0, 0,  0, 0, 1};
int RotationMatrixes::rm_14[] = { 0, 0, 1, -1, 0, 0,  0,-1, 0};
int RotationMatrixes::rm_15[] = { 0,-1, 0, -1, 0, 0,  0, 0,-1};
int RotationMatrixes::rm_16[] = { 0, 0,-1, -1, 0, 0,  0, 1, 0};
int RotationMatrixes::rm_17[] = { 0, 0,-1,  0, 1, 0,  1, 0, 0};
int RotationMatrixes::rm_18[] = { 0, 1, 0,  0, 0, 1,  1, 0, 0};
int RotationMatrixes::rm_19[] = { 0, 0, 1,  0,-1, 0,  1, 0, 0};
int RotationMatrixes::rm_20[] = { 0,-1, 0,  0, 0,-1,  1, 0, 0};
int RotationMatrixes::rm_21[] = { 0, 0,-1,  0,-1, 0, -1, 0, 0};
int RotationMatrixes::rm_22[] = { 0,-1, 0,  0, 0, 1, -1, 0, 0};
int RotationMatrixes::rm_23[] = { 0, 0, 1,  0, 1, 0, -1, 0, 0};
int RotationMatrixes::rm_24[] = { 0, 1, 0,  0, 0,-1, -1, 0, 0};

RotationMatrix RotationMatrixes::m_matrix[] = 
{
    RotationMatrix(rm_01),
    RotationMatrix(rm_02),
    RotationMatrix(rm_03),
    RotationMatrix(rm_04),
    RotationMatrix(rm_05),
    RotationMatrix(rm_06),
    RotationMatrix(rm_07),
    RotationMatrix(rm_08),
    RotationMatrix(rm_09),
    RotationMatrix(rm_10),
    RotationMatrix(rm_11),
    RotationMatrix(rm_12),
    RotationMatrix(rm_13),
    RotationMatrix(rm_14),
    RotationMatrix(rm_15),
    RotationMatrix(rm_16),
    RotationMatrix(rm_17),
    RotationMatrix(rm_18),
    RotationMatrix(rm_19),
    RotationMatrix(rm_20),
    RotationMatrix(rm_21),
    RotationMatrix(rm_22),
    RotationMatrix(rm_23),
    RotationMatrix(rm_24)
};


CoordList::CoordList(Coord* p1, ...)
{
    va_list marker;
    va_start( marker, p1 );
    while(NULL!=p1)
    {
        m_list.push_back(p1);
        p1 = va_arg(marker, Coord*);
    }
    va_end( marker );
}

Piece::Piece() 
        : m_minX(MAXINT), 
          m_maxX(MININT),
          m_minY(MAXINT), 
          m_maxY(MININT),
          m_minZ(MAXINT), 
          m_maxZ(MININT)
{
}

Piece::Piece(vector<Coord*> co)
        : m_minX(MAXINT), 
          m_maxX(MININT),
          m_minY(MAXINT), 
          m_maxY(MININT),
          m_minZ(MAXINT), 
          m_maxZ(MININT)
{
    vector<Coord*>::iterator iter;
    for (iter = co.begin(); iter != co.end(); ++iter )
        add(*iter);
}

void Piece::add(Coord* c)
{
    coords.push_back(c);
    if (c->X() < m_minX)
        m_minX = c->X();
    if (c->X() > m_maxX)
        m_maxX = c->X();
    if (c->Y() < m_minY)
        m_minY = c->Y();
    if (c->Y() > m_maxY)
        m_maxY = c->Y();
    if (c->Z() < m_minZ)
        m_minZ = c->Z();
    if (c->Z() > m_maxZ)
        m_maxZ = c->Z();
}

Coord* Piece::GetCoord(int index) const
{
    return coords[index];
}

Piece* Piece::CopyRotate(const RotationMatrix& m) const
{
    auto pRotated = new Piece();
    vector<Coord*>::const_iterator iter;

    for (iter = coords.begin(); iter != coords.end(); ++iter )
    {
        pRotated->add(new Coord(m.getM()[0] * (*iter)->X() +
                                m.getM()[1] * (*iter)->Y() +
                                m.getM()[2] * (*iter)->Z(),
                                m.getM()[3] * (*iter)->X() +
                                m.getM()[4] * (*iter)->Y() +
                                m.getM()[5] * (*iter)->Z(),
                                m.getM()[6] * (*iter)->X() +
                                m.getM()[7] * (*iter)->Y() +
                                m.getM()[8] * (*iter)->Z())
                                );
    }
    return pRotated;
}

Piece* Piece::CopyTranslate(int xdelta, int ydelta, int zdelta) const
{
    auto pTranslated = new Piece();
    //vector<Coord*>::const_iterator iter;
    for (auto iter = coords.begin(); iter != coords.end(); ++iter )
        pTranslated->add(new Coord((*iter)->X() + xdelta, (*iter)->Y() + ydelta, (*iter)->Z() + zdelta));

    return pTranslated;
}

void Piece::ResetOrigin(void)
{
    //vector<Coord*>::iterator iter;
	for (auto iter = coords.begin(); iter != coords.end(); ++iter )
    {
        (*iter)->decX(m_minX);
        (*iter)->decY(m_minY);
        (*iter)->decZ(m_minZ);
    }
    m_maxX -= m_minX;
    m_maxY -= m_minY;
    m_maxZ -= m_minZ;
    m_minX = 0;
    m_minY = 0;
    m_minZ = 0;
}

unsigned long long Piece::BinaryValue(void) const
{
    unsigned long long res = 0;

    //vector<Coord*>::const_iterator iter;
	for (auto iter = coords.begin(); iter != coords.end(); ++iter )
    {
        unsigned long long u = unsigned long long(1) << ((*iter)->X() + ((*iter)->Y()<<2) + ((*iter)->Z()<<4));
        res |= u;
    }
    return res;
}

void Piece::Dump(void) const
{
    //vector<Coord*>::const_iterator iter;
	for (auto iter = coords.begin(); iter != coords.end(); ++iter )
        cout << (*iter)->X() << "," << (*iter)->Y() << "," << (*iter)->Z() << endl;
    cout << "-------------" << endl;
}

template <typename T, size_t N>
constexpr size_t countOf(T const (&)[N]) noexcept
{
	return N;
}

class BedlamCube : public Cubeoid
{
private:
	const Piece m_pieces[13] =
	{
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(0, 0, 1), new Coord(0, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1), new Coord(2, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(2, 1, 0), new Coord(2, 1, 1), NULL).getList()),
		Piece(CoordList(new Coord(1, 0, 0), new Coord(0, 1, 0), new Coord(1, 1, 0), new Coord(1, 2, 0), new Coord(2, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1), new Coord(2, 1, 1), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 2, 0), new Coord(2, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(0, 0, 1), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(0, 1, 0), new Coord(0, 1, 1), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 0, 1), new Coord(0, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(2, 1, 0), new Coord(2, 2, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(0, 1, 0), new Coord(0, 2, 0), new Coord(0, 2, 1), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(1, 0, 1), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 4; }
	virtual int getY()					const override { return 4; }
	virtual int getZ()					const override { return 4; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

class EasyCubeoid : public Cubeoid
{
private:
	const Piece m_pieces[1] =
	{
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 3; }
	virtual int getY()					const override { return 1; }
	virtual int getZ()					const override { return 1; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

class EasyCubeoid2 : public Cubeoid
{
private:
	const Piece m_pieces[2] =
	{
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 3; }
	virtual int getY()					const override { return 1; }
	virtual int getZ()					const override { return 1; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

class EasyCube3 : public Cubeoid
{
private:
	const Piece m_pieces[2] =
	{
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0),  new Coord(1, 0, 1), new Coord(1, 1, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), new Coord(1, 0, 0),  new Coord(1, 0, 1), new Coord(1, 1, 0), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 2; }
	virtual int getY()					const override { return 2; }
	virtual int getZ()					const override { return 2; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

class TrivialCube : public Cubeoid
{
private:
	const Piece m_pieces[1] =
	{
		Piece(CoordList(new Coord(0, 0, 0), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 1; }
	virtual int getY()					const override { return 1; }
	virtual int getZ()					const override { return 1; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

class NearlyTrivialCubeoid : public Cubeoid
{
private:
	const Piece m_pieces[2] =
	{
		Piece(CoordList(new Coord(0, 0, 0), NULL).getList()),
		Piece(CoordList(new Coord(0, 0, 0), NULL).getList())
	};
public:
	virtual int getNumPieces()			const override { return countOf(m_pieces); }
	virtual int getX()					const override { return 2; }
	virtual int getY()					const override { return 1; }
	virtual int getZ()					const override { return 1; }
	virtual const Piece* getPieces()	const override { return m_pieces; }
};

CuboidDLX::CuboidDLX(
    Cubeoid& rCubeoid, 
    const vector<vector<pair<unsigned long long, Piece*>>>& AllMoves,
    unsigned int maxSols) // 0 for unlimited
  : DLX(maxSols, 64),
  m_rCubeoid(rCubeoid)
{
    unsigned int TotalMoves = 0;
    unsigned int TotalNodes = 0;
	for (unsigned int i = 0; i < AllMoves.size(); ++i)
	{
        TotalMoves += unsigned int (AllMoves[i].size());
        TotalNodes += unsigned int (AllMoves[i].size()) * (m_rCubeoid.getPieces()[i].NumBlocks() + 1);
    }

	Init(m_rCubeoid.getNumPlaces() + m_rCubeoid.getNumPieces(), TotalMoves, TotalNodes);

    unsigned int moveidx = 0;
    int ii = 0;

	auto zSquared = m_rCubeoid.getZ() * m_rCubeoid.getZ();
	for (const auto& iter1 : AllMoves)
	{
		for (const auto& iter2 : iter1)
        {
            const auto p = iter2.second;
            for (unsigned int l = 0; l < p->NumBlocks(); ++l)
            {
                auto c = p->GetCoord(l);
				auto x = c->X();
				auto y = c->Y();
				auto z = c->Z();
				auto b = x + (y * m_rCubeoid.getY()) + (z * zSquared) + 1;
				if (l == 0)
                    AddNode(b, moveidx, false);
                else
                    AddNode(b, moveidx, true);
            }
			AddNode(m_rCubeoid.getNumPlaces() + 1 + ii, moveidx, true);
            ++moveidx;
        }
        ++ii;
    }
};

bool CuboidDLX::contains(const vector<pair<unsigned long long, Piece*>> & list, unsigned long long b)
{
	for (auto& iter : list)
	{
		if (iter.first == b)
			return true;
    }
	return false;
}

int CuboidDLX::doBedlam(unsigned int maxSols)
{
	//TrivialCube cubeoid;
	NearlyTrivialCubeoid cubeoid;
	//EasyCube3 cubeoid;
	//BedlamCube cubeoid;
	auto pieces = cubeoid.getPieces();

	vector<vector<pair<unsigned long long, Piece*>>> AllMoves(cubeoid.getNumPieces());
	// For each piece
	auto numPiecesCubed = cubeoid.getNumPieces()*cubeoid.getNumPieces()*cubeoid.getNumPieces();
    for (int i=0; i<cubeoid.getNumPieces(); ++i)
    {
        auto& p = pieces[i];
        auto& thisPieceList = AllMoves[i];
        thisPieceList.reserve(numPiecesCubed);
        // Apply all the possible rotations
        auto matrices = RotationMatrixes::Length();
        for (int m = 0; m < matrices; ++m)
        {
            auto pnew = p.CopyRotate(RotationMatrixes::matrix(m));
            pnew->ResetOrigin();     // Move origin to 0,0,0
            // Apply all possible translations that remain inside boxed area
            auto maxx = (cubeoid.getX() - pnew->maxX());
            for (int x = 0; x < maxx; ++x)
            {
                auto maxy = (cubeoid.getY() - pnew->maxY());
                for (int y = 0; y < maxy; ++y)
                {
                    auto maxz = (cubeoid.getZ() - pnew->maxZ());
                    for (int z = 0; z < maxz; ++z)
                    {
                        auto translated = pnew->CopyTranslate(x, y, z);
                        // Remove duplicated positions/orientations
                        auto b = translated->BinaryValue();
                        if (!CuboidDLX::contains(thisPieceList, b))
                        {
                            //cout << (long long)(b) << ",";
                            thisPieceList.push_back(pair<unsigned long long, Piece*>(b, translated));
                        }
                    }
                }
            }
        }
        //cout << " Size " << thisPieceList.size() << endl;
        //cout << " Size " << thisPieceList.size() << endl;
        //cout << " Size " << thisPieceList.size() << endl;
    }

    for (int i = 0; i < cubeoid.getNumPieces(); ++i)
    {
        auto& list = AllMoves[i];
        cout << "Piece " << i << ": Unique Positions = " << list.size() << endl;
    }

    CuboidDLX dlx(cubeoid, AllMoves, maxSols);
    auto numsolns = dlx.Solve();
	cout << "Num solutions found = " << numsolns << endl << endl;

    // Code to print last solution 
    {
		auto Square = new int**[cubeoid.getX()];
		for (auto x=0; x<cubeoid.getX(); ++x)
		{
			Square[x] = new int*[cubeoid.getY()];
			for (auto y=0; y<cubeoid.getY(); ++y)
				Square[x][y] = new int[cubeoid.getZ()];
		}

		auto ris = dlx.GetRowsInSolution();
        for (unsigned int s = 1; s <= ris; ++s)
        {
            //cout << dlx.GetSolutionRow(s - 1) << endl;

            unsigned int moveidx = 0;
            int ii=0;
			// vector<vector<pair<unsigned long long, Piece*>>>::const_iterator iter1;
            //for (auto iter1 = AllMoves.begin(); iter1 != AllMoves.end(); ++iter1 )
            for (auto& iter1 : AllMoves)
			{
				// vector<pair<unsigned long long, Piece*>>::const_iterator iter2;
                //for (auto iter2 = iter1->begin(); iter2 != iter1->end(); ++iter2 )
                for (auto& iter2 : iter1)
				{
                    auto p = iter2.second;
                    if (dlx.GetSolutionRow(s-1) == moveidx)
                    {
                        for (unsigned int l = 0; l < p->NumBlocks(); ++l)
                        {
                            auto c = p->GetCoord(l);
                            Square[c->X()][c->Y()][c->Z()] = ii;
                        }
                    }
                    ++moveidx;
                }
                ++ii;
            }
        }
        for (auto x = 0; x < cubeoid.getX(); ++x)
        {
            for (auto y = 0; y < cubeoid.getY(); ++y)
			{
				cout << '\t';
				for (auto z = 0; z < cubeoid.getZ(); ++z)
					cout << (char)(Square[x][y][z] + 'A') << +" ";
                cout << endl;
			}
            cout << endl;
        }
		for (auto x=0; x<cubeoid.getX(); ++x)
		{
			for (auto y=0; y<cubeoid.getY(); ++y)
				delete [] Square[x][y];
			delete [] Square[x];
		}
		delete [] Square;
    }

    return 0;
}

