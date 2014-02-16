/* lsm.h
   header file for C functions that are stored in avlib.dll and
   accessed from ArcView (Avenue) via DLL.Proc calls. 
   Created by Craig N. Goodwin
     */
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

/*  Toggle between these depending upon platform  */
/*  #define WIN  */
#define WIN

/* include file to make things work */

#include "gridio.h"

/*	Defines */
#define PI2 1.5707963268

/* this set up Microsoft specific export decaration and does away with
   the need for a DEF file. Use DEF file in UNIX versions */
#ifdef WIN 

#include <windows.h>

#define DllExport __declspec( dllexport ) 

/* Functions in avlib.dll that are callable from outside */

DllExport int flood(char *demfile, char *pointfile, char *newfile);
DllExport int setdir(char *demfile, char *angfile, char *slopefile);              
DllExport int area(char *pfile, char *afile, int zrow, int zcol, int contCheck);              
DllExport int sindex(char *slopefile,char *areafile,char *sindexfile,
					 char *tergridfile,char *terparfile,char *satfile);
DllExport float siplot(float s,float a,float c1,float c2, 
                  float t1,float t2,float x1,float x2,float r);
// EAC08
DllExport double sindexcell(double s,double a,double c1,double c2, 
                  double t1,double t2,double x1,double x2,double r,
                  double *sat);


#endif

/* Common information to all lsm functions including gloabal variables */

/* prototypes - t********************/

/* used in flood.c */
int		setdf(float mval);
int		vdn(int n);					                    /* also used in setdir */
int		pool(int i,int j);
void	set(int i,int j,float *fact,float mval);		/* also used in setdir */
float	min2(float e1,float e2);
float	max2(float e1,float e2);

/* used in setdir.c */
int		setdfnoflood(float mval);
void	SET2(int I, int J,float *DXX,float DD);
void	setdf2(void );
void	VSLOPE(float E0,float E1, float E2,
             float D1,float D2,float DD,
             float *S,float *A);
int		setdir(char *demfile, char *angfile, char *slopefile);              

/* used in area.c */
float	prop();
void	dparea();

/*  Global Variables  **************************************************/

extern short **dir;                     /* flood, setdir */
extern short **apool;                   /* flood */
extern float **elev;                    /* flood, setdir */
extern float **slope;	                /* setdir */
extern float **ang;                     /* setdir, area */
extern float **sca;		                /* area */

extern short *ipool, *jpool, *tmp;		/* flood */ 
extern short *dn, *is, *js;				/* setdir, flood */

extern int nx, ny;						/* flood, setdir, area */
extern int npool, pooln, pstack;		/* flood */
extern int nis, istack;					/* flood, setdir */
extern int filetype;					/* flood */

/*  i1,n1 are the x range when tiling
    i2,n2 are the y range when tiling
    These are global so as to avoid duplication in recursive routines  */
    
extern int i1,i2,n1,n2;			        /* flood, setdir */

extern char annot[MAXLN],units[MAXLN];	/* flood, setdir */

extern float dx, dy;					/* flood, setdir, area */
extern float emin, et;					/* flood, setdir */
extern float utme,utmn,skew;			/* setdir */
extern float aref[10];					/* area */
extern float ndv;						/* area, setdir */

extern int nf;				            /* flood, setdir */
extern int dd1[8],dd2[8];			
extern int *d1;
extern int *d2;

/* prototypes for functions in AVCalls.c */
// not used EAC08
void SetWorkingStatus(void);

/*  Grid bound variables   */
extern double bndbox[4],csize;
extern float mval;


