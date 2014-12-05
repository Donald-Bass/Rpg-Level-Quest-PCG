@ECHO OFF
REM This is a very simple bat file to run clingo on a windows machine. (The parameters clingo uses in othr OS's should be the same, but the rest will probally have to be rewritten)
REM The "@ECHO OFF" line stops windows from outputing all the commands entered by this batch file to the console

set seed=%RANDOM%
				  REM Clingo will always output the same result unless we give it a parameter to add a bit of randomness to the process. However clingo by default will always use the same seed so we need to generate a random
				  REM seed to give clingo to use


Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\TempWorldDef.txt" --seed=%seed% --rand-freq .01 > "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\GraphVisCode\results.pcg"

																						REM This line actually runs clingo. 
																						REM The PCG.txt and TempWorldDef.txt are the input files. 
																						REM --seed=%seed% sets the seed to the randomly generated seed we just found
																						REM --rand-freq .15 is the parameter that determines how frequently Clingo will try to make random decisions instead of using its usual heuristics.
																						REM		This parameter can be kept rather low and still generate varied results.  Its hard to determine it's effect on speed though. I've had clingo code that
																						REM     has run faster when increasing randomness and other code that has run slower when increasing it
																						REM > results.pcg Outputs the Results Clingo generates to the file results.pcg
																						REM		The .pcg extension doesn't really mean anything. It's just a text file, but giving it a unique extension helps make it stand out
																						
PAUSE REM The pause prints a message and leaves the console window open until you press a key once clingo finishes running. I find its easier to notice when Clingo is done this way then by having the console close automatically.                                                                                                                     
