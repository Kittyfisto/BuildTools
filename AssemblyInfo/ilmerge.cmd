@echo off

set SCRIPT_DIR=%~dp0

:: this script needs https://www.nuget.org/packages/ilmerge

:: Set build, used for directory. Typically Release or Debug
SET ILMERGE_BUILD=Release

:: set your target executable name (typically [projectname].exe)
SET SOURCE_APP_NAME=AssemblyInfo2.exe
set OUTPUT_PATH=%SCRIPT_DIR%..\bin\%ILMERGE_BUILD%
set SOURCE_APP_PATH=%OUTPUT_PATH%\%SOURCE_APP_NAME%
SET DEST_APP_NAME=BuildTool.exe
SET DEST_APP_PATH=%OUTPUT_PATH%\%DEST_APP_NAME%

:: Set platform, typically x64
SET ILMERGE_PLATFORM=x64

:: set your NuGet ILMerge Version, this is the number from the package manager install, for example:
:: PM> Install-Package ilmerge -Version 3.0.21
:: to confirm it is installed for a given project, see the packages.config file
SET ILMERGE_VERSION=3.0.29

:: the full ILMerge should be found here:
SET ILMERGE_PATH=%SCRIPT_DIR%\..\packages\ILMerge.%ILMERGE_VERSION%\tools\net452
:: dir "%ILMERGE_PATH%"\ILMerge.exe

if not exist "%ILMERGE_PATH%" goto :NOILMERGE
if not exist "%SOURCE_APP_PATH%" goto :NOAPP

echo Merging %SOURCE_APP_NAME% ...

:: add project DLL's starting with replacing the FirstLib with this project's DLL
"%ILMERGE_PATH%\ILMerge.exe" "%SOURCE_APP_PATH%" "/out:%DEST_APP_PATH%" "/lib:%OUTPUT_PATH%" CommandLine.dll
goto :DONE


:NOILMERGE
echo Could not find ilmerge at %ILMERGE_PATH%
exit /b -1

:NOAPP
echo Could not find app at %SOURCE_APP_PATH%
exit /b -1

:DONE
echo Done