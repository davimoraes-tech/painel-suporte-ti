@echo off
rem Inicia o Painel de Suporte TI (Versao C# / WPF)
set "EXE_PATH=%~dp0..\src\PainelSuporteTI\bin\Release\net8.0-windows\PainelSuporteTI.exe"

if not exist "%EXE_PATH%" (
    set "EXE_PATH=%~dp0..\src\PainelSuporteTI\bin\Debug\net8.0-windows\PainelSuporteTI.exe"
)

if exist "%EXE_PATH%" (
    start "" "%EXE_PATH%"
    exit /b 0
)

echo Compilando e iniciando versao C# WPF...
dotnet run --project "%~dp0..\src\PainelSuporteTI\PainelSuporteTI.csproj"
