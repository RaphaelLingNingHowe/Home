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
: �v�����o�����C
: =============================

:�󴫤��P�� Service �u�ݭn�B�z�U���� SERVICE_NAME , SERVICE_BIN , SERVICE_DESCRIPTION , SERVICE_DISPPLAY_NAME�A�䥦���ΰʡC
:�p�G�n�K�[���Ʈw���̩ۨʡA�Цb sc create ����K�[�p�U depend= MSSQL$SQLEXPRESS , SQL �W�ٲK�[�ɻݭn�A�T�{�A�p�G�̩ۨʪ��ﹳ�䤣��|�y���A�Ȱ_�ʥ��ѡC
:�w�˪� Script ������b�P Service �����ɦP�ӥؿ����䥦��Ƨ����A�Ҧp AVLS\AVLS.exe , AVLS\Script\Install

ECHO ��W�@�h�ؿ�(�����ɩҦb���ؿ�)
CD..

set HOME=%CD%
set SERVICE_NAME=ProgramGuard.Web
set SERVICE_DISPPLAY_NAME=ProgramGuard.Web
set SERVICE_BIN=ProgramGuard.Web.exe
set SERVICE_DESCRIPTION="ProgramGuard.Web"
set SERVICE_FULL_PATH=%HOME%\%SERVICE_BIN%

ECHO ���U�A��
sc create %SERVICE_NAME% binpath= "%SERVICE_FULL_PATH%" displayname= %SERVICE_DISPPLAY_NAME% start= delayed-auto

ECHO ���A�ȥ[�J�y�z
sc description %SERVICE_NAME% %SERVICE_DESCRIPTION%

ECHO �]�w�A�ȱҰʥ��ѳB�z
sc failure %SERVICE_NAME% reset= 30 actions= restart/5000/restart/5000//
ECHO ...
ECHO �w�˧���

ECHO Service ��� %SERVICE_FULL_PATH%

PAUSE