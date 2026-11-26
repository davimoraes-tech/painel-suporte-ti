# Windows IT Support Panel (Painel de Suporte TI)

Painel centralizado para técnicos de suporte e administradores de sistemas Windows realizarem diagnósticos rápidos, manutenções rotineiras, correções de rede, fila de impressão e auditoria.

Disponível em duas versões integradas: **Interface Visual Moderna (C# / WPF)** e **Console Tradicional (PowerShell)**.

---

## 📂 Estrutura do Projeto

```text
Windows-It-Support-Panel/
├── src/
│   └── PainelSuporteTI/              # Projeto C# WPF (.NET 8)
│       ├── Models/                   # Modelos de telemetria e dados
│       ├── Services/                 # Diagnósticos WMI, PowerShell Runner e Tradução
│       ├── App.xaml                  # Estilos globais Dark Fluent
│       ├── MainWindow.xaml           # Interface gráfica moderna
│       └── PainelSuporteTI.csproj    # Configurações do projeto e manifesto UAC
│
├── scripts/
│   ├── PainelTI.ps1                  # Script de suporte original em PowerShell
│   ├── run-powershell.bat            # Executar diretamente o console PowerShell
│   └── run-wpf.bat                   # Executar diretamente a versão C# WPF
│
├── Iniciar-Painel.bat                # Launcher principal (Menu de inicialização)
├── Windows-It-Support-Panel.sln      # Solução para Visual Studio 2022
├── Windows-It-Support-Panel.slnx     # Solução no formato moderno XML
└── .gitignore
```

---

## ⚡ Como Executar

### Opção 1: Pelo Launcher Principal (Recomendado)
Dê um duplo clique em:
* **`Iniciar-Painel.bat`** (Abre um menu simples para escolher entre a versão Visual C# WPF ou o Console PowerShell).

### Opção 2: Pelo Visual Studio
Abra o arquivo de solução:
* **`Windows-It-Support-Panel.sln`** (Pressione `F5` para compilar e depurar).

---

## 🚀 Funcionalidades da Versão Visual (C# / WPF)

* **Design Dark Fluent:** Visual moderno inspirado no Windows 11.
* **Bilíngue em Tempo Real:** Alternância instantânea entre **Português (PT)** e **Inglês (EN)** no cabeçalho.
* **Dashboard com Telemetria WMI:** Indicadores de CPU, memória RAM, integridade e uso do Disco C: e identificação do usuário logado na estação.
* **Terminal com Streaming:** Saída de texto ao vivo de rotinas demoradas (`sfc`, `dism`, downloads de drivers) com botão de cancelamento seguro.
* **Rotinas Integradas:**
  * 🧹 **Manutenção:** Limpeza de temporários, `cleanmgr`, SFC, DISM, Chkdsk e saúde física SMART de discos.
  * 🌐 **Rede:** Reset completo de pilha IP/Winsock, testes de conectividade, Flush DNS e `ncpa.cpl`.
  * 🖨️ **Impressão:** Destravar fila do Spooler e listar impressoras.
  * 🩺 **Diagnóstico:** Ficha técnica detalhada por pente de memória/GPU e monitor de processos por RAM.
  * ⚙️ **Drivers & Updates:** Detecção de erros no Gerenciador de Dispositivos, atualização via Windows Update API e backup de drivers.
  * ☁️ **Microsoft 365:** Diagnóstico Entra ID (`dsregcmd`), sincronização com Intune e limpeza de cache do Teams.
  * 📝 **Auditoria:** Registro local em `C:\ProgramData\PainelTI\painel.log`.
