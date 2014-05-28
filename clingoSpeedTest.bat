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
for /l %%i in (1,1,30) do (
	Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDefNoID.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	more +10 results.pcg >> timingRoomAndID.txt

)

for /l %%i in (1,1,30) do (
	Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDefNoID.txt" --seed=%%i --rand-freq .15 > results.pcg
	
	more +10 results.pcg >> timingRoomNoID.txt

)

PAUSE
