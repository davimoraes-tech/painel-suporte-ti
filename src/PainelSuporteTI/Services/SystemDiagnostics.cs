using System;
using System.IO;
using System.Management;
using System.Threading.Tasks;
using PainelSuporteTI.Models;

namespace PainelSuporteTI.Services;

/// <summary>
/// [PT-BR] Provedor de diagnosticos de hardware e telemetria do sistema operacional utilizando consultas WMI/CIM nativas.
/// [EN]    Hardware diagnostic and OS telemetry provider using native WMI/CIM queries.
/// </summary>
public static class SystemDiagnostics
{
    /// <summary>
    /// [PT-BR] Coleta assincronamente os dados de telemetria da maquina para alimentar os cards do Dashboard.
    /// [EN]    Asynchronously collects workstation telemetry data to populate the Dashboard cards.
    /// </summary>
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
                // [PT-BR] 1. Consulta dados do Sistema Operacional (Versao, Uptime e Memoria RAM)
                // [EN]    1. Query Operating System metrics (Version, Uptime and RAM memory)
                using var searcherOs = new ManagementObjectSearcher(
                    "SELECT Caption, Version, LastBootUpTime, FreePhysicalMemory, TotalVisibleMemorySize FROM Win32_OperatingSystem");
                
                foreach (ManagementObject os in searcherOs.Get())
                {
                    info.WindowsVersion = $"{os["Caption"]} ({os["Version"]})";
                    
                    if (os["LastBootUpTime"] != null)
                    {
                        var bootDate = ManagementDateTimeConverter.ToDateTime(os["LastBootUpTime"].ToString());
                        var diff = DateTime.Now - bootDate;
                        info.UptimeSummary = $"{diff.Days}d {diff.Hours}h {diff.Minutes}m ({bootDate:dd/MM HH:mm})";
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

                // [PT-BR] 2. Consulta Fabricante, Modelo e o Usuario Real logado na sessao interativa
                // [EN]    2. Query Manufacturer, Model and the active interactive session username
                using var searcherCs = new ManagementObjectSearcher(
                    "SELECT Manufacturer, Model, UserName FROM Win32_ComputerSystem");
                
                foreach (ManagementObject cs in searcherCs.Get())
                {
                    info.ManufacturerAndModel = $"{cs["Manufacturer"]} {cs["Model"]}";
                    info.LoggedUser = cs["UserName"]?.ToString() ?? (LocalizationService.IsEnglish ? "No interactive user" : "Nenhum usuário detectado");
                }

                // [PT-BR] 3. Consulta Processador (CPU), nucleos e threads
                // [EN]    3. Query Processor (CPU), core and logical thread count
                using var searcherCpu = new ManagementObjectSearcher(
                    "SELECT Name, NumberOfCores, NumberOfLogicalProcessors FROM Win32_Processor");
                
                foreach (ManagementObject cpu in searcherCpu.Get())
                {
                    info.CpuName = $"{cpu["Name"]} ({cpu["NumberOfCores"]}c / {cpu["NumberOfLogicalProcessors"]}t)".Trim();
                    break;
                }

                // [PT-BR] 4. Consulta Numero de Serie gravado na BIOS
                // [EN]    4. Query Hardware Serial Number from BIOS
                using var searcherBios = new ManagementObjectSearcher(
                    "SELECT SerialNumber FROM Win32_BIOS");
                
                foreach (ManagementObject b in searcherBios.Get())
                {
                    info.SerialNumber = b["SerialNumber"]?.ToString() ?? "-";
                    break;
                }

                // [PT-BR] 5. Medicao do espaco em disco da unidade do sistema C:
                // [EN]    5. Disk space measurement on C: system drive
                var driveC = new DriveInfo("C");
                if (driveC.IsReady)
                {
                    double totalGb = driveC.TotalSize / (1024.0 * 1024 * 1024);
                    double freeGb = driveC.AvailableFreeSpace / (1024.0 * 1024 * 1024);
                    double usedGb = totalGb - freeGb;
                    info.DiskCUsedPercent = Math.Round((usedGb / totalGb) * 100, 1);

                    var freeText = LocalizationService.IsEnglish ? "free of" : "livres de";
                    var usedText = LocalizationService.IsEnglish ? "used" : "usado";
                    info.DiskCSummary = $"{freeGb:N1} GB {freeText} {totalGb:N1} GB ({info.DiskCUsedPercent}% {usedText})";
                }
            }
            catch (Exception ex)
            {
                LogService.EscreverLog($"WMI telemetry error: {ex.Message}");
            }

            return info;
        });
    }
}
