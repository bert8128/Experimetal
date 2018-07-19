import math
import copy

class Vector2:
    def __init__(self, x=None, y=None):
        if x is None:
            self.x=0.0
            self.y=0.0
        else:
            self.x=float(x)
            self.y=float(y)

    def dot(self, v2):
        return self.x*v2.x + self.y*v2.y

    def magSqr(self):
        return self.x*self.x + self.y*self.y
        
    def mag(self):
        return math.sqrt(self.magSqr())
        
    def unit(self):
        mag = self.mag()
        return Vector2(self.x/mag, self.y/mag)
        
    def distSqr(self, v2):
        dx = self.x-v2.x
        dy = self.y-v2.y
        return dx*dx + dy*dy
        
    def distance(self, v2):
        return math.sqrt(self.distSqr(v2))
        
    def tangent(self):
        return Vector2(-1.0*self.y, self.x)

    def plus(self, v):
        v2 = copy.copy(self)
        v2.x += v.x
        v2.y += v.y
        return v2

    def plus_ip(self, v2):
        self.x += v2.x
        self.y += v2.y

    def sub(self, v):  
        v2 = copy.copy(self)
        v2.x -= v.x
        v2.y -= v.y
        return v2

    def sub_ip(self, v2):
        self.x -= v2.x
        self.y -= v2.y

    def scale(self, factor):
        v2 = copy.copy(self)
        v2.x *= float(factor)
        v2.y *= float(factor)
        return v2
        
    def scale_ip(self, factor):
        self.x *= float(factor)
        self.y *= float(factor)
        
    def oneX(self):
        if self.x >= 0.0:
            return 1.0;
        return -1.0;
        
    def oneY(self):
        if self.y >= 0.0:
            return 1.0;
        return -1.0;
    
    @staticmethod
    def calcCollision(p1, p2, v1, v2, m1, m2):
        p = p1.sub(p2)
        un = p.unit()
        ut = un.tangent()
        
        v1n = un.dot(v1)
        v1t = ut.dot(v1)
        v2n = un.dot(v2)
        v2t = ut.dot(v2)
        
        v1ta = v1t
        v2ta = v2t
        v1na = ( v1n*(m1 - m2) + 2*m2*v2n ) / (m1 + m2)
        v2na = ( v2n*(m2 - m1) + 2*m1 *v1n ) / (m1 + m2)
        
        v1nav = un.scale(v1na)
        v1tav = ut.scale(v1ta)
        v2nav = un.scale(v2na)
        v2tav = ut.scale(v2ta)
        
        v1a = v1nav.plus(v1tav)
        v2a = v2nav.plus(v2tav)
        
        return v1a, v2a


if __name__ == "__main__":
        v01 = Vector2(math.sqrt(8), 0)
        v02 = Vector2(0, 0)
        eb = 0.5*1.0*v01.mag()*v01.mag() + 0.5*1.0*v02.mag()*v02.mag()
        root2 = math.sqrt(2.0)
        v1, v2 = Vector2.calcCollision(Vector2(-root2, root2), Vector2(0,0), v01, v02, 1, 1)
        ea = 0.5*1.0*v1.mag()*v1.mag() + 0.5*1.0*v2.mag()*v2.mag()
        print(v1.x, v1.y, v2.x, v2.y, eb, ea)
        exit(0)
