SET HOUR=%time:~0,2%
SET time9=%date:~-4%%date:~4,2%%date:~7,2%_0%time:~1,1%%time:~3,2%%time:~6,2%%time:~9,2%
SET time24=%date:~-4%%date:~4,2%%date:~7,2%_%time:~0,2%%time:~3,2%%time:~6,2%%time:~9,2%
if "%HOUR:~0,1%" == " " (SET timestamp=%time9%) else (SET timestamp=%time24%)

cd C:\Users\iisadmin\Downloads\ro_ticketing.git\api\ticketing_api

echo %1 > logs/webhook-%timestamp%.txt

git reset --hard HEAD
git pull
dotnet restore
del .\bin\Debug\netcoreapp2.1\publish\appsettings.json.new
dotnet publish
rename .\bin\Debug\netcoreapp2.1\publish\appsettings.json appsettings.json.new

cd C:\Users\iisadmin\Downloads\ro_ticketing_ui.git
git reset --hard HEAD
git pull
call npm install
call npm run build

iisreset /stop
robocopy /E /nodcopy /W:3 /R:15 C:\Users\iisadmin\Downloads\ro_ticketing_ui.git\build C:\inetpub\wwwroot\ro_ticketing

cd C:\Users\iisadmin\Downloads\ro_ticketing.git\api\ticketing_api
robocopy /E /nodcopy /W:3 /R:15 .\bin\Debug\netcoreapp2.1\publish C:\inetpub\wwwroot\ro_ticketing_api

iisreset /start
