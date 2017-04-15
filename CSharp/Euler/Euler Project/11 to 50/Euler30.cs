using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler30
{
    public static string answer()
    {
        int lower = 2;
        int upper = 350000;
        int exp = 5;
        int sum = 0;
        double[] powers = new double[10];
        for (int k = 0; k < 10; ++k)
            powers[k] = Math.Pow(k, exp);
        for (int j = lower; j < upper; ++j)
        {
            int i = j;
            double thisSum = 0;
            while (i != 0)
            {
                int index = i%10;
                thisSum += powers[index];
                i = i/10;
            }
            if (thisSum == j)
                sum += j;
        }
        return Convert.ToString(sum);
    }
}

class Euler31
{
    /*
    public static string answer()
    {
        int count = 0;
        int target = 200;
        int tlb_max = target / 200;
        int lb_max = target / 100;
        int fifty_max = target / 50;
        int twenty_max = target / 20;
        int ten_max = target / 10;
        int five_max = target / 5;
        int two_max = target / 2;
        int one_max = target / 1;
        int sum = 0;
        for (int tlb = 0; tlb <= tlb_max; ++tlb)
        {
            int this_tlb = tlb * 200;
            sum += this_tlb;
            if (sum > target)
            {
                sum -= this_tlb;
                continue;
            }
            for (int lb = 0; lb <= lb_max; ++lb)
            {
                int this_lb = lb * 100;
                sum += this_lb;
                if (sum > target)
                {
                    sum -= this_lb;
                    continue;
                }
                for (int fifty = 0; fifty <= fifty_max; ++fifty)
                {
                    int this_fifty = fifty * 50;
                    sum += this_fifty;
                    if (sum > target)
                    {
                        sum -= this_fifty;
                        continue;
                    }
                    for (int twenty = 0; twenty <= twenty_max; ++twenty)
                    {
                        int this_twenty = twenty * 20;
                        sum += this_twenty;
                        if (sum > target)
                        {
                            sum -= this_twenty;
                            continue;
                        }
                        for (int ten = 0; ten <= ten_max; ++ten)
                        {
                            int this_ten = ten * 10;
                            sum += this_ten;
                            if (sum > target)
                            {
                                sum -= this_ten;
                                continue;
                            }
                            for (int five = 0; five <= five_max; ++five)
                            {
                                int this_five = five * 5;
                                sum += this_five;
                                if (sum > target)
                                {
                                    sum -= this_five;
                                    continue;
                                }
                                for (int two = 0; two <= two_max; ++two)
                                {
                                    int this_two = two * 2;
                                    sum += this_two;
                                    if (sum > target)
                                    {
                                        sum -= this_two;
                                        continue;
                                    }
                                    for (int one = 0; one <= one_max; ++one)
                                    {
                                        sum += one;
                                        if (sum > target)
                                        {
                                            sum -= one;
                                            continue;
                                        }
                                        if (sum == target)
                                        {
                                            ++count;
                                        }
                                        sum -= one;
                                    }
                                    sum -= this_two;
                                }
                                sum -= this_five;
                            }
                            sum -= this_ten;
                        }
                        sum -= this_twenty;
                    }
                    sum -= this_fifty;
                }
                sum -= this_lb;
            }
            sum -= this_tlb;

        }
        return Convert.ToString(count);
    }
     * */

    /*
    public static string answer()
    {
        int count = 0;
        int target = 200;
        int tlb_max = target / 200;
        int lb_max = target / 100;
        int fifty_max = target / 50;
        int twenty_max = target / 20;
        int ten_max = target / 10;
        int five_max = target / 5;
        int two_max = target / 2;
        int one_max = target / 1;
        for (int tlb = 0; tlb <= tlb_max; ++tlb)
        {
            int this_tlb = tlb * 200;
            for (int lb = 0; lb <= lb_max; ++lb)
            {
                int this_lb = lb * 100;
                if (this_tlb + this_lb > target)
                    continue;
                for (int fifty = 0; fifty <= fifty_max; ++fifty)
                {
                    int this_fifty = fifty * 50;
                    if (this_tlb + this_lb + this_fifty > target)
                        continue;
                    for (int twenty = 0; twenty <= twenty_max; ++twenty)
                    {
                        int this_twenty = twenty * 20;
                        if (this_tlb + this_lb + this_fifty + this_twenty > target)
                            continue;
                        for (int ten = 0; ten <= ten_max; ++ten)
                        {
                            int this_ten = ten * 10;
                            if (this_tlb + this_lb + this_fifty + this_twenty + this_ten > target)
                                continue;
                            for (int five = 0; five <= five_max; ++five)
                            {
                                int this_five = five * 5;
                                if (this_tlb + this_lb + this_fifty + this_twenty + this_ten + this_five > target)
                                    continue;
                                for (int two = 0; two <= two_max; ++two)
                                {
                                    int this_two = two * 2;
                                    if (this_tlb + this_lb + this_fifty + this_twenty + this_ten + this_five + this_two > target)
                                        continue;
                                    for (int one = 0; one <= one_max; ++one)
                                    {
                                        if (this_tlb + this_lb + this_fifty + this_twenty + this_ten + this_five + this_two + one == target)
                                        {
                                            ++count;
                                        }
                                    }
                                }
                            }
                        }
                    }h
                }
            }
        }
        return Convert.ToString(count);
    }
     * */
    public static string answer()
    {
        int count = 0;
        for (int a = 1; a >= 0; a--)
            for (int b = (200 - (200 * a)) / 100; b >= 0; b--)
                for (int c = (200 - ((100 * b) + (200 * a))) / 50; c >= 0; c--)
                    for (int d = (200 - ((50 * c) + (100 * b) + (200 * a))) / 20; d >= 0; d--)
                        for (int e = (200 - ((20 * d) + (50 * c) + (100 * b) + (200 * a))) / 10; e >= 0; e--)
                            for (int f = (200 - ((10 * e) + (20 * d) + (50 * c) + (100 * b) + (200 * a))) / 5; f >= 0; f--)
                                for (int g = (200 - ((5 * f) + (10 * e) + (20 * d) + (50 * c) + (100 * b) + (200 * a))) / 2; g >= 0; g--)
                                {
//                                    int h = (200 - ((2 * g) + (5 * f) + (10 * e) + (20 * d) + (50 * c) + (100 * b) + (200 * a))) / 1;
                                    count++;
                                }
        return Convert.ToString(count);
    }
}


