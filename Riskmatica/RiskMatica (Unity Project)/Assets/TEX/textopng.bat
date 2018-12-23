@echo off&setlocal
REM for %%i in ("%~dp0..") do set "folder=%%~fi"
SET texPath=%1
SET name=%2
SET imgPath=%3
SET namepdf=%name%.pdf
SET namepng=question_%name%.png
set dir="%~dp0"
cd %dir%

pdflatex -jobname=%name% %texPath%
REM convert -density 200 -resize 512x512 -alpha on %namepdf% %namepng%
gswin64c -sDEVICE=pngalpha -o %namepng% -r100 %namepdf%

del %dir%*.aux
del %dir%*.log
del %dir%*.nav
del %dir%*.out
del %dir%*.pdf
del %dir%*.snm
del %dir%*.toc
move %dir%%namepng% %imgPath%
EXIT



