public class Board
{
    public Game Game = null;

    private sbyte _id = -1;
    /// <summary>
    /// Zero-based index of the board
    /// </summary>
    public sbyte Id
    {
        get
        {
            return _id;
        }
        set
        {
            _id = value;
        }
    }

    public List<Cell> Cells = new List<Cell>();
    public List<Region> Rows = new List<Region>();
    public List<Region> Cols = new List<Region>();
    public List<Region> Sectors = new List<Region>();
    //public List<Region> Diagonals = new List<Region>(2);

    public Byte Dimension = 9;   //How many numbers (9 = default sudoku)
    public Byte Width
    {
        get
        {
            return Dimension;
        }
    }
    public Byte Height
    {
        get
        {
            return Dimension;
        }
    }
    public Byte SectorWidth = 3;
    public Byte SectorHeight = 3;

    private PencilMarkMode _PencilMarkMode = PencilMarkMode.Automatic;
    public PencilMarkMode PencilMarkMode
    {
        get { return _PencilMarkMode; }
        set
        {
            if (value != _PencilMarkMode)
            {
                _PencilMarkMode = value;
                if (_PencilMarkMode == PencilMarkMode.Automatic)
                {
                    foreach (Cell cell in Cells)
                    {
                        if (cell.Number > 0)
                            cell.AutoSet(cell.Number, true);

                    }
                }
            }
        }
    }

    public bool HasDiagonals = false;
    public bool Squiggly = false;
    public string Shape
    {
        get
        {
            StringBuilder sb = new StringBuilder(Cells.Count + 1);
            foreach (Cell c in Cells)
                sb.Append(c.Sector.Id);
            return sb.ToString();
        }
    }
    public bool HorizontalNumberSymmetry = false;
    public bool VerticalNumberSymmetry = false;

    public Difficulty Difficulty;

    private Random rnd = StaticRandom.Random;

    public Cell this[int x, int y]
    {
        get
        {
            return Cells[x * Height + y];
        }
        set
        {
            Cells[x * Height + y] = value;
        }
    }

    public Board(sbyte id, Game game)
    {
        Game = game;
        Difficulty = Difficulty.Hard;
        SetUp();
        this.Id = id;
    }

    /// <summary>
    /// Sets up an empty grid, with all references
    /// </summary>
    public void SetUp()
    {
        Rows.Clear();
        Cols.Clear();
        Sectors.Clear();
        Cells.Clear();
        Diagonals.Clear();

        for (sbyte i = 0; i < Height; i++)
            Rows.Add(new Region(this, i));
        for (sbyte i = 0; i < Width; i++)
            Cols.Add(new Region(this, i));
        for (sbyte i = 0; i < (Width / SectorWidth) * (Height / SectorHeight); i++)
            Sectors.Add(new Region(this, i));
        if (HasDiagonals)
            for (sbyte i = 0; i < 2; i++)
                Diagonals.Add(new Region(this, i));

        for (sbyte x = 0; x < Width; x++)
        {
            Region col = Cols[x];
            for (sbyte y = 0; y < Height; y++)
            {
                Region row = Rows[y];
                Region sector = Sectors[(Width / SectorWidth) * (y / SectorHeight) + (x / SectorWidth)];
                Cell cell = new Cell(this, x, y);

                cell.Row = row;
                cell.Column = col;
                cell.Sector = sector;
                if (HasDiagonals)
                {
                    if (x == y)
                        cell.Diagonal1 = Diagonals[0];
                    if (x + y == Dimension - 1)
                        cell.Diagonal2 = Diagonals[1];
                }

                if (x > 0)
                {
                    cell.Left = this[x - 1, y];
                    cell.Left.Right = cell;
                }
                if (y > 0)
                {
                    cell.Top = this[x, y - 1];
                    cell.Top.Bottom = cell;
                }

                Cells.Add(cell);
            }
        }
    }

    /// <summary>
    /// Used by 3 (X-Wing, Swordfish, Jellyfish, n-fish)
    /// WARNING: Currently only for rows and columns 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="result"></param>
    /// <param name="item"></param>
    /// <param name="lines"></param>
    /// <param name="currentDepth"></param>
    /// <param name="maxDepth"></param>
    private void SolveFishyRecursive(byte[][] pos, ref List<byte[][]> result, byte item, byte[] lines, byte currentDepth, byte maxDepth)
    {
        for (byte i = item; i < pos.Length; i++)
        {
            List<byte> newLines;
            if (lines == null)
                newLines = new List<byte>();
            else newLines = new List<byte>(lines);
            newLines.Add(i);
            if (currentDepth == maxDepth)
            {
                List<byte> posList = new List<byte>();
                foreach (byte ln in newLines)
                {
                    for (byte b = 1; b <= maxDepth; b++)
                        if ((pos[ln][b] != 0) && (!posList.Contains(pos[ln][b])))
                            posList.Add(pos[ln][b]);
                }

                if (posList.Count == maxDepth)
                {
                    //Found!
                    byte[][] _res = new byte[posList.Count][];
                    for (byte j = 0; j < _res.Length; j++)
                    {
                        _res[j] = new byte[1 + maxDepth];
                        _res[j][0] = posList[j];
                        int b = 1;
                        foreach (byte ln in newLines)
                        {
                            for (byte k = 1; k <= maxDepth; k++)
                            {
                                if (pos[ln][k] == posList[j])
                                    _res[j][b++] = pos[ln][0];
                                if (b > maxDepth)
                                    break;
                            }
                            if (b > maxDepth)
                                break;
                        }
                    }
                    result.Add(_res);
                }
            }
            else if (currentDepth < maxDepth)
            {
                SolveFishyRecursive(pos, ref result, (byte)(i + 1), newLines.ToArray(), (byte)(currentDepth + 1), maxDepth);
            }
        }
    }

    /// <summary>
    /// Fishy (X-Wing, Swordfish, Jellyfish, n-fish)
    /// WARNING: Currently only for rows and columns 
    /// </summary>
    /// <param name="lines1"></param>
    /// <param name="lines2"></param>
    /// <returns></returns>
    private bool SolveFishy(ref List<Region> lines1, ref List<Region> lines2)
    {
        bool res = false;
        //Loop fishs
        for (byte d = 2; d <= (byte)(Dimension / 2); d++) //Each big fish is complemented by a small fish. We only search for the smaller ones
        {
            //Loop numbers
            for (byte n = 1; n <= Dimension; n++)
            {
                List<Region> selectedLines = new List<Region>(Dimension);
                foreach (Region line in lines1)
                {
                    int count = 0;
                    foreach (Cell cell in line.Cells)
                    {
                        if (cell.PencilMarks[n]) count++;
                    }
                    if ((count >= 2) && (count <= d))
                        selectedLines.Add(line);
                }

                //We need at least d lines!
                if (selectedLines.Count >= d)
                {
                    byte[][] pos = new byte[selectedLines.Count][];
                    for (byte l = 0; l < selectedLines.Count; l++)
                    {
                        pos[l] = new byte[1 + d];
                        pos[l][0] = (byte)(selectedLines[l].Id + 1);
                        byte p = 1;
                        for (byte i = 0; i < selectedLines[l].Cells.Count; i++)
                        {
                            if (selectedLines[l].Cells[i].PencilMarks[n])
                            {
                                pos[l][p++] = (byte)(i + 1);
                                if (p == 1 + d)
                                    break;
                            }
                        }
                    }
                    // pos now contains line numbers and positions

                    List<byte[][]> result = new List<byte[][]>(); // P, L1, L2
                    SolveFishyRecursive(pos, ref result, 0, null, 1, d);
                    if (result.Count > 0)
                    {
                        foreach (byte[][] _res in result)
                        {
                            List<HintAction> hintActions = new List<HintAction>();
                            foreach (byte[] ret in _res)
                            {
                                Region line = lines2[ret[0] - 1];
                                for (int i = 0; i < line.Cells.Count; i++)
                                {
                                    bool onLine = false;
                                    for (int j = 1; j <= d; j++)
                                        if (i + 1 == ret[j])
                                        {
                                            onLine = true;
                                            break;
                                        }
                                    if (onLine)
                                        continue;

                                    if (line.Cells[i].PencilMarks[n])
                                    {
                                        line.Cells[i].PencilMarks.UnSet(n);
                                        res = true;
                                        if (Game.HintsEnabled)
                                            hintActions.Add(new HintAction(HintActionType.ClearPencilMark, line.Cells[i].Position, n));
                                    }
                                }
                            }
                            if (res)
                            {
                                if (Game.HintsEnabled)
                                {
                                    Hint hint = new Hint();
                                    hint.Actions.AddRange(hintActions);
                                    hint.Numbers = new byte[] { n };
                                    hint.Text = Hint.GetFishyName(d, Game.Language);

                                    for (int i = 0; i < _res.Length; i++)
                                    {
                                        hint.Highlights.Add(new Highlight(lines2[_res[i][0] - 1]));
                                    }

                                    for (int i = 0; i < _res.Length; i++)
                                    {
                                        for (int j = 1; j <= d; j++)
                                        {
                                            if (_res[i][j] != 0)
                                            {
                                                Highlight highlight = new Highlight(lines1[_res[i][j] - 1]);
                                                highlight.Color = Hint.LightColor;
                                                hint.Highlights.Add(highlight);
                                            }
                                        }
                                    }
                                    hint.BoardId = this.Id;
                                    Game.AddHint(hint);
                                }
                                return res;
                            }
                        }
                    }
                }
            }
        }
        return res;
    }

    public bool SolveVeryEasy()
    {
        // Set all single possibilities
        bool changed = false;
        foreach (Cell cell in Cells)
        {
            if (cell.SolveSinglePossibility())
            {
                changed = true;
            }
        }
        return changed;
    }

    public bool SolveEasy()
    {
        // Check if any number appears only once in any of our predefined regions
        bool changed = false;

        foreach (Region row in Rows)
            if (row.SolveSinglePosition())
                changed = true;
        foreach (Region col in Cols)
            if (col.SolveSinglePosition())
                changed = true;
        foreach (Region sector in Sectors)
            if (sector.SolveSinglePosition())
                changed = true;
        if (HasDiagonals)
            foreach (Region diagonal in Diagonals)
                if (diagonal.SolveSinglePosition())
                    changed = true;

        return changed;
    }

    public bool SolveMedium()
    {
        // Check line-sector intersections

        bool changed = false;
        foreach (Region sector in Sectors)
            if (sector.SolveAllInSectorOnSameLine())
                changed = true;
        foreach (Region row in Rows)
            if (row.SolveAllOnLineInSameSector())
                changed = true;
        foreach (Region col in Cols)
            if (col.SolveAllOnLineInSameSector())
                changed = true;
        if (HasDiagonals)
            foreach (Region diagonal in Diagonals)
                if (diagonal.SolveAllOnLineInSameSector())
                    changed = true;
        return changed;
    }

    public bool SolveHard()
    {
        bool changed = false;
        foreach (Region row in Rows)
            if (row.SolveNaked())
                changed = true;
        foreach (Region col in Cols)
            if (col.SolveNaked())
                changed = true;
        foreach (Region sector in Sectors)
            if (sector.SolveNaked())
                changed = true;
        if (HasDiagonals)
            foreach (Region diagonal in Diagonals)
                if (diagonal.SolveNaked())
                    changed = true;
        return changed;
    }

    public bool SolveHidden()
    {
        bool changed = false;
        foreach (Region row in Rows)
            if (row.SolveHidden())
                changed = true;
        foreach (Region col in Cols)
            if (col.SolveHidden())
                changed = true;
        foreach (Region sector in Sectors)
            if (sector.SolveHidden())
                changed = true;
        if (HasDiagonals)
            foreach (Region diagonal in Diagonals)
                if (diagonal.SolveHidden())
                    changed = true;
        return changed;
    }

    public bool SolveFishy()
    {
        bool changed = false;
        if (SolveFishy(ref Rows, ref Cols))
            changed = true;
        if (SolveFishy(ref Cols, ref Rows))
            changed = true;
        return changed;
    }

    public bool SolveVeryHard()
    {
        if (SolveHidden()) return true;
        if (SolveFishy()) return true;
        return false;
    }

    public bool SolveExpert()
    {
        if (SolveSimpleCoulouring()) return true;
        if (SolveMultiColouring()) return true;
        if (SolveXYWing()) return true;
        return false;
    }

    private void ConjugatePairsSearchRecursive(Cell cell, Cell parentCell, ref ColourSolver colours, int n)
    {
        byte colour = 0;
        if (parentCell == null)
            colour = 1;
        else colour = (byte)(colours.CellColour(parentCell) == 1 ? 2 : 1);
        colours.Add(cell, colour);

        foreach (Region region in cell.Regions)
        {
            int count = 0;
            Cell nextCell = null;
            foreach (Cell c in region.Cells)
            {
                if ((c != cell) && (c.PencilMarks[n]))
                {
                    if (!colours.cells.Contains(c))
                        nextCell = c;
                    count++;
                }
            }
            if ((count == 1) && (nextCell != null))
            {
                ConjugatePairsSearchRecursive(nextCell, cell, ref colours, n);
            }
        }
    }

    private class ColourSolver
    {
        public List<Cell> cells = new List<Cell>();
        public List<byte> colours = new List<byte>();
        public byte number = 0;

        public byte CellColour(Cell cell)
        {
            int id = cells.IndexOf(cell);
            if (id > -1)
                return colours[id];
            return 0;
        }

        internal void Add(Cell cell, byte colour)
        {
            cells.Add(cell);
            colours.Add(colour);
        }

        public Cell[] GetCellsWithColour(byte colour)
        {
            List<Cell> res = new List<Cell>();
            for (int i = 0; i < cells.Count; i++)
            {
                if (colours[i] == colour)
                    res.Add(cells[i]);
            }
            return res.ToArray();
        }

        public byte GetColourOfSingleCellInRegion(Region region)
        {
            for (int i = 0; i < cells.Count; i++)
            {
                Cell cell = cells[i];
                foreach (Region reg in cell.Regions)
                {
                    if (reg == region)
                    {
                        return colours[i];
                    }
                }
            }
            return 0;
        }
    }

    private bool SolveSimpleCoulouring()
    {
        bool res = false;

        List<ColourSolver> results = new List<ColourSolver>();

        for (byte n = 1; n <= Dimension; n++)
        {
            List<Cell> usedCells = new List<Cell>();
            foreach (Cell cell in Cells)
            {
                if ((!usedCells.Contains(cell)) && (cell.Number == 0) && (cell.PencilMarks[n]))
                {
                    ColourSolver colours = new ColourSolver();
                    colours.number = n;
                    ConjugatePairsSearchRecursive(cell, null, ref colours, n);
                    usedCells.AddRange(colours.cells);

                    if (colours.cells.Count > 3)
                        results.Add(colours);
                }
            }
        }

        foreach (ColourSolver solver in results)
        {
            // Type 1: Multiple cells with the same colour in the same region
            for (byte c = 1; c <= 2; c++)
            {
                Cell[] cells = solver.GetCellsWithColour(c);
                bool multiple = false;

                for (int i = 0; i < cells.Length - 1; i++)
                {
                    for (int j = i + 1; j < cells.Length; j++)
                    {
                        if (cells[i].Sees(cells[j]))
                        {
                            multiple = true;
                            break;
                        }
                    }
                    if (multiple)
                        break;
                }
                if (multiple)
                {
                    bool res2 = false;
                    for (int i = 0; i < cells.Length; i++)
                    {
                        if (cells[i].PencilMarks[solver.number])
                        {
                            cells[i].PencilMarks.UnSet(solver.number);
                            res2 = res = true;
                        }
                    }
                    if (Game.HintsEnabled && res2)
                    {
                        Hint hint = new Hint();
                        hint.Numbers = new byte[] { (byte)solver.number };
                        hint.Text = Game.Language["SimpleColouring1"];
                        for (int i = 0; i < solver.cells.Count; i++)
                        {
                            Highlight highlight = new Highlight(solver.cells[i]);
                            if (solver.colours[i] == 1)
                                highlight.Color = Hint.Chain1Dark;
                            else highlight.Color = Hint.Chain1Light;
                            if (solver.colours[i] != c)
                                hint.Actions.Add(new HintAction(HintActionType.SetNumber, solver.cells[i].Position, solver.number));
                            hint.Highlights.Add(highlight);
                        }
                        hint.BoardId = this.Id;
                        Game.AddHint(hint);
                    }
                }
            }
            // Type 2: One cell outside the colour chain sees two different colours
            bool res3 = false;
            Hint hint2 = null;
            if (Game.HintsEnabled)
                hint2 = new Hint();
            foreach (Cell cell in Cells)
            {
                if ((cell.Number == 0) && (cell.PencilMarks[solver.number]) && (!solver.cells.Contains(cell)))
                {
                    byte colour1 = 0;
                    byte colour = 0;
                    foreach (Region reg in cell.Regions)
                    {
                        if ((colour = solver.GetColourOfSingleCellInRegion(reg)) > 0)
                        {
                            if (colour1 == 0)
                            {
                                colour1 = colour;
                            }
                            else if (colour1 != colour)
                            {
                                if (cell.PencilMarks[solver.number])
                                {
                                    cell.PencilMarks.UnSet(solver.number);
                                    res3 = res = true;
                                    if (Game.HintsEnabled)
                                    {
                                        Highlight hl = new Highlight(cell);
                                        hl.Color = Hint.DarkColor;
                                        hint2.Highlights.Add(hl);
                                        hint2.Actions.Add(new HintAction(HintActionType.ClearPencilMark, cell.Position, solver.number));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (res3 && Game.HintsEnabled)
            {
                hint2.Numbers = new byte[] { (byte)solver.number };
                hint2.Text = Game.Language["SimpleColouring2"];
                Highlight highlight;
                for (int i = 0; i < solver.cells.Count; i++)
                {
                    highlight = new Highlight(solver.cells[i]);
                    if (solver.colours[i] == 1)
                        highlight.Color = Hint.Chain1Dark;
                    else highlight.Color = Hint.Chain1Light;
                    hint2.Highlights.Add(highlight);
                }
                hint2.BoardId = this.Id;
                Game.AddHint(hint2);
            }
        }
        return res;
    }

    private bool SolveXYWing()
    {
        bool res = false;
        List<Cell> pairCells = new List<Cell>();
        foreach (Cell cell in Cells)
        {
            if (cell.PencilMarks.GetList().Length == 2)
                pairCells.Add(cell);
        }

        for (int i1 = 0; i1 < pairCells.Count; i1++)
        {
            Cell cell1 = pairCells[i1];
            for (int i2 = 0; i2 < pairCells.Count; i2++)
            {
                if (i2 == i1) continue;
                Cell cell2 = pairCells[i2];
                if (!cell1.Sees(cell2)) continue;
                for (int i3 = 0; i3 < pairCells.Count; i3++)
                {
                    if ((i3 == i2) || (i3 == i1)) continue;
                    Cell cell3 = pairCells[i3];
                    if (!cell1.Sees(cell3)) continue;
                    if (cell3.Sees(cell2)) continue;

                    // [1] = AB. Is [2] = |A/B|C And [3] = |B/A|C?
                    byte[] AB = cell1.PencilMarks.GetList();
                    byte[] AC = cell2.PencilMarks.GetList();

                    byte A = 0;
                    byte C = 0;
                    byte jId = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        for (int j = 0; j < 2; j++)
                        {
                            if (AC[i] == AB[j])
                            {
                                jId = (byte)(j == 0 ? 1 : 0);
                                A = AC[i];
                                C = AC[(i == 0 ? 1 : 0)];
                                break;
                            }
                        }
                        if (A != 0)
                            break;
                    }
                    if (A == 0) continue;

                    byte[] BC = cell3.PencilMarks.GetList();
                    byte B = 0;
                    byte C2 = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        if (BC[i] == AB[jId])
                        {
                            B = BC[i];
                            C2 = BC[(i == 0 ? 1 : 0)];
                            break;
                        }
                    }
                    if (B == 0) continue;
                    if (C2 != C) continue;

                    // We now have a XY-wing, check if 
                    Hint hint = null;
                    if (Game.HintsEnabled)
                        hint = new Hint();

                    foreach (Cell cell in Cells)
                    {
                        if ((cell == cell1) || (cell == cell2) || (cell == cell3)) continue;

                        if ((cell.Sees(cell2)) && (cell.Sees(cell3)))
                        {
                            if (cell.PencilMarks[C])
                            {
                                cell.PencilMarks.UnSet(C);
                                res = true;
                                if (Game.HintsEnabled)
                                {
                                    Highlight highlight = new Highlight(cell);
                                    highlight.Color = Hint.DarkColor;
                                    hint.Highlights.Add(highlight);
                                    hint.Actions.Add(new HintAction(HintActionType.ClearPencilMark, cell.Position, C));
                                }
                            }
                        }
                    }
                    if (res)
                    {
                        if (Game.HintsEnabled)
                        {
                            Highlight highlight = new Highlight();
                            highlight.Cells.AddRange(new CellPosition[] { cell1.Position, cell2.Position, cell3.Position });
                            highlight.Color = Hint.LightColor;
                            hint.Highlights.Add(highlight);
                            hint.Numbers = new byte[] { C };
                            hint.Text = Game.Language["XYWing"];
                            hint.BoardId = this.Id;
                            Game.AddHint(hint);
                        }
                        return res;
                    }
                }
            }
        }

        return res;
    }

    private bool SolveMultiColouring()
    {
        bool res = false;

        List<ColourSolver> results = new List<ColourSolver>();
        byte[] numberStart = new byte[Dimension];
        byte[] numberCount = new byte[Dimension];

        for (byte n = 1; n <= Dimension; n++)
        {
            if (n >= 2)
            {
                numberStart[n - 1] = numberStart[n - 2];
            }
            List<Cell> usedCells = new List<Cell>();
            foreach (Cell cell in Cells)
            {
                if ((!usedCells.Contains(cell)) && (cell.Number == 0) && (cell.PencilMarks[n]))
                {
                    ColourSolver colours = new ColourSolver();
                    colours.number = n;
                    ConjugatePairsSearchRecursive(cell, null, ref colours, n);
                    usedCells.AddRange(colours.cells);

                    if (colours.cells.Count > 1)
                    {
                        results.Add(colours);
                        if (numberCount[n - 1] == 0)
                            numberStart[n - 1] = (byte)(results.Count - 1);
                        numberCount[n - 1]++;
                    }
                }
            }
        }

        for (byte n = 1; n <= Dimension; n++)
        {
            if (numberCount[n - 1] >= 2)
            {
                // Type 1
                for (int i = numberStart[n - 1]; i < numberStart[n - 1] + numberCount[n - 1] - 1; i++)
                {
                    for (int j = i + 1; j < numberStart[n - 1] + numberCount[n - 1]; j++)
                    {
                        //Look at it both ways!
                        for (int k = 0; k < 2; k++)
                        {
                            ColourSolver solver1;
                            ColourSolver solver2;
                            if (k == 0)
                            {
                                solver1 = results[i];
                                solver2 = results[j];
                            }
                            else
                            {
                                solver1 = results[j];
                                solver2 = results[i];
                            }

                            for (byte c = 0; c < 2; c++)
                            {
                                Cell[] cells = solver1.GetCellsWithColour(c);
                                byte seenColour = 0;
                                bool found = false;
                                foreach (Cell cell in cells)
                                {
                                    foreach (Region reg in cell.Regions)
                                    {
                                        byte colour = solver2.GetColourOfSingleCellInRegion(reg);
                                        if (colour != 0)
                                        {
                                            if (seenColour == 0)
                                                seenColour = colour;
                                            else if (colour != seenColour)
                                            {
                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (found)
                                        break;
                                }
                                if (found)
                                {
                                    bool res2 = false;
                                    for (int ix = 0; ix < cells.Length; ix++)
                                    {
                                        if (cells[ix].PencilMarks[solver1.number])
                                        {
                                            cells[ix].PencilMarks.UnSet(solver1.number);
                                            res2 = res = true;
                                        }
                                    }
                                    if (Game.HintsEnabled && res2)
                                    {
                                        Hint hint = new Hint();
                                        hint.Numbers = new byte[] { solver1.number };
                                        hint.Text = Game.Language["MultiColouring1"];
                                        for (int ix = 0; ix < solver1.cells.Count; ix++)
                                        {
                                            Highlight highlight = new Highlight(solver1.cells[ix]);
                                            if (solver1.colours[ix] == 1)
                                                highlight.Color = Hint.Chain1Dark;
                                            else highlight.Color = Hint.Chain1Light;
                                            if (solver1.colours[ix] != c)
                                                hint.Actions.Add(new HintAction(HintActionType.SetNumber, solver1.cells[ix].Position, solver1.number));
                                            hint.Highlights.Add(highlight);
                                        }
                                        for (int ix = 0; ix < solver2.cells.Count; ix++)
                                        {
                                            Highlight highlight = new Highlight(solver2.cells[ix]);
                                            if (solver2.colours[ix] == 1)
                                                highlight.Color = Hint.Chain2Dark;
                                            else highlight.Color = Hint.Chain2Light;
                                            hint.Highlights.Add(highlight);
                                        }
                                        hint.BoardId = this.Id;
                                        Game.AddHint(hint);
                                    }
                                }
                            }
                        }
                    }
                }
                // Type 2
                for (int i = numberStart[n - 1]; i < numberStart[n - 1] + numberCount[n - 1] - 1; i++)
                {
                    for (int j = i + 1; j < numberStart[n - 1] + numberCount[n - 1]; j++)
                    {
                        ColourSolver chain1 = results[i];
                        ColourSolver chain2 = results[j];

                        byte chain1colour1 = 0;
                        byte chain2colour1 = 0;

                        for (int ix = 0; ix < chain1.cells.Count; ix++)
                        {
                            Cell cell = chain1.cells[ix];
                            chain1colour1 = chain1.colours[ix];
                            foreach (Region reg in cell.Regions)
                            {
                                if ((chain2colour1 = chain2.GetColourOfSingleCellInRegion(reg)) > 0)
                                    break;
                            }
                            if (chain2colour1 > 0)
                                break;
                        }


                        if (chain2colour1 > 0)
                        {
                            byte chain1colour2 = (byte)(chain1colour1 == 1 ? 2 : 1);
                            byte chain2colour2 = (byte)(chain2colour1 == 1 ? 2 : 1);

                            //See that no a cells sees b cells
                            Cell[] aCells = chain1.GetCellsWithColour(chain1colour2);
                            bool okay = true;
                            foreach (Cell aCell in aCells)
                            {
                                foreach (Region reg in aCell.Regions)
                                {
                                    byte colour = chain2.GetColourOfSingleCellInRegion(reg);
                                    if (colour == chain2colour2)
                                    {
                                        okay = false;
                                        break;
                                    }
                                }
                                if (!okay)
                                    break;
                            }
                            if (okay)
                            {
                                bool res2 = false;
                                Hint hint = null;
                                if (Game.HintsEnabled)
                                    hint = new Hint();
                                foreach (Cell cell in Cells)
                                {
                                    // Find cells outside both chains
                                    if ((cell.Number == 0) && (cell.PencilMarks[n]) &&
                                        (!chain1.cells.Contains(cell)) && (!chain2.cells.Contains(cell)))
                                    {
                                        //cell sees both chain1colour2 and chain2colour2? unset!
                                        bool sees1 = false;
                                        bool sees2 = false;
                                        foreach (Region reg in cell.Regions)
                                        {
                                            if (sees1 && sees2)
                                                break;
                                            byte colour = chain1.GetColourOfSingleCellInRegion(reg);
                                            if (colour == chain1colour2)
                                            {
                                                sees1 = true;
                                                continue;
                                            }
                                            colour = chain2.GetColourOfSingleCellInRegion(reg);
                                            if (colour == chain2colour2)
                                            {
                                                sees2 = true;
                                                continue;
                                            }
                                        }
                                        if (sees1 && sees2)
                                        {
                                            if (cell.PencilMarks[n])
                                            {
                                                res2 = res = true;
                                                cell.PencilMarks.UnSet(n);
                                                if (Game.HintsEnabled)
                                                {
                                                    Highlight highlight = new Highlight(cell);
                                                    highlight.Color = Hint.DarkColor;
                                                    hint.Highlights.Add(highlight);
                                                    hint.Actions.Add(new HintAction(HintActionType.ClearPencilMark, cell.Position, n));
                                                }
                                            }
                                        }
                                    }
                                }
                                if (res2 && Game.HintsEnabled)
                                {
                                    hint.Numbers = new byte[] { n };
                                    hint.Text = Game.Language["MultiColouring2"];
                                    Highlight highlight;
                                    for (int ix = 0; ix < chain1.cells.Count; ix++)
                                    {
                                        highlight = new Highlight(chain1.cells[ix]);
                                        if (chain1.colours[ix] == 1)
                                            highlight.Color = Hint.Chain1Dark;
                                        else highlight.Color = Hint.Chain1Light;
                                        hint.Highlights.Add(highlight);
                                    }
                                    for (int ix = 0; ix < chain2.cells.Count; ix++)
                                    {
                                        highlight = new Highlight(chain2.cells[ix]);
                                        if (chain2.colours[ix] == 1)
                                            highlight.Color = Hint.Chain2Dark;
                                        else highlight.Color = Hint.Chain2Light;
                                        hint.Highlights.Add(highlight);
                                    }
                                    hint.BoardId = this.Id;
                                    Game.AddHint(hint);
                                }
                            }
                        }
                    }
                }
            }
        }

        return res;
    }

    public struct SolveBruteStruct
    {
        public bool Changed;
        public int NumSolutions;
    }

    public SolveBruteStruct SolveBruteForce(bool fillinsolution)
    {
        SolveBruteStruct result = new SolveBruteStruct();
        SudokuDLX dlx = new SudokuDLX(this);
        result.NumSolutions = dlx.Solve();
        if (result.NumSolutions == 1 && fillinsolution)
        {  // Precisely one solution
            for (int i = 1; i <= dlx.GetRowsInSolution(); i++)
            {
                Cell cell = this[dlx.GetSolnCol(i), dlx.GetSolnRow(i)];
                if (cell.Number == 0)
                {
                    cell.Number = (byte)(dlx.GetSolnDigit(i) + 1);
                    result.Changed = true;
                }
            }
        }
        else
        {
            result.Changed = false;
        }
        return result;
    }

    public bool Fill()
    {
        foreach (Cell cell in Cells)
        {
            if (cell.Number != 0)
                continue;

            List<byte> marks = new List<byte>(cell.PencilMarks.GetList());
            bool valid = false;
            int numSolutions = 0;
            do
            {
                if (marks.Count == 0)
                {
                    return false;
                }
                int i = rnd.Next(marks.Count);

                byte number = marks[i];
                cell.Number = number;
                //Log.WriteLine("Checking");
                numSolutions = SolveBruteForce(false).NumSolutions;
                //Log.WriteLine("Checked");
                valid = (numSolutions > 0);
                if (numSolutions == 1)
                    break;
                if (!valid)
                {
                    cell.Number = 0;
                    marks.RemoveAt(i);
                }
            } while (!valid);
            if (numSolutions == 1)
            {
                SolveBruteForce(true);
                break;
            }
        }
        return true;
    }

    public bool Squigglify(int depth)
    {
        List<Region> ForbiddenSectors = new List<Region>();
        foreach (Region sector in Sectors)
            if (sector.ContainsLinkedCells)
                ForbiddenSectors.Add(sector);

        for (int i = 0; i < depth; i++)
        {
            foreach (Region s1 in Sectors)
            {
                if (ForbiddenSectors.Contains(s1))
                    continue;
                Cell c1 = null;
                Cell c2 = null;
                Cell c3 = null;
                Cell c4 = null;
                Region s2 = null;
                do
                {
                    c1 = s1.Cells[rnd.Next(s1.Cells.Count)];
                    c2 = null;
                    if (c1.Left != null && c1.Left.Sector != s1)
                        c2 = c1.Left;
                    else if (c1.Top != null && c1.Top.Sector != s1)
                        c2 = c1.Top;
                    else if (c1.Right != null && c1.Right.Sector != s1)
                        c2 = c1.Right;
                    else if (c1.Bottom != null && c1.Bottom.Sector != s1)
                        c2 = c1.Bottom;
                } while ((c2 == null) || (ForbiddenSectors.Contains(c2.Sector)));
                s2 = c2.Sector;
                c2.Sector = s1;
                if (!s2.CheckConnected())
                {
                    c2.Sector = s2; //Restore
                    continue;
                }

                do
                {
                    c3 = s2.Cells[rnd.Next(s2.Cells.Count)];
                    c4 = null;
                    if (c3.Bottom != null && c3.Bottom.Sector == s1)
                        c4 = c3.Bottom;
                    else if (c3.Right != null && c3.Right.Sector == s1)
                        c4 = c3.Right;
                    else if (c3.Top != null && c3.Top.Sector == s1)
                        c4 = c3.Top;
                    else if (c3.Left != null && c3.Left.Sector == s1)
                        c4 = c3.Left;
                } while (c4 == null);
                c4.Sector = s2;
                if (!s1.CheckConnected())
                {
                    c2.Sector = s2;
                    c4.Sector = s1;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// Read squiggly sectors from string
    /// </summary>
    /// <param name="shape">The string with sector info.
    /// 000111222
    /// 000111222
    /// 000111222
    /// 333444555
    /// 333444555
    /// 333444555
    /// 666777888
    /// 666777888
    /// 666777888
    /// 
    /// represents a normal 9x9 grid
    /// </param>
    public void Squigglify(string shape)
    {
        foreach (Region s in Sectors)
            s.Cells.Clear();

        StringBuilder sh = new StringBuilder(shape);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                int sector = (int)sh[0] - 48;
                this[x, y].Sector = Sectors[sector];
                sh.Remove(0, 1);
            }
        }
    }

    public void Clear()
    {
        foreach (Cell c in Cells)
            c.Clear();
    }

    public void ClearPencilMarks()
    {
        foreach (Cell c in Cells)
            c.PencilMarks.Clear();
    }

    public void MakeStarting()
    {
        foreach (Cell c in Cells)
            if (!c.Empty)
                c.Starting = true;
    }

    public void Restart()
    {
        foreach (Cell c in Cells)
            if (!c.Starting)
                c.Number = 0;
        ClearPencilMarks();
        //Game = new Game();            
    }

    /// <summary>
    /// Check to see if all rules are followed (not necessarily that the solution is correct)
    /// </summary>
    /// <returns></returns>
    public bool RuleCheck()
    {
        foreach (Region row in Rows)
            for (int i = 0; i < Dimension - 1; i++)
                if (row.Cells[i].Number != 0)
                    for (int j = i + 1; j < Dimension; j++)
                        if (row.Cells[i].Number == row.Cells[j].Number)
                            return false;

        foreach (Region col in Cols)
            for (int i = 0; i < Dimension - 1; i++)
                if (col.Cells[i].Number != 0)
                    for (int j = i + 1; j < Dimension; j++)
                        if (col.Cells[i].Number == col.Cells[j].Number)
                            return false;

        foreach (Region sector in Sectors)
            for (int i = 0; i < Dimension - 1; i++)
                if (sector.Cells[i].Number != 0)
                    for (int j = i + 1; j < Dimension; j++)
                        if (sector.Cells[i].Number == sector.Cells[j].Number)
                            return false;

        if (HasDiagonals)
        {
            foreach (Region diag in Diagonals)
                for (int i = 0; i < Dimension - 1; i++)
                    if (diag.Cells[i].Number != 0)
                        for (int j = i + 1; j < Dimension; j++)
                            if (diag.Cells[i].Number == diag.Cells[j].Number)
                                return false;
        }

        return true;
    }

    /// <summary>
    /// Has the board been solved (all cells filled and all cells correct)
    /// </summary>
    /// <returns></returns>

    public bool NumberCompleted(byte number)
    {
        byte count = 0;
        foreach (Cell c in Cells)
        {
            if (c.Number == number)
                count++;
        }
        return (count == Dimension);
    }
}
