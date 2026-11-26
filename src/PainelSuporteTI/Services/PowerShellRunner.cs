using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PainelSuporteTI.Services;

/// <summary>
/// [PT-BR] Executor assincrono para comandos e rotinas administrativas do Windows com suporte a streaming de saida e cancelamento.
/// [EN]    Asynchronous runner for administrative Windows routines with real-time output streaming and cancellation support.
/// </summary>
public class PowerShellRunner
{
    /// <summary>
    /// [PT-BR] Verifica se o processo atual esta sendo executado com privilegios de Administrador local.
    /// [EN]    Checks whether the current process is running with local Administrator privileges.
    /// </summary>
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

    /// <summary>
    /// [PT-BR] Executa uma rotina administrativa assincronamente, enviando cada linha de saida em tempo real para o terminal visual.
    /// [EN]    Executes an administrative routine asynchronously, streaming each output line in real time to the visual UI terminal.
    /// </summary>
    /// <param name="scriptText">[PT-BR] Codigo da rotina a ser executada / [EN] Routine script code to execute</param>
    /// <param name="actionName">[PT-BR] Nome da acao para registro em log / [EN] Action name for audit logging</param>
    /// <param name="onOutput">[PT-BR] Callback invocado para cada linha (texto, isError) / [EN] Callback invoked per output line (text, isError)</param>
    /// <param name="cancellationToken">[PT-BR] Token para interromper o processo / [EN] Token to abort execution</param>
    public static async Task<int> RunScriptAsync(
        string scriptText,
        string actionName,
        Action<string, bool> onOutput,
        CancellationToken cancellationToken = default)
    {
        LogService.EscreverLog($"Start: {actionName}");
        
        var msgInicio = LocalizationService.IsEnglish
            ? $"[{DateTime.Now:HH:mm:ss}] >>> Starting: {actionName}..."
            : $"[{DateTime.Now:HH:mm:ss}] >>> Iniciando: {actionName}...";
        onOutput?.Invoke(msgInicio, false);

        // [PT-BR] Prepara um arquivo temporario limpo para evitar problemas de escape de aspas e caracteres especiais no terminal
        // [EN]    Prepares an isolated temp file to avoid quote escaping and special character issues in the command line
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

            // [PT-BR] Captura da saida padrao linha a linha (Streaming)
            // [EN]    Line-by-line standard output streaming
            process.OutputDataReceived += (s, e) =>
            {
                if (e.Data != null)
                {
                    onOutput?.Invoke(e.Data, false);
                }
            };

            // [PT-BR] Captura de erros linha a linha
            // [EN]    Line-by-line standard error streaming
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

            // [PT-BR] Registro de cancelamento: se o usuario clicar em 'Interromper', encerra a arvore do processo com seguranca
            // [EN]    Cancellation registration: terminates the process tree safely if operator cancels
            using (cancellationToken.Register(() =>
            {
                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(entireProcessTree: true);
                        var msgCanc = LocalizationService.IsEnglish
                            ? "[!] Process cancelled by operator."
                            : "[!] Processo cancelado pelo operador.";
                        onOutput?.Invoke(msgCanc, true);
                    }
                }
                catch { }
            }))
            {
                await process.WaitForExitAsync(cancellationToken);
                var exitCode = process.ExitCode;
                LogService.EscreverLog($"End: {actionName} (ExitCode: {exitCode})");

                var msgFim = LocalizationService.IsEnglish
                    ? $"[{DateTime.Now:HH:mm:ss}] >>> Completed: {actionName} (Code {exitCode})"
                    : $"[{DateTime.Now:HH:mm:ss}] >>> Concluído: {actionName} (Código {exitCode})";
                onOutput?.Invoke(msgFim, false);

                return exitCode;
            }
        }
        catch (OperationCanceledException)
        {
            LogService.EscreverLog($"Cancelled: {actionName}");
            var msgCanc = LocalizationService.IsEnglish ? "[!] Action cancelled." : "[!] Ação cancelada.";
            onOutput?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msgCanc}", true);
            return -1;
        }
        catch (Exception ex)
        {
            LogService.EscreverLog($"Error in {actionName}: {ex.Message}");
            onOutput?.Invoke($"[X] Error: {ex.Message}", true);
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

    /// <summary>
    /// [PT-BR] Abre ferramentas nativas do Windows de forma desvinculada (ex.: cleanmgr.exe, resmon.exe, devmgmt.msc).
    /// [EN]    Launches native Windows standalone tools (e.g., cleanmgr.exe, resmon.exe, devmgmt.msc).
    /// </summary>
    public static void IniciarProcessoExterno(string fileName, string args = "")
    {
        try
        {
            LogService.EscreverLog($"External tool launched: {fileName} {args}");
            Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = args,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            LogService.EscreverLog($"Failed to launch {fileName}: {ex.Message}");
        }
    }
}
