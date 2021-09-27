@echo off
set /P sure=Are you sure you want to push packages to nuget.org (yes/[n])?
IF /I "%sure%" neq "yes" GOTO END
echo on

@set configuration=release
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set logfile=%datetime:~0,8%-%datetime:~8,6%.log

del ..\bin\%configuration%\*.nupkg
del ..\bin\%configuration%\*.snupkg
dotnet pack ..\DHI.Services.csproj --configuration %configuration% > %logfile% 2>&1
dotnet nuget push ..\bin\%configuration%\DHI.Services.*.nupkg -k %NUGET_API_KEY% -s https://api.nuget.org/v3/index.json >> %logfile% 2>&1

notepad %logfile%

:END
