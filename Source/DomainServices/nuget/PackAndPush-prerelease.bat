@set configuration=debug
for /f "tokens=2 delims==" %%I in ('wmic os get localdatetime /format:list') do set datetime=%%I
set logfile=%datetime:~0,8%-%datetime:~8,6%.log

del ..\bin\%configuration%\*.nupkg
dotnet pack ..\DHI.Services.csproj --configuration %configuration% > %logfile% 2>&1
dotnet nuget push ..\bin\%configuration%\DHI.Services.*.nupkg --source http://dhi-nuget-server.azurewebsites.net/nuget --api-key dhi-nuget-admin >> %logfile% 2>&1

notepad %logfile%
