@echo off
rem Executa o script original do Painel TI em PowerShell como Administrador
if not exist "%~dp0PainelTI.ps1" (
    echo [X] Nao encontrei o script PainelTI.ps1 nesta pasta.
    pause
    exit /b 1
)
powershell -NoProfile -Command "Start-Process powershell -Verb RunAs -ArgumentList ('-NoProfile -ExecutionPolicy Bypass -File ' + [char]34 + '%~dp0PainelTI.ps1' + [char]34)"
