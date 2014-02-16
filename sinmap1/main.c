/*  File: main.c 
	Program to provide main for calling sinmap functions 
	Begining date: 2008/10/23
	Publish date: 
	Version 0.1
  
	This program provides the main() function for calling the SInMap 
	functions. Lunch this file with the name of the function as the
	first parameter and then the other requests.
*/

#include "gridio.h"
//#include "lsm.h"
#include <string.h>
#include <time.h>

int flood(char *demfile, char *pointfile, char *newfile);
int setdir(char *demfile, char *angfile, char *slopefile);
int area(char *pfile, char *afile, int zrow, int zcol, int contCheck);
int sindex(char *slopefile,char *areafile,char *sindexfile,
           char *tergridfile,char *terparfile,char *satfile);

int main(int argc,char **argv)
{
  char command[MAXLN];  
  char demfile[MAXLN], pointfile[MAXLN], newfile[MAXLN]; //flood function
  char angfile[MAXLN], slopefile[MAXLN]; //setdir function
  char areafile[MAXLN]; //area function
  int  row=0,col=0,ccheck=0; //area function
  char sindexfile[MAXLN],tergridfile[MAXLN],terparfile[MAXLN],satfile[MAXLN]; //sindex function
    
  int err;
  char *ext;
  int isCmd;
  //printf("num of arg: %d\n" , argc);
  
  sprintf(command,"%s",argv[1]);
  isCmd = 0;
  
  time_t start,end;

time (&start);


if (strcmp( command, "flood" )== 0)
   {
     isCmd = 1;
    printf("MESSAGE: Executing %s command...\n" , command);
    sprintf(demfile,"%s",argv[2]);
    sprintf(pointfile,"%s",argv[3]);
    sprintf(newfile,"%s",argv[4]);
    
    
    if(err=flood(demfile, pointfile, newfile) != 0)
        printf("ERROR: Flood error %d\n",err);
    } 

if (strcmp( command, "setdir" )== 0)
   {
   isCmd = 1;
   printf("MESSAGE: Executing %s command...\n" , command);
   sprintf(demfile,"%s",argv[2]);
   sprintf(angfile,"%s",argv[3]);
   sprintf(slopefile,"%s",argv[4]);    
   if(err=setdir(demfile, angfile, slopefile) != 0)
        printf("ERROR: Setdir error %d\n",err);        
   }

if (strcmp( command, "area" )== 0)
   {
   isCmd = 1;
   printf("MESSAGE: Executing %s command...\n" , command);
   sprintf(angfile,"%s",argv[2]);
   sprintf(areafile,"%s",argv[3]);
   sscanf(argv[4],"%d",&col);
   sscanf(argv[5],"%d",&row);
   sscanf(argv[6],"%d",&ccheck);
      
   if(err=area(angfile, areafile, row, col, ccheck) != 0)
        printf("ERROR: Area error %d\n",err);        
   }      

if (strcmp( command, "sindex" )== 0)
   {
   isCmd = 1;
   printf("MESSAGE: Executing %s command...\n" , command);
   sprintf(slopefile,"%s",argv[2]);
   sprintf(areafile,"%s",argv[3]);
   sprintf(sindexfile,"%s",argv[4]);
   sprintf(tergridfile,"%s",argv[5]);
   sprintf(terparfile,"%s",argv[6]);
   sprintf(satfile,"%s",argv[7]);
      
   if(err=sindex(slopefile,areafile,sindexfile,tergridfile,terparfile,satfile) != 0)
        printf("ERROR: Area error %d\n",err);        
   }  

if (isCmd != 1)
   {
     printf("\n\n************************************\n\n");
     printf("SINMAP - Stability INdex MAPping\n");
     printf("original package version 1.0  3/30/98\n\n");
     printf("Program to compute stability index and factor of safety for landslide Hazard mapping.\n\n");
     printf("Main Author: David G Tarboton - Utah State University\n\n");
     printf("This version is provided by Enrico A. Chiaradia - University of Milan\n");
     printf("e-mail: enrico.chiaradia@unimi.it\n\n");
     printf("AVAILABLE COMMANDS ARE:\n");
     printf("flood demname=STRING pointname=STRING filldemname=STRING\n");
     printf("setdir filldemname=STRING anglename=STRING slopename=STRING\n");
     printf("area angname=STRING areaname=STRING ccell=INT rcell=INT ccheck=BOOLEAN\n");
     printf("sindex slopename=STRING areaname=STRING sindexname=STRING tergridname=STRING terparname=STRING satname=STRING\n");
     printf("\nUSAGE EXAMPLE:\n");
     printf("sinmap.exe flood dem.asc null.asc filleddem.asc\n");
     
     err = 0;
          }
time (&end);
double dif = difftime (end,start);
printf("Time elapsed: %.2lf seconds\n",dif );

if (err == 0)
   printf("Done.\n");
else
{
   printf("Finished with error!\n");
      
system("PAUSE");
}


return err;
}    
