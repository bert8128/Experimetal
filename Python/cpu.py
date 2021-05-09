'''

An 8-bit CPU emulator

'''

import tkinter as tk
from tkinter.scrolledtext import ScrolledText
import tkinter.font
from tkinter import messagebox
import string
import time

MAX_MEMORY = 256

def numToStr(mem_mode, num):
    if mem_mode == 'D':
        return str(num)
    if mem_mode == 'B':
        return format(num, '08b')
    if mem_mode == 'H':
        return f'{num:0>2X}'
    if mem_mode == 'HH':
        return f'{num:X}'
    if mem_mode == 'A':
        if num >= 33:
            return chr(num)
        return '..'

class Instruction():
    def __init__(self, line, opcode, dest ,src):
        self.line = line
        self.opcode = opcode
        self.dest = dest
        self.src = src

    def __repr__(self): 
        return "%s %s %s %s" % (self.line, self.opcode, self.dest, self.src) 
    
    def __str__(self): 
        return "%s %s %s %s" % (self.line, self.opcode, self.dest, self.src) 

class Emulator():
    def __init__(self):
        self.memory = [0] * MAX_MEMORY
        self.pc = 0
        self.registers = [0,0,0]
        self.sp = 255
        self.op = 0
        self.halted = False
        self.program = []

        self.instructions_table = [
                        #  opcode, mnemonic, dest, src
                         ( 0x0,  'NoOp', '0', '0')
                        ,( 0x1,  'CpAI', 'a', 'v')
                        ,( 0x2,  'CpAR', 'a', 'r')
                        ,( 0x3,  'CpRA', 'r', 'a')
                        ,( 0x4,  'CpRI', 'r', 'v')
                        ,( 0x5,  'CpRR', 'r', 'r')
                        ,( 0x6,  'AddR', 'r', 'r')
                        ,( 0x7,  'SubR', 'r', 'r')
                        ,( 0x8,  'Jump', 'd', '0')
                        ,( 0x9,  'Jmpz', 'd', 'r')
                        ,( 0xA,  'Jmpn', 'd', 'r')
                        ,( 0xB,  'JmpR', 'r', '0')
                        ,( 0xC,  'And' , 'r', 'r')
                        ,( 0xD,  'Or'  , 'r', 'r')
                        ,( 0xE,  'Not' , 'r', '0')
                        ,( 0xF,  'Xor' , 'r', 'r')
                        ,( 0x10, 'Prnt', 'r', '0')
                        ,( 0x11, 'Push', 'r', '0')
                        ,( 0x12, 'Pop' , 'r', '0')
                        ,( 0xFF, 'Halt', '0', '0')
                        ]
        self.opcodes = []
        self.opcodes_to_machine_codes = {}
        for inx in self.instructions_table:
            self.opcodes.append(inx[1])
            self.opcodes_to_machine_codes[inx[1]] = inx[0]

        self.instructions = [
                         'Allinstructions are three bytes:'
                        ,'    Opcode, Destination, Source'
                        ,'Operands types are:'
                        ,'    A : Address'
                        ,'    I : Immediate'
                        ,'    R : Register (0, 1, 2)'
                        ,'    0 : Unnecessary, ignored'
                        ,'Addresses must be in hex'
                        ]
        for inx in self.instructions_table:
            self.instructions.append( ' '.join([numToStr('H', inx[0]), inx[1].ljust(4), inx[2], inx[3]]))

    def reset(self):
        for i in range(len(self.memory)):
            self.memory[i] = 0
        self.pc = 0
        self.registers = [0,0,0]
        self.sp = 255
        self.op = 0
        self.halted = False

    def noop(self, operand1, operand2):
        self.pc += 3
        return [[], []]

    def cpai(self, operand1, operand2):
        self.pc += 3
        self.memory[operand1] = operand2
        return [[], [operand1]]

    def cpar(self, operand1, operand2):
        self.pc += 3
        if operand2 < 0 or operand2 > 2:
            return [[], []]
        self.memory[operand1] = self.registers[operand2]
        return [[], [operand1]]

    def cpra(self, operand1, operand2):
        self.pc += 3
        if operand1 < 0 or operand1 > 2:
            return [[], []]
        self.registers[operand1] = self.memory[operand2]
        return [[operand1]], [[]]

    def cpri(self, operand1, operand2):
        self.pc += 3
        if operand1 < 0 or operand1 > 2:
            return [[], []]
        self.registers[operand1] = operand2
        return [[operand1], []]

    def cprr(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2) or (operand1 == operand2):
            return [[], []]

        self.registers[operand1] = self.registers[operand2]
        return [[operand1], []]

    def addr(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2):
            return [[], []]

        self.registers[operand1] += self.registers[operand2]
        if self.registers[operand1] > 255:
            self.registers[operand1] = self.registers[operand1] - 255
        return [[operand1], []]

    def subr(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2):
            return [[], []]

        self.registers[operand1] -= self.registers[operand2]
        return [[operand1], []]

    def jump(self, operand1, operand2):
        self.pc = operand1
        return [[], []]

    def jmpz(self, operand1, operand2):
        if operand2 < 0 or operand2 > 2:
            self.pc += 3
            return [[], []]

        if self.registers[operand2] == 0:
            self.pc = operand1
            return [[], []]

        self.pc += 3
        return [[], []]

    def jmpn(self, operand1, operand2):
        if operand2 < 0 or operand2 > 2:
            self.pc += 3
            return [[], []]

        if self.registers[operand2] != 0:
            self.pc = operand1
            return [[], []]

        self.pc += 3
        return [[], []]

    def jmpr(self, operand1, operand2):
        if operand1 < 0 or operand1 > 2:
            self.pc += 3
            return [[], []]

        self.pc = self.registers[operand1]
        return [[], []]

    def andd(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2):
            return [[], []]

        self.registers[operand1] &= self.registers[operand2]
        return [[operand1], []]

    def orr(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2):
            return [[], []]

        self.registers[operand1] |= self.registers[operand2]
        return [[operand1], []]

    def nott(self, operand1, operand2):
        self.pc += 3

        if operand1 < 0 or operand1 > 2:
            return [[], []]

        self.registers[operand1] = ~self.registers[operand1]
        return [[operand1], []]

    def xor(self, operand1, operand2):
        self.pc += 3

        if (operand1 < 0 or operand1 > 2) or (operand2 < 0 or operand2 > 2):
            return [[], []]

        self.registers[operand1] ^= self.registers[operand2]
        return [[operand1], []]

    def prnt(self, operand1, operand2):
        self.pc += 3

        if operand1 < 0 or operand1 > 2:
            return [[], []]

        self.op = self.registers[operand1]
        return [[-1], []]

    def push(self, operand1, operand2):
        self.pc += 3

        if operand1 < 0 or operand1 > 2:
            return [[], []]

        self.memory[self.sp] = self.registers[operand1]
        self.sp -= 1
        return [[], [self.sp]]

    def pop(self, operand1, operand2):
        self.pc += 3

        if operand1 < 0 or operand1 > 2:
            return [[], []]

        self.sp += 1
        self.registers[operand1] = self.memory[self.sp]
        return [[operand1], []]

    def halt(self, operand1, operand2):
        self.halted = True
        return [[], []]

    def step(self):
        ''' returns a list ofchanged registers (not including sp or pc) and a list of changed memory locations '''
        if self.halted:
            return None
        opcode   = self.memory[self.pc]
        operand1 = self.memory[self.pc+1]
        operand2 = self.memory[self.pc+2]
        changes = []
        if   opcode == 0x0 : changes = self.noop(operand1, operand2)
        elif opcode == 0x1 : changes = self.cpai(operand1, operand2)
        elif opcode == 0x2 : changes = self.cpar(operand1, operand2)
        elif opcode == 0x3 : changes = self.cpra(operand1, operand2)
        elif opcode == 0x4 : changes = self.cpri(operand1, operand2)
        elif opcode == 0x5 : changes = self.cprr(operand1, operand2)
        elif opcode == 0x6 : changes = self.addr(operand1, operand2)
        elif opcode == 0x7 : changes = self.subr(operand1, operand2)
        elif opcode == 0x8 : changes = self.jump(operand1, operand2)
        elif opcode == 0x9 : changes = self.jmpz(operand1, operand2)
        elif opcode == 0xA : changes = self.jmpn(operand1, operand2)
        elif opcode == 0xB : changes = self.jmpr(operand1, operand2)
        elif opcode == 0xC : changes = self.andd(operand1, operand2)
        elif opcode == 0xD : changes = self.orr (operand1, operand2)
        elif opcode == 0xE : changes = self.nott(operand1, operand2)
        elif opcode == 0xF : changes = self.xor (operand1, operand2)
        elif opcode == 0x10: changes = self.prnt(operand1, operand2)
        elif opcode == 0x11: changes = self.push(operand1, operand2)
        elif opcode == 0x12: changes = self.pop (operand1, operand2)
        elif opcode == 0xFF: changes = self.halt(operand1, operand2)
        
        return changes

    def set_program(self, program_text):
        def get_just_the_code(source_lines):
            ''' Strips out comments and blank lines, leaving code and labels '''
            just_the_code = []
            for source_line in source_lines:
                source_line = source_line.strip()
                tokens = source_line.split()
                if not tokens:
                    continue
                if tokens[0][0] == '#':
                    continue
                just_the_code.append(source_line)
            # make sure that the program terminates with a halt in case of 
            # labels at the end
            if just_the_code:
                if 'Halt' not in just_the_code[-1]:
                    just_the_code.append('Halt 0 0')
            return just_the_code

        def get_lines_with_line_numbers(source_lines):
            ''' returns a list of number/line pairs, with no line numbers of the labels '''
            source_with_line_numbers = []
            line_number = 0
            for source_line in source_lines:
                if source_line[0] == '.':
                    source_with_line_numbers.append((-1, source_line))
                else:
                    source_with_line_numbers.append((line_number, source_line))
                    line_number += 1
            return source_with_line_numbers

        if not program_text:
            return (True, '')

        source_lines = program_text.splitlines()
        if not source_lines:
            return (True, '')
        
        source_lines = get_just_the_code(source_lines)
        # everything now is an instruction or a label
        # allocation a source line to everything that is not a label, 
        # and give the label the address of the next source line
        source_with_line_numbers = get_lines_with_line_numbers(source_lines)

        # now read up from the end, finding the effective line number for each label
        labels = {}
        current_line = -1
        for item in source_with_line_numbers[::-1]:
            if item[0] == -1:
                label = item[1]
                if label in labels:
                    return (False, 'Repeated label ' + label)
                labels[label] = current_line
            else:
                current_line = item[0]

        # the code with no labels, held as objects, with the jumps pointing to line numbers, not labels
        machine_code = []

        for (line_number, code) in source_with_line_numbers:
            if code[0] == '.':
                continue
            tokens = code.split()
            if len(tokens) != 3:
                return (False, 'Invalid number of tokens, line ' + str(line_number))
            if tokens[0] not in self.opcodes:
                return (False, 'Invalid opcode ' + tokens[0] + ' on line ' + str(line_number))

            opcode = tokens[0]

            for tok in range(1,3):
                if tokens[tok][0] == '.':
                    label = tokens[tok]
                    if label not in labels:
                        return (False, 'Unknown label ' + label + ' on line ' + str(line_number))
                    num = labels[label] * 3
                    tokens[tok] = f'{num:0>2X}'
            # jump is special as it uses labels, not numbers
            for tok in range(1,3):
                if not all(c in string.hexdigits for c in tokens[tok]):
                    return (False, 'Invalid non-numeric operand ' + tokens[tok] + ' on line ' + str(line_number))

            #if tokens[0] in ('Jump', 'Jmpz', 'Jmpn'):
            #    label = tokens[1]
            #    if label not in labels:
            #        return (False, 'Unknown label ' + label + ' on line ' + str(line_number))
            #    dest = labels[label] * 3
            #else:
            #if not all(c in string.hexdigits for c in tokens[1]):
            #    return (False, 'Invalid non-hexadecimal operand ' + tokens[1] + ' on line ' + str(line_number))
            dest = int(tokens[1], 16)
            src = int(tokens[2], 16)

            machine_code.append(Instruction(line_number, opcode, dest, src))
        self.reset()
        self.program = machine_code
        # copy into memory
        for line_number in range(len(machine_code)):
            instruction = machine_code[line_number]
            print(instruction)
            self.memory[line_number*3+0] = self.opcodes_to_machine_codes[instruction.opcode]
            self.memory[line_number*3+1] = instruction.dest
            self.memory[line_number*3+2] = instruction.src
        return (True, '')

    def get_n_lines_of_program(self, mem_mode, n):
        max_lines = n
        if not self.program:
            return ''
        start = 0
        end = 0
        if len(self.program) <= max_lines:
            start = 0
            end = len(self.program)-1
        else:
            start = int(self.pc / 3) - int(n/2)
            if start < 0:
                start = 0
            end = start + max_lines
            if end >= len(self.program):
                end = len(self.program)-1
            while end - start < max_lines and start >= 1:
                start -= 1
        lines = []
        for line in range(start, end):
            instruction = self.program[line].opcode + ' ' + numToStr(mem_mode, self.program[line].dest) + ' ' + numToStr(mem_mode, self.program[line].src)
            lines.append((line*3, instruction))
        return lines

'''
It's all GUI from here
'''

class Cell(): 
    def __init__(self, row, col, parent):
        self.r = 0
        self.g = 0
        self.b = 0
        self.address = row*16+col
        self.var = tk.StringVar()
        self.box = tk.Label(parent, textvariable=self.var, borderwidth=0, width=8, font='TkFixedFont')

class SimpleTable(tk.Frame):
    def __init__(self, parent, rows, columns):
        # use black background so it "peeks through" to form grid lines
        tk.Frame.__init__(self, parent, background="black")
        self.cells = []
        self.rows = rows
        self.columns = columns

        italicfont = tk.font.Font(size=8, slant=tk.font.ITALIC)
        
        label = tk.Label(self, text="", borderwidth=0, width=8, font=italicfont)
        label.grid(row=0, column=0, sticky="nsew", padx=1, pady=1)
        for column in range(1, columns+1):
            label = tk.Label(self, text=numToStr('HH', column-1), borderwidth=0, width=8, font=italicfont)
            label.grid(row=0, column=column, sticky="nsew", padx=1, pady=1)

        for row in range(1, rows+1):
            label = tk.Label(self, text=numToStr('HH', row-1), borderwidth=0, width=8, font=italicfont)
            label.grid(row=row, column=0, sticky="nsew", padx=1, pady=1)

            for column in range(1, columns+1):
                c = Cell(row-1, column-1, self)
                self.cells.append(c)
                label = c.box
                label.grid(row=row, column=column, sticky="nsew", padx=1, pady=1)

    def get_cells(self):
        return self.cells

class PopupWindow(object):
    def __init__(self, master, label_text, initial_text, height=20, width=30):
        top = self.top = tk.Toplevel(master)
        self.l = tk.Label(top, text=label_text, font='TkFixedFont')
        self.l.pack()
        self.e = ScrolledText(top, height=height, width=width)
        if initial_text:
            self.e.insert('end', initial_text)
            self.e.configure(height=initial_text.count('\n')+1, state='disabled')
        
        self.e.pack(fill='both', expand=True)
        self.b = tk.Button(top, text='Ok', command=self.cleanup)
        self.b.pack(side='bottom')
        self.value = ''

    def cleanup(self):
        self.value = self.e.get('1.0', tk.END)
        self.top.destroy()

class EmulatorView(tk.Tk):
    def __init__(self):
        tk.Tk.__init__(self)

        self.emulator = Emulator()

        self.mem_mode = 'H' # decimal.  Could be ASCII, Hex or Binary - 'A', 'H' or 'B'
        self.memory_strings = []
        
        computer_frame = tk.Frame(self, borderwidth = 1, highlightbackground="black" , highlightthickness=1)

        #Computer frame

        reg_frame = tk.Frame(computer_frame, borderwidth = 1, highlightbackground="black" , highlightthickness=1)

        self.pc_label=tk.Label(reg_frame, text="PC")
        self.pc_label.grid(row=1,column=1)

        self.pc=tk.StringVar(None)
        self.pc_box=tk.Entry(reg_frame, textvariable=self.pc, width=5, state="readonly", font='TkFixedFont')
        self.pc_box.grid(row=1,column=2)

        self.sp_label=tk.Label(reg_frame, text="SP")
        self.sp_label.grid(row=2,column=1)

        self.sp=tk.StringVar(None)
        self.sp_box=tk.Entry(reg_frame, textvariable=self.sp, width=5, state="readonly", font='TkFixedFont')
        self.sp_box.grid(row=2,column=2)

        self.r0_label=tk.Label(reg_frame, text="R0")
        self.r0_label.grid(row=3,column=1)

        self.r0=tk.StringVar(None)
        self.r0_box=tk.Entry(reg_frame, textvariable=self.r0, width=5, state="readonly", font='TkFixedFont')
        self.r0_box.grid(row=3,column=2)

        self.r1_label=tk.Label(reg_frame, text="R1")
        self.r1_label.grid(row=4,column=1)

        self.r1=tk.StringVar(None)
        self.r1_box=tk.Entry(reg_frame, textvariable=self.r1, width=5, state="readonly", font='TkFixedFont')
        self.r1_box.grid(row=4,column=2)

        self.r2_label=tk.Label(reg_frame, text="R2")
        self.r2_label.grid(row=5,column=1)

        self.r2=tk.StringVar(None)
        self.r2_box=tk.Entry(reg_frame, textvariable=self.r2, width=5, state="readonly", font='TkFixedFont')
        self.r2_box.grid(row=5,column=2)

        reg_frame.grid(row=1, column=1, columnspan=2, rowspan=4, padx=5, pady=5)

        self.memory_table = SimpleTable(computer_frame, 16, 16)
        self.memory_table.grid(row=1, column=4, columnspan=17, rowspan=17, padx=5, pady=5)
        cells = self.memory_table.get_cells()
        for cell in cells:
            self.memory_strings.append(cell.var)

        #button frame
        self.step_button = tk.Button(self)
        self.step_button["text"] = "Step"
        self.step_button["command"] = self.step

        self.run_button = tk.Button(self)
        self.run_button["text"] = "Run"
        self.run_button["command"] = self.run

        self.view_button = tk.Button(self)
        self.view_button["text"] = self.mem_mode + ' :A/B/D/H'
        self.view_button["command"] = self.cycle_view

        self.load_program_button = tk.Button(self)
        self.load_program_button["text"] = 'Load'
        self.load_program_button["command"] = self.load_program

        self.help_button = tk.Button(self)
        self.help_button["text"] = 'Help'
        self.help_button["command"] = self.help

        self.quit_button = tk.Button(self, text="QUIT", fg="red", command=self.destroy)

        self.step_button.grid(row=1, column=1)
        self.run_button.grid(row=1, column=2)
        self.view_button.grid(row=1, column=3)
        self.load_program_button.grid(row=1, column=4)
        self.help_button.grid(row=1, column=5)

        self.quit_button.grid(row=1, column=19)
        computer_frame.grid(row=2, column=1, columnspan=18, rowspan=16, padx=5, pady=5)

        self.assembler_window_height = 9
        self.assembler_window = ScrolledText(self, height=self.assembler_window_height, width=30, font='TkFixedFont')
        self.assembler_window.grid(row=2, column=19, padx=5, pady=5)
        self.assembler_window.configure(state='disabled')
        self.curr_pos_top = 0
        self.curr_pos_bot = self.assembler_window_height

        self.output_window = ScrolledText(self, height=9, width=30, font='TkFixedFont')
        self.output_window.grid(row=3, column=19, padx=5, pady=5)
        self.output_window.configure(state='disabled')

        # set up colour gradients
        self.default_bg = self._root().cget('bg')
        self.default_fg = self.r0_box.cget('fg')
        self.reds = self.compute_colours(self.default_bg, '#FF0000', 5)
        self.greens = self.compute_colours(self.default_bg, '#00FF00', 5)
        self.blues = self.compute_colours(self.default_bg, '#0000FF', 5)

    def compute_colours(self, start, end, limit):
        (r1,g1,b1) = self.winfo_rgb(start)
        (r2,g2,b2) = self.winfo_rgb(end)
        r_ratio = float(r2-r1) / limit
        g_ratio = float(g2-g1) / limit
        b_ratio = float(b2-b1) / limit

        colours = []
        for i in range(limit):
            nr = int(r1 + (r_ratio * i))
            ng = int(g1 + (g_ratio * i))
            nb = int(b1 + (b_ratio * i))
            colour = "#%4.4x%4.4x%4.4x" % (nr,ng,nb)
            colours.append(colour)
        return colours

    def help(self):
        max_width = 0
        for str in self.emulator.instructions:
           if len(str) > max_width:
            max_width = len(str)
        popup=PopupWindow(self, "Opcodes", '\n'.join(self.emulator.instructions), 5, max_width+1)
        self.help_button["state"] = "disabled" 
        self.wait_window(popup.top)
        self.help_button["state"] = "normal"

    def numToStr(self, num):
        return numToStr(self.mem_mode, num)

    def redisplay_memory(self):
        mem = self.emulator.memory
        for i in range(len(mem)):
            self.memory_strings[i].set(self.numToStr(mem[i]))

    def refresh_assembler(self):
        lines = self.emulator.get_n_lines_of_program(self.mem_mode, 256)

        curr_line = self.emulator.pc
        start = None
        line_text = ''
        pos = 0
        for line_number, line in lines:
            if line_number == curr_line:
                start = pos
            line_text = line_text + self.numToStr(line_number).ljust(3) + ' '
            line_text = line_text + line
            line_text = line_text + '\n'
            pos += 1
        line_text = line_text[:-1]

        self.assembler_window.configure(state='normal')

        self.assembler_window.delete('1.0', 'end')
        self.assembler_window.insert('end', line_text)

        if not start is None:
            self.assembler_window.tag_add('hglite', "{:.1f}".format(start+1), "{:.1f}".format(start+2))
            self.assembler_window.tag_config('hglite', background='yellow')

            self.assembler_window.yview_scroll(start-2,  'units')
            
        self.assembler_window.configure(state='disabled')

    def set(self):
        self.redisplay_memory()
        self.pc.set(self.numToStr(self.emulator.pc))
        self.sp.set(self.numToStr(self.emulator.sp))
        self.r0.set(self.numToStr(self.emulator.registers[0]))
        self.r1.set(self.numToStr(self.emulator.registers[1]))
        self.r2.set(self.numToStr(self.emulator.registers[2]))
        self.refresh_assembler()

    def reset(self):
        self.emulator.reset()
        self.set()

    def cycle_view(self):
        if self.mem_mode == 'A':
            self.mem_mode = 'B'
        elif self.mem_mode == 'B':
            self.mem_mode = 'D'
        elif self.mem_mode == 'D':
            self.mem_mode = 'H'
        elif self.mem_mode == 'H':
            self.mem_mode = 'A'
        self.view_button["text"] = self.mem_mode  + ' :A/B/D/H'
        self.set()

    def colour_cells(self, reset, changed_addresses=[]):
        cells = self.memory_table.get_cells()
        for cell in cells:
            if self.emulator.pc <= cell.address and cell.address < self.emulator.pc+3:
                cell.box.configure(bg = self.greens[-1])
                cell.r = 0
                cell.g = len(self.greens)-1
                cell.b = 0
            elif self.emulator.sp == cell.address:
                cell.box.configure(bg = self.blues[-1])
                cell.r = 0
                cell.g = 0
                cell.b = len(self.blues)-1
            elif changed_addresses and cell.address in changed_addresses:
                cell.box.configure(bg = self.reds[-1])
                cell.r = len(self.reds)-1
                cell.g = 0
                cell.b = 0
            elif reset:
                cell.box.configure(bg = self.default_bg)
                cell.r = 0
                cell.g = 0
                cell.b = 0
            else:
                was = False
                will = False
                colour = -1
                if cell.r > 0:
                    was = True
                    cell.r -= 1
                elif cell.g > 0:
                    was = True
                    cell.g -= 1
                elif cell.b > 0:
                    was = True
                    cell.b -= 1
                if cell.r > 0:
                    will = True
                    colour = self.reds[cell.r]
                elif cell.g > 0:
                    will = True
                    colour = self.greens[cell.g]
                elif cell.b > 0:
                    will = True
                    colour = self.blues[cell.b]
                if was and will:
                    cell.box.configure(bg = colour)
                elif not was and will:
                    cell.box.configure(bg = colour)
                elif was and not will:
                    cell.box.configure(bg = self.default_bg)

    def load_program(self):
        popup=PopupWindow(self, "Enter Program", None)
        self.load_program_button["state"] = "disabled" 
        self.wait_window(popup.top)
        self.load_program_button["state"] = "normal"
        (rc, msg) = self.emulator.set_program(popup.value)
        if not rc:
            if not msg:
                msg = "Invalid Program"
            messagebox.showerror("Emulator", msg)
            return
        self.set()
        self.r0_box.configure(fg = self.default_fg)
        self.r1_box.configure(fg = self.default_fg)
        self.r2_box.configure(fg = self.default_fg)
        self.colour_cells(True)

    def step(self):
        changes = self.emulator.step()
        if changes:
            registers = changes[0]
            self.set()
            if registers:
                self.r0_box.configure(fg = self.default_fg)
                self.r1_box.configure(fg = self.default_fg)
                self.r2_box.configure(fg = self.default_fg)
                if registers[0] == 0:
                    self.r0_box.configure(fg = self.reds[-1])
                elif registers[0] == 1:
                    self.r1_box.configure(fg = self.reds[-1])
                elif registers[0] == 2:
                    self.r2_box.configure(fg = self.reds[-1])
                elif registers[0] == -1:
                    self.output_window.configure(state='normal')
                    txt = numToStr('H', self.emulator.op) + " " + numToStr('B', self.emulator.op) + " " + numToStr('D', self.emulator.op).ljust(3) + " " + numToStr('A', self.emulator.op).ljust(2) + "\n"
                    self.output_window.insert('end', txt)
                    self.output_window.see("end")
                    self.output_window.configure(state='disabled')
            addresses = changes[1]
            self.colour_cells(False, addresses)
        self.update()

    def run(self):
        while not self.emulator.halted:
            self.step()


if __name__ == "__main__":
    app = EmulatorView()
    app.reset()
    app.mainloop()

'''
CpRI 2 4
CpRI 1 .ret1
Push 1 0
Push 2 0
Jump .sigma 0
.ret1
Pop 0 0
Prnt 0 0 
Halt 0 0
.sigma
Pop 2 0
Jmpz .break 2
CpRR 0 2
CpRI 1 1
SubR 0 1
Jmpz .break 0
Push 2 0
CpRI 2 .return
Push 2 0
Push 0 0
Jump .sigma 0
.return
Pop 0 0
Pop 2 0
AddR 2 0
.break
Pop 0 0
Push 2 0
JmpR 0 0

'''
