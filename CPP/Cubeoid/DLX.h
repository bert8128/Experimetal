#ifndef DLX_H
#define DLX_H

#include <vector>
#include <ctime>

class DLXNode;
class DLXColumn;
class DLXRow;
class Piece;

#define MAXINT 0x7FFFFFFF
#define MININT 0x80000000

class DLX
{
public:
    DLX(unsigned int maxSols, unsigned int printFreq);
    DLX(unsigned int nc, 
        unsigned int nr, 
        unsigned int nn, 
        unsigned int maxSols, // 0 for unlimited
        unsigned int printFreq); // eg 256 for Sudoku, 32 for Bedlam.  Best if a power of 2

    size_t GetRowsInSolution(void) const;
    unsigned int GetSolutionRow(unsigned int row) const;

    void AddNode(unsigned int colidx, unsigned int rowidx, bool samerow);
    bool GivenRow(unsigned int row);
    bool Given(DLXNode* node);
    bool Given(unsigned int node);
    unsigned long long Solve(void);
    std::vector<unsigned int>& getSolution(void) { return foundsolution; }

protected:
    void initMembers(unsigned int maxSols, unsigned int printFreq);
    bool isValid;
    clock_t start;

    void Init(unsigned int nc, unsigned int nr, unsigned int nn);
    std::vector<DLXNode*>& getNodeList(void) { return Nodes; }

private:
    void CoverCol(DLXColumn* coverCol);
    void UncoverCol(DLXColumn* uncoverCol);
    DLXColumn* ChooseMinCol(void);
    void search(unsigned int k);

    std::vector<unsigned int> trysolution;
    std::vector<unsigned int> foundsolution;
    DLXColumn* root;
    std::vector<DLXColumn*> ColHdrs;
    std::vector<DLXNode*> Nodes;
    std::vector<DLXRow*> Rows;
    unsigned int numcols;
    unsigned int numrows;
    unsigned int numnodes;
    DLXNode* lastnodeadded;
    unsigned long long NumSolns;
    unsigned long long NumAttempts;
    unsigned int m_maxSols;
    unsigned int m_printFreq;
};

#endif