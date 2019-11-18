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

dotnet build -c %build_config% %parent%FMSC.ORM\FMSC.ORM.csproj
::dotnet build -c %build_config% %parent%FMSC.ORM.ModelsGenerator\FMSC.ORM.ModelsGenerator.csproj
dotnet build -c %build_config% %parent%CruiseDAL.Core\CruiseDAL.Core.csproj
dotnet build -c %build_config% %parent%CruiseDAL.V2\CruiseDAL.V2.csproj
dotnet build -c %build_config% %parent%CruiseDAL.V3\CruiseDAL.V3.csproj
dotnet build -c %build_config% %parent%CruiseDAL.V3.Models\CruiseDAL.V3.Models.csproj
dotnet build -c %build_config% %parent%CruiseDAL.V2.Models\CruiseDAL.V2.Models.csproj

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0