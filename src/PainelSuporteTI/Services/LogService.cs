using System;
using System.IO;
using System.Text;

namespace PainelSuporteTI.Services;

public static class LogService
{
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

    public static void EscreverLog(string texto)
    {
        try
        {
            var linha = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {Environment.MachineName} | {Environment.UserName} | {texto}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, linha, Encoding.UTF8);
        }
        catch { }
    }

    public static string[] ObterUltimasLinhas(int quantidade = 50)
    {
        try
        {
            if (!File.Exists(LogFilePath))
                return new[] { "Nenhum registro de log encontrado ainda." };

            var linhas = File.ReadAllLines(LogFilePath, Encoding.UTF8);
            if (linhas.Length <= quantidade)
                return linhas;

            var resultado = new string[quantidade];
            Array.Copy(linhas, linhas.Length - quantidade, resultado, 0, quantidade);
            return resultado;
        }
        catch (Exception ex)
        {
            return new[] { $"Erro ao ler log: {ex.Message}" };
        }
    }
}
