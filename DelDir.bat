taskkill /f /IM XLY.*
cd /d %~dp0 

rd /s /q Trunk\Trunk\Lib\
rd /s /q Trunk\Trunk\Resource\
rd /s /q Trunk\Trunk\Tools\
cd /d Trunk\Trunk\Source\
for /d /r %%d in (*) do (
rd /s /q "%%d"
)
pause
:end



