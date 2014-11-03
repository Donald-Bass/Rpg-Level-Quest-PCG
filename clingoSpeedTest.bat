@ECHO OFF


REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDef.txt" --seed=%%i --rand-freq .1 > results.pcg
	
	REM more +10 results.pcg >> timingNoOpt.txt

REM )

REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCGNonEdge.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDef.txt" --seed=%%i --rand-freq .1 > results.pcg
	
	REM more +10 results.pcg >> timingNoEdge.txt

REM )
REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCGStartRoom.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDef.txt" --seed=%%i --rand-freq .1 > results.pcg
	
	REM more +10 results.pcg >> timingStartRoom.txt

REM )

REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDefWithRoom.txt" --seed=%%i --rand-freq .1 > results.pcg
	
	REM more +10 results.pcg >> timingOneRoom.txt

REM )

REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCGNonEdge.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDefWithRoom.txt" --seed=%%i --rand-freq .1 > results.pcg
	
	REM more +10 results.pcg >> timingNoEdgeAndRoom.txt

REM )

REM for /l %%i in (1,1,30) do (
REM	Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef2010.txt" --seed=%%i --rand-freq .15 > results.pcg
	
REM	more +10 results.pcg >> timingDist20by20.txt

REM )


REM for /l %%i in (16,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef4.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing4.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef5.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing5.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef6.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing6.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef7.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing7.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef8.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing8.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef9.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing9.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef10.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timing10.txt

REM )

for /l %%i in (1,1,30) do (
	Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .05 > results.pcg
	
	more +10 results.pcg >> timingWithDistLim.txt

)


REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCGOld.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .01 > results.pcg
	
	REM more +10 results.pcg >> timing4BranchOld.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .05 > results.pcg
	
	REM more +10 results.pcg >> timingRand05.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .10 > results.pcg
	
	REM more +10 results.pcg >> timingRand10.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timingRand15.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .20 > results.pcg
	
	REM more +10 results.pcg >> timingRand20.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%%i --rand-freq .25 > results.pcg
	
	REM more +10 results.pcg >> timingRand25.txt

REM )

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" > results.pcg
	
	REM more +10 results.pcg >> timingNoRand.txt

REM )

REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDefComplex.txt" > resultsC.pcg

REM for /l %%i in (1,1,30) do (
	REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef20by20.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	REM more +10 results.pcg >> timingStartAndEnd20.txt

REM )



PAUSE
