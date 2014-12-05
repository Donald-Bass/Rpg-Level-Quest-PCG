@ECHO OFF
REM This file is an example of how to run experiments with Clingo to help determine the average runtime, in case you are running into performance problems and want to know whether certain changes 
REM actually help with said problems. Since there is an element of randomness with Clingo any individual run doesn't give you useful information. You need to average the times of a number of runs
REM to get a better idea of what the actual performance is. How many depends on how small of a change you want to detect. If you make a change that has a huge performance implication its often obvious
REM after only 5-6 runs. I usually do 30 runthroughs which seems to be more then sufficient to detect most performance changes
REM This bat file will save the time taken for each runthrough to a file, although currently the user needs to manually find the average themselves.

REM Loop through 30 times. The first 1 is the starting value for i, the second is how much to increase it by, and the third is the highest value of i to use
for /l %%i in (1,1,30) do (
	Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .05 > results.pcg
	
	more +10 results.pcg >> timingWithDistLim.txt

)


PAUSE
