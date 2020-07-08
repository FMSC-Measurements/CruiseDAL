::run as admin

@echo off
::@setlocal enableextensions enabledelayedexpansion
pushd "%~dp0"

if not exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\" (
  mkdir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\"
)
::this actualy breaks net35 builds so we disable this!
::xcopy "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\v3.5\*" "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\"
if not exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\" (
  mkdir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\"
)

if not exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\" (
  mkdir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\"
  xcopy "C:\Program Files (x86)\Microsoft.NET\SDK\CompactFramework\v3.5\Debugger\BCL\*" "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\" /S /Y
)

if not exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\RedistList\" (
  mkdir "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\RedistList\"
)

if not exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\RedistList\FrameworkList.xml" (
  ( 
    echo ^<?xml version="1.0" encoding="utf-8"?^>
    echo ^<FileList Redist="Net35-CF" Name=".NET Compact Framework 3.5"^>
    echo ^</FileList^>
  )  > "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v3.5\Profile\CompactFramework\RedistList\FrameworkList.xml"
)
popd