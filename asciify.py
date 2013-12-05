#!/usr/bin/python

import re
import sys

binary_term = re.compile("(\w+)\(([\d\w]+),([\d\w]+),([\d\w]+)\)")

def display_maze(facts):
  """turn a list of ansprolog facts into a nice ascii-art maze diagram"""
  max_x_i = 1
  max_y_i = 1
  max_x_o = 1
  max_y_o = 1
  
  tile = {}
  floor = {}
  startTile = {}
  wallX = {}
  wallY = {}
  tree = {}

  for fact in facts:
	m = binary_term.match(fact)
	if m:
	  functor, x, y, l = m.groups()
	  x, y = int(x), int(y)
	  pos = (x,y,l)
	  
	  if(l == "0"):
		max_x_i, max_y_i = max(x, max_x_i), max(y, max_y_i)
	  if(l == "1"):
		max_x_o, max_y_o = max(x, max_x_o), max(y, max_y_o)
	  if functor == "tile":
		tile[pos] = True
	  if functor == "floor":
		floor[pos] = True
	  if functor == "wallX":
		wallX[pos] = True
	  if functor == "wallY":
		wallY[pos] = True
	  if functor == "levelStart":
		startTile[pos] = True
	  if functor == "tree":
		tree[pos] = True

  def code(x,y,l):
	"""decide how a maze cell should be typeset"""
	pos = (x,y,l)
	returnchar = " " #every tile should contain something so the default s
	if pos in floor:
	  returnchar = "."
	if pos in tree:
	  returnchar = "%"
	if pos in startTile:
	  returnchar = "s"
	return returnchar

  #display indoors	
  for y in range(0,max_y_i+1):
	#print top wall
	line = ""
	for x in range(0, max_x_i+1):
		pos = (x,y,"0")
		if(pos in wallX):
			line = line + " -"
		else:
			line = line + "  "
	print line
	
	line = ""
	
	for x in range(0, max_x_i+1):		
		pos = (x,y,"0")
		
		if(pos in wallY):
			line = line + "|"
		else:
			line = line + " "
		
		line = line + code(x,y,"0")
	print line
	
  print("\n\n\n\n")
  
  #display outdoors	
  for y in range(0,max_y_o+1):
	
	line = ""
	
	for x in range(0, max_x_o+1):		

		line = line + code(x,y,"1")
	print line

def main():
  """look for lines that contain logical facts and try to turn each of those
  into a maze"""
  
  for line in sys.stdin.xreadlines():
	line = line.strip()
	if line:
	  if line[0].islower():
		facts = line.split(' ')
		display_maze(facts)
	  else:
		print "% " + line

if __name__ == "__main__":
  main()
