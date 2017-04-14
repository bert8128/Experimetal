#include "Prospect.h"

CoordList2::CoordList2(Coord2* p1, ...)
{
    va_list marker;
    va_start( marker, p1 );
    while(NULL!=p1)
    {
        m_list.push_back(p1);
        p1 = va_arg(marker, Coord2*);
    }
    va_end( marker );
}

Piece2::Piece2() 
        : m_minX(MAXINT), 
          m_maxX(MININT),
          m_minY(MAXINT), 
          m_maxY(MININT)
{
}

Piece2::Piece2(vector<Coord2*> co)
        : m_minX(MAXINT), 
          m_maxX(MININT),
          m_minY(MAXINT), 
          m_maxY(MININT)
{
    vector<Coord2*>::iterator iter;
    for (iter = co.begin(); iter != co.end(); iter++ )
        add(*iter);
}

void Piece2::add(Coord2* c)
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
}

Coord2* Piece2::GetCoord(int index) const
{
    return coords[index];
}

Piece2* Piece2::CopyRotate(int r) const
{
    Piece2* pRotated = new Piece2();
    vector<Coord2*>::const_iterator iter;
    if (0 == r)
    {
        for (iter = coords.begin(); iter != coords.end(); iter++ )
        {
            pRotated->add(new Coord2((*iter)->X(), (*iter)->Y()));
        }
    }
    else
    {
        for (iter = coords.begin(); iter != coords.end(); iter++ )
        {
            pRotated->add(new Coord2((*iter)->Y(), (*iter)->X()));
        }
    }
    return pRotated;
}

Piece2* Piece2::CopyTranslate(int xdelta, int ydelta) const
{
    Piece2* pTranslated = new Piece2();
    vector<Coord2*>::const_iterator iter;
    for (iter = coords.begin(); iter != coords.end(); iter++ )
    {
        pTranslated->add(new Coord2((*iter)->X() + xdelta, (*iter)->Y() + ydelta));
    }

    return pTranslated;
}

void Piece2::ResetOrigin(void)
{
    vector<Coord2*>::iterator iter;
    for (iter = coords.begin(); iter != coords.end(); iter++ )
    {
        (*iter)->decX(m_minX);
        (*iter)->decY(m_minY);
    }
    m_maxX -= m_minX;
    m_maxY -= m_minY;
    m_minX = 0;
    m_minY = 0;
}

unsigned long Piece2::BinaryValue(void) const
{
    unsigned long res = 0;

    vector<Coord2*>::const_iterator iter;
    for (iter = coords.begin(); iter != coords.end(); iter++ )
    {
        unsigned long u = unsigned long(1) << ((*iter)->X() + 4 * (*iter)->Y());
        res |= u;
    }
    return res;
}

void Piece2::Dump(void) const
{
    vector<Coord2*>::const_iterator iter;
    for (iter = coords.begin(); iter != coords.end(); iter++ )
        cout << (*iter)->X() << "," << (*iter)->Y() << endl;
    cout << "-------------" << endl;
}

ProspectDLX::ProspectDLX(
    vector<Piece2*>& pieces, 
    const vector<vector<pair<unsigned long, Piece2*>>>& AllMoves,
    unsigned int maxSols) // 0 for unlimited
  : DLX(maxSols, 2)
{
    unsigned int TotalMoves = 0;
    unsigned int TotalNodes = 0;
    for (unsigned int i = 0; i < AllMoves.size(); ++i)
    {
        TotalMoves += unsigned int (AllMoves[i].size());
        TotalNodes += unsigned int (AllMoves[i].size()) * (pieces[i]->NumBlocks() + 2);
    }

    Init(SIDE_SIZE_2-1 + NUM_PLACES_2, TotalMoves, TotalNodes);

    unsigned int moveidx = 0;
    int i = 0;

    vector<vector<pair<unsigned long, Piece2*>>>::const_iterator iter1;
    for (iter1 = AllMoves.begin(); iter1 != AllMoves.end(); iter1++ )
    {
        vector<pair<unsigned long, Piece2*>>::const_iterator iter2;
        for (iter2 = iter1->begin(); iter2 != iter1->end(); iter2++ )
        {
            const Piece2* p = iter2->second;
            AddNode(p->length()+1, moveidx, false);
            AddNode(p->width()+1, moveidx, true);
            for (unsigned int l = 0; l < p->NumBlocks(); ++l)
            {
                Coord2* c = p->GetCoord(l);
                long offset = (SIDE_SIZE_2 - 1);
                long x = c->X();
                long y = c->Y();
                long col = y + x*SIDE_SIZE_2;
                AddNode(offset + col + 1, moveidx, true);
            }
            ++moveidx;
        }
        ++i;
    }
};

bool ProspectDLX::contains(const vector<pair<unsigned long, Piece2*>> & list, const unsigned long b)
{
    vector<pair<unsigned long, Piece2*>>::const_iterator iter;
    for (iter = list.begin(); iter != list.end(); iter++ )
    {
        if (iter->first == b)
            return true;
    }
        
    return false;
}

int ProspectDLX::doProspect(unsigned int maxSols)
{
    static const int numPieces = ((SIDE_SIZE_2-1)*(SIDE_SIZE_2-1) - (SIDE_SIZE_2-1))/2;
    vector<Piece2*> pieces;
    pieces.reserve(numPieces);

    // generate the pieces - 1x1, 1x2, ... 1x10, 2x1, 2x3, ... 2x10 ... 10x10
    int count=0;
    for (int x=SIDE_SIZE_2-1; x>=1; --x)
    {
        for (int y=SIDE_SIZE_2-1; y>=1; --y)
        {
            if (x!=y && y>x)
            {
                Piece2* piece = new Piece2;
                count++;
                for (int j=0; j<x; ++j)
                {
                    for (int k=0; k<y; ++k)
                    {
                        piece->add(new Coord2(j, k));
                    }
                }
                pieces.push_back(piece);
            }
        }
    }

    vector<vector<pair<unsigned long, Piece2*>>> AllMoves(numPieces);
    // For each piece
    for (int i=0; i<numPieces; ++i)
    {
        Piece2* p = pieces[i];
        vector<pair<unsigned long, Piece2*>>& thisPieceList = AllMoves[i];
        thisPieceList.reserve(242);
        // apply each rotation
        for (int r=0;r<2;++r)
        {
            Piece2* pRotated = p->CopyRotate(r);
            pRotated->ResetOrigin();
            // Apply all possible translations that remain inside boxed area
            const int maxx = (SIDE_SIZE_2 - pRotated->maxX() - 1);
            for (int x = 0; x <= maxx; ++x)
            {
                const int maxy = (SIDE_SIZE_2 - pRotated->maxY() - 1);
                for (int y = 0; y <= maxy; ++y)
                {
                    Piece2* translated = pRotated->CopyTranslate(x, y);
                    // Remove duplicated positions/orientations
                    //unsigned long b = translated->BinaryValue();
                    //if (!ProspectDLX::contains(thisPieceList, b))
                    {
                        thisPieceList.push_back(pair<unsigned long, Piece2*>(0/*b*/, translated));
                    }
                }
            }
        }
    }

    for (int i = 0; i < numPieces; ++i)
    {
        Piece2* p = pieces[i];
        const vector<pair<unsigned long, Piece2*>>& list = AllMoves[i];
        cout << "Piece2 " << i << " X: " << p->maxX()+1 << " Y: " << p->maxY()+1 << ": Unique Positions = " << list.size() << endl;
    }

    ProspectDLX dlx(pieces, AllMoves, maxSols);
    /*unsigned long long numsolns =*/ dlx.Solve();

    // Code to print solution 
//    if (numsolns == 1)
    {
        int Square[SIDE_SIZE_2][SIDE_SIZE_2];

        for (unsigned int s = 1; s <= dlx.GetRowsInSolution(); ++s)
        {
            cout << dlx.GetSolutionRow(s-1);

            unsigned int moveidx = 0;
            int i=0;
            vector<vector<pair<unsigned long, Piece2*>>>::const_iterator iter1;
            for (iter1 = AllMoves.begin(); iter1 != AllMoves.end(); iter1++ )
            {
                vector<pair<unsigned long, Piece2*>>::const_iterator iter2;
                for (iter2 = iter1->begin(); iter2 != iter1->end(); iter2++ )
                {
                    const Piece2* p = iter2->second;
                    if (dlx.GetSolutionRow(s-1) == moveidx)
                    {
                        cout << " " << p->length() << " " << p->width() << endl;
                        const unsigned long area  = (p->length()+1)*(p->width()+1);
                        for (unsigned int l = 0; l < p->NumBlocks(); ++l)
                        {
                            Coord2* c = p->GetCoord(l);
                            Square[c->X()][c->Y()] = area;
                        }
                    }
                    ++moveidx;
                }
                ++i;
            }
        }
        for (int x = 0; x < SIDE_SIZE_2; ++x)
        {
            for (int y = 0; y < SIDE_SIZE_2; ++y)
            {
                cout << (Square[x][y]) << + " ";
            }
            cout << endl;
        }
        cout << endl;
    }

    return 0;
}

