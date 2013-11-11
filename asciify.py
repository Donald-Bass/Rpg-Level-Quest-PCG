#!/usr/bin/python

import re
import sys

binary_term = re.compile("(\w+)\(([\d\w]+),([\d\w]+),([\d\w]+)\)")

def display_maze(facts):
  """turn a list of ansprolog facts into a nice ascii-art maze diagram"""
  max_x = 1
  max_y = 1
  tile = {}
  floor = {}
  startTile = {}
  wallX = {}
  wallY = {}

  for fact in facts:
	m = binary_term.match(fact)
	if m:
	  functor, x, y, l = m.groups()
	  x, y = int(x), int(y)
	  pos = (x,y)
	  max_x, max_y = max(x, max_x), max(y, max_y)
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

  def code(x,y):
	"""decide how a maze cell should be typeset"""
	pos = (x,y)
	returnchar = " " #every tile should contain something so the default s
	if pos in floor:
	  returnchar = "."
	if pos in startTile:
	  returnchar = "s"
	return returnchar

  for y in range(0,max_y+1):
	#print top wall
	line = ""
	for x in range(0, max_x+1):
		pos = (x,y)
		if(pos in wallX):
			line = line + " -"
		else:
			line = line + "  "
	print line
	
	line = ""
	
	for x in range(0, max_x+1):		
		pos = (x,y)
		
		if(pos in wallY):
			line = line + "|"
		else:
			line = line + " "
		
		line = line + code(x,y)
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
