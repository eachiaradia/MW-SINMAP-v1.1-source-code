/*  File: floodmn.c 
	Program to provide main for calling flood function in flood.c
	Begining date: 97/8/30
	Publish date: 
	Version 0.1
  
	This program provides the main() function for calling the flood 
	function in flood.c.  This allows use of flood in a stand-alone
	console environment with the flood.c routines linked in as a static
	library.  

*/

#include "gridio.h"

int flood(char *demfile, char *pointfile, char *newfile);

void main1(int argc,char **argv)
{
  char demfile[MAXLN], pointfile[MAXLN], newfile[MAXLN];
  int err,nmain;
  char *ext;

   if(argc != 2)
    {  
       printf("Usage:\n %s filename\n",argv[0]);
       printf("The following are appended BEFORE\n");
       printf("the files are opened:\n");
       printf("elv    Elevation data\n");
       printf("p      D8 flow directions\n");
       printf("fel    Flooded elevation data\n");
       exit(0);  
    }
    sprintf(demfile,"%s",argv[1]);
    ext=strrchr(argv[1],'.');
    if(ext == NULL)
	{
	    strcpy(pointfile,argv[1]);
        strcpy(newfile,argv[1]);
		strcat(pointfile,"p");
		strcat(newfile,"fel");
	}
	else
	{
		nmain=strlen(argv[1])-strlen(ext);
		strncpy(pointfile,argv[1],nmain);
		strcat(pointfile,"p");
		strcat(pointfile,ext);
		strncpy(newfile,argv[1],nmain);
		strcat(newfile,"fel");
		strcat(newfile,ext);
	}
    if(err=flood(demfile, pointfile, newfile) != 0)
        printf("Flood error %d\n",err);
}    
