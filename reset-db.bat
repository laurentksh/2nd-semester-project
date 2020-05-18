cls
@ECHO OFF
title Reset DB

echo Please make sure Visual Studio is closed, then press a key to continue.
pause

del %LOCALAPPDATA%\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\msdbdata.mdf
del %LOCALAPPDATA%\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\tempdb.mdf
del %LOCALAPPDATA%\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\model.mdf
del %LOCALAPPDATA%\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\mssqllocaldb\master.mdf

dotnet ef migrations add Initial
dotnet ef database update

pause