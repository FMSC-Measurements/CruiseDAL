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

dotnet test %parent%FMSC.ORM.Test\FMSC.ORM.Test.csproj -f netcoreapp2.0
dotnet test %parent%FMSC.ORM.Test\FMSC.ORM.Test.csproj -f net472
dotnet test %parent%CruiseDAL.Core.Test\CruiseDAL.Core.Tests.csproj 
dotnet test %parent%CruiseDAL.V2.Test\CruiseDAL.V2.Test.csproj
dotnet test %parent%CruiseDAL.V3.Test\CruiseDAL.V3.Test.csproj
dotnet test %parent%CruiseDAL.V3.UpConvert.Test\CruiseDAL.V3.UpConvert.Test.csproj
dotnet test %parent%CruiseDAL.V3.DownConvert.Test\CruiseDAL.V3.DownConvert.Test.csproj

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0