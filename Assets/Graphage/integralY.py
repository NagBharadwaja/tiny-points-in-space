
from sys import argv
from array import array
from sympy import *
import sys

#---------------------COMMAND LINE ARGUMENT LIST-------------------------------
#argv(1) = minX
#argv(2) = maxX
#argv(3) = minY
#argv(4) = maxY
#argv(5) = resolution
#argv(6) = function
#argv(7) = y0
#argv(8) = y1
#------------------------------------------------------------------------------


#sympy requirements
x = symbols('x')               #create sympy symbols for expressions
y = symbols('y')
z = symbols('z') 

#double refuse to work in place of float
try:
	minX = float(argv[1]) 
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error");
	resultList.close();
	sys.exit()
	
try:
	maxX = float(argv[2])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error");
	resultList.close();
	sys.exit()
try:
	minY = float(argv[3])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error");
	resultList.close();
	sys.exit()
try:
	maxY = float(argv[4])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error");
	resultList.close();
	sys.exit()	
try:
	resolution = int(argv[5])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error");
	resultList.close();
	sys.exit()

try:
	expr = sympify(argv[6])        #convert argument function to usable sympy expr
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error The expression is not working");
	resultList.close();
	sys.exit()
try:	
	y0 = float(argv[7])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error in y0");
	resultList.close();
	sys.exit()
try:	
	y1 = float(argv[8])
except ValueError:
	resultList = open("temp.txt","w")
	resultList.write("Error in y1");
	resultList.close();
	sys.exit()


#find the actual ranges of the x and y axes
rangeX = maxX - minX
rangeY = maxY - minY
 
#find the increments of the x and y axes
incrementX = rangeX / (resolution - 1)  # (-1) because you're splitting the distance to 1 less
incrementY = rangeY / (resolution - 1)  # parts than the resolution (EX. 4 sections between 5 points)
 
#initialize array of X values
arrayX = array('d',[0] * resolution)      #empty array of size (resolution)
for i in range(0, resolution):
    arrayX[i] = minX + (i * incrementX)  #re-draw array with required points
     
#initialize array of Y values
arrayY = array('d',[0] * resolution)      #empty array of size (resolution)
for i in range(0, resolution):
    arrayY[i] = minY + (i * incrementY)   
     
resultList = open("temp.txt","w")

area = integrate(expr,(y,y0,y1))
inter = integrate(expr,y)
resultList.write("Area: " + str(area) + '\n')
for i in range(0,resolution):
	for j in range(0,resolution):
		resultList.write(str(arrayX[i]) + "," + str(arrayY[j]) + ',' + str(inter.subs([(x,arrayX[i]),(y,arrayY[j])])) + '\n')
		#print str(arrayX[i]) + "," + str(arrayY[j]) + ',' + str(expr.subs([(x,arrayX[i]),(y,arrayY[j])])) + '\n'

#close file
resultList.close()  