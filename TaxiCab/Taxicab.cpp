// Taxicab.cpp : Defines the entry point for the console application.
//
/******************************************************************/
/*                                                                */
/*                          Rama4.c                               */
/*                            by                                  */
/*                        Bill Butler                             */
/*                                                                */
/*    This program is dedicated to finding quadruple Ramanujan    */
/*  numbers. A quadruple Ramanujan is a number such that:         */
/*  I^3 + J^3 = K^3 + L^3 = M^3 + N^3 = O^3 + P^3.                */
/*                                                                */
/*    The first quadruple occurs at:                              */
/*                                                                */
/*    13,322^3 + 16,630^3 =                                       */
/*    10,200^3 + 18,072^3 =                                       */
/*    5,436^3 + 18,948^3 =                                        */
/*    2,421^3 + 19,083^3 = 6,963,472,309,248  ( 6963472309248 )   */
/*                                                                */
/*    This program may be used, copied, modified, etc. without any*/
/*  obligation by any person for any purpose. I would appreciate  */
/*  that any published version (modified or original, or any      */
/*  published results) include a note crediting me with the       */
/*  program/algorithm. (e.g. "Original algorithm by Bill Butler.")*/
/*                                                                */
/*    The algorithm (especially the hash index portion) is very   */
/*  efficient and appears to be at least 10 times faster than the */
/*  "heap" algorithm used by David Wilson who published the first */
/*  known Ramanujan quintuple.                                    */
//
/*    The program is in ANSI "C", and runs "as is" without bugs.  */
/*  (Bad news - this is under DOS on an old 80486 computer). It   */
/*  should start displaying Ramanujan quadruples within a few     */
/*  minutes on any contemporary Athlon or Intel processor. The    */
/*  program will eventually run into a precision problem before it*/
/*  gets to the first Ramanujan quintuple. If you have 64 bit     */
/*  integer arithmetic available, you might try converting all    */
/*  "double" (real) numbers to 64 bit integers and then run the   */
/*  program. If you are successful, please let me know how it     */
/*  works out. (E-mail: lisabill@mywdurango.net)                  */
/*  (Remove the "w" for a valid E-mail)                           */
/*  (Using 64 bit integers, it should find the first Ramanujan    */
/*  quintuple in less than a week.)                               */
/*                                                                */
/*  Note: All "int" variables are 32 bits.                        */
/*                                                                */
/******************************************************************/


#include "stdafx.h"
#include <stdio.h>                   /*  (May not need these)     */
#include <stdlib.h>                  /*  Need for atof()          */
#include <ctype.h>                   /*  tolower()                */
#include <math.h>
#include <string.h>

#define HASHMAX    360000            /*  Keep HASHMAX about 20 pct larger than AvgGrpSize.  */
#define AvgGrpSize 300000            

#define MAXi 210000                 //  Maximum trial "I" or "J" that will be used for I^3 + J^3. 
                                     // To extend the search, increase this number. Suggest 400000.
									 // (All other constants are OK for any size search.) 

double Ncubed[MAXi+1];               /*  Ncubed[i] = i^3          */
__int64 Ncubed64[MAXi+1];             /*  Ncubed64[i] = i^3        */
unsigned Bits[MAXi+1];               /*  The rightmost 32 bits of the integer versions of these cubes.          */
int NextJ[MAXi+1];                   /*  Keeps track of the next "J" to try when forming trial I^3 + J^3.         */

#define twoHat19 524287  /* 2^19*/
#define twoHat19Plus1 524288  /* 2^19*/
/*  The size of the next 2 arrays matches the 19 bit hash size. (2^19)        */
unsigned HashHd[twoHat19Plus1];             /*  Heads for hash table.    */
int ChainLen[twoHat19Plus1];                /*  Chain length.            */

double I3J3[HASHMAX+1];              /*  I^3 + J^3                */
__int64 I3J364[HASHMAX+1];            /*  I^3 + J^3                */
/*  "double" will eventually run into a precision problem. If you have 64 bit integers, use them instead of real numbers. */
int Ival[HASHMAX+1];                 /*  The "I" in I^3 + J^3     */
int Jval[HASHMAX+1];                 /*  The "J" in I^3 + J^3     */
int Link[HASHMAX+1];                 /*  Link lists.              */

char Databuff[100];                  /*  Dummy array for input.   */

double UpperLim;                     /* For each iter., find cube sums up to this limit.   */
double Increment;                    /*  Increases the upper limit by this amount on each iter. Note:  "Increment" increases with time.       */
__int64 UpperLim64;                   /* For each iter., find cube sums up to this limit.   */
__int64 Increment64;                  /*  Increases the upper limit by this amount on each iter. Note:  "Increment" increases with time.       */
                                     
int NbrStored;                       /*  Nbr. of items in hash table. */
unsigned Bit19Mask = twoHat19;         /*  Mask for right 19 bits   */

void pausemsg() 
{
	puts("\nPress RETURN to continue");
	gets(Databuff);
}

/******************************************************************/
/*                                                                */
/*                             InitSys                            */
/*                                                                */
/*    This routine initializes the system. The Ncubed[] array is  */
/*  filled with cubes such that Ncubed[i] = I^3.                  */
/*                                                                */
/*    The Bits[] array contains the last 32 bits of the integer   */
/*  representation of all the I^3's. These will be used to form a */
/*  hash index. The hash index system greatly speeds up the       */
/*  process of finding matching pairs of I^3 + J^3 = K^3 + L^3,   */
/*  etc. (The actual hash index adds the 32 bit portions of       */
/*  I^3 + J^3, and uses bits 8 to 26 of the result as the hash    */
/*  index. Note: the least significant bit is bit "0".)           */
/*                                                                */
/*    The NextGroup() routine will generate a large group of      */
/*  trial I^3 + J^3 numbers. The "J's" that will be used in this  */
/*  routine will be >= "I" until the sum of I^3 + J^3 reaches the */
/*  upper limit for the current group. (Group size is kept within */
/*  bounds by stopping the process at "UpperLim".). This routine  */
/*  initializes the NextJ[] array for this process.               */
/*                                                                */
/*    "UpperLim" is set to 200000000 for the first iteration.     */
/*  Subsequently it will be increased by "Increment" to include   */
/*  larger trial values of I^3 + J^3. The density of I^3 + J^3    */
/*  numbers becomes sparser as the number field expands.          */
/*  "Increment" will thus become larger with time. Thus, the      */
/*  number of trial I^3 + J^3 numbers that will be looked at will */
/*  gradually cluster around "AvgGrpSize" (300,000) per iteration.*/
/*                                                                */
/*    Finally, when a group of trial I^3 + J^3 numbers cluster at */
/*  a given hash index, all numbers in this link list group are   */
/*  copied to the end of the I3J3[] array for final sorting. The  */
/*  last position in the array is initialized to -1 to aid the    */
/*  "insertion sort".                                             */
/*                                                                */
/******************************************************************/

void InitSys() 
{
	unsigned i, Ibits;
	double Ifloat, Icubed;
	puts("\nThis program finds Ramanujan quadruples. It will run  until the user manually breaks the program.\n");
	puts("However it will pause anytime a Ramanujan quintuple is found.");
//	pausemsg();
	puts("Initializing the cubes table");
	for (i = 1; i <= MAXi; i++) 
	{
		Ifloat = i;                    /*  Convert int to real.     */
		Icubed = Ifloat * Ifloat * Ifloat;
		Ncubed[i] = Icubed;
		Ibits = i * i * i;             /*  Calculate the rightmost 32 bits. (Note "C" ignores the overflow.) */
		Bits[i] = Ibits;               
		NextJ[i] = i;                  /*  Init the "J's"           */
	}
	puts("Starting search");
	UpperLim = 200000000.0;           /*  Initial upper limit.     */
	Increment = 200000000.0;          /*  Initial increment.       */
	I3J3[HASHMAX] = -1.0;             /*  Used for sort. Requires a negative number.                  */
}
void InitSys64() 
{
	int i;
	__int64 i64;
	unsigned Ibits;
	__int64 Icubed;
	puts("\nThis program finds Ramanujan quadruples. It will run  until the user manually breaks the program.\n");
	puts("However it will pause anytime a Ramanujan quintuple is found.");
//	pausemsg();
	puts("Initializing the cubes table");
	for (i = 1; i <= MAXi; ++i) 
	{
		i64 = i;
		Icubed = i64 * i64 * i64;
		Ncubed64[i] = Icubed;
		Ibits = (unsigned)(Icubed);           /*  Calculate the rightmost 32 bits. (Note "C" ignores the overflow.) */
		Bits[i] = Ibits;              
		NextJ[i] = i;                  /*  Init the "J's"           */
	}
	puts("Starting search");
	UpperLim64 = 200000000;           /*  Initial upper limit.     */
	Increment64 = 200000000;          /*  Initial increment.       */
	I3J364[HASHMAX] = -1;             /*  Used for sort. Requires a negative number.                  */
}

/******************************************************************/
/*                                                                */
/*                            NextGroup                           */
/*                                                                */
/*    This routine generates the next group of I^3 + J^3 and      */
/*  places it in the hash arrays. The number of these trial       */
/*  I^3 + J^3 sums that are processed will oscillate near         */
/*  "AvgGrpSize" (about 300,000).                                 */
/*                                                                */
/******************************************************************/

void NextGroup() 
{
	int Count, i, j;
	unsigned IJhash;
	double LimNbr, i3j3sum;           /*  Forces "LimNbr" to a "Register" position. Otherwise "UpperLim" could be used.           */
	for (i = twoHat19; i >= 0; i--)    /*  Clear old garbage.       */
	{
		HashHd[i] = 0;
		ChainLen[i] = 0;
	}
	Count = 0;
	i = 1;
	LimNbr = UpperLim;                /*  Sets up reg. number      */
	do 
	{                              /*  Do for all "i" in group  */
		j = NextJ[i];
		/*  Do for all "j" such that */
		while(1) 
		{                     /*  i^3 + j^3 is < UpperLim  */
			i3j3sum = Ncubed[i] + Ncubed[j];       /* I^3 + J^3      */
			if (i3j3sum < LimNbr
				 && i<MAXi && j<MAXi)
			{     /*  If within limit, then    */
				Count++;                 /*  include it in the group. */
				if (i>MAXi || j>MAXi)
					int k=0;
				IJhash = Bits[i] + Bits[j];         /*  Generate the hash index.  */
				IJhash = IJhash >> 8;               
				IJhash &= Bit19Mask;
				if (Count>HASHMAX)
					int kk=0;
				I3J3[Count] = i3j3sum;   /*  Add to the hash arrays.  */
				Ival[Count] = i;
				Jval[Count] = j;
				if (IJhash>twoHat19Plus1)
					int kkk=0;
				Link[Count] = HashHd[IJhash];    /*  Update link list */
				HashHd[IJhash] = Count;
				ChainLen[IJhash]++;
				j++;
			}
			else                        /*  Repeat until too big     */
				break;
		}
		NextJ[i] = j;                  /*  Set up for next round    */
		i++;
	} while (j > i);

	NbrStored = Count;
	/*  Optional status check. Will average about 300,000 per crack.       */
	/*
	printf("This iter. stored %d trial I^3+J^3 numbers\n", NbrStored);
	*/
}
void NextGroup64() 
{
	int Count, i, j;
	unsigned IJhash;
	__int64 LimNbr64, i3j3sum64;           /*  Forces "LimNbr" to a "Register" position. Otherwise "UpperLim" could be used.           */
	for (i = twoHat19; i >= 0; i--)    /*  Clear old garbage.       */
	{
		HashHd[i] = 0;
		ChainLen[i] = 0;
	}
	Count = 0;
	i = 1;
	LimNbr64 = UpperLim64;                /*  Sets up reg. number      */
	do 
	{                              /*  Do for all "i" in group  */
		j = NextJ[i];
		/*  Do for all "j" such that */
		while(1) 
		{                     /*  i^3 + j^3 is < UpperLim  */
			i3j3sum64 = Ncubed64[i] + Ncubed64[j];       /* I^3 + J^3      */
			if (i3j3sum64 < LimNbr64 
				&& i<MAXi && j<MAXi && Count<HASHMAX)
			{     /*  If within limit, then include it in the group.    */
				Count++;   
				if (i>MAXi || j>MAXi)
					int k=0;
				IJhash = Bits[i] + Bits[j];         /*  Generate the hash index.  */
				IJhash = IJhash >> 8;               
				IJhash &= Bit19Mask;
				if (Count>HASHMAX)
					int kk=0;
				I3J364[Count] = i3j3sum64;   /*  Add to the hash arrays.  */
				Ival[Count] = i;
				Jval[Count] = j;
				if (IJhash>twoHat19Plus1)
					int kkk=0;
				Link[Count] = HashHd[IJhash];    /*  Update link list */
				HashHd[IJhash] = Count;
				ChainLen[IJhash]++;
				j++;
			}
			else                        /*  Repeat until too big     */
				break;
		}
		NextJ[i] = j;                  /*  Set up for next round    */
		i++;
	} while (j > i);

	NbrStored = Count;
	/*  Optional status check. Will average about 300,000 per crack.       */
	/*
	printf("This iter. stored %d trial I^3+J^3 numbers\n", NbrStored);
	*/
}

/******************************************************************/
/*                                                                */
/*                           CheckGroup                           */
/*                                                                */
/*    This routine checks the hash arrays to see if any matches   */
/*  exist. If at least 4 numbers exist at any hash index, the     */
/*  entire link list is copied to the sort arrays where it is     */
/*  sorted. Note: The end of all arrays dimensioned  by "HASHMAX" */
/*  are used for sorting. (Usually the list is very short.)       */
/*                                                                */
/******************************************************************/
void CheckGroup() 
{
	int i, j, k, EndLoc, Next;
	double TempDbl;

	for (i = twoHat19; i >= 0; i--)
	{
		if (ChainLen[i] < 4)           /*  Chain is too short for quads. */
			continue;                
		EndLoc = HASHMAX;
		for (Next = HashHd[i]; Next; Next = Link[Next])
		{
			TempDbl = I3J3[Next];
			EndLoc--;
			/*  Use "Insertion sort"     */
			for (j = EndLoc, k = j + 1; I3J3[k] > TempDbl; j++, k++)
			{
				I3J3[j] = I3J3[k];
				Ival[j] = Ival[k];
				Jval[j] = Jval[k];
			}
			I3J3[j] = TempDbl;
			Ival[j] = Ival[Next];
			Jval[j] = Jval[Next];
		}
		/*  Check for quadruples.    */
		for (j = HASHMAX-4, k = HASHMAX-1; j >= EndLoc; j--, k--)
		{
			if (I3J3[j] == I3J3[k])
				printf("%7d%7d  %7d%7d  %7d%7d  %7d%7d  %.0lf\n",
				Ival[j], Jval[j], Ival[j+1], Jval[j+1],
				Ival[j+2], Jval[j+2], Ival[k], Jval[k],
				I3J3[j]);
			/*  The following can be deleted  unless the program is modified for 64 bit integers.             */
			if (I3J3[j] == I3J3[k+1])
			{
				puts("Ramanujan Quintuple. Press ENTER to continue.");
				pausemsg();
			}
		}
	}
}

void CheckGroup64() 
{
	int i, j, k, EndLoc, Next;
	__int64 TempDbl;

	for (i = twoHat19; i >= 0; i--)
	{
		if (ChainLen[i] < 4)           /*  Chain is too  short for quads. */
			continue;                
		EndLoc = HASHMAX;
		for (Next = HashHd[i]; Next; Next = Link[Next])
		{
			TempDbl = I3J364[Next];
			EndLoc--;
			/*  Use "Insertion sort"     */
			for (j = EndLoc, k = j + 1; I3J364[k] > TempDbl; j++, k++)
			{
				I3J364[j] = I3J364[k];
				Ival[j] = Ival[k];
				Jval[j] = Jval[k];
			}
			I3J364[j] = TempDbl;
			Ival[j] = Ival[Next];
			Jval[j] = Jval[Next];
		}
		/*  Check for quadruples.    */
		for (j = HASHMAX-4, k = HASHMAX-1; j >= EndLoc; j--, k--)
		{
			if (I3J364[j] == I3J364[k])
				printf("%7d%7d  %7d%7d  %7d%7d  %7d%7d  %zu \n",
				Ival[j], Jval[j], Ival[j+1], Jval[j+1],
				Ival[j+2], Jval[j+2], Ival[k], Jval[k],
				I3J364[j]);
			/*  The following can be deleted unless the program is modified for 64 bit integers.             */
			if (I3J364[j] == I3J364[k+1])
			{
				puts("Ramanujan Quintuple. Press ENTER to continue.");
				pausemsg();
			}
		}
	}
}

int _tmain(int argc, _TCHAR* argv[])
{
	bool b64=false;
	if (!b64)
	{
		InitSys();                        /*  Initialize system.       */
		while(1)
		{                        /*  Do forever.              */
			NextGroup();                   /*  Form next group of cubes.                   */
			CheckGroup();                  /*  Look for matches.        */
			if (NbrStored < AvgGrpSize)
			{  /*  Process about 300,000    */
				/*  trial I^3 + J^3 ea. iter.*/
				Increment *= 1.1;           /* Increase by 10 % as needed*/
			}
			UpperLim += Increment;         /*  Set up for next group    */
		}
	}
	else
	{
		InitSys64();                        /*  Initialize system.       */
		while(1)
		{                        /*  Do forever.              */
			NextGroup64();                   /*  Form next group of cubes.                   */
			CheckGroup64();                  /*  Look for matches.        */
			if (NbrStored < AvgGrpSize) 
			{  /*  Process about 300,000 trial I^3 + J^3 ea. iter.*/
				Increment64 *= 1.1;           /* Increase by 10 % as needed*/
			}
			UpperLim64 += Increment64;         /*  Set up for next group    */
		}
	}
}