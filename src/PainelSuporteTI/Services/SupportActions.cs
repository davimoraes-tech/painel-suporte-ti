namespace PainelSuporteTI.Services;

/// <summary>
/// [PT-BR] Repositorio central contendo os comandos e rotinas administrativas que o C# executa no Windows.
/// [EN]    Central repository containing the administrative routines and command pipelines executed by C# on Windows.
/// </summary>
public static class SupportActions
{
    // [PT-BR] Funcoes auxiliares injetadas para deteccao do perfil do usuario real e medicao de diretorios
    // [EN]    Injected helper functions for interactive user profile detection and folder sizing
    private const string HelperFunctions = @"
function Obter-UsuarioLogado {
    $nome = (Get-CimInstance Win32_ComputerSystem).UserName
    if (-not $nome) { return $null }
    try {
        $sid = (New-Object Security.Principal.NTAccount $nome).Translate([Security.Principal.SecurityIdentifier]).Value
        $perfil = (Get-ItemProperty ""HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\$sid"" -ErrorAction Stop).ProfileImagePath
        return [pscustomobject]@{ Nome = $nome; Sid = $sid; Perfil = $perfil }
    } catch {
        return [pscustomobject]@{ Nome = $nome; Sid = $null; Perfil = $null }
    }
}
function Tamanho-Pasta([string]$caminho) {
    if (-not (Test-Path $caminho)) { return 0 }
    (Get-ChildItem $caminho -Recurse -Force -File -ErrorAction SilentlyContinue | Measure-Object Length -Sum).Sum
}
function Limpar-Pasta([string]$caminho, [int]$diasMinimos = 1) {
    if (-not (Test-Path $caminho)) { return 0 }
    $antes = Tamanho-Pasta $caminho
    $limite = (Get-Date).AddDays(-$diasMinimos)
    Get-ChildItem $caminho -Recurse -Force -File -ErrorAction SilentlyContinue |
        Where-Object { $_.LastWriteTime -lt $limite } |
        Remove-Item -Force -ErrorAction SilentlyContinue
    Get-ChildItem $caminho -Recurse -Force -Directory -ErrorAction SilentlyContinue |
        Sort-Object { $_.FullName.Length } -Descending |
        Where-Object { -not (Get-ChildItem $_.FullName -Force -ErrorAction SilentlyContinue) } |
        Remove-Item -Force -ErrorAction SilentlyContinue
    return [math]::Max(0, $antes - (Tamanho-Pasta $caminho))
}
function MB([double]$bytes) { '{0:N0} MB' -f ($bytes / 1MB) }
function GB([double]$bytes) { '{0:N0} GB' -f ($bytes / 1GB) }
";

    /// <summary>
    /// [PT-BR] Limpeza de arquivos temporarios no Windows e no perfil do usuario ativo.
    /// [EN]    Temporary files cleanup for Windows Temp and active logged-in user Temp directory.
    /// </summary>
    public static string ScriptLimparTemporarios => HelperFunctions + @"
Write-Host 'Iniciando limpeza de arquivos temporarios / Starting temp cleanup...'
$usuario = Obter-UsuarioLogado
$pastas = @(""$env:windir\Temp"")
if ($usuario -and $usuario.Perfil) {
    $pastas += Join-Path $usuario.Perfil 'AppData\Local\Temp'
    Write-Host ""Perfil do usuario ativo / Active profile: $($usuario.Nome)""
} else {
    Write-Host 'Aviso: Perfil de usuario ativo nao encontrado; limpando pasta Temp do Windows.'
}
$total = 0
foreach ($p in $pastas) {
    Write-Host ""Varrendo / Scanning: $p ...""
    $liberado = Limpar-Pasta $p 1
    $total += $liberado
    Write-Host (""  Liberado em {0}: {1}"" -f $p, (MB $liberado))
}
Write-Host '----------------------------------------'
Write-Host ""TOTAL LIBERADO / FREED: $(MB $total)""
Write-Host '(Arquivos em uso ou com menos de 24h foram mantidos / Locked files or files <24h kept).'
";

    /// <summary>
    /// [PT-BR] Verificacao de integridade e reparo de arquivos corrompidos do sistema (SFC).
    /// [EN]    System File Checker scan and automated file repair (SFC).
    /// </summary>
    public static string ScriptRepararSfc => @"
Write-Host 'Executando SFC (System File Checker)...'
Write-Host 'Isso pode levar de 5 a 15 minutos dependendo do computador. Aguarde...'
sfc /scannow
";

    /// <summary>
    /// [PT-BR] Reparo da imagem do Windows utilizando a ferramenta DISM online.
    /// [EN]    Windows system image servicing and restoration via DISM.
    /// </summary>
    public static string ScriptRepararDism => @"
Write-Host 'Executando DISM RestoreHealth...'
Write-Host 'Requer conexao com a internet e pode levar alguns minutos...'
DISM /Online /Cleanup-Image /RestoreHealth
";

    /// <summary>
    /// [PT-BR] Verificacao online da integridade do sistema de arquivos na particao C:.
    /// [EN]    Online read-only file system integrity scan on drive C:.
    /// </summary>
    public static string ScriptVerificarDisco => @"
Write-Host 'Executando verificacao de disco (chkdsk C: /scan em modo somente leitura)...'
chkdsk C: /scan
";

    /// <summary>
    /// [PT-BR] Consulta de confiabilidade fisica de discos (SMART, desgaste de SSD e contadores de erro).
    /// [EN]    Physical drive health query (SMART counters, SSD wear percentage and read errors).
    /// </summary>
    public static string ScriptSaudeDisco => HelperFunctions + @"
Write-Host '=== SAUDE FISICA DOS DISCOS / PHYSICAL DISK HEALTH ==='
Get-PhysicalDisk | ForEach-Object {
    $c = $_ | Get-StorageReliabilityCounter -ErrorAction SilentlyContinue
    [pscustomobject]@{
        Disco        = $_.FriendlyName
        Tipo         = $_.MediaType
        Tamanho      = '{0:N0} GB' -f ($_.Size / 1GB)
        Saude        = $_.HealthStatus
        Situacao     = $_.OperationalStatus
        Temperatura  = if ($c.Temperature) { ""$($c.Temperature) °C"" } else { '-' }
        Desgaste     = if ($null -ne $c.Wear) { ""$($c.Wear)%"" } else { '-' }
        ErrosLeitura = $c.ReadErrorsTotal
    }
} | Format-List

Write-Host '=== UNIDADES E ESPACO LIVRE / VOLUMES & FREE SPACE ==='
Get-Volume | Where-Object DriveLetter | Select-Object DriveLetter, FileSystemLabel,
    @{n = 'Livre'; e = { '{0:N1} GB' -f ($_.SizeRemaining / 1GB) } },
    @{n = 'Total'; e = { '{0:N1} GB' -f ($_.Size / 1GB) } } | Format-Table -AutoSize
";

    /// <summary>
    /// [PT-BR] Redefinicao completa da pilha TCP/IP, catalogo Winsock e liberacao/renovacao de IP.
    /// [EN]    Full network stack reset (TCP/IP, Winsock catalog, IP release/renew and DNS flush).
    /// </summary>
    public static string ScriptResetRede => @"
Write-Host 'Iniciando Redefinicao Completa de Rede / Starting Network Reset...'
ipconfig /release | Out-Null
ipconfig /flushdns
netsh winsock reset
netsh int ip reset | Out-Null
ipconfig /renew | Out-Null
Write-Host '[OK] Pilha de rede resetada com sucesso / Network stack successfully reset!'
Write-Host '[IMPORTANTE] E altamente recomendavel reiniciar o computador para concluir.'
";

    /// <summary>
    /// [PT-BR] Diagnostico de adaptadores de rede ativos e conexao Wi-Fi.
    /// [EN]    Diagnostic query for active network adapters and Wi-Fi signal metrics.
    /// </summary>
    public static string ScriptInfoRede => @"
Write-Host '=== ADAPTADORES ATIVOS E CONFIGURACAO IP / NETWORK CONFIGURATION ==='
Get-NetIPConfiguration | Where-Object { $_.NetAdapter.Status -eq 'Up' } | ForEach-Object {
    [pscustomobject]@{
        Placa   = $_.InterfaceAlias
        IP      = ($_.IPv4Address.IPAddress -join ', ')
        Gateway = ($_.IPv4DefaultGateway.NextHop -join ', ')
        DNS     = ($_.DNSServer | Where-Object AddressFamily -eq 2).ServerAddresses -join ', '
    }
} | Format-List

$wifi = netsh wlan show interfaces 2>$null | Select-String 'SSID|Sinal|Signal' | Select-Object -First 3
if ($wifi) {
    Write-Host '=== CONEXAO WI-FI / WI-FI METRICS ==='
    $wifi | ForEach-Object { Write-Host ""  $($_.Line.Trim())"" }
}
";

    /// <summary>
    /// [PT-BR] Suite de testes de conectividade ate gateway, internet, DNS e servicos Microsoft 365.
    /// [EN]    Connectivity test suite verifying Gateway, Internet, DNS and Microsoft 365 services.
    /// </summary>
    public static string ScriptTestarConectividade => @"
Write-Host '=== TESTES DE CONECTIVIDADE / CONNECTIVITY TESTS ==='
$gateway = (Get-NetIPConfiguration | Where-Object IPv4DefaultGateway | Select-Object -First 1).IPv4DefaultGateway.NextHop
$testes = @(
    @{ Nome = 'Roteador / Gateway Local'; Alvo = $gateway; Porta = $null },
    @{ Nome = 'Internet Publica (Google DNS)'; Alvo = '8.8.8.8'; Porta = $null },
    @{ Nome = 'Resolucao DNS (google.com)'; Alvo = 'google.com'; Porta = 443 },
    @{ Nome = 'Microsoft 365 / Outlook'; Alvo = 'outlook.office365.com'; Porta = 443 },
    @{ Nome = 'Microsoft Teams'; Alvo = 'teams.microsoft.com'; Porta = 443 }
)
foreach ($t in $testes) {
    if (-not $t.Alvo) { Write-Host ""[FALHA] $($t.Nome): Sem gateway configurado""; continue }
    if ($t.Porta) { $ok = Test-NetConnection $t.Alvo -Port $t.Porta -InformationLevel Quiet -WarningAction SilentlyContinue }
    else { $ok = Test-Connection $t.Alvo -Count 2 -Quiet }
    if ($ok) { Write-Host ""[OK] $($t.Nome) - Conectado com sucesso"" }
    else { Write-Host ""[FALHA] $($t.Nome) - Nao respondeu"" }
}
";

    /// <summary>
    /// [PT-BR] Esvaziamento do cache de resolucao de nomes DNS do Windows.
    /// [EN]    Flush local Windows DNS name resolution client cache.
    /// </summary>
    public static string ScriptLimparDNS => @"
Write-Host 'Limpando cache de DNS local (ipconfig /flushdns)...'
ipconfig /flushdns
Write-Host '[OK] Cache de resolucao DNS esvaziado.'
";

    /// <summary>
    /// [PT-BR] Limpeza de arquivos retidos na fila de impressao e reinicio seguro do servico Spooler.
    /// [EN]    Purge stuck printer spool queue files and safely restart the Spooler service.
    /// </summary>
    public static string ScriptLimparFilaImpressao => @"
Write-Host 'Limpando fila de impressao travada / Clearing stuck print spooler...'
Stop-Service Spooler -Force
Remove-Item ""$env:windir\System32\spool\PRINTERS\*"" -Force -ErrorAction SilentlyContinue
Start-Service Spooler
if ((Get-Service Spooler).Status -eq 'Running') {
    Write-Host '[OK] Fila limpa e servico de spooler reiniciado com sucesso.'
} else {
    Write-Host '[ERRO] Nao foi possivel reiniciar o servico Spooler.'
}
";

    /// <summary>
    /// [PT-BR] Listagem de todas as impressoras instaladas no computador.
    /// [EN]    List all configured local and network printers.
    /// </summary>
    public static string ScriptListarImpressoras => @"
Write-Host '=== IMPRESSORAS CONFIGURADAS / PRINTERS ==='
Get-Printer | Select-Object Name, PrinterStatus, PortName, Default | Format-Table -AutoSize
";

    /// <summary>
    /// [PT-BR] Coleta completa de especificacoes de hardware (placa-mae, memoria por pente, GPU, monitores e bateria).
    /// [EN]    Complete hardware inventory (motherboard, per-slot RAM sticks, GPU, WMI monitors and battery health).
    /// </summary>
    public static string ScriptFichaCompleta => HelperFunctions + @"
Write-Host 'Coletando especificacoes completas de hardware / Generating full hardware inventory...'
$linhas = New-Object System.Collections.Generic.List[string]
function Sec([string]$t) { $linhas.Add(''); $linhas.Add(""--- $t ---"") }
function Item([string]$rotulo, $valor) { $linhas.Add(('  {0,-22} {1}' -f $rotulo, $valor)) }

$so = Get-CimInstance Win32_OperatingSystem
$pc = Get-CimInstance Win32_ComputerSystem
$bios = Get-CimInstance Win32_BIOS
$placa = Get-CimInstance Win32_BaseBoard
$chassi = (Get-CimInstance Win32_SystemEnclosure).ChassisTypes
$notebook = [bool]($chassi | Where-Object { $_ -in 8, 9, 10, 11, 12, 14, 18, 21, 30, 31, 32 })
$usuario = Obter-UsuarioLogado
$versao = (Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion' -ErrorAction SilentlyContinue).DisplayVersion

Sec 'COMPUTADOR'
Item 'Nome' $env:COMPUTERNAME
Item 'Usuario logado' $(if ($usuario) { $usuario.Nome } else { '-' })
Item 'Tipo' $(if ($notebook) { 'Notebook' } else { 'Desktop' })
Item 'Fabricante/Modelo' ""$($pc.Manufacturer) $($pc.Model)""
Item 'Numero de serie' $bios.SerialNumber
Item 'Placa-mae' ""$($placa.Manufacturer) $($placa.Product)""
Item 'BIOS' ""$($bios.SMBIOSBIOSVersion) ($('{0:dd/MM/yyyy}' -f $bios.ReleaseDate))""
Item 'Windows' ""$($so.Caption) $versao (build $($so.BuildNumber))""
Item 'Ligado desde' ('{0:dd/MM/yyyy HH:mm}' -f $so.LastBootUpTime)

Sec 'PROCESSADOR'
foreach ($cpu in Get-CimInstance Win32_Processor) {
    Item 'Modelo' $cpu.Name.Trim()
    Item 'Nucleos / threads' ""$($cpu.NumberOfCores) nucleos / $($cpu.NumberOfLogicalProcessors) threads""
    Item 'Clock max' ('{0:N2} GHz' -f ($cpu.MaxClockSpeed / 1000))
    Item 'Uso agora' ""$($cpu.LoadPercentage)%""
}

Sec 'MEMORIA RAM'
$pentes = @(Get-CimInstance Win32_PhysicalMemory)
$slots = (Get-CimInstance Win32_PhysicalMemoryArray | Measure-Object MemoryDevices -Sum).Sum
$tipos = @{ 20 = 'DDR'; 21 = 'DDR2'; 24 = 'DDR3'; 26 = 'DDR4'; 30 = 'LPDDR4'; 34 = 'DDR5'; 35 = 'LPDDR5' }
$livre = $so.FreePhysicalMemory * 1KB
Item 'Total' (GB $pc.TotalPhysicalMemory)
Item 'Em uso agora' ('{0} de {1} ({2:N0}%)' -f (GB ($pc.TotalPhysicalMemory - $livre)), (GB $pc.TotalPhysicalMemory), (100 * ($pc.TotalPhysicalMemory - $livre) / $pc.TotalPhysicalMemory))
Item 'Slots ocupados' $(if ($slots) { ""$($pentes.Count) de $slots"" } else { ""$($pentes.Count)"" })
$i = 0
foreach ($p in $pentes) {
    $i++
    $tipo = if ($tipos.ContainsKey([int]$p.SMBIOSMemoryType)) { $tipos[[int]$p.SMBIOSMemoryType] } else { 'Outro' }
    $velocidade = if ($p.ConfiguredClockSpeed) { $p.ConfiguredClockSpeed } else { $p.Speed }
    Item ""Pente $i"" ('{0} {1} {2} MHz - {3} {4} ({5})' -f (GB $p.Capacity), $tipo, $velocidade, $p.Manufacturer, ""$($p.PartNumber)"".Trim(), $p.DeviceLocator)
}

Sec 'DISCOS'
foreach ($d in Get-PhysicalDisk -ErrorAction SilentlyContinue) {
    Item 'Disco' ('{0} | {1} {2} | {3} | saude: {4}' -f $d.FriendlyName, $d.MediaType, $d.BusType, (GB $d.Size), $d.HealthStatus)
}
foreach ($v in Get-Volume -ErrorAction SilentlyContinue | Where-Object { $_.DriveLetter -and $_.DriveType -eq 'Fixed' -and $_.Size -gt 0 }) {
    $pct = 100 * [double]($v.Size - $v.SizeRemaining) / [double]$v.Size
    Item ""Unidade $($v.DriveLetter):"" ('{0} livres de {1} ({2:N0}% usado)' -f (GB $v.SizeRemaining), (GB $v.Size), $pct)
}

Sec 'PLACA DE VIDEO'
foreach ($g in Get-CimInstance Win32_VideoController) {
    Item 'Modelo' $g.Name
    Item 'Driver' ""$($g.DriverVersion) ($('{0:dd/MM/yyyy}' -f $g.DriverDate))""
    if ($g.CurrentHorizontalResolution) { Item 'Resolucao' ""$($g.CurrentHorizontalResolution) x $($g.CurrentVerticalResolution)"" }
}

$arquivo = Join-Path $env:ProgramData 'PainelTI' (""Ficha_{0}_{1:yyyy-MM-dd}.txt"" -f $env:COMPUTERNAME, (Get-Date))
$linhas | Out-File -FilePath $arquivo -Encoding UTF8
$linhas | ForEach-Object { Write-Host $_ }
Write-Host ''
Write-Host ""[OK] Ficha salva em / Spec sheet saved: $arquivo""
";

    /// <summary>
    /// [PT-BR] Top 10 processos ativos que mais consomem memoria RAM no momento.
    /// [EN]    Top 10 active processes with highest RAM consumption.
    /// </summary>
    public static string ScriptTopProcessos => @"
Write-Host '=== TOP 10 PROCESSOS POR CONSUMO DE MEMORIA / RAM CONSUMPTION ==='
Get-Process | Sort-Object WorkingSet64 -Descending | Select-Object -First 10 Name,
    @{n = 'Memoria MB'; e = { '{0:N0} MB' -f ($_.WorkingSet64 / 1MB) } },
    @{n = 'PID'; e = { $_.Id } } | Format-Table -AutoSize
";

    /// <summary>
    /// [PT-BR] Varredura na arvore Plug and Play em busca de componentes com codigo de erro de driver.
    /// [EN]    Plug and Play device tree scan looking for hardware devices with error status or missing drivers.
    /// </summary>
    public static string ScriptDispositivosComErro => @"
Write-Host 'Verificando dispositivos Plug and Play / Checking PnP Devices...'
$erros = Get-PnpDevice -PresentOnly | Where-Object { $_.Status -notin 'OK', 'Unknown' }
if ($erros) {
    Write-Host '[AVISO] Encontrados dispositivos com problema / Devices with issues:'
    $erros | Group-Object Status, Class, FriendlyName | ForEach-Object {
        [pscustomobject]@{
            Status      = $_.Group[0].Status
            Tipo        = $_.Group[0].Class
            Dispositivo = $_.Group[0].FriendlyName
            Qtde        = $_.Count
        }
    } | Format-Table -AutoSize
} else {
    Write-Host '[OK] Nenhum dispositivo com erro de driver ou status irregular.'
}
";

    /// <summary>
    /// [PT-BR] Busca e instalacao automatizada de atualizacoes de drivers via API COM do Windows Update.
    /// [EN]    Search and automated installation of driver updates via Windows Update COM API.
    /// </summary>
    public static string ScriptAtualizarDrivers => @"
Write-Host 'Consultando Windows Update por drivers / Searching Windows Update for drivers...'
$sessao = New-Object -ComObject Microsoft.Update.Session
$busca = $sessao.CreateUpdateSearcher()
try {
    $resultado = $busca.Search(""IsInstalled=0 and Type='Driver' and IsHidden=0"")
} catch {
    Write-Host ""[ERRO] Nao foi possivel consultar o Windows Update: $($_.Exception.Message)""
    return
}
if ($resultado.Updates.Count -eq 0) {
    Write-Host '[OK] Todos os drivers estao em dia pelo Windows Update / All drivers are up-to-date.'
    return
}
Write-Host ""Encontrados $($resultado.Updates.Count) driver(s) para atualizar:""
$lista = New-Object -ComObject Microsoft.Update.UpdateColl
foreach ($u in $resultado.Updates) {
    Write-Host "" - $($u.Title)""
    if (-not $u.EulaAccepted) { $u.AcceptEula() }
    [void]$lista.Add($u)
}
Write-Host 'Baixando drivers / Downloading drivers...'
$baixar = $sessao.CreateUpdateDownloader()
$baixar.Updates = $lista
[void]$baixar.Download()
Write-Host 'Instalando drivers / Installing drivers...'
$instalar = $sessao.CreateUpdateInstaller()
$instalar.Updates = $lista
$fim = $instalar.Install()
Write-Host ""[OK] Concluido com codigo: $($fim.ResultCode)""
if ($fim.RebootRequired) {
    Write-Host '[AVISO] Reinicializacao do computador necessaria para concluir a instalacao.'
}
";

    /// <summary>
    /// [PT-BR] Exportacao e backup de todos os drivers de terceiros instalados na maquina.
    /// [EN]    Export and backup of all installed third-party drivers.
    /// </summary>
    public static string ScriptBackupDrivers => @"
$destino = Join-Path (Join-Path $env:ProgramData 'PainelTI') (""Drivers_{0}_{1:yyyy-MM-dd}"" -f $env:COMPUTERNAME, (Get-Date))
New-Item -ItemType Directory -Path $destino -Force | Out-Null
Write-Host ""Exportando drivers instalados para: $destino ...""
Export-WindowsDriver -Online -Destination $destino | Out-Null
Write-Host ""[OK] Backup concluido com sucesso em / Backup completed: $destino""
";

    /// <summary>
    /// [PT-BR] Verificacao de vinculo da maquina com Microsoft Entra ID / Azure AD e Intune.
    /// [EN]    Workstation join status query against Microsoft Entra ID / Azure AD and Intune.
    /// </summary>
    public static string ScriptStatusM365 => @"
Write-Host '=== VINCULO ENTRA ID / INTUNE (dsregcmd /status) ==='
$s = dsregcmd /status
$s | Select-String 'AzureAdJoined|DomainJoined|WorkplaceJoined|TenantName|DeviceId|AzureAdPrt ' |
    ForEach-Object { Write-Host ""  $($_.Line.Trim())"" }
Write-Host ''
Write-Host 'Legenda: AzureAdJoined : YES indica maquina conectada ao Microsoft 365 e gerenciavel pelo Intune.'
";

    /// <summary>
    /// [PT-BR] Disparo de sincronizacao forcada de politicas e pacotes do Microsoft Intune (PushLaunch).
    /// [EN]    Force immediate sync with Microsoft Intune compliance policies and apps (PushLaunch task).
    /// </summary>
    public static string ScriptSincronizarIntune => @"
Write-Host 'Solicitando sincronizacao com o Microsoft Intune / Triggering Intune sync...'
$tarefas = Get-ScheduledTask -ErrorAction SilentlyContinue | Where-Object TaskName -eq 'PushLaunch'
if ($tarefas) {
    $tarefas | Start-ScheduledTask
    Write-Host '[OK] Sincronizacao acionada com sucesso. Politicas e pacotes devem chegar em instantes.'
} else {
    Write-Host '[AVISO] Tarefa PushLaunch nao encontrada. Este computador pode nao estar matriculado no Intune.'
}
";

    /// <summary>
    /// [PT-BR] Fechamento seguro do Microsoft Teams e limpeza completa dos caches corrompidos.
    /// [EN]    Safe termination of Microsoft Teams and purge of corrupted local cache directories.
    /// </summary>
    public static string ScriptLimparCacheTeams => HelperFunctions + @"
Write-Host 'Iniciando limpeza de cache do Microsoft Teams / Clearing Teams cache...'
$usuario = Obter-UsuarioLogado
if (-not ($usuario -and $usuario.Perfil)) {
    Write-Host '[ERRO] Usuario ativo nao detectado.'
    return
}
Write-Host ""Encerrando processos do Teams para o usuario $($usuario.Nome)...""
Get-Process ms-teams, Teams -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

$caches = @(
    'AppData\Local\Packages\MSTeams_8wekyb3d8bbwe\LocalCache\Microsoft\MSTeams',
    'AppData\Roaming\Microsoft\Teams'
) | ForEach-Object { Join-Path $usuario.Perfil $_ }

foreach ($c in $caches) {
    if (Test-Path $c) {
        Remove-Item ""$c\*"" -Recurse -Force -ErrorAction SilentlyContinue
        Write-Host ""[OK] Cache limpo em: $c""
    }
}
Write-Host '[OK] Cache do Teams resetado. O usuario ja pode reabrir o aplicativo.'
";
}
