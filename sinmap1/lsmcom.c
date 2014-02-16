/*	lsmcom.c 
		Common information to all lsm functions including gloabal variables */
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
/*	Includes */
#include "lsm.h"

/*  Global Variables  **************************************************/

short **dir;                    /* flood, setdir */
short **apool;                  /* flood */
float **elev;                   /* flood, setdir */
float **slope;		            /* setdir */
float **ang;                    /* setdir, area */
float **sca;	                /* area */

short *ipool, *jpool, *tmp;		/* flood */ 
short *dn, *is, *js;			/* setdir, flood */

								/* area */

int nx, ny;						/* flood, setdir, area */
int npool, pooln, pstack;		/* flood */
int nis, istack;				/* flood, setdir */
int filetype;					/* flood */

/*  i1,n1 are the x range when tiling
    i2,n2 are the y range when tiling
    These are global so as to avoid duplication in recursive routines  */
    
int i1,i2,n1,n2;			    /* flood, setdir */

char annot[MAXLN],units[MAXLN];	/* flood, setdir */

float dx, dy;					/* flood, setdir, area */
float emin, et;					/* flood, setdir */
float utme,utmn,skew;			/* setdir */
float aref[10];					/* area */
float ndv;						/* area, setdir */

int nf;				            /* flood, setdir */
int dd1[8],dd2[8];

/* Offset pointers d1 and d2 */
int *d1 = dd1-1;
int *d2 = dd2-1;

/*  Grid bound variables   */
double bndbox[4],csize;
float mval;


