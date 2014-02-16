/*  Program to copmute stability index and factor of safety for landslide 
   Hazard mapping. 
  David G Tarboton
  Utah State University  
  Started 1/18/97
  Modified 10/18/97 to separate forest and region parameters
  
  SINMAP package version 0.1 9/2/97     */
  
#include "gridio.h"

/*  Prototypes  */
void el();
float flow(),fosval(),fosclass();
int sindex(char *slopefile,char *areafile,char *sindexfile,
           char *tergridfile,char *terparfile, char *satfile);  

void main2(int argc, char **argv)
{
  char slopefile[MAXLN], areafile[MAXLN], sindexfile[MAXLN], 
       regionfile[MAXLN],regparfile[MAXLN], satfile[MAXLN], *ext;
  int err,nmain;
  
  if(argc < 2)
  {
     printf("\nUsage:\n"); 
     printf("\n  %s <filename prefix> \n",argv[0]);
     printf("\n  suffixes:  sca, slp, si, reg, par\n");      
     exit(0);
  }
   ext=strrchr(argv[1],'.');
    if(ext == NULL)
	{
		nmain=strlen(argv[1]);
		sprintf(areafile,"%s%s",argv[1],"sca");
		sprintf(slopefile,"%s%s",argv[1],"slp");
		sprintf(sindexfile,"%s%s",argv[1],"si");
		sprintf(regionfile,"%s%s",argv[1],"cal");
		sprintf(satfile,"%s%s",argv[1],"sat");
	}
	else
	{
		nmain=strlen(argv[1])-strlen(ext);
		strncpy(areafile,argv[1],nmain);
		strcat(areafile,"sca");
		strcat(areafile,ext);
		strncpy(slopefile,argv[1],nmain);
		strcat(slopefile,"slp");
		strcat(slopefile,ext);
		strncpy(sindexfile,argv[1],nmain);
		strcat(sindexfile,"si");
		strcat(sindexfile,ext);
		strncpy(regionfile,argv[1],nmain);
		strcat(regionfile,"cal");
		strcat(regionfile,ext);
		strncpy(satfile,argv[1],nmain);
		strcat(satfile,"sat");
		strcat(satfile,ext);
	}
	/*  Parameter files do not have extensions - They are ASCII  */
	strncpy(regparfile,argv[1],nmain);
    strcat(regparfile,"calp");

  if(err=sindex(slopefile,areafile,sindexfile,regionfile,regparfile,satfile) != 0)
    printf("Sindex Error %d\n",err);
} 

