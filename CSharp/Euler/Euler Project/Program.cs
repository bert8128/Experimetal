using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using EulerUtils;
using bignum;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            double x = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            string s = System.String.Empty;
            GCHQ g = new GCHQ();
            for (int i = 0; i < 1; ++i)
            {
                s = g.answer();
            }
            sw.Stop();

            Console.WriteLine("Elapsed={0}", sw.Elapsed); 
            Console.WriteLine(s);
            Console.ReadLine();
            /*
            WebClient Client = new WebClient ();
            Client.DownloadFile("http://results.prudentialridelondon.co.uk/2014/?page=5&event=I&num_results=100&pid=list&search%5Bsex%5D=M", 
                "C:\\myfile.txt");
            Console.ReadLine();
             */
        }
    }
}
