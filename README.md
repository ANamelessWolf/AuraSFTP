# AuraSFTP
A console file manager for testing and editing a web project on a server via SFTP using dotNET Core.
## Installing
Downloads the file from [here](https://github.com/ANamelessWolf/AuraSFTP/releases/download/v1.0/aura_v1.0.rar)
Unzip the file in a local directory.
### Installing on windows
Open a window console where the aura folder is extracted.

Move the aura directory to the Application folder
```batch
move aura %APPDATA%
```
Check that the file is moved and check the installed version
```batch
cd %APPDATA%
dotnet aura\AuraSFTP.dll -v
```
You should see a similar output
```
Version: v1.0.0
Install Directory: C:\Users\mike\AppData\Roaming
```
Create a bin folder wherever you like, it could also be in %APPDATA%
```batch
cd %APPDATA%
mkdir bin
cd %APPDATA%\bin
```
Inside this folder create a file named `Aura.bat`.
```batch
@echo off
dotnet %APPDATA%\aura\AuraSFTP.dll %*
```
Add the bin folder to the PATH
```
SET PATH=%PATH%;%APPDATA%\bin
```
Now you can run Aura command in any folder.
For a quick tutorial visit the following [link](https://github.com/ANamelessWolf/AuraSFTP/wiki/Quick-tutorial)
