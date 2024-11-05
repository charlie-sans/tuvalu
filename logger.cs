using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;



namespace Tuvalu.logger
{
    public class Logger
    {
        static string time = DateTime.Now.ToString("yyyy-MM-dd");
        public struct LogEntry
        {
            public string Message;
            public string Level;
            public string Timestamp;
        }

        public static void Log(LogEntry entry)
        {   
            
            string logPath = $"log{Logger.time}.txt";
            string logEntry = $"{entry.Timestamp} - {entry.Level}: {entry.Message}\n";
            File.AppendAllText(logPath, logEntry);
        }

        public static void Log(string message, string level)
        {
            LogEntry entry = new LogEntry
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Log(entry);
        }

        public static void Log(string message)
        {
            Log(message, "INFO");
        }

        public static void Log(Exception ex)
        {
            LogEntry entry = new LogEntry
            {
                Message = ex.Message,
                Level = "ERROR",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Log(entry);
        }

        public static void Log(string message, Exception ex)
        {
            LogEntry entry = new LogEntry
            {
                Message = $"{message}: {ex.Message}",
                Level = "ERROR",
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            Log(entry);
        }

        public static void Log(string message, string level, string logPath)
        {
            LogEntry entry = new LogEntry
            {
                Message = message,
                Level = level,
                Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            };
            string logEntry = $"{entry.Timestamp} - {entry.Level}: {entry.Message}";
            File.AppendAllText(logPath, logEntry);
        }


    }
}