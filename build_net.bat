@echo off

SET ILMERGE=0
SET NSISLAUNCHER=1
SET SEVENZIP=1
SET PORTABLEAPPSINSTALLER=1
SET CONEMU=1
SET CYGWIN=1
SET BASE_DIR=%CD%

REM ---------------------------------------------------------------------
echo Prepare NPM
REM ---------------------------------------------------------------------
cd App\app
CMD /C npm install
CMD /C npm install flatten-packages -g
CMD /C flatten-packages
cd %BASE_DIR%

CALL "%VS140COMNTOOLS%vsvars32.bat"

REM ---------------------------------------------------------------------
echo Get Version
REM ---------------------------------------------------------------------
set APPINFOINI=App\AppInfo\appinfo.ini
set APPINFOSECTION=Version
set APPINFOKEY=DisplayVersion

for /f "delims=:" %%a in ('findstr /binc:"[%APPINFOSECTION%]" "%APPINFOINI%"') do (
  for /f "tokens=1* delims==" %%b in ('more +%%a^<"%APPINFOINI%"') do (
    set "APPINFOKEY=%%b"
    set "APPINFOVERSION=%%c"
    setlocal enabledelayedexpansion
    if "!APPINFOKEY:~,1!"=="[" (endlocal&goto notFound)
    if /i "!APPINFOKEY!"=="%APPINFOKEY%" (endlocal&goto found)
    endlocal
  )
) 

:notFound
set APPINFOVERSION=0.0.0.0

:found
echo %APPINFOVERSION%

REM ---------------------------------------------------------------------
echo Compiling
REM ---------------------------------------------------------------------
devenv %CD%\MMOwningLauncher.sln /Clean
devenv %CD%\MMOwningLauncher.sln /Rebuild "Release|Any CPU"

REM ---------------------------------------------------------------------
echo IlMerge Files
REM ---------------------------------------------------------------------
if %ILMERGE%==1 (
del "%CD%\App\MMOwningLauncherMerge.exe"
"C:\Program Files (x86)\Microsoft\ILMerge\ILMerge.exe"^
 /ndebug^
 /copyattrs^
 /targetplatform:4.0,"C:\Windows\Microsoft.NET\Framework64\v4.0.30319"^
 /out:"%CD%\App\MMOwningLauncherMerge.exe"^
 "%CD%\App\MMOLauncher.exe"^ 
 "%CD%\App\Microsoft.AspNet.SignalR.Core.dll"^
 "%CD%\App\Microsoft.Owin.dll"^
 "%CD%\App\Microsoft.Owin.Cors.dll"^
 "%CD%\App\Microsoft.Owin.Hosting.dll"^
 "%CD%\App\Microsoft.Owin.Security.dll"^
 "%CD%\App\Nancy.dll"^
 "%CD%\App\Nancy.Owin.dll"^
 "%CD%\App\Newtonsoft.Json.dll"^
 "%CD%\App\Nowin.dll"^
 "%CD%\App\Owin.dll"^
 "%CD%\App\System.Web.Cors.dll"^
 "%CD%\App\websocket-sharp.dll"
 
del "%CD%\App\*.dll"
del "%CD%\App\MMOwningLauncher.exe"
ren "%CD%\App\MMOwningLauncherMerge.exe" MMOwningLauncher.exe
)

REM ---------------------------------------------------------------------
echo Prepare Release
REM ---------------------------------------------------------------------
rmdir /s /q Release
mkdir Release
robocopy App\ *.exe Release\App\
robocopy App\ *.config Release\App\
robocopy App\ *.pdb Release\App\
robocopy App\ *.dll Release\App\
robocopy App\AppInfo Release\App\AppInfo /s /e
robocopy App\AppInfo\Launcher Release\App\AppInfo\Launcher /s /e
robocopy App\DefaultData Release\App\DefaultData /s /e
robocopy App\app Release\App\app /s /e
robocopy App\Runtime\7zip Release\App\Runtime\7zip /s /e
robocopy Other Release\Other /s /e
if %CONEMU%==1 (
robocopy BinClean Release\Bin /s /e
)
if %CYGWIN%==1 (
robocopy BinClean Release\Bin /s /e
)

copy help.html Release\help.html

REM ---------------------------------------------------------------------
echo Create NSIS Launcher
REM ---------------------------------------------------------------------
if %PORTABLEAPPSINSTALLER%==1 (
..\PortableApps.comLauncher\PortableApps.comLauncherGenerator.exe "%CD%\Release"
)

REM ---------------------------------------------------------------------
echo 7zip Archive
REM ---------------------------------------------------------------------
if %SEVENZIP%==1 (
del "%CD%\MMOwningLauncher_%APPINFOVERSION%.7z"
..\7-ZipPortable\App\7-Zip\7z.exe a -r -t7z -mx=9 MMOwningLauncher_%APPINFOVERSION%.7z %CD%\Release\*
)

REM ---------------------------------------------------------------------
echo Create PortableApps Installer
REM ---------------------------------------------------------------------
if %PORTABLEAPPSINSTALLER%==1 (
..\CybeSystems.comAppInstaller\PortableApps.comInstaller.exe "%CD%\Release"
)

REM ---------------------------------------------------------------------
echo Ready
REM ---------------------------------------------------------------------
pause