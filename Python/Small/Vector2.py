import math
import copy

class Vector2:
    def __init__(self, x=None, y=None):
        if x is None:
            self.x=0.0
            self.y=0.0
        else:
            self.x=x
            self.y=y

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
        
    def tangent(self):
        return Vector2(-1*self.y, self.x)

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
        v2.x *= factor
        v2.y *= factor
        return v2
        
    def scale_ip(self, factor):
        self.x *= factor
        self.y *= factor
        
    def oneX(self):
        if self.x >= 0.0:
            return 1.0;
        return -1.0;
        
    def oneY(self):
        if self.y >= 0.0:
            return 1.0;
        return -1.0;

 
if __name__ == "__main__":
        v1 = Vector2(2,4)
        v3 = v1.unit()
        if v3.x != 5:
            exit(-1)
        exit(0)
