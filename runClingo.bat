@ECHO OFF
set seed=%RANDOM%
REM Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\WorldDef.txt" --seed=%seed% --rand-freq .15 > results.pcg
Clingo.exe "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG_Resources\PCG.txt" "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\PCG GUI\PCG GUI\bin\Debug\WorldDef.txt" --seed=%seed% --rand-freq .01 > "C:\Users\Donald Bass\Documents\Rpg-Level-Quest-PCG\GraphVisCode\results.pcg"
PAUSE                                                                                                                      
