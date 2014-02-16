/* AVCalls.c
		C functions that mimic ArcView procedure calls */
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
 
//#include "avexec32.h"
#include <stdio.h>
#include <string.h>

/* These will go to lsm.h file */
void MsgBoxInfo(char *message, char *title);
void MsgBoxError(char *message, char *title);
void SetWorkingStatus(void);

/* Exported to test from within ArcView. Not needed otherwise */
#define DllExport __declspec( dllexport ) 
DllExport void doitnow(void);

/**************************************************/
void doitnow (void)  /* for testing purposes only */
{
char message[] = "This message should be showing up in the window";
char title[] = "Test Box";

SetWorkingStatus();

}
/**************************************************/
void MsgBoxInfo(char *message, char *title)
{
char *retstr;
char strpassed[200]; /* Better not pass something longer than 200! */
char str1[] = "MsgBox.Info(\"";
char str2[] = "\",\"";
char str3[] = "\")";

strcpy(strpassed, str1);
strcat(strpassed, message);
strcat(strpassed, str2);
strcat(strpassed, title);
strcat(strpassed, str3);

//retstr = AVExec(strpassed);
}
/**************************************************/
void MsgBoxError(char *message, char *title)
{
char *retstr;
char strpassed[200]; /* Better not pass something longer than 200! */
char str1[] = "MsgBox.Error(\"";
char str2[] = "\",\"";
char str3[] = "\")";

strcpy(strpassed, str1);
strcat(strpassed, message);
strcat(strpassed, str2);
strcat(strpassed, title);
strcat(strpassed, str3);

//retstr = AVExec(strpassed);
}
/**************************************************/
void SetWorkingStatus(void)
{
char *retstr;

//retstr = AVExec("av.SetWorkingStatus");
}
