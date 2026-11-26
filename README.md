# Painel de Suporte TI

Painel completo para equipes de suporte do dia a dia nas máquinas Windows: limpeza de temporários, fila de impressão, cache do Teams, diagnóstico de hardware e outras rotinas, com registro de auditoria em log local.

---

## 🚀 Versão Visual (C# / WPF - .NET 8)

Esta branch (`csharp`) contém a versão visual moderna desenvolvida em **C# com WPF**:

- **Interface Fluent Dark:** Visual moderno e intuitivo estilo Windows 11.
- **Bilíngue (PT/EN):** Botão no cabeçalho para alternar entre Português e Inglês em tempo real.
- **Dashboard com Telemetria WMI:** Visualização gráfica de uso de CPU, RAM, espaço em disco (C:) e detecção do usuário ativo.
- **Terminal com Streaming em Tempo Real:** Execução assíncrona de comandos com barra de status e cancelamento.
- **Todas as 24 rotinas originais:**
  - Manutenção e Disco (Temp, Cleanmgr, SFC, DISM, Chkdsk, SMART)
  - Rede e Internet (Reset TCP/IP/Winsock, Testes de conectividade, Flush DNS)
  - Impressão (Destravar Spooler, Listar impressoras)
  - Diagnóstico (Ficha técnica completa de hardware, processos)
  - Drivers e Windows Update (Busca via API COM, backup de drivers)
  - Microsoft 365 (Entra ID, Intune Sync, Cache do Teams)
  - Auditoria (Histórico em `C:\ProgramData\PainelTI\painel.log`)

### Como executar a versão C# / WPF:
- Dê dois cliques em `Abrir Painel TI (Visual).bat` (solicita elevação UAC como administrador).
- Ou abra a solução `PainelSuporteTI.slnx` / `PainelSuporteTI.sln` no Visual Studio.

---

## 📜 Versão em PowerShell (Original)

Para executar a versão original em console PowerShell:
- Dê dois cliques em `Abrir Painel TI.bat` (solicita permissão de administrador).
