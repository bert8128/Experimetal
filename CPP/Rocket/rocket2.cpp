#include <stdio.h>
#include <math.h>
#include <stdlib.h>


#define ndim 4 // dimension of system

// model parameters
static double G		= 6.674e-11;
static double M		= 5.9722e24;
static double GM	= G*M;	// G x MASS of large mass
static double CX	= 0;
static double CY	= 0; // coordinate of heavy mass
static double mass	= 1; // mass of small mass
static double eps	= 0; // regularization of potential



// potential energy
double pot( double f[] )
{
  double x = f[0];
  double y = f[1];
  double rik2 = (x-CX)*(x-CX) + (y-CY)*(y-CY);
  double potential = - GM*mass/sqrt(rik2+eps*eps);
  return potential;
}


// total energy //
double total_energy( double f[] )
{
  double kinetic =  0.5 * mass * ( f[2]*f[2] + f[3]*f[3] );
  double potential = pot( f );
  return kinetic + potential;
}


// force components //
void force( double f[], double *f_x, double *f_y )
{
  double x = f[0];
  double y = f[1];

  double xk = x - CX;
  double yk = y - CY;
  double rk2 = xk*xk + yk*yk;

  *f_x = - GM * mass * xk / pow( rk2+eps*eps, 1.5 );
  *f_y = - GM * mass * yk / pow( rk2+eps*eps, 1.5 );
}

// acceleration components //
void accn( double f[], double *f_x, double *f_y )
{
  double x = f[0];
  double y = f[1];

  double xk = x - CX;
  double yk = y - CY;
  double rk2 = xk*xk + yk*yk;

  *f_x = - GM * xk / pow( rk2+eps*eps, 1.5 );
  *f_y = - GM * yk / pow( rk2+eps*eps, 1.5 );
}


// generates RHS of diff eq //
void gravity_rhs( double t, double f[], double rhsf[] )
{
  double f_x, f_y;

  force( f, &f_x, &f_y );

  rhsf[0] = f[2];
  rhsf[1] = f[3];
  rhsf[2] = f_x;// / mass;
  rhsf[3] = f_y;// / mass;
}

// Runge-Kutta 4th order time step     //
// f_t[] is the function to advance    //
// requires a global definition for    //
// int ndim: dimension of system       //
// void rhs_function (t, f, rhsf )     //
void rk4_step ( 
	double t, double dt, double f_t[],
	void( *rhs_function)( double t, double f[], double rhsf[] )
	)
{
  double k1[ndim], k2[ndim], k3[ndim],
            k4, yt[ndim], rhsf[ndim];
  int i;

  rhs_function( t, f_t, rhsf );
  for ( i=0 ; i<ndim ; i++ )
  {
      k1[i] = dt * rhsf[i];
      yt[i] = f_t[i] + 0.5*k1[i];
  }
  rhs_function( t+0.5*dt, yt, rhsf );
  for ( i=0 ; i<ndim ; i++ )
  {
      k2[i] = dt * rhsf[i];
      yt[i] = f_t[i] + 0.5*k2[i];
  }
  rhs_function( t+0.5*dt, yt, rhsf );
  for ( i=0 ; i<ndim ; i++ )
  {
      k3[i] = dt * rhsf[i];
      yt[i] = f_t[i] + k3[i];
  }
  rhs_function( t+dt, yt, rhsf );
  for ( i=0 ; i<ndim ; i++ )
  {
      k4 = dt * rhsf[i];
      f_t[i] = f_t[i] + ( 0.5*k1[i] + k2[i] + k3[i] + 0.5*k4 ) / 3.0;
  }
}

// define time grid
void define_time_grid( double *t_min, double *t_max, double *dt )
{
  *t_min = 0.0;
  *t_max = 3600.0;
  *dt = 0.1;      // somewhat large (demo)
}

// set initial conditions
void set_initial_conditions(  double f[] )
{
	// position & velocity components
	f[0] = CX + 6371000.0;
	f[1] = CY;
	f[2] = sqrt(GM/f[0]);
	f[3] = 0; 
}

void printHeader(double f[])
{
	double d = sqrt(f[0]*f[0] + f[1]*f[1]);
	double v = sqrt(f[2]*f[2] + f[3]*f[3]);
	printf( "M = %.2f kg, d = %.2f m, v = %.2f m/s\n", M, d, v);
	printf( "\n");
	printf( " time    d    v    x    y    vx    vy    energy\n");
}
void print( double t, double f[], double energy)
{
	double d = sqrt(f[0]*f[0] + f[1]*f[1]);
	double v = sqrt(f[2]*f[2] + f[3]*f[3]);
	printf( " %.2f %.2f %.2f %.2f %.2f %.2f %.2f %.2f\n", t, d, v, f[0], f[1], f[2], f[3], energy );
}

int main( int argc, char * argv[] )
{
  double dt, t_min, t_max;
  define_time_grid( &t_min, &t_max, &dt );
  double t = t_min;

  double f_t[ndim];
  set_initial_conditions( f_t );

                           // program control
                           // print trajectories (or not)
                           // input x & v (or not)
                           // reset tmax
	for ( int n=1; n < argc ; n++ )
	{
		if ( argv[n][0] == '-' )
		switch ( argv[n][1] )
		{
			case 't': 
				n++;
				t_max = atof( argv[n] );
				break;
			case 'd': 
				n++;
				dt = atof( argv[n] );
				break;
			case 'x': 
				n++;
				f_t[0] = atof( argv[n] );
				break;
			case 'v': 
				n++;
				f_t[3] = atof( argv[n] );
				break;
			default:
				fprintf( stderr, "\nSyntax: \n\n");
				fprintf( stderr, "orbit <-p> <-t t_max> <-d dt> <-x x_0> <-v vy_0>\n\n" );
			exit(1);
		}
	}

	double energy = total_energy( f_t );

	printHeader(f_t);
	print(t, f_t, energy );

	while ( t < t_max )
	{
		rk4_step ( t, dt, f_t, &gravity_rhs );
		t = t + dt;
		energy = total_energy( f_t );
		print(t, f_t, energy );
		if (((int)t)%60 ==0)
		{
			int i= 0;
		}
	}
}
