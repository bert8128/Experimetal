#include "DLX.h"

#include <vector>
#include <iostream>

using namespace::std;

class LL2DNode
{
public:
    LL2DNode() : L(0), R(0), U(0), D(0) { }

    void SetLeft (LL2DNode* left ) { L = left;  }
    void SetRight(LL2DNode* right) { R = right; }
    void SetUp   (LL2DNode* up   ) { U = up;    }
    void SetDown (LL2DNode* down ) { D = down;  }

    LL2DNode* GetLeft (void) const { return L; }
    LL2DNode* GetRight(void) const { return R; }
    LL2DNode* GetUp   (void) const { return U; }
    LL2DNode* GetDown (void) const { return D; }

private: 
    LL2DNode* L;   // Pointer to left node
    LL2DNode* R;   // Pointer to right node
    LL2DNode* U;   // Pointer to node above
    LL2DNode* D;   // Pointer to node below
};


class DLXColumn : public LL2DNode
{
public:
    DLXColumn() : size(0)
    {
        SetUp(this);
        SetDown(this);
    }
    unsigned int GetSize(void) const { return size; }
    void DecSize(void) { --size; }
    void IncSize(void) { ++size; }
private:
    unsigned int size;		// Number of items in column
};

class DLXNode : public LL2DNode
{
public:
    DLXNode(DLXColumn* col, unsigned int ri) : C(col), RowIdx(ri)
    {
        col->GetUp()->SetDown(this);
        SetUp(col->GetUp());
        SetDown(col);
        col->SetUp(this);
        col->IncSize();
    }
    DLXColumn* GetColumn() { return C; }
    unsigned int GetRowIdx() { return RowIdx; }
private:
    DLXColumn* C;	// Pointer to Column Header
    unsigned int RowIdx;     // Index to row
};

class DLXRow
{
public:
    DLXRow(DLXNode* first) : FirstNode(first) { }
    DLXNode* FirstNode;
};

DLX::DLX(unsigned int maxSols, unsigned int printFreq)
{
    initMembers(maxSols, printFreq);
}

DLX::DLX(unsigned int nc, unsigned int nr, unsigned int nn, unsigned int maxSols, unsigned int printFreq)
{
    initMembers(maxSols, printFreq);
    Init(nc, nr, nn);
}

void DLX::initMembers(unsigned int maxSols, unsigned int printFreq)
{
    m_maxSols = maxSols;
    m_printFreq = printFreq;
    isValid = true;
    root = new DLXColumn();
    lastnodeadded = 0;
}

size_t DLX::GetRowsInSolution(void) const 
{
    return foundsolution.size(); 
}
unsigned int DLX::GetSolutionRow(unsigned int row) const
{
    return foundsolution[row];
}

void DLX::AddNode(unsigned int colidx, unsigned int rowidx, bool samerow)
{
    DLXColumn* col = ColHdrs[colidx];
    DLXNode* pNode = new DLXNode(col, rowidx);
    Nodes[++numnodes] = pNode;
    if (samerow)
    {
        Nodes[numnodes]->SetLeft(lastnodeadded);
        Nodes[numnodes]->SetRight(lastnodeadded->GetRight());
        lastnodeadded->SetRight(Nodes[numnodes]);
        Nodes[numnodes]->GetRight()->SetLeft(Nodes[numnodes]);
    }
    else
    {
        Rows[++numrows] = new DLXRow(Nodes[numnodes]);
        Nodes[numnodes]->SetLeft(Nodes[numnodes]);
        Nodes[numnodes]->SetRight(Nodes[numnodes]);
    }
    lastnodeadded = Nodes[numnodes];
}

bool DLX::GivenRow(unsigned int row)
{
    return Given(Rows[row]->FirstNode);
}

bool DLX::Given(DLXNode* node)
{
    DLXNode* startNode = node;
    DLXNode* currNode = startNode;
    do
    {
        DLXColumn* ColHdr = currNode->GetColumn();
        // Check if this is still a valid column
        if (ColHdr->GetLeft()->GetRight() != ColHdr)
            return false;
        CoverCol(ColHdr);
        currNode = static_cast<DLXNode*>(currNode->GetRight());
    } while (currNode != startNode);
    trysolution.push_back(currNode->GetRowIdx());
    return true;
}

bool DLX::Given(unsigned int node)
{
    return Given(Nodes[node]);
}

unsigned long long DLX::Solve(void)
{
    if (!isValid)
        return static_cast<unsigned long long>(-1);

    start = clock();

    NumSolns = 0;
    NumAttempts = 0;
    search(unsigned int (trysolution.size()));
    return NumSolns;
}

void DLX::Init(unsigned int nc, unsigned int nr, unsigned int nn)
{
    numcols = nc;
    ColHdrs.resize(numcols + 1);
    //vector<DLXColumn*>::iterator iter;
    //for (auto iter = ColHdrs.begin(); iter != ColHdrs.end(); ++iter )
	for (auto& iter : ColHdrs)
        iter = new DLXColumn();

    Nodes.resize(nn + 1);
    numnodes = 0;   // None allocated

    Rows.resize(nr + 1);
    numrows = 0;    // None allocated

    DLXColumn* prev = root;
    for (unsigned int i = 1; i <= numcols; ++i)
    {
        prev->SetRight(ColHdrs[i]);
        ColHdrs[i]->SetLeft(prev);
        prev = ColHdrs[i];
    }
    root->SetLeft(ColHdrs[numcols]);
    ColHdrs[numcols]->SetRight(root);
}

void DLX::CoverCol(DLXColumn* coverCol)
{
    LL2DNode* i;
    LL2DNode* j;

    coverCol->GetRight()->SetLeft(coverCol->GetLeft());
    coverCol->GetLeft()->SetRight(coverCol->GetRight());

    i = coverCol->GetDown();

    while (i != coverCol)
    {
        j = i->GetRight();
        while (j != i)
        {
            j->GetDown()->SetUp(j->GetUp());
            j->GetUp()->SetDown(j->GetDown());
            (static_cast<DLXNode*>(j))->GetColumn()->DecSize();
            j = j->GetRight();
        }
        i = i->GetDown();
    }
}

void DLX::UncoverCol(DLXColumn* uncoverCol)
{
    LL2DNode* i = NULL;
    LL2DNode* j = NULL;

    i = uncoverCol->GetUp();
    while (i != uncoverCol)
    {
        j = i->GetLeft();
        while (j != i)
        {
            (static_cast<DLXNode*>(j))->GetColumn()->IncSize();
            j->GetDown()->SetUp(j);
            j->GetUp()->SetDown(j);
            j = j->GetLeft();
        }
        i = i->GetUp();
    }
    uncoverCol->GetRight()->SetLeft(uncoverCol);
    uncoverCol->GetLeft()->SetRight(uncoverCol);
}

DLXColumn* DLX::ChooseMinCol(void)
{
    unsigned int minsize = MAXINT;
    DLXColumn* search = NULL;
    DLXColumn* mincol = NULL;

    mincol = search = (static_cast<DLXColumn*>(root->GetRight()));

    int col=0;
    while (search != root)
    {
        ++col;
        if (search->GetSize() < minsize)
        {
            mincol = search;
            minsize = mincol->GetSize();
            if (minsize == 0)
                break;
        }
        search = static_cast<DLXColumn*>(search->GetRight());
    }
    if (minsize==0)
        return 0;
    else
        return mincol;
}
void DLX::search(unsigned int k)
{
    DLXColumn* chosenCol = NULL;
    LL2DNode* r = NULL;
    LL2DNode* j = NULL;

    if (root->GetRight() == root)
    {
        foundsolution = trysolution;
        ++NumSolns;

        if (NumSolns % m_printFreq == 0)
        {
            clock_t now = clock();

            cout << NumSolns << endl;

            long double secondssofar = (double)(now - start) / CLOCKS_PER_SEC;
            cout << "Secs so far: " << secondssofar << endl;

            long double mspersol = (1000 * secondssofar / NumSolns);
            cout << " (" <<  mspersol << "ms per solution; " << endl;

            if (secondssofar >= 1)
            {
                unsigned long long solspersec = NumSolns / ((unsigned long long)(secondssofar));
                cout << solspersec << " solutions/sec)" << endl;
            }

            unsigned long long attemptspersol = NumAttempts/NumSolns;
            cout << "Attempts: " << NumAttempts << " (" << attemptspersol << " attempts per solution)" << endl;

            if (secondssofar >= 1)
            {
                unsigned long long attemptspersec = NumAttempts/((unsigned long long)(secondssofar));
                cout << "Attempts per second: " <<attemptspersec << endl;
            }
        }

        return;
    }
    chosenCol = ChooseMinCol();
    if (chosenCol != 0)
    {
        CoverCol(chosenCol);
        r = chosenCol->GetDown();

        while (r != chosenCol)
        {
            if (k >= trysolution.size())
                trysolution.push_back((static_cast<DLXNode*>(r))->GetRowIdx());
            else
                trysolution[k] = (static_cast<DLXNode*>(r))->GetRowIdx();
            ++NumAttempts;
            j = r->GetRight();
            while (j != r)
            {
                CoverCol((static_cast<DLXNode*>(j))->GetColumn());
                j = j->GetRight();
            }
            search(k + 1);
            if (m_maxSols > 0)
                if (NumSolns >= m_maxSols)   // Stop as soon as we find 1 solution
                    return;
            j = r->GetLeft();
            while (j != r)
            {
                UncoverCol((static_cast<DLXNode*>(j))->GetColumn());
                j = j->GetLeft();
            }
            r = r->GetDown();
        }
        UncoverCol(chosenCol);
    }
    return;
}
