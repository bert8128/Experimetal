using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;
using System.IO;

class Euler89
{
    static int getNumForRomanChar(char ch)
    {
        switch (ch)
        {
            case 'M':
                return 1000;
            case 'D':
                return 500;
            case 'C':
                return 100;
            case 'L':
                return 50;
            case 'X':
                return 10;
            case 'V':
                return 5;
            case 'I':
                return 1;
        }
        return 0;
    }
    static int fromRoman(string s)
    {
        if (s.Length == 1)
            return getNumForRomanChar(s[0]);

        List<string> strs = new List<string>();
        char prev = s[0];
        string current = new string(prev, 1);
        strs.Add(current);
        int currentPos = 0;
        for (int i = 1; i < s.Length; ++i)
        {
            char next = s[i];
            if (next == prev || getNumForRomanChar(prev) < getNumForRomanChar(next))
            {
                current = current + next;
            }
            else
            {
                strs[currentPos] = current;
                ++currentPos;
                current = new string(next, 1);
                strs.Add(current);
            }
            prev = next;
        }
        strs[currentPos] = current;
        int sum = 0;
        for (int i = 0; i < strs.Count; ++i)
        {
            string sub = strs[i];
            if (sub.Length != 0)
            {
                if (sub[0] == sub[sub.Length - 1])
                    sum += sub.Length * getNumForRomanChar(sub[0]);
                else
                    sum += (getNumForRomanChar(sub[sub.Length - 1]) - getNumForRomanChar(sub[0]) * (sub.Length - 1));
            }

        }
        return sum;
    }
    static string toRoman(int i)
    {
        string s = "";
        /*
        while (i >= 1000)
        {
            s = s + 'M';
            i -= 1000;
        }
        if (i >= 900)
        {
            s = s + "CM";
            i -= 900;
        }
        else if (i >= 800)
        {
            s = s + "CCM";
            i -= 800;
        }
        else if (i >= 700)
        {
            s = s + "DCC";
            i -= 700;
        }
        while (i >= 500)
        {
            s = s + 'D';
            i -= 500;
        }
        while (i >= 100)
        {
            s = s + 'C';
            i -= 100;
        }
        while (i >= 50)
        {
            s = s + 'L';
            i -= 50;
        }
        while (i >= 10)
        {
            s = s + 'X';
            i -= 10;
        }
        while (i >= 5)
        {
            s = s + 'V';
            i -= 5;
        }
        while (i >= 1)
        {
            s = s + 'I';
            i -= 1;
        }*/

        // units
        int u = i % 10;
        if (u == 1) s = s + "I";
        else if (u == 2) s = "II";
        else if (u == 3) s = "III";
        else if (u == 4) s = "IV";
        else if (u == 5) s = "V";
        else if (u == 6) s = "VI";
        else if (u == 7) s = "VII";
        else if (u == 8) s = "VIII"; // IIX ?
        else if (u == 9) s = "IX";
        // tens
        int t = i % 100;
        t /= 10;
        if (t == 1) s = "X" + s;
        else if (t == 2) s = "XX" + s;
        else if (t == 3) s = "XXX" + s;
        else if (t == 4) s = "XL" + s;
        else if (t == 5) s = "L" + s;
        else if (t == 6) s = "LX" + s;
        else if (t == 7) s = "LXX" + s;
        else if (t == 8) s = "LXXX" + s; // XXC?
        else if (t == 9) s = "XC" + s;
        // hundreds
        int h = i % 1000;
        h /= 100;
        if      (h == 1) s = "C" + s;
        else if (h == 2) s = "CC" + s;
        else if (h == 3) s = "CCC" + s;
        else if (h == 4) s = "CD" + s;
        else if (h == 5) s = "D" + s;
        else if (h == 6) s = "DC" + s;
        else if (h == 7) s = "DCC" + s;
        else if (h == 8) s = "DCCC" + s; // CCM?
        else if (h == 9) s = "CM" + s;
        int th = i / 1000;
        for (int j = 0; j < th; ++j)
            s = "M" + s;

        return s;
    }
    public static string answer()
    {
        string s = "MDCCC";
        int i = fromRoman(s);
        s = toRoman(i);
        s = toRoman(16);
        s = toRoman(49);
        int oldScore = 0;
        int newScore = 0;
        using (FileStream fs = File.Open("../../EULER_89_roman.txt", FileMode.Open))
        {
            SortedDictionary<string, int> numbers = new SortedDictionary<string, int>();
            string number = string.Empty;
            int b = 0;
            while (-1 != (b = fs.ReadByte()))
            {
                char c = (char)b;
                if (c == '\r')
                {
                }
                else if (c == '\n')
                {
                    oldScore += number.Length;
                    int n = fromRoman(number);
                    string newNumber = toRoman(n);
                    newScore += newNumber.Length;
                    number = string.Empty;
                }
                else
                {
                    number = number + c;
                }
            }
            oldScore += number.Length;
            int n1 = fromRoman(number);
            string newNumber1 = toRoman(n1);
            newScore += newNumber1.Length;
            fs.Close();
        }
            
        return Convert.ToString(oldScore - newScore);
    }
}
