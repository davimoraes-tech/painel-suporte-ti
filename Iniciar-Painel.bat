@echo off
setlocal
title Painel de Suporte TI Windows
cls
echo ================================================================
echo               PAINEL DE SUPORTE TI WINDOWS
echo ================================================================
echo.
echo   [1] Abrir Painel Visual Moderno (C# / WPF - Recomendado)
echo   [2] Abrir Painel Tradicional (PowerShell Console)
echo   [0] Sair
echo.
echo ================================================================
set /p opt="Escolha uma opcao [1]: "
if "%opt%"=="" set opt=1
if "%opt%"=="1" goto LAUNCH_WPF
if "%opt%"=="2" goto LAUNCH_PS
if "%opt%"=="0" exit /b 0
goto LAUNCH_WPF

:LAUNCH_WPF
set "EXE_RELEASE=%~dp0src\PainelSuporteTI\bin\Release\net8.0-windows\PainelSuporteTI.exe"
set "EXE_DEBUG=%~dp0src\PainelSuporteTI\bin\Debug\net8.0-windows\PainelSuporteTI.exe"
set "EXE_PUB=%~dp0Publish\PainelSuporteTI.exe"

if exist "%EXE_PUB%" (
    start "" "%EXE_PUB%"
    exit /b 0
)
if exist "%EXE_RELEASE%" (
    start "" "%EXE_RELEASE%"
    exit /b 0
)
if exist "%EXE_DEBUG%" (
    start "" "%EXE_DEBUG%"
    exit /b 0
)

echo Compilando e iniciando a versao C# WPF...
dotnet run --project "%~dp0src\PainelSuporteTI\PainelSuporteTI.csproj"
exit /b 0

:LAUNCH_PS
powershell -NoProfile -Command "Start-Process powershell -Verb RunAs -ArgumentList ('-NoProfile -ExecutionPolicy Bypass -File ' + [char]34 + '%~dp0scripts\PainelTI.ps1' + [char]34)"
exit /b 0
