@ECHO OFF
SETLOCAL ENABLEEXTENSIONS

::Boilderplate 
::detect if invoked via Window Explorer
SET interactive=1
ECHO %CMDCMDLINE% | FIND /I "/c" >NUL 2>&1
IF %ERRORLEVEL% == 0 SET interactive=0

::name of this script
SET me=%~n0
::directory of script
SET parent=%~dp0

IF NOT DEFINED build_config SET build_config="Release"

IF NOT DEFINED packageOutputDir SET packageOutputDir=%parent%..\PackageOutput

dotnet pack -c %build_config% --include-source -o %packageOutputDir% %parent%CruiseDAL\CruiseDAL.csproj
dotnet pack -c %build_config% --include-source -o %packageOutputDir% %parent%CruiseDAL.Legacy\CruiseDAL.Legacy.csproj

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0