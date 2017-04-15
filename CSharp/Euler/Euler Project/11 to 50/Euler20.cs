using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using bignum;

public class Utils2
{
    public static byte reverse(byte xx)
    {
        uint x = xx;
        uint y = 0;
        for (uint i = 0; i < 32; ++i)
        {
            y <<= 1;
            y |= (x & 1);
            x >>= 1;
        }
        y = y >> 26;
        return (byte)y;
    }
};

public class Piece
{
    public static void test1()
    {
        {
            byte x = Convert.ToByte("100000", 2);
            byte y = Utils2.reverse(x);
            byte z = Utils2.reverse(y);
        }
        {
            byte x = Convert.ToByte("100000", 2);
            byte y = Convert.ToByte("100000", 2);
            bool b = nand(x, y);
        }
        {
            byte x = Convert.ToByte("100000", 2);
            byte y = Convert.ToByte("011111", 2);
            bool b = nand(x, y);
        }
        {
            byte x = Convert.ToByte("010000", 2);
            byte y = Convert.ToByte("001111", 2);
            bool b = nand(x, y);
        }
        {
            byte x = Convert.ToByte("000001", 2);
            byte y = Convert.ToByte("000011", 2);
            bool b = nand(x, y);
        }
    }
    public byte m_Side0;
    public byte m_Side1;
    public byte m_Side2;
    public byte m_Side3;
    public byte m_Side0r;
    public byte m_Side1r;
    public byte m_Side2r;
    public byte m_Side3r;
    //public byte m_UpDown;
    //public byte m_Rotation;
    public byte m_Piece;
    public Piece(byte piece, string side0, string side1, string side2, string side3)
    {
        m_Piece = piece;
        m_Side0 = Convert.ToByte(side0, 2);
        m_Side1 = Convert.ToByte(side1, 2);
        m_Side2 = Convert.ToByte(side2, 2);
        m_Side3 = Convert.ToByte(side3, 2);
        m_Side0r = Utils2.reverse(m_Side0);
        m_Side1r = Utils2.reverse(m_Side1);
        m_Side2r = Utils2.reverse(m_Side2);
        m_Side3r = Utils2.reverse(m_Side3);
        //m_UpDown = 0;
        //m_Rotation = 0;
    }
    //public void up() { m_UpDown = 1; }
    //public void rotate() { m_Rotation += 1; if (m_Rotation == 4) m_Rotation = 0; }
    static public bool nand(byte i, byte j)
    {
        const byte b0 = 1;
        const byte b1 = 2;
        const byte b2 = 4;
        const byte b3 = 8;
        const byte b4 = 16;
        const byte b5 = 32;

        if ((i & b0) > 0 && (j & b0) > 0) return false;
        if ((i & b1) > 0 && (j & b1) > 0) return false;
        if ((i & b2) > 0 && (j & b2) > 0) return false;
        if ((i & b3) > 0 && (j & b3) > 0) return false;
        if ((i & b4) > 0 && (j & b4) > 0) return false;
        if ((i & b5) > 0 && (j & b5) > 0) return false;
        return true;
    }
    public byte top(byte upDown, byte rotation)
    {
        switch (rotation)
        {
            case 0: return (0 == upDown) ? m_Side0 : m_Side0r;
            case 1: return (0 == upDown) ? m_Side1 : m_Side3;
            case 2: return (0 == upDown) ? m_Side2r : m_Side2;
            case 3: return (0 == upDown) ? m_Side3r : m_Side1r;
        }
        return 0;
    }
    public byte bottom(byte upDown, byte rotation)
    {
        switch (rotation)
        {
            case 0: return (0 == upDown) ? m_Side2 : m_Side2r;
            case 1: return (0 == upDown) ? m_Side3 : m_Side1;
            case 2: return (0 == upDown) ? m_Side0r : m_Side0;
            case 3: return (0 == upDown) ? m_Side1r : m_Side3r;
        }
        return 0;
    }
    public byte left(byte upDown, byte rotation)
    {
        switch (rotation)
        {
            case 0: return (0 == upDown) ? m_Side3 : m_Side1;
            case 1: return (0 == upDown) ? m_Side0r : m_Side0;
            case 2: return (0 == upDown) ? m_Side1r : m_Side3r;
            case 3: return (0 == upDown) ? m_Side2 : m_Side2r;
        }
        return 0;
    }
    public byte right(byte upDown, byte rotation)
    {
        switch (rotation)
        {
            case 0: return (0 == upDown) ? m_Side1 : m_Side3;
            case 1: return (0 == upDown) ? m_Side2r : m_Side2;
            case 2: return (0 == upDown) ? m_Side3r : m_Side1r;
            case 3: return (0 == upDown) ? m_Side0 : m_Side0r;
        }
        return 0;
    }
};

class Euler20
{
    /*static void test(byte[] pos, byte[] upDown, byte[] rotation)
    {
        string str = null;// = new string("");// = new string();
        for (int i = 0; i < 6; ++i)
            str = str + Convert.ToString(pos[i]);
        Console.WriteLine(Convert.ToString(i0) +
                            Convert.ToString(i1) +
                            Convert.ToString(i2) +
                            Convert.ToString(i3) +
                            Convert.ToString(i4) +
                            Convert.ToString(i5));
    }*/

    public enum side { top = 0, right, bottom, left };
    public static bool nand(Piece[] pieces, byte[] upDowns, byte[] rotations, byte x, byte y, side xx, side yy, bool bBackwards=false)
    {
        Piece pX = pieces[x];
        Piece pY = pieces[y];
        byte bx = 0, by = 0;
        switch (xx)
        {
            case side.top: bx = pX.top(upDowns[x], rotations[x]); break;
            case side.right: bx = pX.right(upDowns[x], rotations[x]); break;
            case side.bottom: bx = pX.bottom(upDowns[x], rotations[x]); break;
            case side.left: bx = pX.left(upDowns[x], rotations[x]); break;
        }
        switch (yy)
        {
            case side.top: by = pY.top(upDowns[y], rotations[y]); break;
            case side.right: by = pY.right(upDowns[y], rotations[y]); break;
            case side.bottom: by = pY.bottom(upDowns[y], rotations[y]); break;
            case side.left: by = pY.left(upDowns[y], rotations[y]); break;
        }
        if (bBackwards)
            bx = Utils2.reverse(bx);
        bool b = Piece.nand(bx, by);
        return b;
    }

    public static string answer()
    {
        /*
         * //BigNum answer = BigNum.factorial(100);
        //return Convert.ToString(answer.sumDigits());
        // aside- calculate some near miss fermat triples
        //int ia = 1782;// 3987;
        //int ib = 1841;// 4365;
        //int ic = 1922;// 4472;
        int ia = 3987;
        int ib = 4365;
        int ic = 4472;
        BigNum a = new BigNum(ia);
        BigNum aa = new BigNum(a);
        BigNum b = new BigNum(ib);
        BigNum bb = new BigNum(b);
        BigNum c = new BigNum(ic);
        BigNum cc = new BigNum(c);
        int pow = 11;
        for (int i = 0; i < pow; ++i)
        {
            aa = aa.mult(a);
            bb = bb.mult(b);
            cc = cc.mult(c);
        }
        Console.WriteLine(aa.asString());
        Console.WriteLine(bb.asString());
        Console.WriteLine(cc.asString());
        aa = aa.add(bb);
        Console.WriteLine(aa.asString());
        Console.ReadLine();
        return aa.asString();
         * */
        Piece.test1();
        Piece p0 = new Piece(0, "010010", "001100", "001100", "001100");
        Piece p1 = new Piece(1, "001100", "001101", "110011", "010101");
        Piece p2 = new Piece(2, "001010", "010011", "010011", "010010");
        Piece p3 = new Piece(3, "001100", "010010", "110010", "001101");
        Piece p4 = new Piece(4, "101011", "101010", "110010", "110011");
        Piece p5 = new Piece(5, "001100", "001101", "010011", "001100");
        uint count = 0;
        Piece[] pieces = new Piece[6];
        pieces[0] = p0;
        pieces[1] = p1;
        pieces[2] = p2;
        pieces[3] = p3;
        pieces[4] = p4;
        pieces[5] = p5;
        /*{
            for (byte u = 0; u < 2; ++u)
            {
                for (byte ro = 0; ro < 4; ++ro)
                {
                    byte t = p2.top(u, ro);
                    byte r = p2.right(u, ro);
                    byte b = p2.bottom(u, ro);
                    byte l = p2.left(u, ro);
                    if (u == 0)
                        Console.WriteLine("up  , ro = " + Convert.ToString(ro) + ", t/r/b/l = " + Convert.ToString(t, 2) + " " +
                                                         Convert.ToString(r, 2) + " " +
                                                         Convert.ToString(b, 2) + " " +
                                                         Convert.ToString(l, 2));
                    else
                        Console.WriteLine("down, ro = " + Convert.ToString(ro) + ", t/r/b/l = " + Convert.ToString(t, 2) + " " +
                                                         Convert.ToString(r, 2) + " " +
                                                         Convert.ToString(b, 2) + " " +
                                                         Convert.ToString(l, 2));
                }
            }
        }*/
        Piece[] pos = new Piece[6];
        byte[] upDown = new byte[6];
        byte[] rotation = new byte[6];
        for (byte i0 = 0; i0 < 1; ++i0)// piece 0 is also at position 0 - no loss of generality
        {
            for (byte u0 = 0; u0 < 1; ++u0) // coincidentally it is symmetric
            {
                for (byte r0 = 0; r0 < 1; ++r0) // keep it pointing up - no loss of generality
                {
                    pos[0] = pieces[i0];
                    upDown[0] = u0;
                    rotation[0] = r0;
                    for (byte i1 = 0; i1 < 6; ++i1)
                    {
                        if (i1 == i0)
                            continue;
                        for (byte u1 = 0; u1 < 2; ++u1)
                        {
                            for (byte r1 = 0; r1 < 4; ++r1)
                            {
                                pos[1] = pieces[i1];
                                upDown[1] = u1;
                                rotation[1] = r1;
                                // check that the rhs of the piece at pos0 fits the lhs of the piece at pos1 
                                bool b1 = nand(pos, upDown, rotation, 0, 1, side.right, side.left);
                                for (byte i2 = 0; b1 && i2 < 6; ++i2)
                                {
                                    if (i2 == i0 || i2 == i1)
                                        continue;
                                    for (byte u2 = 0; u2 < 2; ++u2)
                                    {
                                        for (byte r2 = 0; r2 < 4; ++r2)
                                        {
                                            pos[2] = pieces[i2];
                                            upDown[2] = u2;
                                            rotation[2] = r2;
                                            // check that the rhs of the piece at pos1 fits the lhs of the piece at pos2 
                                            bool b2 = nand(pos, upDown, rotation, 1, 2, side.right, side.left);
                                            for (byte i3 = 0; b2 && i3 < 6; ++i3)
                                            {
                                                if (i3 == i0 || i3 == i1 || i3 == i2)
                                                    continue;
                                                for (byte u3 = 0; u3 < 2; ++u3)
                                                {
                                                    for (byte r3 = 0; r3 < 4; ++r3)
                                                    {
                                                        pos[3] = pieces[i3];
                                                        upDown[3] = u3;
                                                        rotation[3] = r3;
                                                        // check that the rhs of the piece at pos2 fits the lhs of the piece at pos3 
                                                        bool b3 = nand(pos, upDown, rotation, 2, 3, side.right, side.left) &&
                                                            nand(pos, upDown, rotation, 3, 0, side.right, side.left);
                                                        for (byte i4 = 0; b3 && i4 < 6; ++i4)
                                                        {
                                                            if (i4 == i0 || i4 == i1 || i4 == i2 || i4 == i3)
                                                                continue;
                                                            for (byte u4 = 0; u4 < 2; ++u4)
                                                            {
                                                                for (byte r4 = 0; r4 < 4; ++r4)
                                                                {
                                                                    pos[4] = pieces[i4];
                                                                    upDown[4] = u4;
                                                                    rotation[4] = r4;
                                                                    bool b4 = nand(pos, upDown, rotation, 4, 2, side.bottom, side.top) &&
                                                                        nand(pos, upDown, rotation, 4, 1, side.left, side.top) &&
                                                                        nand(pos, upDown, rotation, 4, 3, side.right, side.top, true) &&
                                                                        nand(pos, upDown, rotation, 4, 0, side.top, side.top, true);
                                                                    for (byte i5 = 0; b4 && i5 < 6; ++i5)
                                                                    {
                                                                        if (i5 == i0 || i5 == i1 || i5 == i2 || i5 == i3 || i5 == i4)
                                                                            continue;
                                                                        for (byte u5 = 0; u5 < 2; ++u5)
                                                                        {
                                                                            for (byte r5 = 0; r5 < 4; ++r5)
                                                                            {
                                                                                pos[5] = pieces[i5];
                                                                                upDown[5] = u5;
                                                                                rotation[5] = r5;
                                                                                bool b5 = nand(pos, upDown, rotation, 5, 2, side.top, side.bottom) &&
                                                                                    nand(pos, upDown, rotation, 5, 1, side.left, side.bottom, true) &&
                                                                                    nand(pos, upDown, rotation, 5, 3, side.right, side.bottom) &&
                                                                                    nand(pos, upDown, rotation, 5, 0, side.bottom, side.bottom, true);
                                                                                if (b5)
                                                                                {
                                                                                    Console.WriteLine("Solution:");
                                                                                    for (byte x = 0; x < 6; ++x)
                                                                                    {
                                                                                        Piece p = pos[x];
                                                                                        if (upDown[x] == 0)
                                                                                            Console.WriteLine("\tPiece: " + Convert.ToString(p.m_Piece) + ", u/d = up  , rotation = " + Convert.ToString(rotation[x]));
                                                                                        else
                                                                                            Console.WriteLine("\tPiece: " + Convert.ToString(p.m_Piece) + ", u/d = down, rotation = " + Convert.ToString(rotation[x]));
                                                                                    }
                                                                                    ++count;
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return Convert.ToString(count);
    }
}