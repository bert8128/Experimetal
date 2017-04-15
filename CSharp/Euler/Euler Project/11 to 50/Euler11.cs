using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler11
{
    /* Not used anymore
        static int getMaxProd(List<int> ar, int frame)
        {
            int max = -1;
            for (int i=0; i<ar.Count()-frame; ++i)
            {
                int prod = 1;
                bool zero = false;
                for (int j=frame-1; j>=0 && !zero; --j) // reverse to find zeros faster
                {
                    if (ar[i + j] == 0)
                        zero = true;
                    else
                        prod *= ar[i+j];
                }
                if (zero)
                    i += frame;
                else if (max < prod)
                    max = prod;
               }
               return max;
        }


        static List<List<int>> getListsOfInts(int[,] grid, int _size, int frame)
        {
            List<List<int>> diagonals = new List<List<int>>((_size*2) * 6);
            // horizontal
            for (int i = 0; i < _size; ++i)
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = 0, y = i;
                while (x >= 0 && y >= 0 && x < _size && y < _size)
                    diagonal.Add(grid[x++, y]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }

            // vertical
            for (int i = 0; i < _size; ++i)
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = i, y = 0;
                while (x >= 0 && y >= 0 && x < _size && y < _size)
                    diagonal.Add(grid[x, y++]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }

            for (int i = _size - 1; i >= 0; --i)
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = 0, y = i;
                while (x >= 0 && y >= 0 && x < _size && y < _size)
                    diagonal.Add(grid[x++, y++]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }

            for (int i = 1; i < _size; ++i)// i = 1 because we already did i=0 previously
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = i, y = 0;
                while (x >= 0 && y >= 0 && x < _size && y < _size)
                    diagonal.Add(grid[x++, y++]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }

            for (int i = 0; i < _size; ++i)
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = i, y = 0;
                while (x >= 0 && y >= 0 && x < _size && y < _size)
                    diagonal.Add(grid[x--, y++]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }

            for (int i = 1; i < _size; ++i)
            {
                List<int> diagonal = new List<int>(_size*2);
                int x = _size-1, y = i;
                while (x < _size && y < _size)
                    diagonal.Add(grid[x--, y++]);
                if (diagonal.Count() >= frame)
                    diagonals.Add(diagonal);
            }
            return diagonals;
        }

     * */
    class GridPoint // a cellwith a value and pointers to its surrounding cells
    {
        public int value;
        public GridPoint S;
        public GridPoint E;
        public GridPoint SE;
        public GridPoint SW;
        public GridPoint(int x) { value = x; S = null; E = null; SE = null; SW = null; }
    }

    // create an array of GridPoints from the ints fed in
    static GridPoint[,] getGridPoints(int[,] grid)
    {
        GridPoint[,] ar = new GridPoint[grid.GetUpperBound(0), grid.GetUpperBound(1)];
        // Create the array of values
        for (int i = 0; i < grid.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < grid.GetUpperBound(1); ++j)
            {
                GridPoint gp = new GridPoint(grid[i, j]);
                ar[i, j] = gp;
            }
        }
        // Now set up the pointers to neighbouring cells
        for (int i = 0; i < ar.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < ar.GetUpperBound(1); ++j)
            {
                GridPoint gp = ar[i, j];
                if (j < ar.GetUpperBound(1) - 1)
                    ar[i, j].E = ar[i, j + 1];
                if (i < ar.GetUpperBound(0) - 1)
                {
                    ar[i, j].S = ar[i + 1, j];
                    if (j < ar.GetUpperBound(1) - 1)
                        ar[i, j].SE = ar[i + 1, j + 1];
                }
                if (j > 0)
                    ar[i, j].SW = ar[i + 1, j - 1];
            }
        }
        return ar;
    }

    // From a GridPoint, move in a particular direction
    static GridPoint moveGP(GridPoint gp, int direction)
    {
        switch (direction)
        {
            case 0:
                return gp.S;
            case 1:
                return gp.E;
            case 2:
                return gp.SE;
        }
        return gp.SW;
    }
    // Get the product from a GridPoint in a particular direction
    static int getProdCompass(GridPoint gp, int frame, int direction)
    {
        int prod = 1;
        GridPoint gp1 = gp;
        for (int i = 0; i < frame && gp1 != null; ++i)
        {
            prod *= gp1.value;
            gp1 = moveGP(gp1, direction);
        }
        return prod;
    }
    // Get the highest product in any direction from a given GridPoint
    static int getMaxProdCompass(GridPoint gp, int frame)
    {
        int maxProd = -1;
        for (int i = 0; i < 4; ++i)
        {
            int prod = getProdCompass(gp, frame, i);
            if (prod > maxProd)
                maxProd = prod;
        }
        return maxProd;
    }
    public static int answer()
    {
        // Create a list of strings from the input
        var gridStr = new List<string>(20);

        gridStr.Add("08 02 22 97 38 15 00 40 00 75 04 05 07 78 52 12 50 77 91 08 ");
        gridStr.Add("49 49 99 40 17 81 18 57 60 87 17 40 98 43 69 48 04 56 62 00 ");
        gridStr.Add("81 49 31 73 55 79 14 29 93 71 40 67 53 88 30 03 49 13 36 65 ");
        gridStr.Add("52 70 95 23 04 60 11 42 69 24 68 56 01 32 56 71 37 02 36 91 ");
        gridStr.Add("22 31 16 71 51 67 63 89 41 92 36 54 22 40 40 28 66 33 13 80 ");
        gridStr.Add("24 47 32 60 99 03 45 02 44 75 33 53 78 36 84 20 35 17 12 50 ");
        gridStr.Add("32 98 81 28 64 23 67 10 26 38 40 67 59 54 70 66 18 38 64 70 ");
        gridStr.Add("67 26 20 68 02 62 12 20 95 63 94 39 63 08 40 91 66 49 94 21 ");
        gridStr.Add("24 55 58 05 66 73 99 26 97 17 78 78 96 83 14 88 34 89 63 72 ");
        gridStr.Add("21 36 23 09 75 00 76 44 20 45 35 14 00 61 33 97 34 31 33 95 ");
        gridStr.Add("78 17 53 28 22 75 31 67 15 94 03 80 04 62 16 14 09 53 56 92 ");
        gridStr.Add("16 39 05 42 96 35 31 47 55 58 88 24 00 17 54 24 36 29 85 57 ");
        gridStr.Add("86 56 00 48 35 71 89 07 05 44 44 37 44 60 21 58 51 54 17 58 ");
        gridStr.Add("19 80 81 68 05 94 47 69 28 73 92 13 86 52 17 77 04 89 55 40 ");
        gridStr.Add("04 52 08 83 97 35 99 16 07 97 57 32 16 26 26 79 33 27 98 66 ");
        gridStr.Add("88 36 68 87 57 62 20 72 03 46 33 67 46 55 12 32 63 93 53 69 ");
        gridStr.Add("04 42 16 73 38 25 39 11 24 94 72 18 08 46 29 32 40 62 76 36 ");
        gridStr.Add("20 69 36 41 72 30 23 88 34 62 99 69 82 67 59 85 74 04 36 16 ");
        gridStr.Add("20 73 35 29 78 31 90 01 74 31 49 71 48 86 81 16 23 57 05 54 ");
        gridStr.Add("01 70 54 71 83 51 54 69 16 92 33 48 61 43 52 01 89 19 67 48 ");

        // Now convert those to numbers
        int[,] grid = new int[20, 20];

        for (int j = 0; j < 20; ++j)
        {
            for (int i = 0; i < 20; ++i)
            {
                int one = Utils.getIntForChar(gridStr[j][i * 3]);
                int two = Utils.getIntForChar(gridStr[j][i * 3 + 1]);
                grid[j, i] = one * 10 + two;
            }
        }
        int max = -1;
        int frame = 4;
        //List<List<int>> listOfIntLists = getListsOfInts(grid, 20, frame);
        //for (int i = 0; i < listOfIntLists.Count(); ++i)
        //{
        //    int maxProd = getMaxProd(listOfIntLists[i], frame);
        //    if (maxProd > max)
        //        max = maxProd;
        //}
        // Create an array of GridPoints from the original data
        GridPoint[,] ar = getGridPoints(grid);
        // Find the highest product
        for (int i = 0; i < ar.GetUpperBound(0); ++i)
        {
            for (int j = 0; j < ar.GetUpperBound(1); ++j)
            {
                GridPoint gp = ar[i, j];
                int maxProd = getMaxProdCompass(gp, frame);
                if (maxProd > max)
                    max = maxProd;
            }
        }
        return max;
    }
}