@echo off
echo Optimizing JPEG ^& PNG Images...
forfiles /s /m *.jpg /c "cmd /c @\"E:\Image Optimization\cwebp.exe\" @file -o @file"
echo. & echo Process done!
pause