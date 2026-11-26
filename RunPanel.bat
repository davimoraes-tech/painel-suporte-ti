@echo off
setlocal
rem =========================================================================
rem [PT-BR] Inicializador do Painel de Suporte TI Windows (C# / WPF - .NET 8)
rem [EN]    Launcher for the Windows IT Support Panel (C# / WPF - .NET 8)
rem =========================================================================

title Windows IT Support Panel

set "EXE_PUB=%~dp0Publish\PainelSuporteTI.exe"
set "EXE_RELEASE=%~dp0src\PainelSuporteTI\bin\Release\net8.0-windows\PainelSuporteTI.exe"
set "EXE_DEBUG=%~dp0src\PainelSuporteTI\bin\Debug\net8.0-windows\PainelSuporteTI.exe"

rem [PT-BR] 1. Tenta executar o binario de publicacao otimizado
rem [EN]    1. Attempt to run optimized publish binary
if exist "%EXE_PUB%" (
    start "" "%EXE_PUB%"
    exit /b 0
)

rem [PT-BR] 2. Tenta executar a versao compilada em Release
rem [EN]    2. Attempt to run compiled Release build
if exist "%EXE_RELEASE%" (
    start "" "%EXE_RELEASE%"
    exit /b 0
)

rem [PT-BR] 3. Tenta executar a versao compilada em Debug
rem [EN]    3. Attempt to run compiled Debug build
if exist "%EXE_DEBUG%" (
    start "" "%EXE_DEBUG%"
    exit /b 0
)

rem [PT-BR] 4. Se nenhum executavel for encontrado, compila e roda via .NET SDK
rem [EN]    4. If no binary exists, build and run directly via .NET SDK
echo [PT-BR] Compilando e iniciando a versao C# WPF...
echo [EN]    Building and starting C# WPF application...
dotnet run --project "%~dp0src\PainelSuporteTI\PainelSuporteTI.csproj"
exit /b 0
