using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;
using System.Collections;

class GCHQ
{
    private static void test()
    {
        /* test
        Int32 test = 22; // 10110
        Int32 num = getNum(test);
        Console.WriteLine(test.ToString() + " " + num.ToString());
        test = 14; // 1110
        num = getNum(test);
        Console.WriteLine(test.ToString() + " " + num.ToString());
        test = 27; // 11011
        num = getNum(test);
        Console.WriteLine(test.ToString() + " " + num.ToString());
         * */
    }
    private const Int32 size = 25;
    private Int32[] masks = new Int32[size];
    private Int32[] row = { 73117,
                              112211,
                              13131131,
                              13116131,
                              13152131,
                              11211,
                              7111117,
                              33,
                              123113112,
                              113211,
                              414212,
                              11111413,
                              211125,
                              322631,
                              191121,
                              212231,
                              3111151,
                              1225,
                              7121113,
                              1121221,
                              131451,
                              1313102, // ******* 10, not 1 0 = 24,502,267  Row 21
                              131166,
                              112112,
                              72125 };
    List<Int32>[] rows = new List<Int32>[size];
    private Int32[] col = { 72117,
                              112211,
                              131313131,
                              13115131,
                              13114131,
                              111211,
                              7111117,
                              113,
                              2121821,
                              22121112,
                              17321,
                              12311111,
                              41126,
                              3311131,
                              12522,
                              221111121,
                              1332181,
                              621,
                              714113,
                              11114,
                              131371,
                              131112114,
                              131433,
                              1122261,
                              713211
                          };
    List<Int32>[] cols = new List<Int32>[size];
    Tuple<Int32, Int32>[] initialBlackCells = { 
                                       new Tuple<Int32,Int32>(03,03),
                                       new Tuple<Int32,Int32>(03,04),
                                       new Tuple<Int32,Int32>(03,12),
                                       new Tuple<Int32,Int32>(03,13),
                                       new Tuple<Int32,Int32>(03,21),

                                       new Tuple<Int32,Int32>(08,06),
                                       new Tuple<Int32,Int32>(08,07),
                                       new Tuple<Int32,Int32>(08,10),
                                       new Tuple<Int32,Int32>(08,14),
                                       new Tuple<Int32,Int32>(08,15),
                                       new Tuple<Int32,Int32>(08,18),

                                       new Tuple<Int32,Int32>(16,06),
                                       new Tuple<Int32,Int32>(16,11),
                                       new Tuple<Int32,Int32>(16,16),
                                       new Tuple<Int32,Int32>(16,20),

                                       new Tuple<Int32,Int32>(21,03),
                                       new Tuple<Int32,Int32>(21,04),
                                       new Tuple<Int32,Int32>(21,09),
                                       new Tuple<Int32,Int32>(21,10),
                                       new Tuple<Int32,Int32>(21,15),
                                       new Tuple<Int32,Int32>(21,20),
                                       new Tuple<Int32,Int32>(21,21)
                                   };
    HashSet<Int32>[] blackCellsR = new HashSet<Int32>[size];
    HashSet<Int32>[] blackCellsC = new HashSet<Int32>[size];
    Int32[] cells = new Int32[size*size];
    private void addConfirmedCell(Int32 r, Int32 c, Int32 bOrW)
    {
        if (1 == bOrW)
        {
            if (null == blackCellsR[r])
                blackCellsR[r] = new HashSet<Int32>();
            blackCellsR[r].Add(c);

            if (null == blackCellsC[c])
                blackCellsC[c] = new HashSet<Int32>();
            blackCellsC[c].Add(r);
        }

        cells[r * size + c] = bOrW;
    }
    Dictionary<Int32, Int32> rowHash = new Dictionary<Int32, Int32>();
    Dictionary<Int32, Int32> colHash = new Dictionary<Int32, Int32>();
    private void init()
    {
        for (int i = 0; i < size*size; ++i)
        {
            cells[i] = 0;
        }
        for (int i = 0; i < size; ++i)
        {
            rowHash.Add(row[i], i);
            colHash.Add(col[i], i);
        }
        for (int i = 0; i < size; ++i)
        {
            rows[i] = new List<Int32>();
            cols[i] = new List<Int32>();
        }

        Int32 mask = 1;
        for (Int32 i = 0; i < size; ++i)
        {
            masks[i] = mask;
            mask = mask << 1;
        }

        for (Int32 i = 0; i < initialBlackCells.Length; ++i)
            addConfirmedCell(initialBlackCells[i].Item1, initialBlackCells[i].Item2, 1);

        // check output
        {
            for (Int32 i = 0; i < size; ++i)
            {
                if (null != blackCellsR[i])
                {
                    Console.Write("{0}", i);
                    foreach (var item in blackCellsR[i])
                        Console.Write(" {0}", item);
                    Console.WriteLine(" ");
                }
            }
        }
        Console.WriteLine(" ");
        {
            for (Int32 i = 0; i < size; ++i)
            {
                if (null != blackCellsC[i])
                {
                    Console.Write("{0}", i);
                    foreach (var item in blackCellsC[i])
                        Console.Write(" {0}", item);
                    Console.WriteLine(" ");
                }
            }
        }
        Console.WriteLine(" ");
    }
    private void addRow(Int32 num, Int32 index)
    {
    }
    private void findAndAdd(Int32 inp, Int32 num, int[] rca, List<int>[] rcl, HashSet<int>[] hsi, Dictionary<Int32, Int32> hash, bool bRow)
    {
        if (!hash.ContainsKey(num))
            return;
        Int32 i = hash[num]; // row or column number
        bool ok = true;
        //if (null != hsi[i])
        {
            /*
            foreach (var item in hsi[i])
            {
                Int32 mask = masks[size - item - 1];
                if ((inp & mask) == 0)
                {
                    ok = false;
                    break;
                }
            }
            */
            for (int j = 0; j < size; ++j)
            {
                Int32 c = (bRow) ? cells[i * 25 + j] : cells[j * 25 + i];
                if (c != 0) // 0 means unitialised
                {
                    Int32 mask = masks[size - j - 1];
                    Int32 x = inp & mask;
                    Int32 y = c & mask;
                    if (
                        (x != 0 && c < 0) // black and white
                        || (x == 0 && c > 0) // white and black
                       )
                    {
                        ok = false;
                        break;
                    }
                }
            }
        }
        if (ok)
            rcl[i].Add(inp);
    }
    private void findAndAddRow(Int32 inp, Int32 num)
    {
        findAndAdd(inp, num, row, rows, blackCellsR, rowHash, true);
    }
    private void findAndAddCol(Int32 inp, Int32 num)
    {
        findAndAdd(inp, num, col, cols, blackCellsC, colHash, false);
    }
    private Int32 compact()
    {
        for (int i = 0; i < size; ++i)
        {
            List<Int32> r = rows[i];
            if (r.Count != 1)
            {
                List<Int32> r2 = new List<Int32>();
                foreach (var item in r)
                {
                    bool bAdd = true;
                    for (int j = 0; j < size; ++j)
                    {
                        Int32 c = cells[size * i + j];
                        if (c != 0)// uninitialised
                        {
                            Int32 bOrW = item & masks[size - j - 1];
                            if ((bOrW == 0 && c == 1) || // white and black
                                (bOrW != 0 && c == -1)) // black and white
                            {
                                bAdd = false;
                                break;
                            }
                        }
                    }
                    if (bAdd)
                        r2.Add(item);
                }
                rows[i] = r2;
            }
            List<Int32> c1 = cols[i];
            if (c1.Count != 1)
            {
                List<Int32> c2 = new List<Int32>();
                foreach (var item in c1)
                {
                    bool bAdd = true;
                    for (int j = 0; j < size; ++j)
                    {
                        Int32 c = cells[size * j + i];
                        if (c != 0)// uninitialised
                        {
                            Int32 bOrW = item & masks[size - j - 1];
                            if ((bOrW == 0 && c == 1) || // white and black
                                (bOrW != 0 && c == -1)) // black and white
                            {
                                bAdd = false;
                                break;
                            }
                        }
                    }
                    if (bAdd)
                        c2.Add(item);
                }
                cols[i] = c2;
            }
        }
        for (int i = 0; i < size; ++i)
        {
            List<Int32> r = rows[i];
            if (r.Count == 1)
            {
                foreach (var item in r) // there's only one
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if ((item & masks[size - j - 1]) != 0)
                            addConfirmedCell(i, j, 1);
                        else
                            addConfirmedCell(i, j, -1);
                    }
                }
            }
            List<Int32> c = cols[i];
            if (c.Count == 1)
            {
                foreach (var item in c) // there's only one
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if ((item & masks[size - j - 1]) != 0)
                            addConfirmedCell(j, i, 1);
                        else
                            addConfirmedCell(j, i, -1);
                    }
                }
            }
        }
        Int32 count2 = 0;
        for (int i = 0; i < size; ++i)
        {
            List<Int32> r = rows[i];
            List<Int32> c = cols[i];
            Console.WriteLine("{0} {1} {2}", i, r.Count, c.Count);
            count2 += r.Count;
            count2 += c.Count;
        }
        return count2;
    }
    private Int32 getNum(Int32 bits)
    {
        Int32 count = 0;
        Int32 last = 0;
        Int32 num = 0;
        for (Int32 i = size - 1; i >= 0; --i)
        {
            Int32 res = bits & masks[i];
            if (res == 0)
            {
                if (last != 0)
                    num *= 10;
                num += count;
                count = 0;
            }
            else
            {
                ++count;
                if (count > 9)
                    return 0;
            }
            last = res;
        }
        {
            if (last != 0)
                num *= 10;
            num += count;
            count = 0;
        }
        return num;
    }
    public string answer()
    {
        init();
        Int32 max = (Int32)Math.Pow(2, size);
        for (Int32 i = 0; i < max; ++i)
        {
            Int32 num = getNum(i);
            if (0 != num)
            {
                findAndAddRow(i, num);
                findAndAddCol(i, num);
            };
        }
        rows[21].Clear();
        rows[21].Add(24502267);

        Int64 count;
        count = 0;
        for (int i = 0; i < size; ++i)
        {
            List<Int32> r = rows[i];
            List<Int32> c = cols[i];
            Console.WriteLine("{0} {1} {2}", i, r.Count, c.Count);
            count += r.Count;
            count += c.Count;
        }

        for (int i=0; i<size; ++i)
        {
            List<Int32> r = rows[i];
            if (r.Count == 1)
            {
                foreach (var item in r) // there's only one
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if ((item & masks[size - j - 1]) != 0)
                            addConfirmedCell(i, j, 1);
                        else
                            addConfirmedCell(i, j, -1);
                    }
                }
            }
            List<Int32> c = cols[i];
            if (c.Count == 1)
            {
                foreach (var item in c) // there's only one
                {
                    for (int j = 0; j < size; ++j)
                    {
                        if ((item & masks[size - j - 1]) != 0)
                            addConfirmedCell(j, i, 1);
                        else
                            addConfirmedCell(j, i, -1);
                    }
                }
            }
        }

        Console.WriteLine("    0123456789012345678901234");
        for (int i = 0; i < size; ++i)
        {
            if (i<10)
                Console.Write(" {0}: ", i);
            else
                Console.Write("{0}: ", i);
            HashSet<Int32> r = blackCellsR[i];
            if (null != r)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (r.Contains(j))
                        Console.Write("X");
                    else
                        Console.Write(".");
                }
            }
            Console.WriteLine(" ");
        }
        Console.WriteLine(" ");

/*        Console.WriteLine("    0123456789012345678901234");
        for (int i = 0; i < size; ++i)
        {
            if (i < 10)
                Console.Write(" {0}: ", i);
            else
                Console.Write("{0}: ", i);
            HashSet<Int32> c = blackCellsC[i];
            if (null != c)
            {
                for (int j = 0; j < size; ++j)
                {
                    if (c.Contains(j))
                        Console.Write("X");
                    else
                        Console.Write(".");
                }
            }
            Console.WriteLine(" ");
        }
        Console.WriteLine(" ");
  */
        while (true)
        {
            Int32 count2 = compact();
            if (count2 == count)
            {
                int edge = 100;
                for (int i = 0; i < size; ++i)
                {
                    List<Int32> r = rows[i];
                    if (r.Count < edge)
                    {
                        for (int j = 0; j < size; ++j)
                        {
                            if (cells[i * size + j] != 0)
                                continue;
                            Int32 x = 0;
                            foreach (var item in r)
                            {
                                Int32 thisOne = item & masks[size - j - 1];
                                if (x == 0)
                                {
                                    if (thisOne == 0)
                                        x = -1;
                                    else
                                        x = 1;
                                }
                                else
                                {
                                    if (thisOne == 0 && x == 1 || thisOne != 0 && x == -1)
                                    {
                                        x = 0;
                                        break;
                                    }
                                }
                            }
                            if (x != 0)
                                addConfirmedCell(i, j, x);
                        }
                    }
                    List<Int32> c = cols[i];
                    if (c.Count < edge)
                    {
                        for (int j = 0; j < size; ++j)
                        {
                            if (cells[j * size + i] != 0)
                                continue;
                            Int32 x = 0;
                            foreach (var item in c)
                            {
                                Int32 thisOne = item & masks[size - j - 1];
                                if (x == 0)
                                {
                                    if (thisOne == 0)
                                        x = -1;
                                    else
                                        x = 1;
                                }
                                else
                                {
                                    if (thisOne == 0 && x == 1 || thisOne != 0 && x == -1)
                                    {
                                        x = 0;
                                        break;
                                    }
                                }
                            }
                            if (x != 0)
                                addConfirmedCell(j, i, x);
                        }
                    }
                }
                count2 = compact();
            }
            Console.WriteLine("    0123456789012345678901234");
            for (int i = 0; i < size; ++i)
            {
                if (i < 10)
                    Console.Write(" {0}: ", i);
                else
                    Console.Write("{0}: ", i);
                for (int j = 0; j<size; ++j)
                {
                    int c = cells[i*size+j];
                    if (c == 1)
                        Console.Write("X");
                    else if (c == -1)
                        Console.Write(" ");
                    else
                        Console.Write(".");
                }
                Console.WriteLine(" ");
            }
            Console.WriteLine(" ");
            if (count2 == count)
                break;
            count = count2;
        }

        //Console.ReadLine();
        return count.ToString();
    }
}