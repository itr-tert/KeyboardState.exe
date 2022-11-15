REM オプションは1文字.
REM   w: target:winexe
REM   e: target:exe  意味上の初期設定
REM   p: pauseしない

REM ・環境変数 FW に存在する Microsoft.NET\Framework\v* のいずれか1つを設定する。
REM ・PATH へ FW を追加する。

set arg=%1 %2 %3 %4 %5 %6 %7 %8 %9

goto main


:if_exist_add_to_CSCLIBPATH
	echo %~1
	if exist %1 set CSCLIBPATH=%CSCLIBPATH%,%~1
	exit /b


:main

IF        EXIST C:\Windows\Microsoft.NET\Framework\v3.5 (
             set FW=C:\Windows\Microsoft.NET\Framework\v3.5
) ELSE IF EXIST C:\Windows\Microsoft.NET\Framework64\v3.5 (
             set FW=C:\Windows\Microsoft.NET\Framework64\v3.5
) ELSE IF EXIST C:\Windows\Microsoft.NET\Framework\v2.0.50727 (
             set FW=C:\Windows\Microsoft.NET\Framework\v2.0.50727
) ELSE IF EXIST C:\Windows\Microsoft.NET\Framework64\v2.0.50727 (
             set FW=C:\Windows\Microsoft.NET\Framework64\v2.0.50727
)
REM C:\Windows\Microsoft.NET\Framework\v4.0.30319\WPF

PATH=%PATH%;%FW%

set CSCLIBPATH=C:\Program Files\Reference Assemblies\Microsoft\Framework\v3.0,C:\Windows\Microsoft.NET\Framework\v2.0.50727

call :if_exist_add_to_CSCLIBPATH "C:\Program Files (x86)\Microsoft.NET\Primary Interop Assemblies"
call :if_exist_add_to_CSCLIBPATH "C:\Program Files\Microsoft.NET\Primary Interop Assemblies"

set CSCOPTS=-optimize+

if not "%arg:w=%" == "%arg%" (
    REM -target:winexe  DOS窓出ない
    set CSCOPTS=%CSCOPTS% -target:winexe
)

if not "%arg:e=%" == "%arg%" (
    REM -target:exe     常に標準出力あり
    set CSCOPTS=%CSCOPTS% -target:exe
)

csc %CSCOPTS% KeyboardState.cs

if "%arg:p=%" == "%arg%" (
    pause
)
