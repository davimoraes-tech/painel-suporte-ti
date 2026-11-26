using System;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using PainelSuporteTI.Models;

namespace PainelSuporteTI.Services;

public static class SystemDiagnostics
{
    public static async Task<DashboardInfo> ObterDashboardInfoAsync()
    {
        return await Task.Run(() =>
        {
            var info = new DashboardInfo
            {
                ComputerName = Environment.MachineName,
                IsAdmin = PowerShellRunner.EhAdmin()
            };

            try
            {
                // Obter sistema operacional
                using var searcherOs = new ManagementObjectSearcher("SELECT Caption, Version, LastBootUpTime, FreePhysicalMemory, TotalVisibleMemorySize FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcherOs.Get())
                {
                    info.WindowsVersion = $"{os["Caption"]} ({os["Version"]})";
                    
                    if (os["LastBootUpTime"] != null)
                    {
                        var bootDate = ManagementDateTimeConverter.ToDateTime(os["LastBootUpTime"].ToString());
                        var diff = DateTime.Now - bootDate;
                        info.UptimeSummary = $"{diff.Days}d {diff.Hours}h {diff.Minutes}m (desde {bootDate:dd/MM HH:mm})";
                    }

                    if (os["TotalVisibleMemorySize"] != null && os["FreePhysicalMemory"] != null)
                    {
                        double totalKb = Convert.ToDouble(os["TotalVisibleMemorySize"]);
                        double freeKb = Convert.ToDouble(os["FreePhysicalMemory"]);
                        double usedKb = totalKb - freeKb;
                        double totalGb = totalKb / (1024 * 1024);
                        double usedGb = usedKb / (1024 * 1024);
                        info.RamUsedPercent = Math.Round((usedKb / totalKb) * 100, 1);
                        info.RamSummary = $"{usedGb:N1} GB / {totalGb:N1} GB ({info.RamUsedPercent}%)";
                    }
                }

                // Obter Fabricante e Usuário Logado
                using var searcherCs = new ManagementObjectSearcher("SELECT Manufacturer, Model, UserName FROM Win32_ComputerSystem");
                foreach (ManagementObject cs in searcherCs.Get())
                {
                    info.ManufacturerAndModel = $"{cs["Manufacturer"]} {cs["Model"]}";
                    info.LoggedUser = cs["UserName"]?.ToString() ?? "Nenhum usuário detectado";
                }

                // Obter Processador
                using var searcherCpu = new ManagementObjectSearcher("SELECT Name, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
                foreach (ManagementObject cpu in searcherCpu.Get())
                {
                    info.CpuName = $"{cpu["Name"]} ({cpu["NumberOfCores"]}c / {cpu["NumberOfLogicalProcessors"]}t)".Trim();
                    break;
                }

                // Obter Serial BIOS
                using var searcherBios = new ManagementObjectSearcher("SELECT SerialNumber FROM Win32_BIOS");
                foreach (ManagementObject b in searcherBios.Get())
                {
                    info.SerialNumber = b["SerialNumber"]?.ToString() ?? "-";
                    break;
                }

                // Disco C:
                var driveC = new DriveInfo("C");
                if (driveC.IsReady)
                {
                    double totalGb = driveC.TotalSize / (1024.0 * 1024 * 1024);
                    double freeGb = driveC.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    double usedGb = totalGb - freeGb;
                    info.DiskCUsedPercent = Math.Round((usedGb / totalGb) * 100, 1);
                    info.DiskCSummary = $"{freeGb:N1} GB livres de {totalGb:N1} GB ({info.DiskCUsedPercent}% usado)";
                }
            }
            catch (Exception ex)
            {
                LogService.EscreverLog($"Erro ao ler telemetria WMI: {ex.Message}");
            }

            return info;
        });
    }
}
