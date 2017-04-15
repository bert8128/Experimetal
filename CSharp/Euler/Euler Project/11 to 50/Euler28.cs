using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EulerUtils;

class Euler28
{
    public static string answer()
    {
        int max = 1001;
        int[,] grid = new int[max, max];
        int x = max-1;
        int y = max-1;
        int dx = -1;
        int dy = 0;
        int upper1 = max-1;
        int upper2 = max*max;
        int lastSwitch = 0;
        grid[upper1, upper1] = upper2;
        int sumdiags = upper2;
        for (int n=upper2-1; n>0; --n)
        {
            if (x + dx > upper1 || x + dx < 0 || y + dy > upper1 || y + dy < 0 || grid[x + dx, y + dy] != 0)
            {
                switch (lastSwitch)
                {
                    case 0:
                        {
                            sumdiags += n+1;
                            dx = 0;
                            dy = -1;
                            break;
                        }
                    case 1:
                        {
                            sumdiags += n+1;
                            dx = 1;
                            dy = 0;
                            break;
                        }
                    case 2:
                        {
                            sumdiags += n+1;
                            dx = 0;
                            dy = 1;
                            break;
                        }
                    case 3:
                        {
                            sumdiags += n;
                            dx = -1;
                            dy = 0;
                            break;
                        }
                }
                ++lastSwitch;
                if (lastSwitch > 3)
                    lastSwitch = 0;
            }
            x += dx;
            y += dy;
            grid[x, y] = n;
        }

        return Convert.ToString(sumdiags);
    }
}