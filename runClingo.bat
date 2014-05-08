@ECHO OFF
set seed=%RANDOM%
Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" --seed=%seed% --rand-freq .1 > results.pcg
PAUSE
