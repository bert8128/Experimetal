using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EulerUtils;

class Euler22
{
    static int nameScore(string s)
    {
        int sum = 0;
        foreach(char c in s)
        {
            sum += c - 'A' + 1;
        }
        return sum;
    }
    public static string answer()
    {
        using (FileStream fs = File.Open("../../Euler22_names.txt", FileMode.Open))
        {
            SortedDictionary<string, int> names = new SortedDictionary<string, int>();
            string name = string.Empty;
            int b = 0;
            while (-1 != (b = fs.ReadByte()))
            {
                char c = (char)b;
                if (c == '\"')
                {
                }
                else if (c == ',')
                {
                    names.Add(name, nameScore(name));
                    name = string.Empty;
                }
                else
                {
                    name = name + c;
                }
            }
            names.Add(name, nameScore(name));
            fs.Close();

            int maxScore = 0;

            int i = 0;
            foreach( KeyValuePair<string, int> kvp in names)
            {
                ++i;
                maxScore += (kvp.Value * i);
            }
            return Convert.ToString(maxScore);
        }
    }
}