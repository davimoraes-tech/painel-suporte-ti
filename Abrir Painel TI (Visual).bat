@echo off
rem Inicia o Painel de Suporte TI (Versao Visual C# / WPF)
set "EXE_PATH=%~dp0PainelSuporteTI\bin\Release\net8.0-windows\PainelSuporteTI.exe"

if not exist "%EXE_PATH%" (
    set "EXE_PATH=%~dp0PainelSuporteTI\bin\Debug\net8.0-windows\PainelSuporteTI.exe"
)

if exist "%EXE_PATH%" (
    start "" "%EXE_PATH%"
    exit /b 0
)

echo Compilando e iniciando versao C# WPF...
dotnet run --project "%~dp0PainelSuporteTI\PainelSuporteTI.csproj"
