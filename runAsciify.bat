@ECHO OFF
set seed=%RANDOM%
Clingo.exe PCG.txt --seed=%seed% --rand-freq .5 > results.txt 
python asciify.py < results.txt
copy results.txt "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug"
PAUSE