::@ECHO OFF
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
SET zip="%parent%tools\zip.cmd"

IF NOT DEFINED build_config SET build_config="Release"

IF NOT DEFINED dateCode (SET dateCode=%date:~10,4%%date:~4,2%%date:~7,2%)
IF NOT DEFINED packageOutputDir SET packageOutputDir=%parent%..\PackageOutput\%dateCode%

call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% "%parent%FMSC.ORM\FMSC.ORM.csproj"
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%FMSC.ORM.ModelsGenerator\FMSC.ORM.ModelsGenerator.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.Core\CruiseDAL.Core.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V2\CruiseDAL.V2.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V3\CruiseDAL.V3.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V3.UpConvert\CruiseDAL.V3.UpConvert.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V3.DownConvert\CruiseDAL.V3.DownConvert.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V3.Models\CruiseDAL.V3.Models.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V2.Models\CruiseDAL.V2.Models.csproj
call %msbuild% -t:pack /p:PackageOutputPath=%packageOutputDir%;Configuration=%build_config% %parent%CruiseDAL.V3.Sync\CruiseDAL.V3.Sync.csproj

::build and pack CruiseCLI
call %msbuild% /p:Configuration=%build_config% %parent%CruiseCLI\CruiseCLI.csproj
pushd %parent%CruiseCLI\bin\Release\netcoreapp3.1
call %zip% a -tzip -spf %packageOutputDir%\CruiseCLI.zip *.exe *.dll *.runtimeconfig.json *.deps.json runtimes\win-x86\native\*.dll runtimes\win-x64\native\*.dll
popd

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0 