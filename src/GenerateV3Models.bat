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

SET msbuild="%parent%tools\msbuild.cmd"

IF NOT DEFINED build_config SET build_config="Release"
IF NOT DEFINED tf SET tf="net40"

call %msbuild% /p:Configuration=%build_config% %parent%CruiseDAL.V3.Models\CruiseDAL.V3.Models.csproj
call %msbuild% /p:Configuration=%build_config% %parent%CruiseDAL.V3.Models\CruiseDAL.V3.Models.csproj -target:GenerateModels -p:TargetFramework=%tf%

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0