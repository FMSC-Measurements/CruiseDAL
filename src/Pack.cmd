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
SET zip="%parent%tools\zip.cmd"


IF NOT DEFINED dateCode (SET dateCode=%date:~10,4%_%date:~4,2%_%date:~7,2%)
IF NOT DEFINED packageOutputDir SET packageOutputDir=%parent%..\PackageOutput\%dateCode%


SET build_params=PackageOutputPath="%packageOutputDir%"
IF NOT DEFINED targets	SET targets="pack"
call %parent%Build.cmd 

::pack CruiseCLI
IF %ERRORLEVEL% EQU 0 (
	pushd %parent%CruiseCLI\bin\Release\net48
	call %zip% a -tzip -spf %packageOutputDir%\CruiseCLI.zip *.exe *.dll runtimes\win-x86\native\*.dll runtimes\win-x64\native\*.dll
	popd
)

::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B %ERRORLEVEL%