@ECHO OFF
set seed=%RANDOM%
Clingo.exe PCG.txt --seed=%seed% --rand-freq .5 > results.txt
PAUSE