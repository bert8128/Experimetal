#ifndef SudokuDLX_H
#define SudokuDLX_H

#include "DLX.h"

class SudokuDLX : public DLX
{
public:
    SudokuDLX(int dim,   // size of sudoku (9x9=3)
              int* sod, // an array of dim*dim*dim ints, 0 means blank
              unsigned int maxSols);// 0 for unlimited
    int GetSolnDigit(int row);
    int GetSolnCol(int row);
    int GetSolnRow(int row);

    static int doSudoku(unsigned int maxSols);

private:
    int BOARD;
    int BOARD2;
    int BOARD3;
};


#endif
