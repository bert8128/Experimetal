using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using EulerUtils;

class Euler42
{
    public static string answer()
    {
        using (FileStream fs = File.Open("../../Euler42_words.txt", FileMode.Open))
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
                    names.Add(name, Utils.wordScore(name));
                    name = string.Empty;
                }
                else
                {
                    name = name + c;
                }
            }
            names.Add(name, Utils.wordScore(name));
            fs.Close();

            int maxScore = 0;

            int i = 0;
            foreach( KeyValuePair<string, int> kvp in names)
            {
                ++i;
                double x = Math.Sqrt(0.25 + 2 * kvp.Value) - 0.5;
                double xx = Math.Round(x);
                if (x == xx)
                {
                    int n = (int)xx;
                    n = (n * (n + 1));
                    if (n % 2 == 0 && n/2 == kvp.Value)
                        maxScore++;
                }
            }
            return Convert.ToString(maxScore);
        }
    }
}