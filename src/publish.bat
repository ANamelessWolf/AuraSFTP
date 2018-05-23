dotnet publish
xcopy /s /Y C:\Actica\Projects\tools\AuraSFTP\src\bin\Debug\netcoreapp2.0\publish\*.json %APPDATA%\aura\
xcopy /s /Y C:\Actica\Projects\tools\AuraSFTP\src\bin\Debug\netcoreapp2.0\publish\*.dll %APPDATA%\aura\