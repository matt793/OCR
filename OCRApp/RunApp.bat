@echo off
echo Running OCR Application...
cd /d %~dp0
dotnet run --no-build
echo.
echo If the application didn't start, press any key to exit.
pause
