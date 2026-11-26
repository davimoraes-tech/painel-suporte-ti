namespace PainelSuporteTI.Models;

/// <summary>
/// [PT-BR] Modelo de dados que encapsula as metricas de telemetria exibidas no Dashboard.
/// [EN]    Data model encapsulating system telemetry metrics displayed on the Dashboard.
/// </summary>
public class DashboardInfo
{
    /// <summary>
    /// [PT-BR] Nome do computador na rede (Hostname).
    /// [EN]    Computer hostname on the network.
    /// </summary>
    public string ComputerName { get; set; } = Environment.MachineName;

    /// <summary>
    /// [PT-BR] Nome da conta de usuario ativa conectada a sessao interativa do Windows.
    /// [EN]    Username of the active interactive Windows session.
    /// </summary>
    public string LoggedUser { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Fabricante e modelo da placa-mae ou dispositivo (ex.: Dell OptiPlex).
    /// [EN]    Manufacturer and device/motherboard model (e.g., Dell OptiPlex).
    /// </summary>
    public string ManufacturerAndModel { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Numero de serie gravado na BIOS / firmware.
    /// [EN]    Hardware serial number retrieved from BIOS / firmware.
    /// </summary>
    public string SerialNumber { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Nome do sistema operacional e versao da build (ex.: Windows 11 Pro 23H2).
    /// [EN]    Operating system caption and build version (e.g., Windows 11 Pro 23H2).
    /// </summary>
    public string WindowsVersion { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Descricao do processador com quantidade de nucleos e threads.
    /// [EN]    Processor description including core count and logical threads.
    /// </summary>
    public string CpuName { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Texto resumido da memoria RAM (ex.: 8.2 GB / 16.0 GB (51%)).
    /// [EN]    RAM usage summary string (e.g., 8.2 GB / 16.0 GB (51%)).
    /// </summary>
    public string RamSummary { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Percentual de memoria RAM consumida (0 a 100).
    /// [EN]    Percentage of consumed RAM memory (0 to 100).
    /// </summary>
    public double RamUsedPercent { get; set; }

    /// <summary>
    /// [PT-BR] Texto resumido de espaco livre na unidade do sistema C:.
    /// [EN]    Free space summary string on the C: system drive.
    /// </summary>
    public string DiskCSummary { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Percentual de espaco ocupado no disco C: (0 a 100).
    /// [EN]    Percentage of used disk space on drive C: (0 to 100).
    /// </summary>
    public double DiskCUsedPercent { get; set; }

    /// <summary>
    /// [PT-BR] Tempo total desde a ultima inicializacao do Windows (Uptime).
    /// [EN]    Total elapsed time since last Windows boot (Uptime).
    /// </summary>
    public string UptimeSummary { get; set; } = "-";

    /// <summary>
    /// [PT-BR] Indica se a aplicacao esta rodando com privilegios elevados de Administrador.
    /// [EN]    Indicates whether the application is running with elevated Administrator privileges.
    /// </summary>
    public bool IsAdmin { get; set; }
}
