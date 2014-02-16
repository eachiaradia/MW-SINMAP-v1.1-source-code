#include "gridio.h"

int setdir(char *demfile, char *angfile, char *slopefile);              



int main(int argc,char **argv)
{
  char demfile[MAXLN],slopefile[MAXLN],angfile[MAXLN];
  int err,nmain;
  char *ext;
   if(argc != 2)
    {  
       printf("Usage:\n %s filename\n",argv[0]);
       printf("The following are appended BEFORE\n");
       printf("the files are opened:\n");
       printf("fel    Pit filled elevation data (input)\n");
       printf("ang    D inf flow directions(output)\n");
       printf("slp    Slopes (Output)\n");
       exit(0);  
    }
    ext=strrchr(argv[1],'.');
    if(ext == NULL)
	{
       sprintf(demfile,"%s%s",argv[1],"fel");
       sprintf(angfile,"%s%s",argv[1],"ang");
       sprintf(slopefile,"%s%s",argv[1],"slp");
 	}
	else
	{
		nmain=strlen(argv[1])-strlen(ext);
		strncpy(demfile,argv[1],nmain);
		strcat(demfile,"fel");
		strcat(demfile,ext);
		strncpy(angfile,argv[1],nmain);
		strcat(angfile,"ang");
		strcat(angfile,ext);
		strncpy(slopefile,argv[1],nmain);
		strcat(slopefile,"slp");
		strcat(slopefile,ext);
	}
    if(err=setdir(demfile, angfile, slopefile) != 0)
        printf("Setdir error %d\n",err);
}     
