namespace PainelSuporteTI.Models;

public class DashboardInfo
{
    public string ComputerName { get; set; } = Environment.MachineName;
    public string LoggedUser { get; set; } = "-";
    public string ManufacturerAndModel { get; set; } = "-";
    public string SerialNumber { get; set; } = "-";
    public string WindowsVersion { get; set; } = "-";
    public string CpuName { get; set; } = "-";
    public string RamSummary { get; set; } = "-";
    public double RamUsedPercent { get; set; }
    public string DiskCSummary { get; set; } = "-";
    public double DiskCUsedPercent { get; set; }
    public string UptimeSummary { get; set; } = "-";
    public bool IsAdmin { get; set; }
}
