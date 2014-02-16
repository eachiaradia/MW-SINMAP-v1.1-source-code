/***********************************************************/
/*                                                         */
/* gridio.h                                                */
/*                                                         */
/* Grid Data manipulation functions -- header file         */
/*                                                         */
/*                                                         */
/* David Tarboton                                          */
/* Utah Water Research Laboratory                          */
/* Utah State University                                   */
/* Logan, UT 84322-8200                                    */
/*                                                         */
/***********************************************************/
/*
  David G Tarboton
  Utah State University   
  
  SINMAP package version 1.0  3/30/98

Acknowledgement
---------------
This program developed with the support of Forest Renewal of British Columbia, 
in collaboration with Canadian Forest Products Ltd., Vancouver, British Columbia 
and Terratech Consulting Ltd., British Columbia, http://www.tclbc.com (Bob Pack)
and Craig Goodwin, 1610 E 1185 North, Logan, UT 84341-3036.

These programs are distributed freely with the following conditions.
1.  In any publication arising from the use for research purposes the source of 
the program should be properly acknowledged and a pre-print of the publication 
sent to David Tarboton at the address below.
2.  In any use for commercial purposes or inclusion in any commercial software 
appropriate acknowledgement is to be included and a free copy of this software 
made available to David Tarboton, Terratech and CanFor.

David Tarboton
Utah Water Research Laboratory
Utah State University
Logan, UT, 84322-8200
U. S. A.

tel:  (+1) 435 797 3172
email: dtarb@cc.usu.edu   */
#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <ctype.h>
#include <limits.h>
#include <float.h>
#include <math.h>
#include "gioapi.h"

/*  ESRI Application Programmers Interface include file  */

#define NA (10.0*FLT_MIN)
#ifndef PI
#define PI 3.14159265359
#endif
#define LINELEN 40
#ifndef MAXLN
#define MAXLN 4096
#endif

/* data types */
#define RPSHRDTYPE  1
#define RPINTDTYPE  2
#define RPFLTDTYPE  3

/* byte sizes corresponding to data types above */
#define RPSHRSIZE (sizeof(short))
#define RPINTSIZE (sizeof(int))
#define RPFLTSIZE (sizeof(float))

int gridwrite(char *file, void **data, int datatype, int nx, int ny, float dx, 
 float dy, double bndbox[4], double csize, float ndv, int filetype);

int gridread(char *file, void ***data, int datatype, int *nx, int *ny,
 float *dx, float *dy, double bndbox[4], double *csize, float *ndv, int *filetype);

void eol(FILE *fp);

int readline(FILE *fp,char *fline);

/*
  matalloc(...) allocates memory for matrix navigation pointer
  and for matrix data, then returns matrix navigation pointer
  DGT version without so many pointers
*/
void **matalloc(int nx,int ny,int datatype);

