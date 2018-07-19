from Vector2 import Vector2
import pygame
import random
import datetime
import copy
import math

# Define some colors
BLACK = (0, 0, 0)
RED = (255, 0, 0)
WHITE = (255, 255, 255)

wwidth = 10.0 # world is 10m wide
max_start_wheight = 6.5 # start height is up to 8m
ceiling = max_start_wheight*2.0 # off screen ceiling
fpa=100
g=Vector2(0.0,-9.81)
max_energy=g.y*max_start_wheight*-1.0

totalEnergy = 0.0 
totalCalcEnergy = 0.0 

class Ball:
    """
    Class to keep track of a ball's location and vector.
    """
    def __init__(self):
        self.p = Vector2(0.0, 0.0) # position in metres
        self.pb = Vector2(0.0, 0.0) # previous position in metres
        self.v = Vector2(0.0, 0.0) # velocity metres per second
        self.collide = False
        self.redness = 255
        self.density = 1000.0 #grams/m^2
        self.ball_size = random.random()*0.1 + 0.05 #metres
        self.mass = math.pi*self.ball_size*self.ball_size*self.density #grams
        self.energy = self.calcEnergy() # joules
        
    def initPos(self):
        self.p.x = random.randrange(int(self.ball_size), int(wwidth - self.ball_size))
        self.p.y = random.randrange(int(self.ball_size), int(max_start_wheight - self.ball_size))
        self.pb = self.p
        self.energy = self.calcEnergy()
        
    def initPosXY(self, x, y):
        self.p.x = float(x)
        self.p.y = float(y)
        self.pb = self.p
        self.energy = self.calcEnergy()
        
    def randspeed(self, lo, hi, step):
        speed=0.0
        while speed==0.0:
            speed = random.randrange(lo, hi, step) # metres per second
        return speed
    
    def initVel(self, maxEnergy):
        while True:
            #everything is ,ultiplied by 100 as andrange takes ints only
            self.v.x = random.randrange(-200, 300, 1)/100.0
            self.v.y = self.randspeed(-300, 400, 1)/100.0
            self.energy = self.calcEnergy()
            if self.energy < maxEnergy:
                break
        
    def initVelV(self, vx, vy):
        self.v.x =  float(vx)
        self.v.y =  float(vy)
        self.energy = self.calcEnergy()
        
    def move(self, timeDelta):
        self.pb = copy.deepcopy(self.p)
        v2 = self.v.scale(timeDelta)
        self.p.plus_ip(v2)
        
    def accelerate(self, delta, timeDelta):
        a2 = delta.scale(timeDelta)
        self.v.plus_ip(a2)
        
    def scaleV(self, f_x, f_y):
        self.v.x *= float(f_x)
        self.v.y *= float(f_y)
        
    def ke(self):
        return 0.5 * self.mass * self.v.magSqr()
 
    def pe(self):
        return -1.0 * self.mass * g.y * self.p.y
        
    def calcEnergy(self):
        return self.pe() + self.ke()
    
def make_ball():
    """  
    Function to make a new, random ball.
    """
    global totalEnergy
    global totalCalcEnergy

    ball = Ball()
    ball.initPos()
    ball.initVel(max_energy*ball.mass)

    totalEnergy += ball.energy
    totalCalcEnergy += ball.energy
 
    return ball

def make_test_ball():
    """  
    Function to make a new, non-random ball.
    """
    global totalEnergy
    global totalCalcEnergy

    ball = Ball()
    ball.initPosXY(1.0, 6.0)
    ball.initVelV(0.0, -1.0)
    totalEnergy += ball.energy
    totalCalcEnergy += ball.energy
 
    return ball

def calcCollision(p1, p2, v1, v2, m1, m2):
    return Vector2.calcCollision(p1, p2, v1, v2, m1, m2)

def handleCollisions(balls, collisions):
    global totalEnergy
    global totalCalcEnergy

    for i in collisions:
        j = collisions[i]
        ballsEnergyBefore = balls[i].energy + balls[j].energy

        v1a, v2a = calcCollision(balls[i].p, balls[j].p, balls[i].v, balls[j].v, balls[i].mass, balls[j].mass)
        balls[i].v = v1a   
        balls[j].v = v2a

        eI = balls[i].calcEnergy()
        eJ = balls[j].calcEnergy()

        energyGain = eI + eJ - ballsEnergyBefore
        balls[i].energy = eI
        balls[j].energy = eJ
        if energyGain >= 0.0:
            # take it from each ball's, pro-rata by their ke
            ike = balls[i].ke()
            totKe = ike+balls[j].ke()
            if totKe > 0.0:
                iGain = energyGain * ike / totKe
                jGain = energyGain - iGain

                balls[i].energy -= iGain
                balls[j].energy -= jGain

                newIke = balls[i].ke()
                oldIke = newIke - iGain
                if newIke > 0.0 and oldIke >= 0.0:
                    mult = math.sqrt(oldIke/newIke)
                    balls[i].v.scale_ip(mult)

                newJke = balls[j].ke()
                oldJke = newJke - jGain
                if newJke > 0.0 and oldJke >= 0.0:
                    mult = math.sqrt(oldJke/newJke)
                    balls[j].v.scale_ip(mult)

        totalEnergy -= ballsEnergyBefore
        totalEnergy += balls[i].energy
        totalEnergy += balls[j].energy
        totalCalcEnergy -= ballsEnergyBefore
        totalCalcEnergy += balls[i].calcEnergy()
        totalCalcEnergy += balls[j].calcEnergy()

def updateBalls(balls, timeDelta):
    cols = {}
    revcols = {}
    for ball in balls:
        ball.collide = False
        ball.move(timeDelta)
        ball.accelerate(g, timeDelta)

        if ball.p.y >= ceiling - ball.ball_size or ball.p.y <= ball.ball_size:
            ball.v.y *= -1.0
            # Fudges - if the balls go off screen bring them back
            if ball.p.y > ceiling - ball.ball_size:
                ball.p.y -= ball.p.y - (ceiling - ball.ball_size)
            if ball.p.y < ball.ball_size:
                ball.p.y += ball.ball_size - ball.p.y
        if ball.p.x >= wwidth - ball.ball_size or ball.p.x <= ball.ball_size:
            ball.v.x *= -1.0
            # Fudges - if the balls go off screen bring them back
            if ball.p.x > wwidth - ball.ball_size:
                ball.p.x -= ball.p.x - (wwidth - ball.ball_size)
            if ball.p.x < ball.ball_size:
                ball.p.x += ball.ball_size - ball.p.x

        # Fudge - make sure that the total energy of the ball doesn't change by scaling the velocity
        newEnergy = ball.calcEnergy()
        ke = ball.ke()
        energyGain = newEnergy - ball.energy
        prevKE = ke - energyGain
        if ke > 0.0: 
            mult = math.sqrt(prevKE/ke)
            ball.v.scale_ip(mult)
        
    numballs = len(balls)
    for i in range (0, numballs):
        if balls[i].p.x <= 0.0 or balls[i].p.x >= wwidth or balls[i].p.y <= 0.0 or balls[i].p.y >= ceiling:
            break # fudge - if the balls go outside the container then they never collide
        for j in range (i+1, numballs):
            # this isn't great.  The step size means that there may be two or more balls all colliding
            # with this ball. we should collide with the cosest ball. but this algo just collides
            # with the first one it finds
            # note that a ball can only ever collide with one ball in a single frame.
            # maybe that is the fix - handle every collision pair 
            twoRadii = balls[i].ball_size + balls[j].ball_size
            minDistSqrd = twoRadii * twoRadii
            distSqrd = balls[i].p.distSqr(balls[j].p)
            if distSqrd <= minDistSqrd:
                if i not in cols and j not in revcols and i not in revcols:
                    distSqrd0 = balls[i].pb.distSqr(balls[j].pb)
                    if distSqrd < distSqrd0: # closer than it was before?
                        cols[i] = j
                        revcols[j] = i
                        balls[i].redness = 0
                        balls[j].redness = 0
                        break
                    
    handleCollisions(balls, cols)
    
def drawBalls(screen, balls, ppm, v_ceiling):
    for ball in balls:
        x = ball.p.x*ppm
        y = float(v_ceiling)-(ball.p.y*ppm)
        sz = ball.ball_size*ppm
        pygame.draw.circle(screen, (255, ball.redness, ball.redness), [int(x), int(y)], int(sz))
        if ball.redness < 255:
            ball.redness += fpa / 5
        if ball.redness > 255:
            ball.redness = 255

def main():
    """
    This is our main program.
    """
    global totalEnergy
    global totalCalcEnergy

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
 
    totalEnergy = 0.0
    totalCalcEnergy = 0.0
    balls = []
 
    ball = make_ball()
    balls.append(ball)

    start = datetime.datetime.now().timestamp()

    # -------- Main Program Loop -----------
    while not done:
        end = datetime.datetime.now().timestamp()
        timeDelta = end - start
        start = end
        #timeDelta = 0.05
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
 
        drawBalls(screen, balls, pixels_per_metre, SCREEN_HEIGHT)

        print(totalEnergy, totalCalcEnergy)
  
        clock.tick(fpa)
 
        # Go ahead and update the screen with what we've drawn.
        pygame.display.flip()
 
    # Close everything down
    pygame.quit()
 
if __name__ == "__main__":
    main()
