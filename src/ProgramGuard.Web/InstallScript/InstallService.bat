@echo off
CLS
ECHO.
ECHO =============================
ECHO Running Admin Shell
ECHO =============================

:init
setlocal DisableDelayedExpansion
set "batchPath=%~0"
for %%k in (%0) do set batchName=%%~nk
set "vbsGetPrivileges=%temp%\OEgetPriv_%batchName%.vbs"
setlocal EnableDelayedExpansion

:checkPrivileges
NET FILE 1>NUL 2>NUL
if '%errorlevel%' == '0' ( goto gotPrivileges ) else ( goto getPrivileges )

:getPrivileges
if '%1'=='ELEV' (echo ELEV & shift /1 & goto gotPrivileges)
ECHO.
ECHO **************************************
ECHO Invoking UAC for Privilege Escalation
ECHO **************************************

ECHO Set UAC = CreateObject^("Shell.Application"^) > "%vbsGetPrivileges%"
ECHO args = "ELEV " >> "%vbsGetPrivileges%"
ECHO For Each strArg in WScript.Arguments >> "%vbsGetPrivileges%"
ECHO args = args ^& strArg ^& " "  >> "%vbsGetPrivileges%"
ECHO Next >> "%vbsGetPrivileges%"
ECHO UAC.ShellExecute "!batchPath!", args, "", "runas", 1 >> "%vbsGetPrivileges%"
"%SystemRoot%\System32\WScript.exe" "%vbsGetPrivileges%" %*
exit /B

:gotPrivileges
setlocal & pushd .
cd /d %~dp0
if '%1'=='ELEV' (del "%vbsGetPrivileges%" 1>nul 2>nul  &  shift /1)
: ============================= 
: 權限取得完成。
: =============================

:更換不同的 Service 只需要處理下面的 SERVICE_NAME , SERVICE_BIN , SERVICE_DESCRIPTION , SERVICE_DISPPLAY_NAME，其它不用動。
:如果要添加對資料庫的相依性，請在 sc create 那行添加如下 depend= MSSQL$SQLEXPRESS , SQL 名稱添加時需要再確認，如果相依性的對像找不到會造成服務起動失敗。
:安裝的 Script 必須放在與 Service 執行檔同個目錄的其它資料夾內，例如 AVLS\AVLS.exe , AVLS\Script\Install

ECHO 到上一層目錄(執行檔所在的目錄)
CD..

set HOME=%CD%
set SERVICE_NAME=ProgramGuard.Web
set SERVICE_DISPPLAY_NAME=ProgramGuard.Web
set SERVICE_BIN=ProgramGuard.Web.exe
set SERVICE_DESCRIPTION="ProgramGuard.Web"
set SERVICE_FULL_PATH=%HOME%\%SERVICE_BIN%

ECHO 註冊服務
sc create %SERVICE_NAME% binpath= "%SERVICE_FULL_PATH%" displayname= %SERVICE_DISPPLAY_NAME% start= delayed-auto

ECHO 為服務加入描述
sc description %SERVICE_NAME% %SERVICE_DESCRIPTION%

ECHO 設定服務啟動失敗處理
sc failure %SERVICE_NAME% reset= 30 actions= restart/5000/restart/5000//
ECHO ...
ECHO 安裝完成

ECHO Service 位於 %SERVICE_FULL_PATH%

PAUSE