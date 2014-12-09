from sys import argv
from array import array
from sympy import *
#---------------------COMMAND LINE ARGUMENT LIST-------------------------------
#argv(1) = from x
#argv(2) = to x
#argv(3) = from y 
#argv(4) = to y

#argv(5) = resolution
#argv(6) = function
#argv(7) = x0 (point at which to find tangent line)
#argv(8) = y0 (point at which to find tangent line)
#------------------------------------------------------------------------------

lowRangeX = float(argv[1])  
highRangeX = float(argv[2])
lowRangeY = float(argv[3])
highRangeY = float(argv[4])
resolution = int(argv[5])

#sympy requirements
x = symbols('x')               #create sympy symbols for expressions
y = symbols('y')
expr = sympify(argv[6])        #convert argument function to usable sympy expr
expr1 = diff(expr,x)
expr = expr1


#find the actual ranges of the x and y axes
rangeX = highRangeX - lowRangeX
rangeY = highRangeY - lowRangeY

#find the increments of the x and y axes
incrementX = rangeX / (resolution - 1)  # (-1) because you're splitting the distance to 1 less
incrementY = rangeY / (resolution - 1)  # parts than the resolution (EX. 4 sections between 5 points)

#initialize array of X values
arrayX = array('d',[0] * resolution)      #empty array of size (resolution)
for i in range(0, resolution):
	arrayX[i] = lowRangeX + (i * incrementX)  #re-draw array with required points
	
#initialize array of Y values
arrayY = array('d',[0] * resolution)      #empty array of size (resolution)
for i in range(0, resolution):
	arrayY[i] = lowRangeY + (i * incrementY)	
	
#before evaluating points, open file for writing.
resultList = open("temp.txt","w")



#evaluate each coordinate pair's z-axis counterpart
for i in range(0,resolution):
		for j in range(0,resolution):
		
			#Printing results for purpose of debugging.
			#print "x: " + str(arrayX[i])
			#print "y: " + str(arrayY[j])
			#print "z: " + str(expr.subs([(x,arrayX[i]),(y,arrayY[j])]))
			
			#write the results to file (Format "x,y,z\n")
			resultList.write(str(arrayX[i]) + "," + str(arrayY[j]) + ',' +
			                 str(expr.subs([(x,arrayX[i]),(y,arrayY[j])])) + '\n')
			
#close file
resultList.close()

#alternative notifier that python is done-zo
doneFile = open("doneFile.txt","w")
doneFile.write("ok")
doneFile.close()

	
	


