using System;
using System.Collections;
using System.Collections.Generic;

namespace CSDLX
{
    abstract class LL2DNode
    {
        private LL2DNode L;   // Pointer to left node
        private LL2DNode R;   // Pointer to right node
        private LL2DNode U;   // Pointer to node above
        private LL2DNode D;   // Pointer to node below
        public void SetLeft(LL2DNode left) { L = left; }
        public void SetRight(LL2DNode right) { R = right; }
        public void SetUp(LL2DNode up) { U = up; }
        public void SetDown(LL2DNode down) { D = down; }
        public LL2DNode GetLeft() { return L; }
        public LL2DNode GetRight() { return R; }
        public LL2DNode GetUp() { return U; }
        public LL2DNode GetDown() { return D; }
        public LL2DNode()
        {
            L = R = U = D = null;
        }
    }


    class DLXNode : LL2DNode
    {
        private DLXColumn C;	// Pointer to Column Header
        private int RowIdx;     // Index to row
        public DLXNode(DLXColumn col, int ri)
        {
            RowIdx = ri;
            C = col;
            col.GetUp().SetDown(this);
            SetUp(col.GetUp());
            SetDown(col);
            col.SetUp(this);
            col.IncSize();
        }
        public DLXColumn GetColumn() { return C; }
        public int GetRowIdx() { return RowIdx; }
    }


    class DLXColumn : LL2DNode
    {
        private int size;		// Number of items in column
        public DLXColumn()
        {
            size = 0;
            SetUp(this);
            SetDown(this);
        }
        public int GetSize() { return size; }
        public void DecSize() { size--; }
        public void IncSize() { size++; }
    }

    class DLXRow
    {
        public DLXRow(DLXNode first)
        {
            FirstNode = first;
        }

        public DLXNode FirstNode;
    }


    class DLX
    {
        private DLXColumn root = new DLXColumn();
        private DLXColumn[] ColHdrs;
        private DLXNode[] Nodes;
        private DLXRow[] Rows;
        private int numcols, numrows, numnodes;
        private DLXNode lastnodeadded;
        private ArrayList trysolution = new ArrayList();
        private ArrayList foundsolution = new ArrayList();
        private int NumSolns, NumAttempts;
        protected bool isValid = true;

        public ArrayList getSolution() { return foundsolution; }

        public DLX()
        {
        }

        public DLX(int nc, int nr, int nn)
        {
            Init(nc, nr, nn);
        }

        protected void Init(int nc, int nr, int nn)
        {
            numcols = nc;
            ColHdrs = new DLXColumn[numcols + 1];
            for (int c = 1; c <= numcols; c++)
                ColHdrs[c] = new DLXColumn();

            Nodes = new DLXNode[nn + 1];
            numnodes = 0;   // None allocated

            Rows = new DLXRow[nr + 1];
            numrows = 0;    // None allocated

            DLXColumn prev = root;
            for (int i = 1; i <= numcols; i++)
            {
                prev.SetRight(ColHdrs[i]);
                ColHdrs[i].SetLeft(prev);
                prev = ColHdrs[i];
            }
            root.SetLeft(ColHdrs[numcols]);
            ColHdrs[numcols].SetRight(root);
        }
        public int GetRowsInSolution() { return foundsolution.Count; }
        public int GetSolutionRow(int row)
        {
            return (int)foundsolution[row - 1];
        }
        private void CoverCol(DLXColumn coverCol)
        {
            LL2DNode i, j;
            coverCol.GetRight().SetLeft(coverCol.GetLeft());
            coverCol.GetLeft().SetRight(coverCol.GetRight());

            i = coverCol.GetDown();
            while (i != coverCol)
            {
                j = i.GetRight();
                while (j != i)
                {
                    j.GetDown().SetUp(j.GetUp());
                    j.GetUp().SetDown(j.GetDown());
                    ((DLXNode)j).GetColumn().DecSize();
                    j = j.GetRight();
                }
                i = i.GetDown();
            }
        }
        private void UncoverCol(DLXColumn uncoverCol)
        {
            LL2DNode i, j;

            i = uncoverCol.GetUp();
            while (i != uncoverCol)
            {
                j = i.GetLeft();
                while (j != i)
                {
                    ((DLXNode)j).GetColumn().IncSize();
                    j.GetDown().SetUp(j);
                    j.GetUp().SetDown(j);
                    j = j.GetLeft();
                }
                i = i.GetUp();
            }
            uncoverCol.GetRight().SetLeft(uncoverCol);
            uncoverCol.GetLeft().SetRight(uncoverCol);
        }
        private DLXColumn ChooseMinCol()
        {
            int minsize = System.Int32.MaxValue;
            DLXColumn search, mincol;
            int colNum = 0;

            mincol = search = (DLXColumn)root.GetRight();

            while (search != root)
            {
                if (search.GetSize() < minsize)
                {
                    mincol = search;
                    minsize = mincol.GetSize();
                    if (minsize == 0)
                    {
                        break;
                    }
                }
                search = (DLXColumn)search.GetRight();
                ++colNum;
            }
            if (minsize==0)
                return null;
            else
                return mincol;
        }

        public void AddNode(int colidx, int rowidx, bool samerow)
        {
            Nodes[++numnodes] = new DLXNode(ColHdrs[colidx], rowidx);
            if (samerow)
            {
                Nodes[numnodes].SetLeft(lastnodeadded);
                Nodes[numnodes].SetRight(lastnodeadded.GetRight());
                lastnodeadded.SetRight(Nodes[numnodes]);
                Nodes[numnodes].GetRight().SetLeft(Nodes[numnodes]);
            }
            else
            {
                Rows[++numrows] = new DLXRow(Nodes[numnodes]);
                Nodes[numnodes].SetLeft(Nodes[numnodes]);
                Nodes[numnodes].SetRight(Nodes[numnodes]);
            }
            lastnodeadded = Nodes[numnodes];
        }

        public bool GivenRow(int row)
        {
            return Given(Rows[row].FirstNode);
        }

        public bool Given(DLXNode node)
        {
            DLXNode startNode = node;
            DLXNode currNode = startNode;
            do
            {
                DLXColumn ColHdr = currNode.GetColumn();
                // Check if this is still a valid column
                if (ColHdr.GetLeft().GetRight() != ColHdr)
                    return false;
                CoverCol(ColHdr);
                currNode = (DLXNode)currNode.GetRight();
            } while (currNode != startNode);
            trysolution.Add(currNode.GetRowIdx());
            return true;
        }

        public bool Given(int node)
        {
            return Given(Nodes[node]);
        }

        DateTime start;

        public int Solve()
        {
            if (!isValid)
                return -1;

            start = DateTime.Now;
            NumSolns = 0;
            NumAttempts = 0;
            search(trysolution.Count);
            return NumSolns;
        }
        private void search(int k)
        {
            DLXColumn chosenCol;
            LL2DNode r, j;

            if (root.GetRight() == root)
            {
                foundsolution = (ArrayList) trysolution.Clone();
                NumSolns++;
                long sofar = (long)(DateTime.Now - start).TotalMilliseconds;
                Console.Write(NumSolns);
                Console.Write(" ("+sofar / NumSolns+"ms per solution; ");
                if (sofar > 0)
                    Console.WriteLine(1000 * NumSolns / sofar + " solutions/sec)");
                return;
            }
            chosenCol = ChooseMinCol();
            if (chosenCol != null) {
                CoverCol(chosenCol);
                r = chosenCol.GetDown();

                while (r != chosenCol)
                {
                    if (k >= trysolution.Count)
                        trysolution.Add(((DLXNode)r).GetRowIdx());
                    else
                        trysolution[k] = ((DLXNode)r).GetRowIdx();
                    NumAttempts++;
                    j = r.GetRight();
                    while (j != r)
                    {
                        CoverCol(((DLXNode)j).GetColumn());
                        j = j.GetRight();
                    }
                    search(k + 1);
                 //   if (NumSolns > 0)   // Stop as soon as we find 1 solution
                   //     return;
                    j = r.GetLeft();
                    while (j != r)
                    {
                        UncoverCol(((DLXNode)j).GetColumn());
                        j = j.GetLeft();
                    }
                    r = r.GetDown();
                }
                UncoverCol(chosenCol);
            }
        return;
        }
    }

    class SudokuDLX : DLX
    {
        /* this is all Steve's code, apart from the call to Init, and the comparison to 0 */
        const int dim = 3;
        int BOARD  = 0;
        int BOARD2 = 0;
        int BOARD3 = 0;
        public SudokuDLX(sbyte[,] known)
        {
            BOARD = dim * dim;
            BOARD2 = BOARD * BOARD;
            BOARD3 = BOARD2 * BOARD;
            Init(BOARD2*4, BOARD3, BOARD3 * 4);

            int d, r, c;
            int moveidx = 0;
            // Setup all possible "moves" and the conditions they affect
            for (d = 1; d <= BOARD; d++)
                for (r = 1; r <= BOARD; r++)
                    for (c = 1; c <= BOARD; c++)
                    {
                        AddNode((r - 1) * BOARD + c, moveidx, false);           // <r,c>
                        AddNode(BOARD2 + (d - 1) * BOARD + r, moveidx, true);   // <d,r>
                        AddNode(BOARD2 * 2 + (d - 1) * BOARD + c, moveidx, true);     //<d,c>
                        AddNode(BOARD2 * 3 + (d - 1) * BOARD + (r - 1) / 3 * 3 + (c - 1) / 3 + 1, moveidx, true); // <d,s>
                        moveidx++;
                    }
            // Now apply the "moves" we already know
            for (r = 0; r < 9; r++)
            {
                for (c = 0; c < 9; c++)
                {
                    if (known[c, r] != 0)
                    {
                        if (!Given(((known[c, r] - 1) * BOARD2 + r * BOARD + c + 1) * 4))
                        {
                            isValid = false;
                            return;
                        }
                    }
                }
            }
        }
        public int GetSolnDigit(int row)
        {
            return GetSolutionRow(row) / BOARD2;
        }

        public int GetSolnCol(int row)
        {
            return GetSolutionRow(row) % BOARD;
        }

        public int GetSolnRow(int row)
        {
            return (GetSolutionRow(row) % BOARD2) / BOARD;
        }
    }

    class BedlamDLX : DLX
    {
        public BedlamDLX(Bedlam.Piece[] pieces, SortedList<ulong, Bedlam.Piece>[] AllMoves)
        {
            int TotalMoves = 0;
            int TotalNodes = 0;
            for (int i = 0; i < AllMoves.Length; i++)
            {
                TotalMoves += AllMoves[i].Count;
                TotalNodes += AllMoves[i].Count * (pieces[i].NumBlocks + 1);
            }

            Init(64 + pieces.Length, TotalMoves, TotalNodes);

            int moveidx = 0;

            for (int i = 0; i < AllMoves.Length; i++)
            {
                foreach (Bedlam.Piece p in AllMoves[i].Values)
                {
                    for (int l = 0; l < p.NumBlocks; l++)
                    {
                        Bedlam.Coord c = p.GetCoord(l);
                        if (l == 0)
                            AddNode(c.X + c.Y * 4 + c.Z * 16+1, moveidx, false);
                        else
                            AddNode(c.X + c.Y * 4 + c.Z * 16+1, moveidx, true);
                    }
                    AddNode(65 + i, moveidx, true);
                    moveidx++;
                }

            }
        }
    }
}
