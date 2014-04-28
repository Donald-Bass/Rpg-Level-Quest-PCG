@ECHO OFF
set seed=%RANDOM%
Clingo.exe "C:\Users\Donald\Documents\Coding\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" --seed=%seed% --rand-freq .1 > results.pcg
PAUSE