@ECHO OFF
set seed=%RANDOM%
Clingo.exe PCG.txt WorldDef.txt --seed=%seed% --rand-freq .1 > results.pcg
PAUSE