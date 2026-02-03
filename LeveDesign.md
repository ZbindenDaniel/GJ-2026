x-a is a random variable but not a
x is a random variable but not none, y, z
y is a random variable but not none, x,z
z is a random variable but not none, y,x

(note that the npc 
- node if two or more are correct)
- neutral if one is correct
- shake head when none is correct
- last ist the color, if not mentioned, all have the same or no color


p is player mask

npc is a npc mask

e is the elevator mask

 $"M{shapeCode}.{mouthCode}{eyeCode}";
 is the code
 (shapeCode,mouthCode,eyeCode) is how its written in this file.

 n means none

*2 means that this npc is two times there.

the elevator number has nothing to do with the index and gets randomly distributed.

e1=p means that the elevator has the same as the player
e2=npc1 means that the elveator has the same as npc1


# Level 1
p=(x,n,n)

npc1=(x,n,n)*2
npc2=(y,n,n)*2
npc3=(z,n,n)*2

e1=npc1
e2=npc2
e3=npc3

# Level 2
p=(x,n,n)

npc2=(y,n,n)*2
npc3=(z,n,n)*2

e1=p
e2=npc2
e3=npc3


# Level 3
p=(x,x,x)

npc1=(x,n,n)
npc2=(y,n,n)
npc3=(z,n,n)
npc4=(x,y,y)
npc5=(z,x,x)
npc6=(z,y,y)

e1=(x,x,x)
e2=(x,y,y)
e3=(z,x,x)


# Level 4

Starting at this level, there is no none anymore

7 npc
3 evelator

# Level 5

7 npc
4 evelator

# Level 6

Some additional message like congrates, the easy rounds are over, now its getting hard.

p=(x,x,x)
npc1=(x,x,y)
npc2=(y,x,x)
npc3=(x,z,x)

e1=(x,x,x)
e2=(x,y,y)
e3=(y,z,z)
e4=(y,x,x)

# Level 7


p=(x,x,x)
npc1=(x,x,y)


e1=(x,x,x)
e2=(x,y,y)
e3=(y,x,z)
e4=(y,z,y)

# Level 8

Here, we start with different colors

p=(x,x,x,x)
4 elevators
8 npc
should be solvable



# Level 9

Here, we start with different colors

p=(x,x,x,x)
4 elevators
6 npc
should be solvable



# Level 10

Message: Final level: Can you get the neutral level?

All npc only have one correct.


P=(x,x,x,x)

npc1=(x,z,z,y)
npc3=(y,x,z,y)
npc5=(y,z,x,y)
npc7=(y,z,z,x)
npc8=(y,z,y,x)



e1=(x,x,x,x)
e2=(z,y,y,y)
e3=(z,x,y,z)
e4=(y,y,y,z)

# Level 11

Message: Congrates, you reached the end. Keep playing? UI popup


Just generate more difficult but solvable levels with 4 different elevators