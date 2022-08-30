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

IF NOT DEFINED buildNetCF SET buildNetCF="true"
IF NOT DEFINED build_config SET build_config="Release"
IF NOT DEFINED targets	SET targets="build"

call %msbuild% %parent%CruiseDAL.sln -t:restore

IF DEFINED build_params (
	SET build_params=%build_params%;Configuration=%build_config%
) ELSE (
	SET build_params=Configuration=%build_config%
)

IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params%;BuildCF=%buildNetCF% %parent%FMSC.ORM\FMSC.ORM.csproj
IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params%;BuildCF=%buildNetCF% %parent%CruiseDAL.Core\CruiseDAL.Core.csproj
IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params%;BuildCF=%buildNetCF% %parent%CruiseDAL.V2\CruiseDAL.V2.csproj
IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params%;BuildCF=%buildNetCF% %parent%CruiseDAL.V2.Models\CruiseDAL.V2.Models.csproj
IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params%;BuildCF=%buildNetCF% %parent%CruiseDAL.V2.Updater\CruiseDAL.V2.Updater.csproj

IF NOT %buildNetCF%=="true" (
	IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params% %parent%CruiseDAL.V3\CruiseDAL.V3.csproj
	IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params% %parent%CruiseDAL.V3.UpConvert\CruiseDAL.V3.UpConvert.csproj
	IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params% %parent%CruiseDAL.V3.DownConvert\CruiseDAL.V3.DownConvert.csproj
	IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params% %parent%CruiseDAL.V3.Models\CruiseDAL.V3.Models.csproj
	IF %ERRORLEVEL% EQU 0 call %msbuild% -t:%targets% -p:%build_params% %parent%CruiseCLI\CruiseCLI.csproj
)

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL

EXIT /B %ERRORLEVEL%