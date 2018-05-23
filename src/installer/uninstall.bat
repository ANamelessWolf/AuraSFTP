REM Remove Aura bin if exists
IF EXIST %APPDATA%\bin\Aura.bat ( echo del %APPDATA%\bin\Aura.bat ) ELSE ( echo "Aura.bat was not found" )
REM Delete Aura folder if exists
IF EXIST %APPDATA%\aura ( echo rmdir /s /q %APPDATA%\aura ) ELSE ( echo "install folder was not found" )