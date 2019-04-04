@echo off

set Solution=%~dp0src\IxMilia.Dxf.sln

dotnet restore "%Solution%"
if errorlevel 1 goto error

dotnet build "%Solution"%
if errorlevel 1 goto error

dotnet test "%Solution"%
if errorlevel 1 goto error

goto :eof

:error
echo Error building project.
exit /b 1
