        /*  Program to compute stability index and factor of safety for landslide 
            Hazard mapping. 
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
        
        
        #include "lsm.h"
        
        /*  Prototypes  */
        double fs(double s,double w,double c,double t,double r);
        double af(double angle,double c,double t,double x,double r,double fs);
        double csw(double t,double c,double r,double w,double fs);
        double f3s(double x1, double x2, double y1, double y2, double b1, 
                   double b2, double z );
        double fai(double y1,double y2,double b1,double b2,double a);
        double fai2(double y1,double y2,double b1,double b2,double a);
        double fai3(double y1,double y2,double b1,double b2,double a);
        double fai4(double y1,double y2,double b1,double b2,double a);
        double fai5(double y1,double y2,double b1,double b2,double a);
        double f2s(double x1, double x2, double y1, double y2, double z);
        double fa(double y1, double y2, double b1, double b2, double a);
        //EAC08: exported in dll
        //double sindexcell(double s,double a,double c1,double c2, 
        //                  double t1,double t2,double x1,double x2,double r,
        //                  double *sat);
        
        /*********************************************************************/
        int sindex(char *slopefile,  char *areafile,   char *sindexfile,
                   char *tergridfile,char *terparfile, char *satfile)
        {
          float **fos, **sat;
          int **tergrid;
          FILE *fp;
          int i,j,mter,err,filetype, index;
          float ndva,ndvs,ndvter;
          double bndbox[4],csize, X1, X2, cellsat;
          float rs,rw,g;
          float tqmin,tqmax,cmin,cmax,tphimin,tphimax, r;
        
          struct {
            float tqmin;
            float tqmax;
            float cmin;
            float cmax;
            float tphimin;
            float tphimax;
            float r;
           } region[100];  /* this could be malloced or should be checked
                              to ensure fewer than 100 regions */
         
         //printf("PERCENT: %d\n",0);
         printf("MESSAGE: Open calibration file %s\n" , terparfile);
         
         fp = fopen(terparfile,"r");
         if (fp == NULL)return 15;
        
         i = -1;
         do
          {
          if (i<0)
            {
            j = fscanf(fp,"%f %f %i",&g, &rw, &mter); 
            //r = rw/rs;
            }
          else
           {
           j = fscanf(fp,"%i %f %f %f %f %f %f %f \n", &index,&tqmin,&tqmax,&cmin,
                                                    &cmax,&tphimin,&tphimax,&rs);
           region[index].tqmin = tqmin;
           region[index].tqmax = tqmax;
           region[index].cmin  = cmin;
           region[index].cmax  = cmax;
           region[index].tphimin = tphimin;
           region[index].tphimax = tphimax;
           region[index].r = rw/rs;
           }
          i++;
          if (i>99) /* check for array size */
            {
            fclose(fp);
            return 16;
            }
          }
          while (j != EOF);
        fclose(fp);
        
        /* read grid files */
        //printf("PERCENT: %d\n",20);
        printf("MESSAGE: Open grid file %s\n" , slopefile);
            
        err=gridread(slopefile,(void ***)&slope,RPFLTDTYPE,&nx,&ny,&dx,&dy,
                     bndbox,&csize,&ndvs,&filetype);
                     
        if(err != 0)goto ERROR0;
        
        //printf("PERCENT: %d\n",40);
        printf("MESSAGE: Open grid file %s\n" , areafile);
        err=gridread(areafile,(void ***)&sca,RPFLTDTYPE,&nx,&ny,&dx,&dy,
                     bndbox,&csize,&ndva,&filetype);
        if(err != 0)goto ERROR1;
        
        //printf("PERCENT: %d\n",60);
        printf("MESSAGE: Open grid file %s\n" , tergridfile);
        err=gridread(tergridfile,(void ***)&tergrid,RPINTDTYPE,&nx,&ny,&dx,&dy,
                     bndbox,&csize,&ndvter,&filetype);
        if(err != 0)goto ERROR2;
        
        //printf("PERCENT: %d\n",80);
        printf("MESSAGE: Allocate memory\n");
              
        if((fos = (float **)matalloc(nx,ny, RPFLTDTYPE)) == NULL)
          {
          err=7;
          goto ERROR4;
          }  
                
        if((sat = (float **)matalloc(nx,ny, RPFLTDTYPE)) == NULL)
          {
          err=7;
          goto ERROR5;
          }
        
        /* go do the science */
        
        dx = (float) csize;
        //printf("PERCENT: %d\n",100);
        //printf("PERCENT: %d\n",0);
        printf("MESSAGE: Do stability index ...\n");
        for(i=0; i < ny; i++)
          {
          //SetWorkingStatus();
          for(j=0; j<nx; j++)
            {
            mter = tergrid[j][i];
            if(mter <= ndvter || sca[j][i] < 0. || slope[j][i] == ndvs)
              {
              fos[j][i]= -999.;
              sat[j][i]= -999.;
              }
            else
              {
              X2 = 1.0 / region[mter].tqmin; 
              X1 = 1.0 / region[mter].tqmax;
              fos[j][i] = (float) sindexcell(slope[j][i],          sca[j][i],
                                region[mter].cmin,    region[mter].cmax, 
                                region[mter].tphimin, region[mter].tphimax,
                                X1, X2, region[mter].r, &cellsat);
              sat[j][i] = (float) cellsat;
              }
            }
          } 
          
        //printf("PERCENT: %d\n",90);
        printf("MESSAGE: Write output data\n");
        
        err = gridwrite(sindexfile,(void **)fos,RPFLTDTYPE,nx,ny,dx,dy,
                        bndbox,csize,-999.,filetype);
        err = gridwrite(satfile,(void **)sat,RPFLTDTYPE,nx,ny,dx,dy,
                        bndbox,csize,-999.,filetype);
        
        /*  wrapping up  */
          free(sat[0]);
          free(sat);
        ERROR5:
          free(fos[0]);
          free(fos);
        ERROR4:             
          free(tergrid[0]);
          free(tergrid);
        ERROR2:
          free(sca[0]);
          free(sca);
        ERROR1:  
          free(slope[0]);
          free(slope);
        ERROR0:
        
        //printf("PERCENT: %d\n",100);
          return(err);
        }
        /*******************************************************************/
        /* this function is called by arcview to determine the SI for a given
        slope and angle in the SA Plot. It merely calls sindexcell, but has
        floats as arguments (not doubles) which is acceptable to avenue. */
        float siplot(float s,float a,float c1,float c2, 
                          float t1,float t2,float x1,float x2,float r)
        {
        double *sat;
        double dummy = 0.0;
        double tanofs;
        
        sat = &dummy;
        
        /* feeding real s out of progam, but this has to compensate for the fact
        that the grid file slope is tangent of angle */
        tanofs = tan(s);
        /*  SindexCell is now expecting angles   */
        t1 = atan(t1);
        t2 = atan(t2);
        
        return (float) sindexcell(tanofs,(double) a,
                                   (double) c1,(double) c2,
                                   (double) t1,(double) t2,
                                   (double) x1,(double) x2,
                                   (double) r,sat);
        }
        /*******************************************************************/
        /* New code that is conversion of DGT SPlus probabilistic theory code */
        
        /* function to compute stability index with potentially 
           uncertain parameters  for a specific grid cell */
        double sindexcell(double s,double a,double c1,double c2, 
                          double t1,double t2,double x1,double x2,double r,
                          double *sat)
        {
        /*  c1, c2  bounds on dimensionless cohesion
            t1,t2 bounds on friction angle
            x1,x2 bounds on wetness parameter q/T
            r Density ratio
            s slope angle
            a specific catchment area */
        
        double cs, ss, w1, w2, fs2, fs1, cdf1, cdf2, y1, y2, as, si;
        
        /* cng - added this to prevent division by zero */
        if (s == 0.0)
          {
          *sat = 3.0;
          return 10;
          }
        
        /*  DGT - The slope in the GIS grid file is the tangent of the angle  */
        s=atan(s);
        /*  t1 and t2 input as angle must be converted to tan  */
        t1=tan(t1);
        t2=tan(t2);
        
        cs = cos(s);
        ss = sin(s);
        
        if(x1 > x2)
        {  /*  Error these are wrong way round - switch them  */
        	w1=x2;   /*  Using w1 as a temp variable  */
        	x2=x1;
        	x1=w1;
        }
        /* Wetness is coded between 0 and 1.0 based on the lower T/q parameter 
          (this is conservative).  If wetness based on lower T/q is > 1.0 and wetness based 
          on upper T/q <1.0, then call it the "threshold saturation zone" and hard code it 
          to 2.0.  Then if wetness based on upper T/q >1.0, then call it the 
          "saturation zone" and hard code it to 3.0.  
          Lower T/q correspounds to upper q/T = x = x2 ->  w2  */
        w2 = x2*a/ss;
        w1 = x1*a/ss;
        *sat = w2;
        if(w2 > 1.0)
        {
        	w2 = 1.0;
        	*sat = 2.0;
        }
        if(w1 > 1.0)
        {
        	w1 = 1.0;
        	*sat = 3.0;
        }
        
        
        fs2 = fs(s,w1,c2,t2,r);
        
        if (fs2 < 1)  /* always unstable */
        	{
        	si  =  0;
        	}
        else
        	{
        	fs1 = fs(s,w2,c1,t1,r);
        	if(fs1 >= 1) /*  Always stable */
        		{
         		si  =  fs1;
        		}
        	else
        		{
        		if(w1 == 1) /* region 1 */
        			{
        			si = 1-f2s(c1/ss,c2/ss,cs*(1-r)/ss*t1,cs*(1-r)/ss*t2,1);
        			}
        		else
        			{
        			if(w2 == 1) /* region 2 */
        				{
        				as = a/ss;
        				y1 = cs/ss*(1-r);
        				y2 = cs/ss*(1-x1*as*r);
        				cdf1 = (x2*as-1)/(x2*as-x1*as)*f2s(c1/ss,c2/ss,cs*(1-r)/ss*t1,cs*(1-r)/ss*t2,1);
        				cdf2 = (1-x1*as)/(x2*as-x1*as)*f3s(c1/ss,c2/ss,y1,y2,t1,t2,1);
        				si = 1-cdf1-cdf2;
        				}
        			else  /* region 3 */
        				{
        				as = a/ss;
        				y1 = cs/ss*(1-x2*as*r);
        				y2 = cs/ss*(1-x1*as*r);
        				si = 1-f3s(c1/ss,c2/ss,y1,y2,t1,t2,1);
        				}
        			}
          	}
        	}
        
        /* cng - need to limit the size spread for arcview since it has
           difficulties with equal intervals over a large range of numbers */
        if (si > 10.0)
          si = 10.0;
        
        return si;
        }
        /**********************************************************************/
        /*  Generic function to implement dimensionless form of infinite slope
            stability model */
        double fs(double s,double w,double c,double t,double r)
        {
        double cs, ss;
        
        cs = cos(s);
        ss = sin(s);
        return ((c+cs*(1-w*r)*t)/ss);
        }
        
        
        /**********************************************************************/
        /*  function to return specific catchment area that results in the given factor of safety
            for the given parameters */
        double af(double angle,double c,double t,double x,double r,double fs)
        {
        /* angle = slope angle (radians)
           c = dimensionless cohesion
           t = tan of friction angle
           x = q/T wetness indicator  w = q a/(T sin slope angle) = x a/sin ...
           r = density ratio
           fs = safety factor  */
        
        double ss, cs, a;
        
        ss = sin(angle);
        cs = cos(angle);
        a = ss*(1-(fs*ss-c)/(t*cs))/(r*x);
        
        return a;
        }
        /**********************************************************************/
        /* Function to return angle of slope with given conditions and safety factor*/
        double csw(double t,double c,double r,double w,double fs)
        {
        double rwt, cs;
        
        rwt = (1-r*w)*t;
        cs = 0;   /*  default - this will give 90 degrees for large cohesion */
        if(c < 1)cs = (sqrt(fs*fs*(fs*fs+rwt*rwt-c*c))-c*rwt)/(fs*fs+rwt*rwt);
        return acos(cs);
        }
        /**********************************************************************/
        double f3s(double x1, double x2, double y1, double y2, double b1, 
                   double b2, double z )
        {
        double p;
        
        if (x2 < x1 || y2 < y1 || b2 < b1)
        	{
        /*	cat("Bad input parameters, upper limits less than lower limits\n");
        	p = NA; */
        /* cng - make a big number to be subtracted and yield ndv */
        /*  p = 1000.0 */
          p = 1000.0;
          }
        else if	(x1 < 0 || y1 < 0 || b1 < 0)
        	{
        /* 	cat("Bad input parameters, Variables cannot be negative\n");
        	p = NA; */
          p = 1000.;
          }
        else
        	{
        	if(y1 == y2 || b1 == b2)
        		{  /* degenerate on y or b - reverts to additive convolution */
            p = f2s(x1,x2,y1*b1,y2*b2,z);
            }
        	else
        		{
            if(x2 == x1) 
            	{
              p = fa(y1, y2, b1, b2, z - x1);
              }
            else 
              {
              p = (fai(y1, y2, b1, b2, z - x1) - 
              		 fai(y1, y2, b1, b2, z - x2)) / (x2 - x1);
              }
        		}
        	}
        return p;
        }
        /**********************************************************************/
        /*  y2 > y1 and x2 > x1. */
        double fai(double y1,double y2,double b1,double b2,double a)
        {
        double p, t, a1, a2, a3, a4, c2, c3, c4, c5;
        
        /* p = rep(0,length(a)); */
        p = 0.0;
        
        if (y1*b2 > y2*b1)
        	{  					/* Invoke symmetry and switch so that y1*b2 < y2*b1 */
        	t = y1;
        	y1 = b1;
        	b1 = t;
        	t = y2;
        	y2 = b2;
        	b2 = t;
        	}
        							/* define limits */
        a1 = y1*b1;
        a2 = y1*b2;
        a3 = y2*b1;
        a4 = y2*b2;
        
        							/* Integration constants */
        c2 =  - fai2(y1,y2,b1,b2,a1);
        
        							/* Need to account for possibility of 0 lower bounds */
        if(a2 == 0)
          c3  =  0;
        else
          c3 =  fai2(y1,y2,b1,b2,a2)+c2-fai3(y1,y2,b1,b2,a2);
        
        if(a3 == 0)
          c4  =  0;
        else
          c4 =  fai3(y1,y2,b1,b2,a3)+c3-fai4(y1,y2,b1,b2,a3);
        
        c5 =  fai4(y1,y2,b1,b2,a4)+c4-fai5(y1,y2,b1,b2,a4);
        
        /* evaluate */
        p = (a1 < a && a <= a2) ? fai2(y1,y2,b1,b2,a)+c2 : p;
        p = (a2 < a && a <= a3) ? fai3(y1,y2,b1,b2,a)+c3 : p;
        p = (a3 < a && a <  a4) ? fai4(y1,y2,b1,b2,a)+c4 : p;
        p = (a >= a4) ? fai5(y1,y2,b1,b2,a)+c5 : p;
        
        return p;
        }
        /**********************************************************************/
        double fai2(double y1,double y2,double b1,double b2,double a)
        {
        double adiv;
        
        adiv = (y2-y1)*(b2-b1);
        return (a*a*log(a/(y1*b1))/2 - 3*a*a/4+y1*b1*a)/adiv;
        }
        /**********************************************************************/
        double fai3(double y1,double y2,double b1,double b2,double a)
        {
        double adiv;
        
        adiv = (y2-y1)*(b2-b1);
        return (a*a*log(b2/b1)/2-(b2-b1)*y1*a)/adiv;
        }
        /**********************************************************************/
        double fai4(double y1,double y2,double b1,double b2,double a)
        {
        double adiv;
        adiv = (y2-y1)*(b2-b1);
        return (a*a*log(b2*y2/a)/2+3*a*a/4+b1*y1*a-b1*y2*a-b2*y1*a)/adiv;
        }
        /**********************************************************************/
        double fai5(double y1,double y2,double b1,double b2,double a)
        {
        return a;
        }
        /**********************************************************************/
        double f2s(double x1, double x2, double y1, double y2, double z)
        {
        double p, mn, mx, d, d1, d2;
        
        /* p = rep(0, length(z)); */
        p = 0.0;
        
        mn = min(x1 + y2, x2 + y1);
        mx = max(x1 + y2, x2 + y1);
        d1 = min(x2 - x1, y2 - y1);
        d = z - y1 - x1;
        d2 = max(x2 - x1, y2 - y1);
        p = (z > (x1 + y1) && z < mn) ? (d*d)/(2 * (x2 - x1) * (y2 - y1)) : p;
        p = (mn <= z & z <= mx) ? (d - d1/2)/d2 : p;
        p = (mx < z & z < (x2 + y2)) ? 1-pow((z-y2-x2),2)/(2*(x2-x1)*(y2-y1)) : p;
        p =  z >= (x2 + y2) ? 1 : p;
        
        return p;
        }
        
        /**********************************************************************/
        double fa(double y1, double y2, double b1, double b2, double a)
        {
        double p, adiv, t;
        
        /* p = rep(0, length(a)); */
        p = 0.0;
        
        if(y1 * b2 > y2 * b1) 
       	  { /* Invoke symmetry and switch so that y1*b2 < y2*b1 */
          t = y1;
          y1 = b1;
          b1 = t;
          /* was   y <- y2  which is wrong */
          t = y2;
          y2 = b2;
          b2 = t;
          }
        
        adiv = (y2 - y1) * (b2 - b1);
        p = (y1 * b1 < a && a <= y1 * b2) ? 
            (a * log(a/(y1 * b1)) - a + y1 * b1)/adiv : p;
        p = (y1 * b2 < a && a <= y2 * b1) ? 
            (a * log(b2/b1) - (b2 - b1) * y1)/adiv : p;
        p = (y2 * b1 < a && a < y2 * b2) ? 
            (a * log((b2 * y2)/a) + a + b1 * y1 - b1 * y2 - b2 * y1)/adiv : p;
        p =  a >= (b2 * y2) ? 1 : p;
        
        return p;
        }
