using System;
using System.IO;
using System.Text;

namespace PainelSuporteTI.Services;

/// <summary>
/// [PT-BR] Servico responsavel por registrar operacoes no arquivo de log de auditoria em C:\ProgramData\PainelTI\painel.log.
/// [EN]    Service responsible for writing operations into the local audit log file at C:\ProgramData\PainelTI\painel.log.
/// </summary>
public static class LogService
{
    // [PT-BR] Diretorio persistente em ProgramData para gravacao do log acessivel a todos os usuarios
    // [EN]    Persistent ProgramData directory allowing audit log write access across all user sessions
    private static readonly string LogDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "PainelTI");
    public static readonly string LogFilePath = Path.Combine(LogDir, "painel.log");

    static LogService()
    {
        try
        {
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }
        }
        catch { }
    }

    /// <summary>
    /// [PT-BR] Registra uma acao no log com data/hora, nome da maquina, usuario e descricao da operacao.
    /// [EN]    Appends an action to the log with timestamp, machine name, active username and action description.
    /// </summary>
    public static void EscreverLog(string texto)
    {
        try
        {
            var linha = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {Environment.MachineName} | {Environment.UserName} | {texto}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, linha, Encoding.UTF8);
        }
        catch { }
    }

    /// <summary>
    /// [PT-BR] Retorna as ultimas linhas do arquivo de log para exibicao na tela de Auditoria.
    /// [EN]    Retrieves the most recent lines from the audit log to display on the Audit screen.
    /// </summary>
    public static string[] ObterUltimasLinhas(int quantidade = 50)
    {
        try
        {
            if (!File.Exists(LogFilePath))
            {
                var msgVazia = LocalizationService.IsEnglish
                    ? "No audit records found yet."
                    : "Nenhum registro de log encontrado ainda.";
                return new[] { msgVazia };
            }

            var linhas = File.ReadAllLines(LogFilePath, Encoding.UTF8);
            if (linhas.Length <= quantidade)
                return linhas;

            var resultado = new string[quantidade];
            Array.Copy(linhas, linhas.Length - quantidade, resultado, 0, quantidade);
            return resultado;
        }
        catch (Exception ex)
        {
            return new[] { $"Error reading log: {ex.Message}" };
        }
    }
}
