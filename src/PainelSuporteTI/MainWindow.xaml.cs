using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PainelSuporteTI.Services;

namespace PainelSuporteTI;

/// <summary>
/// [PT-BR] Janela principal do Painel de Suporte TI gerenciando navegacao, telemetria e o terminal de saida.
/// [EN]    Main window of the IT Support Panel managing navigation, telemetry and output terminal.
/// </summary>
public partial class MainWindow : Window
{
    private CancellationTokenSource? _cts;
    private bool _isExecuting = false;
    private string _currentActionName = "";

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }

    /// <summary>
    /// [PT-BR] Evento disparado quando a janela e carregada pela primeira vez.
    /// [EN]    Event triggered when the window is loaded for the first time.
    /// </summary>
    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        AplicarIdioma();
        VerificarAdmin();
        await CarregarDashboardAsync();
        CarregarLogs();
        
        var msgInicial = LocalizationService.IsEnglish 
            ? "=== Windows IT Support Panel successfully started ===\nSelect an option from the menu or use the Dashboard quick actions."
            : "=== Painel de Suporte TI iniciado com sucesso ===\nSelecione uma opção no menu ou utilize os atalhos rápidos do Dashboard.";
        AdicionarLinhaTerminal(msgInicial, false);
    }

    /// <summary>
    /// [PT-BR] Verifica se a janela esta elevada como Administrador e atualiza o badge visual.
    /// [EN]    Verifies whether the window is elevated as Administrator and updates visual badge.
    /// </summary>
    private void VerificarAdmin()
    {
        var isAdmin = PowerShellRunner.EhAdmin();
        if (isAdmin)
        {
            TxtAdminBadge.Text = LocalizationService.Get("AdminMode");
            TxtAdminBadge.Foreground = (Brush)FindResource("Success");
        }
        else
        {
            TxtAdminBadge.Text = LocalizationService.Get("UserMode");
            TxtAdminBadge.Foreground = (Brush)FindResource("Warning");
            var aviso = LocalizationService.IsEnglish
                ? "[WARNING] Panel was not started as Administrator. Some system functions may fail."
                : "[AVISO] O painel não foi aberto como Administrador. Algumas funções do sistema podem falhar.";
            AdicionarLinhaTerminal(aviso, true);
        }
    }

    /// <summary>
    /// [PT-BR] Alterna dinamicamente o idioma da aplicacao entre Portugues e Ingles.
    /// [EN]    Dynamically toggles application language between Portuguese and English.
    /// </summary>
    private void BtnIdioma_Click(object sender, RoutedEventArgs e)
    {
        LocalizationService.CurrentLanguage = LocalizationService.IsEnglish ? AppLanguage.PT : AppLanguage.EN;
        AplicarIdioma();
        VerificarAdmin();
    }

    /// <summary>
    /// [PT-BR] Atualiza todos os textos e rotulos de todos os controles para o idioma selecionado.
    /// [EN]    Updates all UI text and control labels to the selected language.
    /// </summary>
    private void AplicarIdioma()
    {
        Title = LocalizationService.Get("WindowTitle");
        TxtAppSubtitle.Text = LocalizationService.Get("AppSubtitle");
        TxtSecGeral.Text = LocalizationService.Get("SecGeral");
        TxtSecRotinas.Text = LocalizationService.Get("SecRotinas");
        TxtSecAuditoria.Text = LocalizationService.Get("SecAuditoria");
        BtnAtualizarGeral.Content = LocalizationService.Get("BtnRefresh");
        BtnIdioma.Content = LocalizationService.Get("BtnLang");

        // Menu Lateral / Sidebar
        TxtNavDashboard.Text = LocalizationService.Get("NavDashboard");
        TxtNavManutencao.Text = LocalizationService.Get("NavManutencao");
        TxtNavRede.Text = LocalizationService.Get("NavRede");
        TxtNavImpressao.Text = LocalizationService.Get("NavImpressao");
        TxtNavDiagnostico.Text = LocalizationService.Get("NavDiagnostico");
        TxtNavDrivers.Text = LocalizationService.Get("NavDrivers");
        TxtNavM365.Text = LocalizationService.Get("NavM365");
        TxtNavLogs.Text = LocalizationService.Get("NavLogs");

        // Dashboard
        LblCardDevice.Text = LocalizationService.Get("CardDevice");
        LblCardCpu.Text = LocalizationService.Get("CardCpu");
        LblCardRam.Text = LocalizationService.Get("CardRam");
        LblCardDiskC.Text = LocalizationService.Get("CardDiskC");
        LblStatusActive.Text = LocalizationService.Get("StatusActive");
        LblQuickActionsTitle.Text = LocalizationService.Get("QuickActionsTitle");

        TxtQuickCleanTempTitle.Text = LocalizationService.Get("QuickCleanTempTitle");
        TxtQuickCleanTempDesc.Text = LocalizationService.Get("QuickCleanTempDesc");
        BtnQuickCleanTemp.Content = LocalizationService.Get("QuickCleanTempBtn");

        TxtQuickResetNetTitle.Text = LocalizationService.Get("QuickResetNetTitle");
        TxtQuickResetNetDesc.Text = LocalizationService.Get("QuickResetNetDesc");
        BtnQuickResetNet.Content = LocalizationService.Get("QuickResetNetBtn");

        TxtQuickSpoolerTitle.Text = LocalizationService.Get("QuickSpoolerTitle");
        TxtQuickSpoolerDesc.Text = LocalizationService.Get("QuickSpoolerDesc");
        BtnQuickSpooler.Content = LocalizationService.Get("QuickSpoolerBtn");

        TxtQuickTeamsTitle.Text = LocalizationService.Get("QuickTeamsTitle");
        TxtQuickTeamsDesc.Text = LocalizationService.Get("QuickTeamsDesc");
        BtnQuickTeams.Content = LocalizationService.Get("QuickTeamsBtn");

        // Manutencao / Maintenance
        TxtManutTempTitle.Text = LocalizationService.Get("ManutTempTitle");
        TxtManutTempDesc.Text = LocalizationService.Get("ManutTempDesc");
        BtnManutTemp.Content = LocalizationService.Get("ManutTempBtn");

        TxtManutCleanmgrTitle.Text = LocalizationService.Get("ManutCleanmgrTitle");
        TxtManutCleanmgrDesc.Text = LocalizationService.Get("ManutCleanmgrDesc");
        BtnManutCleanmgr.Content = LocalizationService.Get("ManutCleanmgrBtn");

        TxtManutSfcTitle.Text = LocalizationService.Get("ManutSfcTitle");
        TxtManutSfcDesc.Text = LocalizationService.Get("ManutSfcDesc");
        BtnManutSfc.Content = LocalizationService.Get("ManutSfcBtn");

        TxtManutDismTitle.Text = LocalizationService.Get("ManutDismTitle");
        TxtManutDismDesc.Text = LocalizationService.Get("ManutDismDesc");
        BtnManutDism.Content = LocalizationService.Get("ManutDismBtn");

        TxtManutChkdskTitle.Text = LocalizationService.Get("ManutChkdskTitle");
        TxtManutChkdskDesc.Text = LocalizationService.Get("ManutChkdskDesc");
        BtnManutChkdsk.Content = LocalizationService.Get("ManutChkdskBtn");

        TxtManutSmartTitle.Text = LocalizationService.Get("ManutSmartTitle");
        TxtManutSmartDesc.Text = LocalizationService.Get("ManutSmartDesc");
        BtnManutSmart.Content = LocalizationService.Get("ManutSmartBtn");

        // Rede / Network
        TxtRedeResetTitle.Text = LocalizationService.Get("RedeResetTitle");
        TxtRedeResetDesc.Text = LocalizationService.Get("RedeResetDesc");
        BtnRedeReset.Content = LocalizationService.Get("RedeResetBtn");

        TxtRedeInfoTitle.Text = LocalizationService.Get("RedeInfoTitle");
        TxtRedeInfoDesc.Text = LocalizationService.Get("RedeInfoDesc");
        BtnRedeInfo.Content = LocalizationService.Get("RedeInfoBtn");

        TxtRedeTestTitle.Text = LocalizationService.Get("RedeTestTitle");
        TxtRedeTestDesc.Text = LocalizationService.Get("RedeTestDesc");
        BtnRedeTest.Content = LocalizationService.Get("RedeTestBtn");

        TxtRedeDnsTitle.Text = LocalizationService.Get("RedeDnsTitle");
        TxtRedeDnsDesc.Text = LocalizationService.Get("RedeDnsDesc");
        BtnRedeDns.Content = LocalizationService.Get("RedeDnsBtn");

        TxtRedeNcpaTitle.Text = LocalizationService.Get("RedeNcpaTitle");
        TxtRedeNcpaDesc.Text = LocalizationService.Get("RedeNcpaDesc");
        BtnRedeNcpa.Content = LocalizationService.Get("RedeNcpaBtn");

        // Impressao / Printing
        TxtImpSpoolerTitle.Text = LocalizationService.Get("ImpSpoolerTitle");
        TxtImpSpoolerDesc.Text = LocalizationService.Get("ImpSpoolerDesc");
        BtnImpSpooler.Content = LocalizationService.Get("ImpSpoolerBtn");

        TxtImpListTitle.Text = LocalizationService.Get("ImpListTitle");
        TxtImpListDesc.Text = LocalizationService.Get("ImpListDesc");
        BtnImpList.Content = LocalizationService.Get("ImpListBtn");

        TxtImpSettingsTitle.Text = LocalizationService.Get("ImpSettingsTitle");
        TxtImpSettingsDesc.Text = LocalizationService.Get("ImpSettingsDesc");
        BtnImpSettings.Content = LocalizationService.Get("ImpSettingsBtn");

        // Diagnostico / Diagnostics
        TxtDiagFichaTitle.Text = LocalizationService.Get("DiagFichaTitle");
        TxtDiagFichaDesc.Text = LocalizationService.Get("DiagFichaDesc");
        BtnDiagFicha.Content = LocalizationService.Get("DiagFichaBtn");

        TxtDiagTopRamTitle.Text = LocalizationService.Get("DiagTopRamTitle");
        TxtDiagTopRamDesc.Text = LocalizationService.Get("DiagTopRamDesc");
        BtnDiagTopRam.Content = LocalizationService.Get("DiagTopRamBtn");

        TxtDiagResmonTitle.Text = LocalizationService.Get("DiagResmonTitle");
        TxtDiagResmonDesc.Text = LocalizationService.Get("DiagResmonDesc");
        BtnDiagResmon.Content = LocalizationService.Get("DiagResmonBtn");

        // Drivers & Updates
        TxtDrvErrorsTitle.Text = LocalizationService.Get("DrvErrorsTitle");
        TxtDrvErrorsDesc.Text = LocalizationService.Get("DrvErrorsDesc");
        BtnDrvErrors.Content = LocalizationService.Get("DrvErrorsBtn");

        TxtDrvUpdateTitle.Text = LocalizationService.Get("DrvUpdateTitle");
        TxtDrvUpdateDesc.Text = LocalizationService.Get("DrvUpdateDesc");
        BtnDrvUpdate.Content = LocalizationService.Get("DrvUpdateBtn");

        TxtDrvBackupTitle.Text = LocalizationService.Get("DrvBackupTitle");
        TxtDrvBackupDesc.Text = LocalizationService.Get("DrvBackupDesc");
        BtnDrvBackup.Content = LocalizationService.Get("DrvBackupBtn");

        TxtDrvWinUpdateTitle.Text = LocalizationService.Get("DrvWinUpdateTitle");
        TxtDrvWinUpdateDesc.Text = LocalizationService.Get("DrvWinUpdateDesc");
        BtnDrvWinUpdate.Content = LocalizationService.Get("DrvWinUpdateBtn");

        TxtDrvDevmgmtTitle.Text = LocalizationService.Get("DrvDevmgmtTitle");
        TxtDrvDevmgmtDesc.Text = LocalizationService.Get("DrvDevmgmtDesc");
        BtnDrvDevmgmt.Content = LocalizationService.Get("DrvDevmgmtBtn");

        // Microsoft 365
        TxtM365StatusTitle.Text = LocalizationService.Get("M365StatusTitle");
        TxtM365StatusDesc.Text = LocalizationService.Get("M365StatusDesc");
        BtnM365Status.Content = LocalizationService.Get("M365StatusBtn");

        TxtM365IntuneTitle.Text = LocalizationService.Get("M365IntuneTitle");
        TxtM365IntuneDesc.Text = LocalizationService.Get("M365IntuneDesc");
        BtnM365Intune.Content = LocalizationService.Get("M365IntuneBtn");

        TxtM365TeamsTitle.Text = LocalizationService.Get("M365TeamsTitle");
        TxtM365TeamsDesc.Text = LocalizationService.Get("M365TeamsDesc");
        BtnM365Teams.Content = LocalizationService.Get("M365TeamsBtn");

        // Logs
        TxtLogsHistoryTitle.Text = LocalizationService.Get("LogsHistoryTitle");
        TxtLogsHistoryDesc.Text = LocalizationService.Get("LogsHistoryDesc");
        BtnLogsOpenFolder.Content = LocalizationService.Get("LogsOpenFolderBtn");
        BtnLogsReload.Content = LocalizationService.Get("LogsReloadBtn");

        // Terminal
        LblTerminalHeader.Text = LocalizationService.Get("TerminalHeader");
        BtnCopiarTerminal.Content = LocalizationService.Get("TerminalBtnCopy");
        BtnLimparTerminal.Content = LocalizationService.Get("TerminalBtnClear");
        BtnCancelarExecucao.Content = LocalizationService.Get("TerminalBtnCancel");
        ChkAutoScroll.Content = LocalizationService.Get("TerminalAutoScroll");

        if (_isExecuting)
        {
            TxtTerminalStatus.Text = $"{LocalizationService.Get("TerminalRunning")}: {_currentActionName}...";
        }
        else
        {
            TxtTerminalStatus.Text = LocalizationService.Get("TerminalReady");
        }

        AtualizarTitulosViewAtual();
    }

    /// <summary>
    /// [PT-BR] Atualiza o titulo e subtitulo da tela conforme a aba atualmente selecionada.
    /// [EN]    Updates header title and subtitle based on the currently selected navigation view.
    /// </summary>
    private void AtualizarTitulosViewAtual()
    {
        if (NavDashboard.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleDashboard");
            TxtViewSubtitle.Text = LocalizationService.Get("SubDashboard");
        }
        else if (NavManutencao.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleManutencao");
            TxtViewSubtitle.Text = LocalizationService.Get("SubManutencao");
        }
        else if (NavRede.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleRede");
            TxtViewSubtitle.Text = LocalizationService.Get("SubRede");
        }
        else if (NavImpressao.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleImpressao");
            TxtViewSubtitle.Text = LocalizationService.Get("SubImpressao");
        }
        else if (NavDiagnostico.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleDiagnostico");
            TxtViewSubtitle.Text = LocalizationService.Get("SubDiagnostico");
        }
        else if (NavDrivers.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleDrivers");
            TxtViewSubtitle.Text = LocalizationService.Get("SubDrivers");
        }
        else if (NavM365.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleM365");
            TxtViewSubtitle.Text = LocalizationService.Get("SubM365");
        }
        else if (NavLogs.IsChecked == true)
        {
            TxtViewTitle.Text = LocalizationService.Get("TitleLogs");
            TxtViewSubtitle.Text = LocalizationService.Get("SubLogs");
        }
    }

    /// <summary>
    /// [PT-BR] Carrega telemetria de hardware e atualiza os cards de CPU, RAM e Disco no Dashboard.
    /// [EN]    Loads hardware telemetry and refreshes CPU, RAM and Disk cards on Dashboard.
    /// </summary>
    private async Task CarregarDashboardAsync()
    {
        try
        {
            var info = await SystemDiagnostics.ObterDashboardInfoAsync();
            var pcLabel = LocalizationService.Get("PcLabel");
            var userLabel = LocalizationService.Get("UserLabel");
            var uptimeLabel = LocalizationService.Get("UptimePrefix");

            TxtHeaderPcInfo.Text = $"{pcLabel}: {info.ComputerName}";
            TxtHeaderUser.Text = $"{userLabel}: {info.LoggedUser}";

            DashTxtModelo.Text = info.ManufacturerAndModel;
            DashTxtSerial.Text = $"Serial: {info.SerialNumber}";
            DashTxtWindows.Text = info.WindowsVersion;
            DashTxtUptime.Text = $"{uptimeLabel}: {info.UptimeSummary}";

            DashTxtCpu.Text = info.CpuName;

            DashTxtRam.Text = info.RamSummary;
            DashProgressRam.Value = info.RamUsedPercent;

            DashTxtDiscoC.Text = info.DiskCSummary;
            DashProgressDiscoC.Value = info.DiskCUsedPercent;
        }
        catch (Exception ex)
        {
            AdicionarLinhaTerminal($"[!] Error: {ex.Message}", true);
        }
    }

    /// <summary>
    /// [PT-BR] Le os registros mais recentes do arquivo de auditoria em C:\ProgramData\PainelTI\painel.log.
    /// [EN]    Reads most recent records from audit file at C:\ProgramData\PainelTI\painel.log.
    /// </summary>
    private void CarregarLogs()
    {
        try
        {
            var linhas = LogService.ObterUltimasLinhas(100);
            TxtBoxLogHistory.Text = string.Join(Environment.NewLine, linhas);
            TxtBoxLogHistory.ScrollToEnd();
        }
        catch (Exception ex)
        {
            TxtBoxLogHistory.Text = $"Log error: {ex.Message}";
        }
    }

    /// <summary>
    /// [PT-BR] Manipulador de eventos de troca de navegacao nas opcoes do menu lateral.
    /// [EN]    Event handler for sidebar navigation view switching.
    /// </summary>
    private void Nav_Checked(object sender, RoutedEventArgs e)
    {
        if (ViewDashboard == null) return;

        // Oculta todas as telas / Hide all views
        ViewDashboard.Visibility = Visibility.Collapsed;
        ViewManutencao.Visibility = Visibility.Collapsed;
        ViewRede.Visibility = Visibility.Collapsed;
        ViewImpressao.Visibility = Visibility.Collapsed;
        ViewDiagnostico.Visibility = Visibility.Collapsed;
        ViewDrivers.Visibility = Visibility.Collapsed;
        ViewM365.Visibility = Visibility.Collapsed;
        ViewLogs.Visibility = Visibility.Collapsed;

        if (NavDashboard.IsChecked == true)
        {
            ViewDashboard.Visibility = Visibility.Visible;
        }
        else if (NavManutencao.IsChecked == true)
        {
            ViewManutencao.Visibility = Visibility.Visible;
        }
        else if (NavRede.IsChecked == true)
        {
            ViewRede.Visibility = Visibility.Visible;
        }
        else if (NavImpressao.IsChecked == true)
        {
            ViewImpressao.Visibility = Visibility.Visible;
        }
        else if (NavDiagnostico.IsChecked == true)
        {
            ViewDiagnostico.Visibility = Visibility.Visible;
        }
        else if (NavDrivers.IsChecked == true)
        {
            ViewDrivers.Visibility = Visibility.Visible;
        }
        else if (NavM365.IsChecked == true)
        {
            ViewM365.Visibility = Visibility.Visible;
        }
        else if (NavLogs.IsChecked == true)
        {
            ViewLogs.Visibility = Visibility.Visible;
            CarregarLogs();
        }

        AtualizarTitulosViewAtual();
    }

    /// <summary>
    /// [PT-BR] Executa uma rotina assincrona capturando a saida e atualizando o terminal em tempo real.
    /// [EN]    Executes a routine asynchronously, capturing output and streaming to the terminal in real time.
    /// </summary>
    private async Task ExecutarScriptAsync(string nomeAcao, string scriptPowerShell)
    {
        if (_isExecuting)
        {
            var aviso = LocalizationService.IsEnglish
                ? "A routine is already executing. Please wait or click 'Cancel'."
                : "Já existe uma rotina em execução no momento. Aguarde ou clique em 'Interromper'.";
            MessageBox.Show(aviso, "Busy", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        _isExecuting = true;
        _currentActionName = nomeAcao;
        _cts = new CancellationTokenSource();
        BtnCancelarExecucao.IsEnabled = true;
        TxtTerminalStatus.Text = $"{LocalizationService.Get("TerminalRunning")}: {nomeAcao}...";
        TxtTerminalStatus.Foreground = (Brush)FindResource("Accent");

        try
        {
            await PowerShellRunner.RunScriptAsync(
                scriptPowerShell,
                nomeAcao,
                (linha, isError) =>
                {
                    Dispatcher.InvokeAsync(() => AdicionarLinhaTerminal(linha, isError));
                },
                _cts.Token
            );
        }
        catch (Exception ex)
        {
            AdicionarLinhaTerminal($"[X] Exceção geral: {ex.Message}", true);
        }
        finally
        {
            _isExecuting = false;
            _currentActionName = "";
            BtnCancelarExecucao.IsEnabled = false;
            TxtTerminalStatus.Text = LocalizationService.Get("TerminalReady");
            TxtTerminalStatus.Foreground = (Brush)FindResource("Success");
            _cts?.Dispose();
            _cts = null;
        }
    }

    /// <summary>
    /// [PT-BR] Adiciona uma linha de texto na caixa do terminal visual.
    /// [EN]    Appends a line of text into the visual UI terminal text box.
    /// </summary>
    private void AdicionarLinhaTerminal(string texto, bool isError)
    {
        if (isError)
        {
            TxtTerminal.AppendText($"[!] {texto}{Environment.NewLine}");
        }
        else
        {
            TxtTerminal.AppendText($"{texto}{Environment.NewLine}");
        }

        if (ChkAutoScroll.IsChecked == true)
        {
            TxtTerminal.ScrollToEnd();
        }
    }

    // =========================================================================
    // [PT-BR] CONTROLES DO TERMINAL
    // [EN]    TERMINAL CONTROLS
    // =========================================================================

    private void BtnLimparTerminal_Click(object sender, RoutedEventArgs e)
    {
        TxtTerminal.Clear();
    }

    private void BtnCopiarTerminal_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Clipboard.SetText(TxtTerminal.Text);
            var copiado = LocalizationService.IsEnglish
                ? "Terminal output copied to clipboard!"
                : "Conteúdo do terminal copiado para a área de transferência!";
            MessageBox.Show(copiado, "OK", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch { }
    }

    private void BtnCancelarExecucao_Click(object sender, RoutedEventArgs e)
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            var cancelando = LocalizationService.IsEnglish
                ? "[!] Requesting cancellation of the active process..."
                : "[!] Solicitando cancelamento do processo em andamento...";
            AdicionarLinhaTerminal(cancelando, true);
            _cts.Cancel();
        }
    }

    private async void BtnAtualizarDashboard_Click(object sender, RoutedEventArgs e)
    {
        await CarregarDashboardAsync();
        CarregarLogs();
    }

    // =========================================================================
    // [PT-BR] ROTINAS: MANUTENCAO E DISCO
    // [EN]    ROUTINES: MAINTENANCE & DISK
    // =========================================================================

    private async void BtnLimparTemp_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Clean Temp Files" : "Limpeza de Temporários";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptLimparTemporarios);
        await CarregarDashboardAsync();
    }

    private void BtnCleanmgr_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("cleanmgr.exe", "/d C:");
        var msg = LocalizationService.IsEnglish
            ? "[OK] Disk Cleanup tool (cleanmgr.exe) opened for drive C:."
            : "[OK] Ferramenta Limpeza de Disco (cleanmgr.exe) aberta na unidade C:.";
        AdicionarLinhaTerminal(msg, false);
    }

    private async void BtnSfc_Click(object sender, RoutedEventArgs e)
    {
        var pergunta = LocalizationService.IsEnglish
            ? "SFC scan may take between 5 and 15 minutes. Start now?"
            : "A verificação SFC pode levar entre 5 e 15 minutos. Deseja iniciar agora?";
        var titulo = LocalizationService.IsEnglish ? "Confirm SFC Repair" : "Confirmar Reparo SFC";

        if (MessageBox.Show(pergunta, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            var nomeAcao = LocalizationService.IsEnglish ? "System File Checker (SFC)" : "Verificação e Reparo de Arquivos (SFC)";
            await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptRepararSfc);
        }
    }

    private async void BtnDism_Click(object sender, RoutedEventArgs e)
    {
        var pergunta = LocalizationService.IsEnglish
            ? "DISM RestoreHealth requires internet access and can take up to 20 minutes. Continue?"
            : "O DISM RestoreHealth precisa de conexão com a internet e pode levar até 20 minutos. Deseja continuar?";
        var titulo = LocalizationService.IsEnglish ? "Confirm DISM Repair" : "Confirmar Reparo DISM";

        if (MessageBox.Show(pergunta, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            var nomeAcao = LocalizationService.IsEnglish ? "Windows Image Repair (DISM)" : "Reparo da Imagem do Windows (DISM)";
            await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptRepararDism);
        }
    }

    private async void BtnChkdsk_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Drive C: Scan (CHKDSK)" : "Verificação do Disco C: (CHKDSK)";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptVerificarDisco);
    }

    private async void BtnSaudeDisco_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Physical Disk Health (SMART)" : "Saúde Física dos Discos (SMART)";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptSaudeDisco);
    }

    // =========================================================================
    // [PT-BR] ROTINAS: REDE E CONECTIVIDADE
    // [EN]    ROUTINES: NETWORK & CONNECTIVITY
    // =========================================================================

    private async void BtnResetRede_Click(object sender, RoutedEventArgs e)
    {
        var pergunta = LocalizationService.IsEnglish
            ? "Network reset will reset Winsock, release/renew IP and flush DNS. Connection will drop temporarily and PC must be rebooted. Continue?"
            : "O reset de rede irá redefinir Winsock, liberar e renovar o IP e limpar o DNS. A conexão cairá temporariamente e será necessário reiniciar o PC. Deseja continuar?";
        var titulo = LocalizationService.IsEnglish ? "Confirm Network Reset" : "Confirmar Reset de Rede";

        if (MessageBox.Show(pergunta, titulo, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            var nomeAcao = LocalizationService.IsEnglish ? "Full Network Reset" : "Reset Completo de Rede";
            await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptResetRede);
        }
    }

    private async void BtnInfoRede_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Network Diagnostics" : "Informações de Rede";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptInfoRede);
    }

    private async void BtnTestarConexao_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Connectivity Tests" : "Testes de Conectividade";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptTestarConectividade);
    }

    private async void BtnLimparDNS_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Flush DNS Cache" : "Limpeza de Cache DNS";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptLimparDNS);
    }

    private void BtnNcpa_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("ncpa.cpl");
        var msg = LocalizationService.IsEnglish
            ? "[OK] Network Connections panel (ncpa.cpl) opened."
            : "[OK] Painel Conexões de Rede (ncpa.cpl) aberto.";
        AdicionarLinhaTerminal(msg, false);
    }

    // =========================================================================
    // [PT-BR] ROTINAS: IMPRESSAO E SPOOLER
    // [EN]    ROUTINES: PRINTING & SPOOLER
    // =========================================================================

    private async void BtnLimparFila_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Clear Print Spooler Queue" : "Limpar Fila de Impressão (Spooler)";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptLimparFilaImpressao);
    }

    private async void BtnListarImpressoras_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "List Installed Printers" : "Listar Impressoras";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptListarImpressoras);
    }

    private void BtnAbrirImpressorasConfig_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("ms-settings:printers");
        var msg = LocalizationService.IsEnglish
            ? "[OK] Printers & Scanners panel opened."
            : "[OK] Painel de Impressoras e Scanners aberto.";
        AdicionarLinhaTerminal(msg, false);
    }

    // =========================================================================
    // [PT-BR] ROTINAS: DIAGNOSTICO E HARDWARE
    // [EN]    ROUTINES: DIAGNOSTICS & HARDWARE
    // =========================================================================

    private async void BtnFichaCompleta_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Complete Hardware Specs Sheet" : "Ficha Técnica Completa de Hardware";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptFichaCompleta);
    }

    private async void BtnTopProcessos_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Top RAM Consumer Processes" : "Top Processos por Consumo de RAM";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptTopProcessos);
    }

    private void BtnResmon_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("resmon.exe");
        var msg = LocalizationService.IsEnglish
            ? "[OK] Resource Monitor (resmon.exe) opened."
            : "[OK] Monitor de Recursos (resmon.exe) aberto.";
        AdicionarLinhaTerminal(msg, false);
    }

    // =========================================================================
    // [PT-BR] ROTINAS: DRIVERS E WINDOWS UPDATE
    // [EN]    ROUTINES: DRIVERS & WINDOWS UPDATE
    // =========================================================================

    private async void BtnDispositivosErro_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Check Device Errors" : "Verificação de Dispositivos com Erro";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptDispositivosComErro);
    }

    private async void BtnAtualizarDrivers_Click(object sender, RoutedEventArgs e)
    {
        var pergunta = LocalizationService.IsEnglish
            ? "Check and install available driver updates via Windows Update now?"
            : "Deseja verificar e instalar atualizações de drivers pelo Windows Update agora?";
        var titulo = LocalizationService.IsEnglish ? "Update Drivers" : "Atualizar Drivers";

        if (MessageBox.Show(pergunta, titulo, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            var nomeAcao = LocalizationService.IsEnglish ? "Driver Updates (Windows Update)" : "Atualização de Drivers (Windows Update)";
            await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptAtualizarDrivers);
        }
    }

    private async void BtnBackupDrivers_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Backup Third-Party Drivers" : "Backup de Drivers";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptBackupDrivers);
    }

    private void BtnWindowsUpdate_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("ms-settings:windowsupdate");
        try { PowerShellRunner.IniciarProcessoExterno("UsoClient.exe", "StartScan"); } catch { }
        var msg = LocalizationService.IsEnglish
            ? "[OK] Windows Update opened and update scan triggered."
            : "[OK] Tela de Windows Update aberta e verificação iniciada.";
        AdicionarLinhaTerminal(msg, false);
    }

    private void BtnDevmgmt_Click(object sender, RoutedEventArgs e)
    {
        PowerShellRunner.IniciarProcessoExterno("devmgmt.msc");
        var msg = LocalizationService.IsEnglish
            ? "[OK] Device Manager (devmgmt.msc) opened."
            : "[OK] Gerenciador de Dispositivos (devmgmt.msc) aberto.";
        AdicionarLinhaTerminal(msg, false);
    }

    // =========================================================================
    // [PT-BR] ROTINAS: MICROSOFT 365, ENTRA ID E TEAMS
    // [EN]    ROUTINES: MICROSOFT 365, ENTRA ID & TEAMS
    // =========================================================================

    private async void BtnStatusM365_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Entra ID / Intune Join Status" : "Status de Vínculo Entra ID / Intune";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptStatusM365);
    }

    private async void BtnSincronizarIntune_Click(object sender, RoutedEventArgs e)
    {
        var nomeAcao = LocalizationService.IsEnglish ? "Force Intune Policy Sync" : "Sincronização com o Intune";
        await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptSincronizarIntune);
    }

    private async void BtnLimparTeams_Click(object sender, RoutedEventArgs e)
    {
        var pergunta = LocalizationService.IsEnglish
            ? "This will terminate Microsoft Teams for the active user and clear local cache files. Continue?"
            : "Esta ação irá encerrar o Microsoft Teams da pessoa e limpar o cache local. Deseja continuar?";
        var titulo = LocalizationService.IsEnglish ? "Clear Teams Cache" : "Confirmar Limpeza de Cache Teams";

        if (MessageBox.Show(pergunta, titulo, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            var nomeAcao = LocalizationService.IsEnglish ? "Clear Teams Cache" : "Limpeza de Cache do Teams";
            await ExecutarScriptAsync(nomeAcao, SupportActions.ScriptLimparCacheTeams);
        }
    }

    // =========================================================================
    // [PT-BR] ROTINAS: AUDITORIA E LOGS
    // [EN]    ROUTINES: AUDIT & LOGS
    // =========================================================================

    private void BtnRecarregarLogs_Click(object sender, RoutedEventArgs e)
    {
        CarregarLogs();
        var msg = LocalizationService.IsEnglish ? "[OK] Audit logs reloaded." : "[OK] Histórico de logs recarregado.";
        AdicionarLinhaTerminal(msg, false);
    }

    private void BtnAbrirPastaLogs_Click(object sender, RoutedEventArgs e)
    {
        var dir = Path.GetDirectoryName(LogService.LogFilePath);
        if (Directory.Exists(dir))
        {
            PowerShellRunner.IniciarProcessoExterno("explorer.exe", dir);
            var msg = LocalizationService.IsEnglish
                ? $"[OK] Logs folder opened: {dir}"
                : $"[OK] Pasta de logs aberta: {dir}";
            AdicionarLinhaTerminal(msg, false);
        }
        else
        {
            var msg = LocalizationService.IsEnglish
                ? $"[!] Logs folder does not exist yet: {dir}"
                : $"[!] Pasta de logs ainda não existe: {dir}";
            AdicionarLinhaTerminal(msg, true);
        }
    }
}
