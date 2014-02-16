        /*  area.c. Program to compute area contributing to each Pixel in DEM 
           for cell outflow based on angles.
             
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
          
        
        #include "lsm.h"
        
        /* prototypes */
        
        /*        float 
                prop();
                void dparea();
        */
        /*  Global Variables   */
        
        /*   int nx,ny;
           float dx,dy,ndv,aref[10];
           int dd1[8],dd2[8],*d1,*d2;
           float **ang, **sca;
        */ 
        
        int ccheck; //flag for check boundary detection
        
        
        int area(char *pfile, char *afile, int irz, int icz, int contCheck)
        {
           int i,j,filetype,err;
           double bndbox[4],csize;
           ccheck = contCheck;
           /* Define directions */
           /*  d1a=dd1-1; d2a=dd2-1;  */
           d1[1]=0; d1[2]= -1; d1[3]= -1; d1[4]= -1; d1[5]=0; d1[6]=1; d1[7]=1; d1[8]=1;
           d2[1]=1; d2[2]=1; d2[3]=0; d2[4]= -1; d2[5]= -1; d2[6]= -1; d2[7]=0; d2[8]=1;
        
        
        
           /* read angles */
           //printf("PERCENT: %d\n",25);
           printf("MESSAGE: Open file %s\n" , pfile);
           err=gridread(pfile,(void ***)&ang,RPFLTDTYPE,&nx,&ny,&dx,&dy,
                     bndbox,&csize,&ndv,&filetype);
           if(err != 0)goto ERROR1;           
        
           /*  Allocate memory  */
           //printf("PERCENT: %d\n",50);
           printf("MESSAGE: Allocate memory...\n");
           sca = (float **) matalloc(nx, ny, RPFLTDTYPE);
        
           /* Calculate angles for interpolation */                                      
           aref[0]= -atan2(dy,dx);
           aref[1]=0.;
           aref[2]= -aref[0];
           aref[3]=0.5*PI;
           aref[4]=PI-aref[2];
           aref[5]=PI;
           aref[6]=PI+aref[2];
           aref[7]=1.5*PI;
           aref[8]=2.*PI-aref[2];
           aref[9]=2.*PI;
           
           /* initialize sca array to 0 */
           for(i=0; i<ny; i++)
               {
               for(j=0; j<nx; j++)
                 {
                 if(i!=0 && i!=ny-1 && j!=0 && j!=nx-1 && ang[j][i]> ndv)
                    sca[j][i]=0;
                 else sca[j][i]= -1;
                 }
               }
           if(irz > 0 && icz > 0)
           {
           /* call drainage area subroutine for pixel to zero on  */
             irz=irz-1;   /* decrease index for c indexing starting from 0 */
             icz=icz-1;
             dparea(irz,icz);
           }
           else
           {
        
           /* Call drainage area subroutine for each cell           */
           /* working from the middle out to avoid deep recursions  */ 
        
           /* "lower block" */
              for(i=ny/2; i<ny-1; i++)
              {
                //SetWorkingStatus();
                 for(j=nx/2; j<nx-1; j++)
                   dparea(i,j);
                 for(j=nx/2-1; j>=1; j--)
                   dparea(i,j);
              }
           /* "upper block" */
              for(i=ny/2-1; i>=1; i--)
              {
                //SetWorkingStatus();
                 for(j=nx/2; j<nx-1; j++)
                   dparea(i,j);
                 for(j=nx/2-1; j>=1; j--)
                   dparea(i,j);
              }
            }
        
           /*  Write out areas  */  
           //printf("PERCENT: %d\n",75);
           printf("MESSAGE: Writing file...\n");
           
           err = gridwrite(afile,(void **)sca,RPFLTDTYPE,nx,ny,dx,dy,
                     bndbox,csize,-1.,filetype);
                     
           //printf("PERCENT: %d\n",100);
           
           free(sca[0]);
           free(sca);        
        ERROR1:
            free(ang[0]);
            free(ang);
            return(err);              
        }  
        
        /* function to compute partial areas recursively */
        
        void dparea(i,j)
        int i,j;
        {
           int in,jn,k,kback,con=0;
            /* con is a flag that signifies possible contaminatin of sca
                 due to edge effects  */
           float p;
           if(sca[j][i]==0.0)    /* i.e., if the cell hasn't been looked at yet */
              {
                if(i!=0 && i!=ny-1 && j!=0 && j!=nx-1 && ang[j][i] > ndv)
               /* i.e. the cell isn't outside domain */
                 {
         /*  Specific catchment area of single grid cell is dx
        			This is area per unit contour width.  */
        			sca[j][i] = dx;
         /*			sca[j][i] = 1.0;  */
                   for(k=1; k<=8; k++)
                   {
                     in = i + d1[k];
                     jn = j + d2[k];
        /* for each neighboring cell, find the proportion of of  outflow */
        /* draining back to the cell in question */
                     kback = k + 4;
                     if(kback>8)kback = kback - 8;
                     p=prop(ang[jn][in],kback);
                     if(p>0.)
                        {
                         dparea(in,jn);
                         if(sca[jn][in] < -0.5)con = -1;
                         sca[j][i] = sca[j][i] + sca[jn][in]*p;
                        }
                     if(ang[jn][in] <= ndv)con = -1;         
                    }
                    // added by EAC08: check for edge contamination
                    if(con == -1 && ccheck == 1)
        			{
        				sca[j][i]= -1.;
        			}
        //           if(con == -1)sca[j][i]= -1.; //EAC08: original version
                 }
             }
        } 
        
        float prop(a,k)
        float a;
        int k;
        {
        float p=0.;
        if(k == 1 && a > PI)a=a-2.*PI;
        if(a > aref[k-1] && a < aref[k+1]){
          if( a > aref[k])
               p=(aref[k+1]-a)/(aref[k+1]-aref[k]);
          else 
              p=(a-aref[k-1])/(aref[k]-aref[k-1]);
          }
        return(p);
        }
                        
         
           
