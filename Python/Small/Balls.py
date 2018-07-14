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
        
    def test(self):
        v1 = Vector2(2,3)
        v2 = Vector2(4,5)
        
        v3 = v1.unit()

import pygame
import random
import datetime
#import Vector2

# Define some colors
BLACK = (0, 0, 0)
RED = (255, 0, 0)
WHITE = (255, 255, 255)

wwidth = 10.0 # world is 10m wide
max_start_wheight = 8.0 # start height is up to 8m
ceiling = max_start_wheight*2.0
fpa=10
g=Vector2(0.0,-9.81)
max_energy=g.y*max_start_wheight*-1.0

class Energy:
    def __init__(self):
        self.e = 0

e = Energy()

class Ball:
    """
    Class to keep track of a ball's location and vector.
    """
    def __init__(self):
        self.p = Vector2(0.0, 0.0) # position in metres
        self.pb = Vector2(0.0, 0.0) # previous position in metres
        self.v = Vector2(0.0, 0.0) # velocity metres per second
        self.energy=0.0  #joules
        self.collide = False
        self.redness = 255
        self.mass=100 #grams
        self.ball_size = 0.5 # 0.5m
        self.rect = pygame.Rect(0, 0, self.ball_size*2, self.ball_size*2)
        
    def initPos(self):
        self.p.x = random.randrange(int(self.ball_size), int(wwidth - self.ball_size))
        self.p.y = random.randrange(int(self.ball_size), int(max_start_wheight - self.ball_size))
        self.pb = self.p
        
    def randspeed(self, lo, hi, step):
        speed=0.0;
        while speed==0.0:
            speed = random.randrange(lo, hi, step) # metres per second
        return speed
    
    def initVel(self, maxEnergy):
        nrg = 0.
        while True:
            #everything is ,ultiplied by 100 as andrange takes ints only
            self.v.x = random.randrange(-200, 300, 1)/100.0
            self.v.y = self.randspeed(-300, 400, 1)/100.0
            self.energy = self.calcEnergy()
            if self.energy < maxEnergy:
                break
        
    def move(self, timeDelta):
        self.pb = copy.deepcopy(self.p)
        v2 = self.v.scale(timeDelta)
        self.p.plus_ip(v2)
        self.rect.x=self.p.x-self.ball_size;
        self.rect.y=self.p.y-self.ball_size;
        
    def accelerate(self, delta, timeDelta):
        a2 = delta.scale(timeDelta)
        self.v.plus_ip(a2)
        
    def scaleV(self, f_x, f_y):
        self.v.x *= f_x
        self.v.y *= f_y
        
    def ke(self):
        return 0.5 * self.v.magSqr()
 
    def pe(self):
        return -1.0*g.y*self.p.y
        
    def calcEnergy(self):
        return self.pe() + self.ke()
    
    def getRect(self):
        return pygame.Rect(left, top, width, height)
        
def make_ball():
    """  
    Function to make a new, random ball.
    """
    ball = Ball()
    ball.initPos()
    ball.initVel(max_energy)

    e.e += ball.energy
 
    return ball

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

def handleCollisions(balls, collisions):
    for i in collisions:
        j = collisions[i]
        v1a, v2a = calcCollision(balls[i].p, balls[j].p, balls[i].v, balls[j].v, balls[i].mass, balls[j].mass)
        ebefore = e.e  
        e.e -= balls[i].energy
        e.e -= balls[j].energy
        balls[i].v = v1a   
        balls[j].v = v2a
        balls[i].energy = balls[i].calcEnergy()
        balls[j].energy = balls[j].calcEnergy()
        e.e += balls[i].energy
        e.e += balls[j].energy
        eafter = e.e
        
def updateBalls(balls, timeDelta):
    cols = {}
    revcols = {}
    for ball in balls:
        ball.collide = False
        ball.move(timeDelta)
        ball.accelerate(g, timeDelta)

        if ball.p.y >= ceiling - ball.ball_size or ball.p.y <= ball.ball_size:
            ball.v.y *= -1
            # Fudges - if the balls go off screen bring them back
            if ball.p.y > ceiling - ball.ball_size:
                ball.p.y -= ball.p.y - (ceiling - ball.ball_size)
            if ball.p.y < ball.ball_size:
                ball.p.y += ball.ball_size - ball.p.y
        if ball.p.x >= wwidth - ball.ball_size or ball.p.x <= ball.ball_size:
            ball.v.x *= -1
            # Fudges - if the balls go off screen bring them back
            if ball.p.x > wwidth - ball.ball_size:
                ball.p.x -= ball.p.x - (wwidth - ball.ball_size)
            if ball.p.x < ball.ball_size:
                ball.p.x += ball.ball_size - ball.p.x

        # Fudge - make sure that the total energy of thw ball doesnt chnage by scaling the velocity
        mult = ball.energy/ball.calcEnergy()
        ball.scaleV(1, math.sqrt(mult)) # is the sqrt necessary for a simple sim?
        
    numballs = len(balls)
    for i in range (0, numballs):
        if balls[i].p.x <= 0 or balls[i].p.x >= wwidth or balls[i].p.y <= 0 or balls[i].p.y >= ceiling:
            break # fudge - if the balls go outside the container then they never collide
        sqrdist=2.0*2.0*balls[i].ball_size
        for j in range (i+1, numballs):
            # this isn't great.  The step size means that there may be two or more balls all colliding
            # with this ball. we should collide with the cosest ball. but this algo just collidez
            # with the first one it finds
            # note that a ball can only ever collide with one ball in a single frame.
            # maybe that is the fix - handle every collision pair 
            sqrdist *= balls[j].ball_size 
            dist = balls[i].p.distSqr(balls[j].p)
            if dist <= sqrdist:
                if i not in cols and j not in revcols and i not in revcols:
                    dist0 = balls[i].pb.distSqr(balls[j].pb)
                    if dist < dist0:
                        cols[i] = j
                        revcols[j] = i
                        balls[i].redness = 0
                        balls[j].redness = 0
                        break
                    
    handleCollisions(balls, cols)
    
def drawBalls(screen, balls, ppm):
    for ball in balls:
        x = ball.p.x*ppm
        y = (max_start_wheight - ball.p.y)*ppm
        sz = ball.ball_size*ppm
        print(ball.p.x, ball.p.y, x, y)
        pygame.draw.circle(screen, (255, ball.redness, ball.redness), [int(x), int(y)], int(sz))
        if ball.redness < 255:
            ball.redness += fpa / 5
        if ball.redness > 255:
            ball.redness = 255

def main():
    """
    This is our main program.
    """
    pygame.init()
 
    # Set the height and width of the screen
    SCREEN_WIDTH = 800
    SCREEN_HEIGHT = 500
    pixels_per_metre = float(SCREEN_WIDTH)/wwidth #pixels per metre
    size = [SCREEN_WIDTH, SCREEN_HEIGHT]
    screen = pygame.display.set_mode(size)
 
    pygame.display.set_caption("Bouncing Balls")
  
    # Loop until the user clicks the close button.
    done = False
 
    # Used to manage how fast the screen updates
    clock = pygame.time.Clock() 
 
    balls = []
 
    ball = make_ball()
    balls.append(ball)

    start = datetime.datetime.now().timestamp()

    print(pixels_per_metre, ceiling, max_start_wheight)

    # -------- Main Program Loop -----------
    while not done:
        end = datetime.datetime.now().timestamp()
        timeDelta = end - start
        start = end
        # --- Event Processing
        for event in pygame.event.get():
            if event.type == pygame.QUIT:
                done = True
            elif event.type == pygame.KEYDOWN:
                # R - reset to 1 ball
                if event.key == pygame.K_r:
                    balls = []
                    ball = make_ball()
                    balls.append(ball)
                # Space bar! Spawn a new ball.
                elif event.key == pygame.K_SPACE:
                    ball = make_ball()
                    balls.append(ball)
 
        updateBalls(balls, timeDelta)
 
        screen.fill(BLACK)
 
        drawBalls(screen, balls, pixels_per_metre)
  
        clock.tick(fpa)
 
        # Go ahead and update the screen with what we've drawn.
        pygame.display.flip()
 
    # Close everything down
    pygame.quit()
 
if __name__ == "__main__":
    main()
