using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using CSDLX;

namespace Bedlam
{
    class Coord
    {
        private int m_x, m_y, m_z;

        public Coord (int x, int y, int z)
        {
            m_x=x;
            m_y=y;
            m_z=z;
        }

        public Coord ()
        {
            m_x = 0;
            m_y = 0;
            m_z = 0;
        }

        public int X
        {
            get { return m_x; }
            set { m_x = value; }
        }
        public int Y
        {
            get { return m_y; }
            set { m_y = value; }
        }
        public int Z
        {
            get { return m_z; }
            set { m_z = value; }
        }
    }

    class Piece
    {
        List<Coord> coords = new List<Coord>();
        private int m_minX = int.MaxValue, m_maxX = int.MinValue;
        private int m_minY = int.MaxValue, m_maxY = int.MinValue;
        private int m_minZ = int.MaxValue, m_maxZ = int.MinValue;

        public Piece()
        { }

        public Piece(Coord[] coords)
        {
            foreach (Coord c in coords)
                add(c);
        }

        public void add(Coord c)
        {
            coords.Add(c);
            if (c.X < m_minX)
                m_minX = c.X;
            if (c.X > m_maxX)
                m_maxX = c.X;
            if (c.Y < m_minY)
                m_minY = c.Y;
            if (c.Y > m_maxY)
                m_maxY = c.Y;
            if (c.Z < m_minZ)
                m_minZ = c.Z;
            if (c.Z > m_maxZ)
                m_maxZ = c.Z;
        }
        public Coord GetCoord(int index)
        {
            return coords[index];
        }

        public int maxX
        {
            get { return m_maxX;}
        }

        public int maxY
        {
            get { return m_maxY; }
        }

        public int maxZ
        {
            get { return m_maxZ; }
        }

        public int NumBlocks
        {
            get { return coords.Count; }
        }

        public Piece CopyRotate(RotationMatrix m)
        {
            Piece rotated = new Piece();
            foreach (Coord c in coords)
                rotated.add(new Coord(m.m[0, 0] * c.X +
                                      m.m[0, 1] * c.Y +
                                      m.m[0, 2] * c.Z,
                                      m.m[1, 0] * c.X +
                                      m.m[1, 1] * c.Y +
                                      m.m[1, 2] * c.Z,
                                      m.m[2, 0] * c.X +
                                      m.m[2, 1] * c.Y +
                                      m.m[2, 2] * c.Z)
                           );
            return rotated;
        }

        public Piece CopyTranslate(int xdelta, int ydelta, int zdelta)
        {
            Piece translated = new Piece();
            foreach (Coord c in coords)
                translated.add(new Coord(c.X + xdelta, c.Y + ydelta, c.Z + zdelta));

            return translated;
        }

        public void ResetOrigin()
        {
            foreach (Coord c in coords)
            {
                c.X -= m_minX;
                c.Y -= m_minY;
                c.Z -= m_minZ;
            }
            m_maxX -= m_minX;
            m_maxY -= m_minY;
            m_maxZ -= m_minZ;
            m_minX = 0;
            m_minY = 0;
            m_minZ = 0;
        }

        public ulong BinaryValue ()
        {
            ulong res = 0;

            foreach (Coord c in coords)
                res |= ((ulong)1 << c.X + 4 * c.Y + 16 * c.Z);

            return res;
        }

        public void Dump()
        {
            foreach (Coord c in coords)
                Console.WriteLine(c.X + "," + c.Y + "," + c.Z);
            Console.WriteLine("-------------");
        }
    }

    class RotationMatrix
    {
        public int[,] m;

        public RotationMatrix(int[,] a)
        { m = a; }
    }


    static class RotationMatrixes
    {
        static private RotationMatrix[] m_matrix = {
            new RotationMatrix(new int[3, 3]{{ 1, 0, 0}, { 0, 1, 0}, { 0, 0, 1}}),
            new RotationMatrix(new int[3, 3]{{ 1, 0, 0}, { 0, 0,-1}, { 0, 1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 1, 0, 0}, { 0,-1, 0}, { 0, 0,-1}}),
            new RotationMatrix(new int[3, 3]{{ 1, 0, 0}, { 0, 0, 1}, { 0,-1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0,-1, 0}, { 1, 0, 0}, { 0, 0, 1}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0, 1}, { 1, 0, 0}, { 0, 1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 1, 0}, { 1, 0, 0}, { 0, 0,-1}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0,-1}, { 1, 0, 0}, { 0,-1, 0}}),
            new RotationMatrix(new int[3, 3]{{-1, 0, 0}, { 0,-1, 0}, { 0, 0, 1}}),
            new RotationMatrix(new int[3, 3]{{-1, 0, 0}, { 0, 0,-1}, { 0,-1, 0}}),
            new RotationMatrix(new int[3, 3]{{-1, 0, 0}, { 0, 1, 0}, { 0, 0,-1}}),
            new RotationMatrix(new int[3, 3]{{-1, 0, 0}, { 0, 0, 1}, { 0, 1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 1, 0}, {-1, 0, 0}, { 0, 0, 1}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0, 1}, {-1, 0, 0}, { 0,-1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0,-1, 0}, {-1, 0, 0}, { 0, 0,-1}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0,-1}, {-1, 0, 0}, { 0, 1, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0,-1}, { 0, 1, 0}, { 1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 1, 0}, { 0, 0, 1}, { 1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0, 1}, { 0,-1, 0}, { 1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0,-1, 0}, { 0, 0,-1}, { 1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0,-1}, { 0,-1, 0}, {-1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0,-1, 0}, { 0, 0, 1}, {-1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 0, 1}, { 0, 1, 0}, {-1, 0, 0}}),
            new RotationMatrix(new int[3, 3]{{ 0, 1, 0}, { 0, 0,-1}, {-1, 0, 0}})
        };

        static public int Length 
        {
            get { return m_matrix.Length; }
        }

        static public RotationMatrix matrix(int index)
        {
            if (index < 0 || index >= m_matrix.Length)
            {
                throw (new ArgumentOutOfRangeException());
            }
            else
                return m_matrix[index];
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Piece[] pieces = new Piece[13];
            pieces[0] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(0, 0, 1), new Coord(0, 1, 0) });
            pieces[1] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1), new Coord(2, 1, 0) });
            pieces[2] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(2, 1, 0), new Coord(2, 1, 1) });
            pieces[3] = new Piece(new Coord[] { new Coord(1, 0, 0), new Coord(0, 1, 0), new Coord(1, 1, 0), new Coord(1, 2, 0), new Coord(2, 1, 0) });
            pieces[4] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1), new Coord(2, 1, 1) });
            pieces[5] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(1, 2, 0), new Coord(2, 1, 0) });
            pieces[6] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(1, 1, 1) });
            pieces[7] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(0, 0, 1) });
            pieces[8] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(0, 1, 0), new Coord(0, 1, 1) });
            pieces[9] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 0, 1), new Coord(0, 1, 0) });
            pieces[10] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(1, 1, 0), new Coord(2, 1, 0), new Coord(2, 2, 0) });
            pieces[11] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(0, 1, 0), new Coord(0, 2, 0), new Coord(0, 2, 1) });
            pieces[12] = new Piece(new Coord[] { new Coord(0, 0, 0), new Coord(1, 0, 0), new Coord(2, 0, 0), new Coord(1, 1, 0), new Coord(1, 0, 1) });

            SortedList<ulong, Piece> [] AllMoves = new SortedList<ulong, Piece>[pieces.Length];
            int piece_num = 0;
            // For each piece
            foreach (Piece p in pieces)
            {
                AllMoves[piece_num] = new SortedList<ulong, Piece>();
                // Apply all the possible rotations
                for (int m = 0; m < RotationMatrixes.Length; m++)
                {
                    Piece pnew = p.CopyRotate(RotationMatrixes.matrix(m));
                    pnew.ResetOrigin();     // Move origin to 0,0,0
                    // Apply all possible translations that remain inside boxed area
                    for (int x = 0; x < (4 - pnew.maxX); x++)
                        for (int y = 0; y < (4 - pnew.maxY); y++)
                            for (int z = 0; z < (4 - pnew.maxZ); z++)
                            {
                                Piece translated = pnew.CopyTranslate(x, y, z);
                                // Remove duplicated positions/orientations
                                if (!AllMoves[piece_num].ContainsKey(translated.BinaryValue()))
                                    AllMoves[piece_num].Add(translated.BinaryValue(), translated);
                            }
                }
                piece_num++;
            }
            for (int i = 0; i < pieces.Length; i++)
                Console.WriteLine("Piece " + i + ": Unique Positions = " + AllMoves[i].Count);
            BedlamDLX dlx = new BedlamDLX(pieces, AllMoves);
            int numsolns = dlx.Solve();

            // Code to print solution
            if (numsolns == 1)
            {
                int[, ,] Square = new int[4, 4, 4];

                for (int s = 1; s <= dlx.GetRowsInSolution(); s++)
                {
                    Console.WriteLine(dlx.GetSolutionRow(s));

                    int moveidx = 0;
                    for (int i = 0; i < AllMoves.Length; i++)
                        foreach (Piece p in AllMoves[i].Values)
                        {
                            if (dlx.GetSolutionRow(s) == moveidx)
                                for (int l = 0; l < p.NumBlocks; l++)
                                {
                                    Bedlam.Coord c = p.GetCoord(l);
                                    Square[c.X, c.Y, c.Z] = i;
                                }
                            moveidx++;
                        }
                }
                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 4; y++)
                        {
                            Console.Write((char)(Square[x, y, z] + 'A') + " ");
                        }
                        Console.WriteLine();
                    }
                    Console.WriteLine();
                }

            }
        }
    }
}
