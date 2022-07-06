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

setlocal
set first=1
set fileName="combinedFiles.csv"
>%fileName% (
  for %%F in (*.csv) do (
    if not "%%F"==%fileName% (
      if defined first (
        type "%%F"
        set "first="
      ) else more +1 "%%F"
    )
  )
)


::if invoked from windows explorer, pause
IF "%interactive%"=="0" PAUSE
ENDLOCAL
EXIT /B 0