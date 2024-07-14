using System;
using System.IO;

public static class Logger
{
    private static string? logFilePath;

    public static void Initialize(string path)
    {
        logFilePath = Path.GetFullPath(path); // Resolve the absolute path
        var logDir = Path.GetDirectoryName(logFilePath);

        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir!);
        }

        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }
    }

    public static void WriteLog(string message)
    {
        if (string.IsNullOrEmpty(logFilePath))
        {
            throw new InvalidOperationException("Logger has not been initialized with a valid log file path.");
        }

        var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
        //Console.WriteLine(logMessage);
        File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
    }
}
