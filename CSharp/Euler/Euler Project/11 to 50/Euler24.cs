using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler24
{
    public static string answer()
    {
        List<string> strings = new List<string>(10 * 9 * 8 * 7 * 6 * 5 * 4 * 3 * 2 * 1);
        char[] digits = new char[] { '0', '1', '2', '3','4', '5','6', '7', '8', '9' };
        string s = System.String.Empty;
        Permute.permute(digits, strings);
        string s1 = strings[1000000-1];
        return s1;
    }
}