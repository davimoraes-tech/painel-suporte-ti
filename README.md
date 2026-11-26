# Windows IT Support Panel (Painel de Suporte TI)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-C%23%20%7C%20PowerShell-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Learning%20Project%20%2F%20WIP-orange.svg)]()

> **Navigation / Navegação:**  
> 🇺🇸 [English Version](#-english-version) | 🇧🇷 [Versão em Português](#-versão-em-português)

---

## 🇺🇸 English Version

### ⚠️ Disclaimer & Project Purpose
This repository is an **educational and learning project** (Work-In-Progress). It is not an officially complete or production-hardened enterprise tool, but an experimental study designed to explore:
- Porting legacy Windows administrative **PowerShell CLI** routines into a modern **C# / WPF (.NET 8)** desktop application.
- Real-time command streaming and asynchronous process execution in WPF.
- Hardware telemetry extraction via WMI/CIM namespaces.
- Dynamic runtime bilingual localization (PT/EN).

### 👏 Credits & Acknowledgments
This project is an evolution based on the original **PowerShell IT Support Panel** created by **Davi Moraes** ([@davimoraes-dev](https://github.com/davimoraes-dev)).  
The underlying administrative routines (temporary file purges, print spooler unclogging, network stack reinitialization, and M365/Intune health checks) were originally conceived and scripted by him. All original credits for the foundational automation scripts belong to the author.

---

### 📂 Repository Structure

```text
Windows-It-Support-Panel/
├── src/
│   └── PainelSuporteTI/              # C# / WPF Application (.NET 8)
│       ├── Models/                   # Data and telemetry models
│       ├── Services/                 # WMI diagnostics, PowerShell runner & i18n
│       ├── App.xaml                  # Dark Fluent UI themes
│       ├── MainWindow.xaml           # Graphical interface
│       └── PainelSuporteTI.csproj    # Project & UAC manifest
│
├── scripts/
│   ├── PainelTI.ps1                  # Original PowerShell script by Davi Moraes
│   ├── run-powershell.bat            # Quick launcher for PowerShell console
│   └── run-wpf.bat                   # Quick launcher for WPF application
│
├── RunPanel.bat                      # Unified interactive launcher
├── Windows-It-Support-Panel.sln      # Visual Studio Solution
└── .gitignore
```

---

### 🚀 How to Run

1. **Via Unified Launcher (Recommended):**  
   Double-click `RunPanel.bat` at the root folder to choose between launching the **Modern WPF Panel** or the **Traditional PowerShell Console**.
2. **Via Visual Studio:**  
   Open `Windows-It-Support-Panel.sln` in Visual Studio 2022 and press `F5`.

---

## 🇧🇷 Versão em Português

### ⚠️ Aviso e Propósito do Projeto
Este repositório é um **projeto experimental de aprendizado e estudos** (em desenvolvimento / *Work-In-Progress*). Não se trata de uma ferramenta empresarial finalizada, mas de um estudo prático focado em:
- Transformar rotinas administrativas de suporte em **PowerShell (console)** em uma interface visual moderna usando **C# e WPF (.NET 8)**.
- Execução assíncrona com streaming de saída de terminal em tempo real.
- Leitura de telemetria de hardware via WMI/CIM.
- Localização dinâmica e bilíngue em tempo de execução (Português e Inglês).

### 👏 Créditos e Reconhecimento
Este projeto é baseado e inspirado no script original **Painel de Suporte TI** desenvolvido por **Davi Moraes** ([@davimoraes-dev](https://github.com/davimoraes-dev)).  
A concepção da lógica operacional (limpeza de temporários no perfil ativo do usuário, destravamento de spooler, reset de pilha IP/Winsock e diagnósticos de M365/Intune) foi originalmente criada por ele. Todos os créditos pela base dos scripts pertencem ao autor original.

---

### ⚡ Como Executar

1. **Pelo Inicializador Unificado (Recomendado):**  
   Dê um duplo clique em `RunPanel.bat` na raiz para escolher entre a versão Visual C# WPF ou o Console PowerShell.
2. **Pelo Visual Studio:**  
   Abra `Windows-It-Support-Panel.sln` no Visual Studio 2022 e pressione `F5`.

---

### 🛠️ Funcionalidades Implementadas

* **Design Dark Fluent:** Visual moderno inspirado nas diretrizes do Windows 11.
* **Bilíngue em Tempo Real:** Botão no cabeçalho para alternar entre **Português (PT)** e **Inglês (EN)** instantaneamente.
* **Dashboard com Telemetria WMI:** Identificação da estação, processador, percentual de uso de RAM, saúde do Disco C: e detecção do usuário ativo logado.
* **Terminal com Streaming:** Exibição linha por linha em tempo real da saída de rotinas demoradas (`sfc`, `dism`, drivers) com botão de cancelamento seguro.
* **Rotinas de Suporte:**
  * 🧹 **Manutenção:** Limpeza de arquivos temporários (Windows + AppData), `cleanmgr`, reparo de integridade SFC, restauração de imagem DISM, CHKDSK e SMART dos discos.
  * 🌐 **Rede:** Reset completo de TCP/IP, Winsock e DNS, testes de conectividade e atalho para `ncpa.cpl`.
  * 🖨️ **Impressão:** Limpeza de fila travada com reinicialização segura do Spooler e listagem de impressoras.
  * 🩺 **Diagnóstico:** Ficha técnica detalhada por pente de RAM/GPU (exporta `.txt`) e análise de consumo de memória por processo.
  * ⚙️ **Drivers & Updates:** Detecção de erros no Gerenciador de Dispositivos, atualização via Windows Update COM API e backup de drivers instalados.
  * ☁️ **Microsoft 365:** Status de adesão corporativa (`dsregcmd`), sincronização com Intune e limpeza de cache do Teams.
  * 📝 **Auditoria:** Gravação de histórico em `C:\ProgramData\PainelTI\painel.log`.
