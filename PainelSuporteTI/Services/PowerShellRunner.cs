using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PainelSuporteTI.Services;

public class PowerShellRunner
{
    public static bool EhAdmin()
    {
        try
        {
            var id = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(id);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch
        {
            return false;
        }
    }

    public static async Task<int> RunScriptAsync(
        string scriptText,
        string actionName,
        Action<string, bool> onOutput,
        CancellationToken cancellationToken = default)
    {
        LogService.EscreverLog($"Início: {actionName}");
        onOutput?.Invoke($"[{DateTime.Now:HH:mm:ss}] >>> Iniciando: {actionName}...", false);

        // Prepara script temporário para evitar problemas de escape de aspas
        var tempScript = Path.Combine(Path.GetTempPath(), $"PainelTI_{Guid.NewGuid():N}.ps1");
        
        try
        {
            var fullScript = new StringBuilder();
            fullScript.AppendLine("$ErrorActionPreference = 'Continue'");
            fullScript.AppendLine("[Console]::OutputEncoding = [System.Text.Encoding]::UTF8");
            fullScript.AppendLine(scriptText);
            
            await File.WriteAllTextAsync(tempScript, fullScript.ToString(), Encoding.UTF8, cancellationToken);

            var startInfo = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{tempScript}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true };

            var tcs = new TaskCompletionSource<int>();

            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    onOutput?.Invoke(e.Data, false);
                }
            };

            process.ErrorDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    onOutput?.Invoke(e.Data, true);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            using (cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        onOutput?.Invoke("[!] Processo cancelado pelo operador.", true);
                    }
                }
                catch { }
            }))
            {
                await process.WaitForExitAsync(cancellationToken);
                var exitCode = process.ExitCode;
                LogService.EscreverLog($"Fim: {actionName} (ExitCode: {exitCode})");
                onOutput?.Invoke($"[{DateTime.Now:HH:mm:ss}] >>> Concluído: {actionName} (Código {exitCode})", false);
                return exitCode;
            }
        }
        catch (OperationCanceledException)
        {
            LogService.EscreverLog($"Cancelado: {actionName}");
            onOutput?.Invoke($"[{DateTime.Now:HH:mm:ss}] [!] Ação cancelada.", true);
            return -1;
        }
        catch (Exception ex)
        {
            LogService.EscreverLog($"Erro em {actionName}: {ex.Message}");
            onOutput?.Invoke($"[X] Erro ao executar: {ex.Message}", true);
            return -1;
        }
        finally
        {
            try
            {
                if (File.Exists(tempScript))
                    File.Delete(tempScript);
            }
            catch { }
        }
    }

    public static void IniciarProcessoExterno(string fileName, string args = "")
    {
        try
        {
            LogService.EscreverLog($"Abriu ferramenta externa: {fileName} {args}");
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            LogService.EscreverLog($"Falha ao abrir {fileName}: {ex.Message}");
        }
    }
}
