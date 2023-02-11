import copy
import time
from itertools import combinations 
  

size = 9
square_size = 3
do_square = True

def print_grid(g):
    for r in range(0,size):
        print(g[r])

class KillerSudokuSolver:
    def __init__(self, grid, sets, totals):

        tic = time.perf_counter()
        self.little_zeros = [0,1, 3, 6,10,15,21,28,36,45]
        self.big_zeros =    [0,9,17,24,30,35,39,42,44,45]

        all_the_options = list(range(1,10))

        self.all_combinations_tuples = []
        for i in range(0,10):
            self.all_combinations_tuples.append([])
            for j in range(0,46):
                self.all_combinations_tuples[i].append([])

        for sz in range(1,10):
            for tot in range(self.little_zeros[sz], self.big_zeros[sz]+1):
                for tpl in list(combinations(all_the_options, sz)):
                    if tot == sum(tpl):
                        self.all_combinations_tuples[sz][tot].append(tpl)

        self.all_combinations = []
        for i in range(0,10):
            self.all_combinations.append([])
            for j in range(0,46):
                self.all_combinations[i].append([])

        for sz in range(0,len(self.all_combinations_tuples)):
            for tot in range(0,len(self.all_combinations_tuples[sz])):
                if self.all_combinations_tuples[sz][tot]:
                    tpls = self.all_combinations_tuples[sz][tot]
                    lst = []
                    for i in tpls:
                        for j in i:
                            if j not in lst:
                                lst.append(j)
                    self.all_combinations[sz][tot] = lst

        self.g = grid
        self.s = sets
        self.t = totals
        self.set_list = {}
        self.lower = []
        for i in range(0,size):
            self.lower.append((i//square_size)*square_size)
        for row in range(size):
            for col in range(size):
                set = self.s[row][col]
                if set not in self.set_list:
                    self.set_list[set] = []
                self.set_list[set].append((row, col))
        for key, value in sorted(self.set_list.items()):
            print(key, value)
        self.solved_g = []

        def sort_pr_len_1(pr):
          return len(pr[1])
          
        def calc_new_cages():
            def get_cages_totally_contained(left, right, top, bottom):
                contained = []
                for set in self.set_list:
                    cells = self.set_list[set]
                    b = True
                    for cell in cells:
                        row = cell[0]
                        col = cell[1]
                        if col < left or col > right or row < top or row > bottom:
                            b = False
                            break
                    if b:
                        contained.append(set)
                return contained
                
            def cell_in_contained(row, col, contained):
                for set in contained:
                    existing_cells = self.set_list[set]
                    for cell in existing_cells:
                        if cell[0] == row and cell[1] == col:
                            return True
                return False
            new_cages = []
            def new_cage(new_cages, left, right,top, bottom):
                contained = get_cages_totally_contained(left, right, top, bottom)
                if contained:
                    total_contained = 0
                    for set in contained:
                        total_contained += self.t[set-1]
                    total_missing = 45 - total_contained
                    if total_missing != 0:
                        cells = []
                        for row in range(top, bottom+1):
                            for col in range(left, right+1):
                                if not cell_in_contained(row, col, contained):
                                    cells.append((row,col))
                        new_cages.append((total_missing, cells, left, right,top, bottom))
            for width in range(1,2): #(1,8):
                max = 9-width
                for left in range(0,max+1):
                    new_cage(new_cages, left, left+width-1, 0, 8)
                for top in range(0,max+1):
                    new_cage(new_cages, 0, 8,top, top+width-1)
            width = 3
            for left in (0, 3, 6):
                right = left + 2
                for top in (0, 3, 6):
                    bottom = top + 2
                    new_cage(new_cages, left, right, top, bottom)
            return new_cages
        self.secondary_cages = calc_new_cages()
        print('')
        for cage in self.secondary_cages:
            print(cage)
            cage_no = len(self.set_list) + 1
            self.set_list[cage_no] = cage[1]
            self.t.append(cage[0])
        print('')
        for key, value in sorted(self.set_list.items()):
            print(key, self.t[key-1], value)

        self.sl = []
        for set in self.set_list:
            self.sl.append((set,self.set_list[set]))
        self.sl.sort(key=sort_pr_len_1,reverse=False)
        print('')
        for set in self.sl:
            print(set)

        toc = time.perf_counter()
        print(f"Init took {toc - tic:0.4f} seconds")

    def possible_square(self,lower_row,lower_col,n):
        for r in range(lower_row, lower_row+square_size):
            for c in range(lower_col, lower_col+square_size):
                if self.g[r][c] == n:
                    return False
        return True

    def possible_row_and_col(self,row,col,n):
        for c in range(size):
            if self.g[row][c] == n:
                return False
        for r in range(size):
            if self.g[r][col] == n:
                return False
        return True

    def possible_set(self, row, col, n):
        set_num = self.s[row][col]
        set = self.set_list[set_num]
        total = self.t[set_num-1]
        return self.possible_set2(row, col, n, set, total)

    def possible_set2(self, row, col, n, set, total):
        tot = n
        zeros = 0
        for cell in set:
            if cell[0] != row or cell[1] != col:
                c = self.g[cell[0]][cell[1]]
                if n == c:
                    return False
                elif c == 0:
                    zeros += 1
                else:
                    tot += c
                if tot + self.little_zeros[zeros] > total:
                    return False
        if zeros == 0:
            if tot != total:
                return False
        else:
            if tot < total-self.big_zeros[zeros]:
                return False
        return True

    def solve_recursively_cage(self):
        if self.solved_g:
            return

        for st in self.sl:
            set = st[0]
            cells = self.set_list[set]
            total = self.t[set-1]
            possible_by_size_and_total = self.all_combinations[len(cells)][total]
            for cell in cells:
                row = cell[0]
                col = cell[1]
                if self.g[row][col] == 0:
                    for n in possible_by_size_and_total:
                        if self.possible_row_and_col(row,col,n):
                            if self.possible_square(self.lower[row],self.lower[col],n):
                                if self.possible_set2(row, col, n, cells, total):
                                    self.g[row][col] = n
                                    self.solve_recursively_cage()
                                    self.g[row][col] = 0
                    return
        self.solved_g = copy.deepcopy(self.g)

    def solve_recursively_cage3(self):
        if self.solved_g:
            return

        for set in self.set_list:
            cells = self.set_list[set]
            total = self.t[set-1]
            possible_by_size_and_total = self.all_combinations[len(cells)][total]
            for cell in cells:
                row = cell[0]
                col = cell[1]
                if self.g[row][col] == 0:
                    for n in possible_by_size_and_total:
                        if self.possible_row_and_col(row,col,n):
                            if self.possible_square(self.lower[row],self.lower[col],n):
                                if self.possible_set2(row, col, n, cells, total):
                                    self.g[row][col] = n
                                    self.solve_recursively_cage3()
                                    self.g[row][col] = 0
                    return
        self.solved_g = copy.deepcopy(self.g)

    def solve_recursively_cage4(self):
        if self.solved_g:
            return

        for set in self.set_list:
            cells = self.set_list[set]
            total = self.t[set-1]
            possible_tuples = self.all_combinations_tuples[len(cells)][total]
            possible_by_size_and_total = self.all_combinations[len(cells)][total]
            for cell in cells:
                row = cell[0]
                col = cell[1]
                if self.g[row][col] == 0:
                    if len(possible_tuples) < 4:
                        for tpl in possible_tuples:
                            for n in tpl:
                                if self.possible_row_and_col(row,col,n):
                                    if self.possible_square(self.lower[row],self.lower[col],n):
                                        #if self.possible_set2(row, col, n, cells, total):
                                            self.g[row][col] = n
                                            self.solve_recursively_cage4()
                                            self.g[row][col] = 0
                    else:
                        for n in possible_by_size_and_total:
                            if self.possible_row_and_col(row,col,n):
                                if self.possible_square(self.lower[row],self.lower[col],n):
                                    if self.possible_set2(row, col, n, cells, total):
                                        self.g[row][col] = n
                                        self.solve_recursively_cage4()
                                        self.g[row][col] = 0
                    return
        self.solved_g = copy.deepcopy(self.g)


    def solve_recursively_row_col(self):
        if self.solved_g:
            return
        for row in range(size):
            for col in range(size):
                if self.g[row][col] == 0:
                    for n in range(1,size+1):
                        if self.possible_row_and_col(row,col,n):
                            if self.possible_square(self.lower[row],self.lower[col],n):
                                if self.possible_set(row,col,n):
                                    self.g[row][col] = n
                                    self.solve_recursively_row_col()
                                    self.g[row][col] = 0
                    return
        self.solved_g = copy.deepcopy(self.g)

    def solve(self):
        self.solve_recursively_row_col() #solve_recursively_cage()
        return self.solved_g

grid=[
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0],
[0,0,0, 0,0,0, 0,0,0]]
"""
grid_sets=[
[ 1, 1, 7,10,10,16,19,21,21],
[ 2, 2, 7, 7,16,16,19,22,21],
[ 3, 3, 8,11,11,11,19,22,22],
[ 4, 3, 8,12,11,17,17,23,22],
[ 4, 4, 8,12,17,17,20,23,23],
[ 5, 4,12,12,14,17,20,24,23],
[ 5, 5, 9,14,14,14,20,24,24],
[ 6, 5, 9,13,13,18,18,25,25],
[ 6, 6, 9,13,15,15,18,26,26]]

sets_totals = [
8,11,18,20,26,7,
14,16,17,
11,22,24,10,
21,15,
12,21,19,
9,14,
23,17,16,14,11,9]
"""
"""
grid_sets=[
[ 1, 1, 1,10,13,15,15,15,15],
[ 2, 1,10,10,13,16,20,20,24],
[ 2, 6, 6,10,16,16,16,20,24],
[ 3, 6, 8, 8,14,17,17,17,24],
[ 3, 7, 7, 8,14,18,18,18,24],
[ 4, 7, 9, 8,14,19,19,23,23],
[ 4, 9, 9,11,11,19,23,23,25],
[ 4, 4, 9,12,12,19,21,21,25],
[ 5, 5, 5, 5,12,12,22,22,25]]

sets_totals = [
16,16,9,20,17,
7,20,
14,25,
27,17,12,
7,19,
26,21,17,12,22,
9,7,17,
16,
19,13]
"""

grid_sets=[
[ 1, 1, 8, 8,13,15,15,20,20],
[ 1, 5, 5,13,13,13,18,18,20],
[ 2, 5, 9, 9,13,16,16,18,23],
[ 2, 6, 9,12,12,12,16,21,23],
[ 6, 6, 6,12,12,12,21,21,21],
[ 3, 6,10,12,12,12,19,21,24],
[ 3, 7,10,10,14,19,19,22,24],
[ 4, 7, 7,14,14,14,22,22, 0],
[ 4, 4,11,11,14,17,17, 0, 0]]

sets_totals = [
10,14,7,14,16,
9,29,15,
16,10,11,12,
45,
26,22,
3,16,12,
23,15,
15,28,22,
10,5
]


tic = time.perf_counter()
solver = KillerSudokuSolver(grid, grid_sets, sets_totals)
soln =  solver.solve()
toc = time.perf_counter()
print(f"Took {toc - tic:0.4f} seconds")
if soln:
    print_grid(soln)
else:
    print('No Solution')

