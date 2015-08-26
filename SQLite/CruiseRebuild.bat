echo %1 
cd %~p0
sqlite3.exe %1 ".dump" >> "%~1.sqlDump"
sqlite3.exe "%~p1\clean_%~nx1" ".read '%~1.sqlDump'"
pause