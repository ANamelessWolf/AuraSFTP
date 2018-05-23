REM Move the file to application data folder
move aura %APPDATA%
REM Create a binary folder in %APPDATA%
IF EXIST %APPDATA%\bin ( echo "bin already exists" ) ELSE ( mkdir %APPDATA%\bin )
REM Move the application bat to the bin folder
move Aura.bat %APPDATA%\bin
REM Add the bin folder to the PATH
set pth=%APPDATA%\bin
call inPath pth && ( echo "PATH already defined" ) || ( setx path "%PATH%;%APPDATA%\bin" )