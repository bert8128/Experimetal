using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler29
{
    public static string answer()
    {
        int lower = 2;
        int upper = 100;
        int[] primes = Utils.seive(upper);
        List<SortedDictionary<int, int> > sequences = new List<SortedDictionary<int, int>>(upper*upper);
        for (int i = lower; i <= upper; ++i)
        {
            int[] pfs = Utils.primeFactors(i, primes);
            if (pfs.Length > 0)
            {
                SortedDictionary<int, int> factors = new SortedDictionary<int, int>();
                int prime = pfs[0];
                int count = 0;
                int position = 0;
                while (position < pfs.Length)
                {
                    if (pfs[position] == prime)
                        ++count;
                    else
                    {
                        factors.Add(prime, count);
                        count = 1;
                        prime = pfs[position];
                    }
                    ++position;
                }
                factors.Add(prime, count);
                sequences.Add(factors);
            }
        }
        List<SortedDictionary<int, int>> numbers = new List<SortedDictionary<int, int>>();
        Dictionary<int, IntTree> roots = new Dictionary<int, IntTree>();
        for (int j = 0; j < sequences.Count; ++j)
        {
            SortedDictionary<int, int> factors = sequences[j];
            for (int k=lower; k<=upper; ++k)
            {
                SortedDictionary<int, int> f = new SortedDictionary<int, int>();
                foreach (KeyValuePair<int,int> kvp in factors)
                    f[kvp.Key] = kvp.Value * k;
                if (!numbers.Contains(f))
                    numbers.Add(f);
                IntTree x;
                int prime = f.Keys.First();
                if (!roots.TryGetValue(prime, out x))
                {
                    x = new IntTree();
                    roots.Add(prime, x);
                }
                x.insert(f);
            }
        }

        List<string> strings = new List<string>(upper*upper);
        foreach (KeyValuePair<int, IntTree> kvp in roots)
        {
            IntTree x = kvp.Value;
            List<string> strs = x.asStrings();
            foreach(string s in strs)
                strings.Add(s);
        }
        string ss = string.Empty;
        foreach (string s in strings)
            ss = ss + s;
        foreach (string s in strings)
        {
            Console.WriteLine(s);
        }
        List<string> strings2 = new List<string>();
        foreach (SortedDictionary<int, int> number in numbers)
        {
            string s = ("\t{ ");
            double sum = 0;
            foreach (KeyValuePair<int, int> kvp in number)
            {
                s = s + "(";
                s = s + Convert.ToString(kvp.Key);
                s = s + ",";
                s = s + Convert.ToString(kvp.Value);
                s = s + "), ";
                sum += Math.Pow(kvp.Key, kvp.Value);
            }
            s = Convert.ToString(sum) + s;
            s = s + " }\r\n";
            //Console.Write(s);
            if (!strings2.Contains(s))
                strings2.Add(s);
        }
        return Convert.ToString(strings2.Count);
    }
}