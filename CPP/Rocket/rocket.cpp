#include <stdio.h>
#include <iostream.h>
#include <math.h>
#include <sstream>
#include <string>
#include <vector>
#include <i86.h>

using namespace std;

static double massE     (void) { return double(  5.9742e24); } // kg
static double radiusE   (void) { return double(  6378100.0); } // metres
static double massM     (void) { return double(  7.3477e22); } // kg
static double radiusM   (void) { return double(  1738000.0); } // metres
static double radiusLEO (void) { return double(   500000.0); } // metres
static double radiusLMO (void) { return double(    50000.0); } // metres
static double earthMoon (void) { return double(384403000.0); } // metres
static double BigG      (void) { return double(6.67428e-11); } //m^3 kg^-1 s^-2

static double tick      (void) { return 1.0; } // second

static double Pi    (void) { static double hp = (2.0 * asin(1.0)); return hp; }
string convertDouble(double value)
{
    char buffer[20];
    sprintf(buffer, "%.1lf", value);
    return string(buffer);
}

// polar/cartesian conversion
static double c2p_radius(double x, double y) { return sqrt(x*x+y*y); }
static double p2c_x     (double r, double a) { return r * cos(a); }
static double p2c_y     (double r, double a) { return r * sin(a); }
static double c2p_angle (double x, double y)
{
    if (x > 0.0)
    {
        if (y >= 0.0)
            return atan(y/x);
        else // y < 0.0
            return 2.0 * Pi() + atan(y/x);
    }
    else if (x < 0.0)
    {
        return atan(y/x) + Pi();
    }
    else //x == 0.0
    {
        if (y > 0.0)
            return Pi() / 2.0;
        else if (y < 0.0)
            return 3.0 * Pi() / 2.0;
        else //y == 0.0
            return 0.0;
    }
}
static double radToDeg(double rad)
{
    static const double c = 360.0 / (2.0 * Pi());
    return rad * c;
}
/*
static void testc2p_angle(void)
{
    test( 2.0,  0.0);
    test( 2.0,  1.0);
    test( 2.0,  2.0);
    test( 1.0,  2.0);
    test( 0.0,  2.0);
    test(-1.0,  2.0);
    test(-2.0,  2.0);
    test(-2.0,  1.0);
    test(-2.0,  0.0);
    test(-2.0, -1.0);
    test(-2.0, -2.0);
    test(-1.0, -2.0);
    test( 0.0, -2.0);
    test( 1.0, -2.0);
    test( 2.0, -2.0);
    test( 2.0, -1.0);
}
static void test(double x, double y)
{
    cout << "x: " << x << " y: " << y << ", " << radToDeg(c2p_angle(x, y)) << endl;
}
*/

//#define dist 0.1                /* stepsize */
//#define xf 5                    /* max for x */ 

 
/*main()
{
    double t, y, h;
    int n;
  
    output=fopen("xydata.dat", "w");    //* External filename 
    h=0.1;
    y=1;                                            //* Initial condition 
    fprintf(output, "0\t%f\n", y);

    for (n=0;dist*n<=xf;n++)        // The time loop 
    {
       t=n*dist;
          y=rkutta(t, y, dist);
 
   fprintf (output, "%f\t%f\n", t, y);
   }

   fclose(output);
}                   // End of main function
*/
double rk4(
    double x,
    double y,
    double h,
    double (*F)(double, double)
//std::vector<double> (*derivatives)(double x, std::vector<double> y)
)
{
    const double H = h/2.0;
          
    double k1 = F(x, y);
    double k2 = F(x+H, y+H*k1);
    double k3 = F(x+H, y+H*k2);
    double k4 = F(x+h, y+h*k3);
    return y + (h/6.0 * (k1+ 2.0*k2 + 2.0*k3 + k4));
}


class Vector
{
public:
    Vector() : m_x(0.0), m_y(0.0) {}
    Vector(double x, double y) : m_x(x), m_y(y) {}
    Vector(const Vector& v) : m_x(v.m_x), m_y(v.m_y) {}
    Vector& operator=(const Vector& v);
    string DumpCart (void) const;
    string DumpPolar(void) const;
    double radius(void) const;
    double angle (void) const;
    double angleDeg (void) const;
    double x     (void) const { return m_x; }
    double y     (void) const { return m_y; }
    void setX    (double x)  { m_x = x; }
    void setY    (double y)  { m_y = y; }
    void incX    (double dx) { m_x += dx; }
    void incY    (double dy) { m_y += dy; }
    bool operator==(const Vector& rhs) const { return x() == rhs.x() && y() == rhs.y(); }
    bool operator!=(const Vector& rhs) const { return !(*this == rhs); }
    Vector operator+(const Vector& v) const;
    Vector operator*(double d) const;
    Vector operator-(const Vector& v) const;
    Vector operator/(double d) const;
    Vector& operator+=(const Vector& v);
    Vector& operator-=(const Vector& v);
    Vector& operator*=(double d);
    Vector& operator/=(double d);
private:
    double m_x;
    double m_y;
};
Vector& Vector::operator=(const Vector& v)
{
    if (&v != this)
    {
        m_x = v.m_x;
        m_y = v.m_y;
    }
    return *this;
}
Vector Vector::operator+(const Vector& v) const
{
    Vector vv(*this);
    vv.m_x += v.m_x;
    vv.m_y += v.m_y;
    return vv;
}
Vector Vector::operator*(double d) const
{
    Vector vv(*this);
    vv.m_x *= d;
    vv.m_y *= d;
    return vv;
}
Vector Vector::operator-(const Vector& v) const
{
    Vector vv(*this);
    vv.m_x -= v.m_x;
    vv.m_y -= v.m_y;
    return vv;
}
Vector Vector::operator/(double d) const   
{
    Vector vv(*this);
    vv.m_x /= d;
    vv.m_y /= d;
    return vv;
}
Vector& Vector::operator*=(double d)
{
    m_x *= d;
    m_y *= d;
    return *this;
}
Vector& Vector::operator/=(double d)
{
    m_x /= d;
    m_y /= d;
    return *this;
}
Vector& Vector::operator+=(const Vector& v)
{
    m_x += v.m_x;
    m_y += v.m_y;
    return *this;
}
Vector& Vector::operator-=(const Vector& v)
{
    m_x -= v.m_x;
    m_y -= v.m_y;
    return *this;
}
string Vector::DumpCart (void) const
{
    string s = "X: " + convertDouble(m_x)      + " Y: " + convertDouble(m_y);
    return s;
}
string Vector::DumpPolar(void) const
{
    string s = "R:\t" + convertDouble(radius()) + "\tA:\t" + convertDouble(radToDeg(angle()));
    return s;
}
double Vector::radius(void) const
{
    return c2p_radius(m_x, m_y);
}
double Vector::angle (void) const
{
    return c2p_angle (m_x, m_y);
}
double Vector::angleDeg (void) const
{
    return radToDeg(angle());
}

typedef Vector Position;
typedef Vector Velocity;
typedef Vector Acceleration;
typedef Vector Force;
/*
class Position : public Vector
{
public:
    Position() {}
    Position(double x, double y) : Vector(x, y) {}
    string Where(void) const { return DumpCart(); }
    Position(const Position& p) : Vector(p) {}
    Position& operator=(const Position& p) { Vector::operator=(static_cast<const Vector&>(p)); return *this;}
};

class Velocity : public Vector
{
public:
    Velocity() {}
    Velocity(double x, double y) : Vector(x, y) {}
    string HowFast(void) const { return DumpPolar(); }
    Velocity(const Velocity& v) : Vector(v) {}
    Velocity& operator=(const Velocity& v) { Vector::operator=(static_cast<const Vector&>(v)); return *this;}
};

class Acceleration : public Vector
{
public:
    Acceleration() {}
    Acceleration(double x, double y) : Vector(x, y) {}
    Acceleration(const Acceleration& a) : Vector(a) {}
    Acceleration& operator=(const Acceleration& v) { Vector::operator=(static_cast<const Vector&>(v)); return *this;}
};

class Force : public Vector
{
public:
    Force() {}
    Force(double x, double y) : Vector(x, y) {}
    void add(const Force& f) { incX(f.x()); incY(f.y()); }
    string HowBig(void) const { return DumpPolar(); }
    Force(const Force& f) : Vector(f) {}
    Force& operator=(const Force& f) { Vector::operator=(static_cast<const Vector&>(f)); return *this;}
    bool operator==(const Force& rhs) const { return Vector::operator==(rhs); }
    bool operator!=(const Force& rhs) const { return Vector::operator!=(rhs); }
};
*/
static double accelerationDueToGravity(double m, double d)
{
    return BigG() * m / (d*d);
}

static double escapeVelocity(double m, double d)
{
    return sqrt(2.0 * BigG() * m / d);
}

static double orbitVelocity(double m, double d)
{
    return sqrt(BigG() * m / d);
}

static double orbitPeriod(double a, double m)
{
    return 2.0 * Pi() * sqrt(a * a * a / (BigG() * m));
}

static double forceDueToGravity(double m1, double m2, double d)
{
    return BigG() * m1 * m2 / (d*d);
}

static double deltaS(double u, double t, double force, double mass)
{
    return u*t + t*t*0.5*force/mass;
}

static double deltaV(double t, double force, double mass)
{
    return t*force/mass;
}

class Object
{
public:
    const Position& Pos(void) const { return m_Pos; }
    void setPos(const Position& pos) { m_Pos = pos; }
    const Velocity& Vel(void) const { return m_Vel; }
    void setVel(const Velocity& vel) { m_Vel = vel; }
    double Mass(void) const { return m_Mass; }
    void setMass(double m) { m_Mass = m; }
    Object(const Position& pos, const Velocity& vel, double mass)
        : m_Pos(pos), m_Vel(vel), m_Mass(mass) {}
    string Heisenberg(void)
        {
            return "Position: " + m_Pos.DumpCart() + ". Velocity: " + m_Vel.DumpCart() + ".";
        }
    double distanceBetween(const Object& o) const;
    double directionBetween(const Object& o) const;
    Force getResultantForce(vector<Object*> objects) const;
    void applyForce(const Force& f, double t);
private:
    Position m_Pos;
    Velocity m_Vel;
    double m_Mass;
};
double Object::distanceBetween(const Object& o) const
{
    double dx = o.m_Pos.x() - m_Pos.x();
    double dy = o.m_Pos.y() - m_Pos.y();
    return c2p_radius(dx, dy);
}
double Object::directionBetween(const Object& o) const
{
    double dx = o.m_Pos.x() - m_Pos.x();
    double dy = o.m_Pos.y() - m_Pos.y();
    return c2p_angle(dx, dy);
}
Force Object::getResultantForce(vector<Object*> objects) const
{
    // I am in list
    Force resultant(0, 0);
    for(vector<Object*>::const_iterator it = objects.begin(); it != objects.end(); ++it)
    {
        const Object* pO = *it;
        if (pO->m_Pos != m_Pos)
        {
            double f = forceDueToGravity(pO->m_Mass, m_Mass, distanceBetween(*pO));
            double a = directionBetween(*pO);
            double x = p2c_x(f, a);
            double y = p2c_y(f, a);
            resultant += Force(x, y);
        }
    }
    return resultant;
}

void Object::applyForce(const Force& f, double interval)
{
    double sdeltaX = deltaS(m_Vel.x(), interval, f.x(), m_Mass);
    double sdeltaY = deltaS(m_Vel.y(), interval, f.y(), m_Mass);
    double vdeltaX = deltaV(interval, f.x(), m_Mass);
    double vdeltaY = deltaV(interval, f.y(), m_Mass);

    m_Pos.incX(sdeltaX);
    m_Pos.incY(sdeltaY);
    m_Vel.incX(vdeltaX);
    m_Vel.incY(vdeltaY);
}

struct Deriv
{
    Velocity dx;
    Vector dv;
};

Vector acceleration(const Object& o, double t, vector<Object*> objList)
{
    Force f = o.getResultantForce(objList);
    return f/o.Mass();
}

Deriv evaluate(Object& o, double t, double dt, const Deriv &d, vector<Object*> objList)
{
    o.setPos(o.Pos() + (d.dx.operator*(dt)));
    o.setVel(o.Vel() + (d.dv.operator*(dt)));

    Deriv output;
    output.dx = o.Vel();
    output.dv = acceleration(o, t+dt, objList);
    return output;
}


static bool Newton(vector<Object*>& objList, vector<Force>& forces, double interval, bool bPrint)
{
    /*
    should have this in for safety, but it slows it down...
    if (forces.size() < objList.size())
    {
        cout << "Can't continue - size - forces " << forces.size() << " objList " << objList.size() << endl;
        return false;
    }*/
 
    if (bPrint)
    {
        const Object* pO2 = objList.back();
        cout << "Loc: " << pO2->Pos().DumpPolar().c_str()
             << "\tSpd:\t" << pO2->Vel().radius()
             << "\tDir:\t" << pO2->Vel().angleDeg();
    }

    for(int i=0; i<objList.size(); ++i)
        forces[i] = objList[i]->getResultantForce(objList);
 
    for(int i=0; i<objList.size(); ++i)
        objList[i]->applyForce(forces[i], interval);
    
    if (bPrint)
        cout << endl;

    return true;
}

void blast(double tick, long reps)
{
    double gEarth    = accelerationDueToGravity(massE(), radiusE());
    double gLEO      = accelerationDueToGravity(massE(), radiusLEO());
    double gMoon     = accelerationDueToGravity(massM(), radiusM());
    double gLMO      = accelerationDueToGravity(massM(), radiusLMO());
    double escapeE   = escapeVelocity(massE(), radiusE());
    double escapeM   = escapeVelocity(massM(), radiusM());
    double massA     = 3000000.0;
    double orbitE   =  orbitVelocity(massE(), radiusE());

    Object earth(Position(0.0, 0.0), Velocity(0.0, 0.0), massE());
    Object apollo11(Position(radiusE(), 0.0), Velocity(0.0, orbitE), massA);
    vector<Object*> objList;
    objList.push_back(&earth);
    objList.push_back(&apollo11);
    // define once for speed
    vector<Force> forces(objList.size());
    //while (true)
    bool half = false;
    for (int i=0; i<=reps; ++i)
    {
        if (!Newton(objList, forces, tick, i==reps || i==0)) break;
        if (!half && apollo11.Pos().y() < 0.0)
            half = true;
        if (half && apollo11.Pos().y() >= 0.0)
        {
            if (Newton(objList, forces, tick, true))
            {
                cout << "t (secs): " << (i*tick) << endl;
            }
            break;
        }
    }
    double orbitPE = orbitPeriod(radiusE(), massE());
    cout << "Theoretical Orbit Period Earth Radius: " << orbitPE << endl;
}

int main( int argc, const char* argv[] )
{
    cout << "T=1" << endl;
    blast(1.0, 100000);
    cout << "T=0.01" << endl;
    blast(0.01, 2000000);
    cout << "T=0.001" << endl;
    blast(0.001, 200000000);
    cout << "T=0.0001" << endl;
    blast(0.0001, 200000000);

    return 0;
}

