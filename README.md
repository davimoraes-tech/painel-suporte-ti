# Windows IT Support Panel (Painel de Suporte TI)

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/)
[![Language](https://img.shields.io/badge/Language-100%25%20C%23%20%2F%20WPF-blue.svg)]()
[![Status](https://img.shields.io/badge/Status-Learning%20Project%20%2F%20WIP-orange.svg)]()

> **Navigation / Navegação:**  
> 🇺🇸 [English Version](#-english-version) | 🇧🇷 [Versão em Português](#-versão-em-português)

---

## 🇺🇸 English Version

### ⚠️ Disclaimer & Educational Purpose
This repository is an **educational and learning study project** (*Work-In-Progress*). It is not an officially finalized or commercial enterprise tool, but an experimental project developed to study:
- Porting legacy Windows administrative IT support routines into a **100% modern C# / WPF (.NET 8)** desktop application.
- Real-time command streaming and asynchronous execution with cancellation support.
- Direct hardware telemetry queries via native WMI/CIM.
- Dynamic runtime bilingual localization (Portuguese & English) with fully commented source code in both languages.

### 👏 Credits & Inspiration
This project is inspired by the original **Painel de Suporte TI** CLI concept created by **Davi Moraes** ([@davimoraes-dev](https://github.com/davimoraes-dev)).  
The administrative workflow ideas (active user temp cleanup, print spooler unclogging, network stack reinitialization and M365 diagnostics) were originally conceived by him. All foundational credit for the original routines belongs to the author.

---

### 📂 Repository Structure

```text
Windows-It-Support-Panel/
├── src/
│   └── PainelSuporteTI/              # 100% C# / WPF Application (.NET 8)
│       ├── Models/                   # Telemetry data models
│       ├── Services/                 # WMI diagnostics, execution runner & i18n
│       ├── App.xaml                  # Dark Fluent themes
│       ├── MainWindow.xaml           # Modern visual UI
│       └── PainelSuporteTI.csproj    # Project & UAC administrator manifest
│
├── RunPanel.bat                      # Direct one-click launcher
├── Windows-It-Support-Panel.sln      # Visual Studio 2022 Solution
├── Windows-It-Support-Panel.slnx     # Modern XML Solution format
├── .gitattributes                    # GitHub language stats mapper
└── .gitignore
```

---

### 🚀 How to Run

1. **Via Direct Launcher (Recommended):**  
   Double-click `RunPanel.bat` in the root folder to start the application immediately.
2. **Via Visual Studio:**  
   Open `Windows-It-Support-Panel.sln` in Visual Studio 2022 and press `F5`.

---

## 🇧🇷 Versão em Português

### ⚠️ Aviso e Propósito Educacional
Este repositório é um **projeto experimental de estudos e aprendizado** (*Work-In-Progress*). Não se trata de uma ferramenta comercial finalizada, mas de um estudo prático focado em:
- Migrar rotinas administrativas legadas de suporte no Windows para um aplicativo desktop moderno **100% em C# / WPF (.NET 8)**.
- Execução assíncrona com streaming de saída de terminal em tempo real e cancelamento seguro.
- Leitura direta de telemetria de hardware via WMI/CIM.
- Suporte bilíngue em tempo de execução (PT-BR / EN) com código-fonte amplamente comentado em ambos os idiomas.

### 👏 Créditos e Reconhecimento
Este projeto foi inspirado no conceito original do **Painel de Suporte TI** desenvolvido por **Davi Moraes** ([@davimoraes-dev](https://github.com/davimoraes-dev)).  
A concepção da lógica operacional de suporte (limpeza de temporários no perfil ativo do usuário, destravamento de spooler, reset de pilha IP/Winsock e diagnósticos de M365/Intune) foi originalmente idealizada por ele. Todos os créditos pela inspiração das rotinas pertencem ao autor.

---

### ⚡ Como Executar

1. **Pelo Inicializador Direto (Recomendado):**  
   Dê um duplo clique em `RunPanel.bat` na raiz para iniciar a aplicação visual instantaneamente.
2. **Pelo Visual Studio:**  
   Abra `Windows-It-Support-Panel.sln` no Visual Studio 2022 e pressione `F5`.

---

### 🛠️ Funcionalidades Implementadas

* **Design Dark Fluent:** Visual moderno e limpo inspirado no Windows 11.
* **Bilíngue em Tempo Real:** Botão no topo para alternar entre **Português (PT)** e **Inglês (EN)** instantaneamente.
* **Dashboard com Telemetria WMI:** Identificação da estação, processador, consumo de RAM, saúde do Disco C: e detecção do usuário ativo logado.
* **Terminal com Streaming:** Exibição linha por linha em tempo real da saída de rotinas demoradas (`sfc`, `dism`, drivers) com botão de cancelamento seguro.
* **Rotinas Integradas:**
  * 🧹 **Manutenção:** Limpeza de arquivos temporários (Windows + AppData), `cleanmgr`, reparo SFC, restauração de imagem DISM, CHKDSK e SMART dos discos.
  * 🌐 **Rede:** Reset de TCP/IP, Winsock e DNS, testes de conectividade e atalho para `ncpa.cpl`.
  * 🖨️ **Impressão:** Limpeza de fila travada com reinicialização segura do Spooler e listagem de impressoras.
  * 🩺 **Diagnóstico:** Ficha técnica detalhada por pente de RAM/GPU (exporta `.txt`) e análise de consumo de memória por processo.
  * ⚙️ **Drivers & Updates:** Detecção de erros no Gerenciador de Dispositivos, atualização via Windows Update COM API e backup de drivers instalados.
  * ☁️ **Microsoft 365:** Status de adesão corporativa (`dsregcmd`), sincronização com Intune e limpeza de cache do Teams.
  * 📝 **Auditoria:** Gravação de histórico em `C:\ProgramData\PainelTI\painel.log`.
