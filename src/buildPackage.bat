SET ver=v1.0
rmdir /s /q ..\release\aura
del /f /s /q ..\release\*
mkdir ..\release\aura
dotnet publish
xcopy /s /Y C:\Actica\Projects\tools\AuraSFTP\src\bin\Debug\netcoreapp2.0\publish\*.json ..\release\aura
xcopy /s /Y C:\Actica\Projects\tools\AuraSFTP\src\bin\Debug\netcoreapp2.0\publish\*.dll ..\release\aura
xcopy /s /Y C:\Actica\Projects\tools\AuraSFTP\src\installer\* ..\release\
REM Use echo to see preview of the command for /d %%a in (..\release) do (ECHO zip -r -p "..\release\aura_%version%.zip" ".\%%a\*")
CD ..
CD release
RAR a -r aura_%ver%.rar
CD ..
CD src
del /f /s /q ..\release\*bat
rmdir /s /q ..\release\aura