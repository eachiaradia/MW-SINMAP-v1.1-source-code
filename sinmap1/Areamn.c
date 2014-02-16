/*  area.c. Program to compute area contributing to each Pixel in DEM 
   for cell outflow based on angles.
     
  David G Tarboton
  Utah State University   
  
  SINMAP package version 0.1 9/2/97   */
  

#include "lsm.h"

void main(int argc,char **argv)
{
   char pfile[MAXLN],afile[MAXLN], *ext;
   int err,row=0,col=0,nmain;
   
   if(argc < 2)
    {  
       printf("Usage:\n %s filename [outletrow outletcol]\n",argv[0]);
       printf("(The optional outletrow and outletcol are outlet coordinates\n");
       printf("for the area to be computed.  If they are not given, or are \n");
       printf("0 0 the whole file is computed.)\n");
       printf("The following are appended to the file names\n");
       printf("the files are opened:\n");
       printf("ang    D inf flow directions(Input)\n");
       printf("sca    Specific Catchment Area (Output)\n");
       exit(0);  
    }
    ext=strrchr(argv[1],'.');
    if(ext == NULL)
	{
       sprintf(pfile,"%s%s",argv[1],"ang");
      sprintf(afile,"%s%s",argv[1],"sca");
	}
	else
	{
		nmain=strlen(argv[1])-strlen(ext);
		strncpy(afile,argv[1],nmain);
		strcat(afile,"sca");
		strcat(afile,ext);
		strncpy(pfile,argv[1],nmain);
		strcat(pfile,"ang");
		strcat(pfile,ext);
	}
    
    if(argc >2)
    {
       sscanf(argv[2],"%d",&row);
       sscanf(argv[3],"%d",&col);
    }
    if(err=area(pfile,afile,row,col) != 0)
        printf("area error %d\n",err);
} 
