#include "SudokuDLX.h"
#include <iostream>

using namespace::std;

SudokuDLX::SudokuDLX(
    int dim, 
    int* sod, // an array of dim*dim*dim ints, 0 means blank
    unsigned int maxSols) // 0 for unlimited
  : DLX(maxSols, 1024)
{
    BOARD = dim * dim;
    BOARD2 = BOARD * BOARD;
    BOARD3 = BOARD2 * BOARD;

    Init(BOARD2*4, BOARD3, BOARD3 * 4);

    int r, c;
    int moveidx = 0;

    // Setup all possible "moves" and the conditions they affect
    int rr, cc, ss, at;
    int first  = 0 * BOARD2;
    int second = 1 * BOARD2;
    int third  = 2 * BOARD2;
    int fourth = 3 * BOARD2;

    for (int d = 1; d <= BOARD; ++d)
    {
        for (r = 1; r <= BOARD; ++r)
        {
            for (c = 1; c <= BOARD; ++c)
            {
                rr = (d - 1) * BOARD + r;
                cc = (d - 1) * BOARD + c;
                ss = (d - 1) * BOARD + (r - 1) / dim * dim + (c - 1) / dim + 1;

                at = (r - 1) * BOARD + c + first; // should be moved out of this inner loop
                rr += second;
                cc += third;
                ss += fourth;

                AddNode(at, moveidx, false);  // cell contains something
                AddNode(rr, moveidx, true);   // row r contains n
                AddNode(cc, moveidx, true);   // col c contains n
                AddNode(ss, moveidx, true);   // sector s contains n
                ++moveidx;
            }
        }
    }

    // Now apply the "moves" we already know
    if (sod != NULL) 
    {
        for (r = 0; r < BOARD; ++r)
        {
            for (c = 0; c < BOARD; ++c)
            {
                if (sod[c*BOARD + r] != 0)
                {
                    int y = ((sod[c*BOARD + r] - 1) * BOARD2 + r * BOARD + c + 1) * 4;
                    if (!Given(y))
                    {
                        isValid = false;
                        return;
                    }
                }
            }
        }
    }
    return;
}

int SudokuDLX::GetSolnDigit(int row)
{
    return GetSolutionRow(row) / BOARD2;
}

int SudokuDLX::GetSolnCol(int row)
{
    return GetSolutionRow(row) % BOARD;
}

int SudokuDLX::GetSolnRow(int row)
{
    return (GetSolutionRow(row) % BOARD2) / BOARD;
}

int SudokuDLX::doSudoku(unsigned int maxSols)
{
    int ints0[] = { // blank
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0, 
                   
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0, 
                   
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,0, 0,0,0
                 };
    int ints1[] = { 
                   5,3,0,  0,7,0, 0,0,0, 
                   6,0,0,  1,9,5, 0,0,0, 
                   0,9,8,  0,0,0, 0,6,0, 
                   
                   8,0,0,  0,6,0, 0,0,3, 
                   4,0,0,  8,0,3, 0,0,1, 
                   7,0,0,  0,2,0, 0,0,6, 
                   
                   0,6,0,  0,0,0, 2,8,0, 
                   0,0,0,  4,1,9, 0,0,5, 
                   0,0,0,  0,8,0, 0,7,9
                 };
    int ints2[] = { // this is apparently hard for brute force
                   0,0,0,  0,0,0, 0,0,0, 
                   0,0,0,  0,0,3, 0,8,5, 
                   0,0,1,  0,2,0, 0,0,0, 
                   
                   0,0,0,  5,0,7, 0,0,0, 
                   0,0,4,  0,0,0, 1,0,0, 
                   0,9,0,  0,0,0, 0,0,0, 
                   
                   5,0,0,  0,0,0, 0,7,3, 
                   0,0,2,  0,1,0, 0,0,0, 
                   0,0,0,  0,4,0, 0,0,9
                 };
    int ints3[] = { // impossible for humans, apparently
                   1,0,0,  0,0,0, 0,0,2, 
                   0,9,0,  4,0,0, 0,5,0, 
                   0,0,6,  0,0,0, 7,0,0, 
                   
                   0,5,0,  9,0,3, 0,0,0, 
                   0,0,0,  0,7,0, 0,0,0, 
                   0,0,0,  8,5,0, 0,4,0, 
                   
                   7,0,0,  0,0,0, 6,0,0, 
                   0,3,0,  0,0,9, 0,8,0, 
                   0,0,2,  0,0,0, 0,0,1
                 };
    int dim = 3;
    int dim2 = dim*dim;
    int dim3 = dim*dim*dim;
    SudokuDLX* dlx = new SudokuDLX(dim, ints3, maxSols);
    /* unsigned long long numsolns =*/ dlx->Solve();
    /* std::vector<unsigned int>& foundsolution =*/ dlx->getSolution();
    cout << endl;

    int* answer = new int[dim3*3];
    int rowsinsol = dlx->GetRowsInSolution();
    for (int i = 0; i < rowsinsol; i++)
    {
		int y = dlx->GetSolnRow(i)*dim2 + dlx->GetSolnCol(i);
        answer[y] = dlx->GetSolnDigit(i)+1;
    }
    static const int BUFSIZE = 128;
    char buf1[BUFSIZE];
    for (int c = 0; c < dim2; ++c)
    {
        for (int r = 0; r < dim2; ++r)
        {
			int y = r*dim2 + c;
			int x = answer[y];
            _itoa_s(x, buf1, BUFSIZE, 10);
            cout << buf1 << " ";
        }
        cout << endl;
    }
    cout << endl;

    delete [] answer;

    return 0;
}

