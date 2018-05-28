dotnet publish
xcopy /s /Y .\bin\Debug\netcoreapp2.0\publish\*.json %APPDATA%\aura\
xcopy /s /Y .\bin\Debug\netcoreapp2.0\publish\*.dll %APPDATA%\aura\
xcopy /s /Y .\data\* %APPDATA%\aura\data