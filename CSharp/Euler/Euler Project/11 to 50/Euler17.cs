using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler17
{
    static List<string> numbers;
    static List<string> tens;
    static void init()
    {
        numbers = new List<string>();
        numbers.Add("zero");
        numbers.Add("one");
        numbers.Add("two");
        numbers.Add("three");
        numbers.Add("four");
        numbers.Add("five");
        numbers.Add("six");
        numbers.Add("seven");
        numbers.Add("eight");
        numbers.Add("nine");
        numbers.Add("ten");
        numbers.Add("eleven");
        numbers.Add("twelve");
        numbers.Add("thirteen");
        numbers.Add("fourteen");
        numbers.Add("fifteen");
        numbers.Add("sixteen");
        numbers.Add("seventeen");
        numbers.Add("eighteen");
        numbers.Add("nineteen");
        tens = new List<string>();
        tens.Add("zero");
        tens.Add("ten");
        tens.Add("twenty");
        tens.Add("thirty");
        tens.Add("forty");
        tens.Add("fifty");
        tens.Add("sixty");
        tens.Add("seventy");
        tens.Add("eighty");
        tens.Add("ninety");
    }

    static string getNumberAsString(int i, int j)
    {
        string s = string.Empty;
        if (j > 0)
            s = numbers[j] + "hundred";
        if (0 < i)
        {
            if (j > 0)
                s = s + "and";
            if (0 < i && i <= 19)
            {
                s = s + numbers[i];
            }
            else if (20 <= i && i <= 99)
            {
                s = s + tens[i / 10];
                if ((i % 10) != 0)
                {
                    //s = s + " ";
                    s = s + numbers[i % 10];
                }
            }
        }
        return s;
    }
    public static string answer()
    {
        init();
        List<string> numbersTo1000 = new List<string>();
        int count = 0;
        for (int j = 0; j < 10; ++j)
        {
            for (int i = 0; i < 100; ++i)
            {
                string s = getNumberAsString(i, j);
                if (i > 0 || j > 0)
                {
                    numbersTo1000.Add(s);
                    count += s.Length;
                }
            }
        }
        string s2 = "onethousand";
        numbersTo1000.Add(s2);
        count += s2.Length;
        return Convert.ToString(count);
    }
}