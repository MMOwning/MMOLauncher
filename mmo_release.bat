cd app
CMD /C npm install
cd ..
electron-packager %CD%\\app MMOwning --overwrite --platform=win32 --arch=ia32 --icon=appicon.ico --out=release --version=0.35.4 --version-string.CompanyName=string.ProductName=MMOwning --version-string.ProductName=MMOwning --version-string.ProductVersion=1
pause