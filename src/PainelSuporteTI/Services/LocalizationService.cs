using System.Collections.Generic;

namespace PainelSuporteTI.Services;

public enum AppLanguage
{
    PT,
    EN
}

public static class LocalizationService
{
    public static AppLanguage CurrentLanguage { get; set; } = AppLanguage.PT;

    public static bool IsEnglish => CurrentLanguage == AppLanguage.EN;

    private static readonly Dictionary<string, string> StringsPT = new()
    {
        // Geral / Janela
        ["WindowTitle"] = "Painel de Suporte TI - Central de Atendimento",
        ["AppSubtitle"] = "Suporte Windows v2.0",
        ["SecGeral"] = "GERAL",
        ["SecRotinas"] = "ROTINAS DE SUPORTE",
        ["SecAuditoria"] = "AUDITORIA",
        ["AdminMode"] = "🛡️ Modo Administrador",
        ["UserMode"] = "⚠️ Modo Usuário Comum",
        ["BtnRefresh"] = "🔄 Atualizar",
        ["BtnLang"] = "🌐 English",
        ["UserLabel"] = "Usuário",
        ["PcLabel"] = "PC",
        ["UptimePrefix"] = "Ligado",

        // Menu Lateral
        ["NavDashboard"] = "Visão Geral / Dashboard",
        ["NavManutencao"] = "Manutenção & Disco",
        ["NavRede"] = "Rede & Internet",
        ["NavImpressao"] = "Impressão / Spooler",
        ["NavDiagnostico"] = "Diagnóstico & Hardware",
        ["NavDrivers"] = "Drivers & Updates",
        ["NavM365"] = "Microsoft 365 & Teams",
        ["NavLogs"] = "Registros de Log",

        // Títulos e Subtítulos de Telas
        ["TitleDashboard"] = "Visão Geral do Sistema",
        ["SubDashboard"] = "Diagnóstico rápido e atalhos para as rotinas mais frequentes.",

        ["TitleManutencao"] = "Manutenção e Otimização de Disco",
        ["SubManutencao"] = "Limpeza de arquivos temporários, reparo do Windows (SFC / DISM) e integridade de disco.",

        ["TitleRede"] = "Rede e Conectividade",
        ["SubRede"] = "Redefinição completa de pilha IP, testes de conexão e limpeza de cache DNS.",

        ["TitleImpressao"] = "Impressão e Spooler",
        ["SubImpressao"] = "Destrave documentos retidos, reinicie o spooler de impressão e gerencie impressoras.",

        ["TitleDiagnostico"] = "Diagnóstico de Hardware & Processos",
        ["SubDiagnostico"] = "Ficha técnica completa detalhada por pente de RAM, GPU e monitoramento de processos.",

        ["TitleDrivers"] = "Drivers e Atualizações do Windows",
        ["SubDrivers"] = "Verificação de drivers defeituosos, busca de atualizações e backup de drivers.",

        ["TitleM365"] = "Microsoft 365, Entra ID & Teams",
        ["SubM365"] = "Diagnóstico de adesão corporativa, sincronização com Intune e limpeza de cache do Teams.",

        ["TitleLogs"] = "Registros de Auditoria Local",
        ["SubLogs"] = "Histórico de ações executadas nesta máquina com identificação do operador.",

        // Dashboard Cards
        ["CardDevice"] = "💻 DISPOSITIVO",
        ["CardCpu"] = "⚡ PROCESSADOR",
        ["CardRam"] = "🧠 MEMÓRIA RAM",
        ["CardDiskC"] = "💾 DISCO C:",
        ["StatusActive"] = "Status: Ativo",
        ["QuickActionsTitle"] = "⚡ AÇÕES RÁPIDAS MAIS FREQUENTES",

        ["QuickCleanTempTitle"] = "🧹 Limpar Temporários",
        ["QuickCleanTempDesc"] = "Libera espaço limpando Temp do Windows e do usuário.",
        ["QuickCleanTempBtn"] = "Executar Limpeza",

        ["QuickResetNetTitle"] = "🔄 Reset de Rede",
        ["QuickResetNetDesc"] = "Redefine IP, pilha Winsock e limpa cache de DNS.",
        ["QuickResetNetBtn"] = "Resetar Rede",

        ["QuickSpoolerTitle"] = "🖨️ Destravar Spooler",
        ["QuickSpoolerDesc"] = "Limpa documentos presos e reinicia serviço de impressão.",
        ["QuickSpoolerBtn"] = "Limpar Spooler",

        ["QuickTeamsTitle"] = "💬 Limpar Cache Teams",
        ["QuickTeamsDesc"] = "Fecha o Teams e limpa os arquivos de cache corrompidos.",
        ["QuickTeamsBtn"] = "Limpar Teams",

        // Manutenção
        ["ManutTempTitle"] = "Limpeza de Arquivos Temporários",
        ["ManutTempDesc"] = "Limpa C:\\Windows\\Temp e a pasta Temp do usuário atualmente logado (mantém arquivos abertos e com menos de 24h).",
        ["ManutTempBtn"] = "Executar Limpeza",

        ["ManutCleanmgrTitle"] = "Limpeza de Disco Avançada (cleanmgr)",
        ["ManutCleanmgrDesc"] = "Abre a ferramenta nativa do Windows permitindo apagar instalações anteriores e atualizações antigas.",
        ["ManutCleanmgrBtn"] = "Abrir Cleanmgr",

        ["ManutSfcTitle"] = "Verificação e Reparo de Arquivos do Windows (SFC)",
        ["ManutSfcDesc"] = "Executa sfc /scannow para validar e recuperar arquivos de sistema corrompidos (5 a 15 min).",
        ["ManutSfcBtn"] = "Rodar SFC",

        ["ManutDismTitle"] = "Reparo da Imagem do Windows (DISM)",
        ["ManutDismDesc"] = "Restaura a integridade da imagem do sistema via Windows Update (requer internet).",
        ["ManutDismBtn"] = "Rodar DISM",

        ["ManutChkdskTitle"] = "Verificação de Integridade do Disco C: (CHKDSK)",
        ["ManutChkdskDesc"] = "Executa chkdsk C: /scan em modo somente leitura (online, sem reiniciar).",
        ["ManutChkdskBtn"] = "Verificar Disco",

        ["ManutSmartTitle"] = "Saúde Física e Confiabilidade do Disco (SMART)",
        ["ManutSmartDesc"] = "Exibe temperatura, porcentagem de desgaste de SSDs e contadores de erro físico.",
        ["ManutSmartBtn"] = "Consultar SMART",

        // Rede
        ["RedeResetTitle"] = "Reset Completo da Pilha de Rede",
        ["RedeResetDesc"] = "Executa release/renew de IP, flushdns, reset de Winsock e pilha TCP/IP (requer reiniciar).",
        ["RedeResetBtn"] = "Resetar Rede",

        ["RedeInfoTitle"] = "Diagnóstico e Informações de Rede",
        ["RedeInfoDesc"] = "Lista adaptadores ativos, IP, Gateway, Servidores DNS e detalhes do sinal Wi-Fi.",
        ["RedeInfoBtn"] = "Obter Detalhes",

        ["RedeTestTitle"] = "Bateria de Testes de Conectividade",
        ["RedeTestDesc"] = "Testa conexão até o Roteador/Gateway, Internet (8.8.8.8), DNS, Outlook 365 e Teams.",
        ["RedeTestBtn"] = "Testar Conexões",

        ["RedeDnsTitle"] = "Limpar Cache de DNS (Flush DNS)",
        ["RedeDnsDesc"] = "Esvazia o cache local de resolução de nomes DNS do Windows.",
        ["RedeDnsBtn"] = "Flush DNS",

        ["RedeNcpaTitle"] = "Abrir Conexões de Rede (ncpa.cpl)",
        ["RedeNcpaDesc"] = "Abre a janela tradicional de adaptadores de rede do Painel de Controle.",
        ["RedeNcpaBtn"] = "Abrir Painel",

        // Impressão
        ["ImpSpoolerTitle"] = "Limpar Fila de Impressão Travada",
        ["ImpSpoolerDesc"] = "Para o serviço Spooler, apaga os arquivos da pasta System32\\spool\\PRINTERS e reinicia o serviço.",
        ["ImpSpoolerBtn"] = "Destravar Spooler",

        ["ImpListTitle"] = "Listar Impressoras Instaladas",
        ["ImpListDesc"] = "Exibe o nome das impressoras, status da fila, porta de comunicação e impressora padrão.",
        ["ImpListBtn"] = "Listar Impressoras",

        ["ImpSettingsTitle"] = "Abrir Central de Dispositivos e Impressoras",
        ["ImpSettingsDesc"] = "Abre a interface do Windows para adicionar ou remover impressoras.",
        ["ImpSettingsBtn"] = "Abrir Central",

        // Diagnóstico
        ["DiagFichaTitle"] = "Gerar Ficha Técnica Completa do Computador",
        ["DiagFichaDesc"] = "Levanta placa-mãe, pentes de memória (capacidade/clock/slots), vídeo, monitores WMI e bateria. Salva em .txt.",
        ["DiagFichaBtn"] = "Gerar Relatório",

        ["DiagTopRamTitle"] = "Top 10 Processos por Uso de Memória",
        ["DiagTopRamDesc"] = "Descobre quais aplicativos estão consumindo mais RAM neste momento.",
        ["DiagTopRamBtn"] = "Analisar Processos",

        ["DiagResmonTitle"] = "Abrir Monitor de Recursos (resmon)",
        ["DiagResmonDesc"] = "Abre a ferramenta gráfica detalhada de CPU, Disco, Rede e Memória do Windows.",
        ["DiagResmonBtn"] = "Abrir Resmon",

        // Drivers
        ["DrvErrorsTitle"] = "Dispositivos com Erro (Gerenciador de Dispositivos)",
        ["DrvErrorsDesc"] = "Varre a árvore Plug and Play procurando componentes com interrogação amarela ou falhas.",
        ["DrvErrorsBtn"] = "Verificar Erros",

        ["DrvUpdateTitle"] = "Atualizar Drivers via Windows Update API",
        ["DrvUpdateDesc"] = "Consulta a API COM do Windows Update por novos pacotes de drivers e instala automaticamente.",
        ["DrvUpdateBtn"] = "Atualizar Drivers",

        ["DrvBackupTitle"] = "Exportar Backup dos Drivers de Terceiros",
        ["DrvBackupDesc"] = "Exporta todos os drivers instalados nesta máquina para C:\\ProgramData\\PainelTI\\Drivers_...",
        ["DrvBackupBtn"] = "Fazer Backup",

        ["DrvWinUpdateTitle"] = "Abrir Windows Update & Iniciar Verificação",
        ["DrvWinUpdateDesc"] = "Abre a tela nativa de configurações e aciona o UsoClient para buscar atualizações.",
        ["DrvWinUpdateBtn"] = "Abrir Windows Update",

        ["DrvDevmgmtTitle"] = "Abrir Gerenciador de Dispositivos (devmgmt.msc)",
        ["DrvDevmgmtDesc"] = "Abre a árvore tradicional de componentes de hardware do Windows.",
        ["DrvDevmgmtBtn"] = "Abrir Gerenciador",

        // Microsoft 365
        ["M365StatusTitle"] = "Status do Vínculo Microsoft Entra ID / Intune",
        ["M365StatusDesc"] = "Executa dsregcmd /status para verificar se a máquina está conectada à conta corporativa.",
        ["M365StatusBtn"] = "Verificar Vínculo",

        ["M365IntuneTitle"] = "Forçar Sincronização com o Microsoft Intune",
        ["M365IntuneDesc"] = "Dispara a tarefa PushLaunch para puxar novas políticas de conformidade e aplicativos da nuvem.",
        ["M365IntuneBtn"] = "Sincronizar Intune",

        ["M365TeamsTitle"] = "Limpar Cache do Microsoft Teams",
        ["M365TeamsDesc"] = "Resolve problemas de login e travamento fechando o Teams e limpando o diretório de cache do usuário ativo.",
        ["M365TeamsBtn"] = "Limpar Cache Teams",

        // Logs
        ["LogsHistoryTitle"] = "Histórico de Auditoria Local",
        ["LogsHistoryDesc"] = "Registra data, hora, máquina, operador e ação realizada em C:\\ProgramData\\PainelTI\\painel.log",
        ["LogsOpenFolderBtn"] = "📂 Abrir Pasta",
        ["LogsReloadBtn"] = "🔄 Recarregar",

        // Terminal
        ["TerminalHeader"] = "📟 TERMINAL / SAÍDA EM TEMPO REAL",
        ["TerminalReady"] = "Pronto",
        ["TerminalRunning"] = "Executando",
        ["TerminalBtnCopy"] = "📋 Copiar",
        ["TerminalBtnClear"] = "🗑️ Limpar",
        ["TerminalBtnCancel"] = "⏹️ Interromper",
        ["TerminalAutoScroll"] = "Auto-scroll"
    };

    private static readonly Dictionary<string, string> StringsEN = new()
    {
        // General / Window
        ["WindowTitle"] = "IT Support Panel - Service Desk Center",
        ["AppSubtitle"] = "Windows Support v2.0",
        ["SecGeral"] = "GENERAL",
        ["SecRotinas"] = "SUPPORT ROUTINES",
        ["SecAuditoria"] = "AUDITING",
        ["AdminMode"] = "🛡️ Administrator Mode",
        ["UserMode"] = "⚠️ Standard User Mode",
        ["BtnRefresh"] = "🔄 Refresh",
        ["BtnLang"] = "🌐 Português",
        ["UserLabel"] = "User",
        ["PcLabel"] = "PC",
        ["UptimePrefix"] = "Uptime",

        // Sidebar Navigation
        ["NavDashboard"] = "System Overview / Dashboard",
        ["NavManutencao"] = "Maintenance & Disk",
        ["NavRede"] = "Network & Internet",
        ["NavImpressao"] = "Printing / Spooler",
        ["NavDiagnostico"] = "Diagnostics & Hardware",
        ["NavDrivers"] = "Drivers & Updates",
        ["NavM365"] = "Microsoft 365 & Teams",
        ["NavLogs"] = "Audit Logs",

        // Screen Titles and Subtitles
        ["TitleDashboard"] = "System Overview",
        ["SubDashboard"] = "Quick diagnostics and one-click shortcuts for frequent routines.",

        ["TitleManutencao"] = "Maintenance & Disk Optimization",
        ["SubManutencao"] = "Temporary files cleanup, Windows image repair (SFC / DISM) and disk integrity.",

        ["TitleRede"] = "Network & Connectivity",
        ["SubRede"] = "Full IP stack reset, connectivity tests and DNS cache flush.",

        ["TitleImpressao"] = "Printing & Spooler",
        ["SubImpressao"] = "Clear stuck print jobs, safely restart print spooler service and manage printers.",

        ["TitleDiagnostico"] = "Hardware & Process Diagnostics",
        ["SubDiagnostico"] = "Full technical hardware specs per RAM slot, GPU metrics and process memory inspection.",

        ["TitleDrivers"] = "Drivers & Windows Updates",
        ["SubDrivers"] = "Detect faulty hardware devices, download driver updates and backup third-party drivers.",

        ["TitleM365"] = "Microsoft 365, Entra ID & Teams",
        ["SubM365"] = "Corporate cloud join diagnostics, force Intune policy sync and clean Teams cache.",

        ["TitleLogs"] = "Local Audit Records",
        ["SubLogs"] = "Log records of actions executed on this workstation with operator identification.",

        // Dashboard Cards
        ["CardDevice"] = "💻 DEVICE",
        ["CardCpu"] = "⚡ PROCESSOR",
        ["CardRam"] = "🧠 RAM MEMORY",
        ["CardDiskC"] = "💾 DISK C:",
        ["StatusActive"] = "Status: Active",
        ["QuickActionsTitle"] = "⚡ FREQUENT QUICK ACTIONS",

        ["QuickCleanTempTitle"] = "🧹 Clean Temp Files",
        ["QuickCleanTempDesc"] = "Frees up storage by cleaning Windows and active user temp folders.",
        ["QuickCleanTempBtn"] = "Run Cleanup",

        ["QuickResetNetTitle"] = "🔄 Reset Network",
        ["QuickResetNetDesc"] = "Resets IP configuration, Winsock catalog and flushes DNS cache.",
        ["QuickResetNetBtn"] = "Reset Network",

        ["QuickSpoolerTitle"] = "🖨️ Fix Print Spooler",
        ["QuickSpoolerDesc"] = "Clears stuck print queue and restarts Windows printing service.",
        ["QuickSpoolerBtn"] = "Clean Spooler",

        ["QuickTeamsTitle"] = "💬 Clear Teams Cache",
        ["QuickTeamsDesc"] = "Closes Microsoft Teams and purges corrupted local cache files.",
        ["QuickTeamsBtn"] = "Clear Teams",

        // Maintenance
        ["ManutTempTitle"] = "Temporary Files Cleanup",
        ["ManutTempDesc"] = "Cleans C:\\Windows\\Temp and active logged user Temp folder (keeps locked files and files < 24h old).",
        ["ManutTempBtn"] = "Run Cleanup",

        ["ManutCleanmgrTitle"] = "Advanced Disk Cleanup (cleanmgr)",
        ["ManutCleanmgrDesc"] = "Opens native Windows Disk Cleanup to remove previous installations and update residues.",
        ["ManutCleanmgrBtn"] = "Open Cleanmgr",

        ["ManutSfcTitle"] = "System File Checker & Repair (SFC)",
        ["ManutSfcDesc"] = "Executes sfc /scannow to detect and repair corrupted system files (takes 5 to 15 min).",
        ["ManutSfcBtn"] = "Run SFC",

        ["ManutDismTitle"] = "Deployment Image Servicing and Repair (DISM)",
        ["ManutDismDesc"] = "Restores Windows system image health via Windows Update (internet connection required).",
        ["ManutDismBtn"] = "Run DISM",

        ["ManutChkdskTitle"] = "Disk C: Integrity Scan (CHKDSK)",
        ["ManutChkdskDesc"] = "Runs chkdsk C: /scan in online read-only mode (no reboot required).",
        ["ManutChkdskBtn"] = "Scan Disk",

        ["ManutSmartTitle"] = "Physical Disk Health & Reliability (SMART)",
        ["ManutSmartDesc"] = "Displays temperature, SSD wear percentage and hardware read error counters.",
        ["ManutSmartBtn"] = "Check SMART",

        // Network
        ["RedeResetTitle"] = "Full Network Stack Reset",
        ["RedeResetDesc"] = "Executes IP release/renew, flushdns, Winsock and TCP/IP stack reset (requires reboot).",
        ["RedeResetBtn"] = "Reset Network",

        ["RedeInfoTitle"] = "Network Diagnostics & Info",
        ["RedeInfoDesc"] = "Lists active network adapters, IP address, Gateway, DNS servers and Wi-Fi signal metrics.",
        ["RedeInfoBtn"] = "Get Info",

        ["RedeTestTitle"] = "Connectivity Test Suite",
        ["RedeTestDesc"] = "Tests connectivity to Local Gateway, Public Internet (8.8.8.8), DNS, Outlook 365 and Teams.",
        ["RedeTestBtn"] = "Run Tests",

        ["RedeDnsTitle"] = "Clear DNS Cache (Flush DNS)",
        ["RedeDnsDesc"] = "Flushes the local Windows DNS name resolution client cache.",
        ["RedeDnsBtn"] = "Flush DNS",

        ["RedeNcpaTitle"] = "Open Network Connections (ncpa.cpl)",
        ["RedeNcpaDesc"] = "Opens classic Control Panel Network Adapters window.",
        ["RedeNcpaBtn"] = "Open Panel",

        // Printing
        ["ImpSpoolerTitle"] = "Clear Stuck Print Queue",
        ["ImpSpoolerDesc"] = "Stops Print Spooler, purges System32\\spool\\PRINTERS files and safely restarts service.",
        ["ImpSpoolerBtn"] = "Fix Spooler",

        ["ImpListTitle"] = "List Installed Printers",
        ["ImpListDesc"] = "Displays printer name, queue status, port binding and default printer status.",
        ["ImpListBtn"] = "List Printers",

        ["ImpSettingsTitle"] = "Open Printers & Scanners Center",
        ["ImpSettingsDesc"] = "Opens Windows Settings to view, add or configure printers.",
        ["ImpSettingsBtn"] = "Open Settings",

        // Diagnostics
        ["DiagFichaTitle"] = "Generate Complete Computer Spec Sheet",
        ["DiagFichaDesc"] = "Retrieves motherboard, memory sticks (clock/slots), GPU, WMI monitors and battery. Saves to .txt.",
        ["DiagFichaBtn"] = "Generate Report",

        ["DiagTopRamTitle"] = "Top 10 Processes by Memory Usage",
        ["DiagTopRamDesc"] = "Identifies which software applications are consuming the most RAM right now.",
        ["DiagTopRamBtn"] = "Analyze Processes",

        ["DiagResmonTitle"] = "Open Resource Monitor (resmon)",
        ["DiagResmonDesc"] = "Opens detailed Windows tool for real-time CPU, Disk, Network and Memory metrics.",
        ["DiagResmonBtn"] = "Open Resmon",

        // Drivers
        ["DrvErrorsTitle"] = "Devices with Error / Missing Drivers",
        ["DrvErrorsDesc"] = "Scans Plug & Play tree looking for components with yellow warning signs or failures.",
        ["DrvErrorsBtn"] = "Scan Errors",

        ["DrvUpdateTitle"] = "Update Drivers via Windows Update API",
        ["DrvUpdateDesc"] = "Queries Microsoft Update COM API for missing driver packages and installs them.",
        ["DrvUpdateBtn"] = "Update Drivers",

        ["DrvBackupTitle"] = "Export Third-Party Driver Backup",
        ["DrvBackupDesc"] = "Exports all installed third-party drivers to C:\\ProgramData\\PainelTI\\Drivers_...",
        ["DrvBackupBtn"] = "Backup Drivers",

        ["DrvWinUpdateTitle"] = "Open Windows Update & Trigger Scan",
        ["DrvWinUpdateDesc"] = "Opens native update settings and signals UsoClient to check for updates.",
        ["DrvWinUpdateBtn"] = "Open Windows Update",

        ["DrvDevmgmtTitle"] = "Open Device Manager (devmgmt.msc)",
        ["DrvDevmgmtDesc"] = "Opens classic Windows Device Manager hardware tree.",
        ["DrvDevmgmtBtn"] = "Open Device Manager",

        // Microsoft 365
        ["M365StatusTitle"] = "Microsoft Entra ID / Intune Join Status",
        ["M365StatusDesc"] = "Executes dsregcmd /status to check if workstation is joined to corporate cloud.",
        ["M365StatusBtn"] = "Check Join Status",

        ["M365IntuneTitle"] = "Force Microsoft Intune Policy Sync",
        ["M365IntuneDesc"] = "Triggers PushLaunch scheduled task to fetch compliance policies and cloud packages.",
        ["M365IntuneBtn"] = "Sync Intune",

        ["M365TeamsTitle"] = "Clear Microsoft Teams Cache",
        ["M365TeamsDesc"] = "Fixes login and crash issues by terminating Teams and cleaning local user cache folders.",
        ["M365TeamsBtn"] = "Clear Teams Cache",

        // Logs
        ["LogsHistoryTitle"] = "Local Audit History",
        ["LogsHistoryDesc"] = "Records date, time, workstation, operator and action in C:\\ProgramData\\PainelTI\\painel.log",
        ["LogsOpenFolderBtn"] = "📂 Open Folder",
        ["LogsReloadBtn"] = "🔄 Reload",

        // Terminal
        ["TerminalHeader"] = "📟 TERMINAL / REAL-TIME OUTPUT",
        ["TerminalReady"] = "Ready",
        ["TerminalRunning"] = "Running",
        ["TerminalBtnCopy"] = "📋 Copy",
        ["TerminalBtnClear"] = "🗑️ Clear",
        ["TerminalBtnCancel"] = "⏹️ Cancel",
        ["TerminalAutoScroll"] = "Auto-scroll"
    };

    public static string Get(string key)
    {
        var dict = IsEnglish ? StringsEN : StringsPT;
        if (dict.TryGetValue(key, out var val))
            return val;
        if (StringsPT.TryGetValue(key, out var fallback))
            return fallback;
        return key;
    }
}
