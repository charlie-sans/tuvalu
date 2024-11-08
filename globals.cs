using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
namespace Tuvalu
{
    public class Globals
    {
        public static string Version = "0.0.1";
        public static string AppName = "Tuvalu";
        public static string AppPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "AppData");
        public static string AppDataPath = Path.Combine(AppPath, AppName);
        public static string AppLogPath = Path.Combine(AppDataPath, "logs");
        public static string AppConfigPath = Path.Combine(AppDataPath, "config");
        public static string AppDBPath = Path.Combine(AppDataPath, "db");
        public static string AppTempPath = Path.Combine(AppDataPath, "temp");
        public static string AppConfigFile = Path.Combine(AppConfigPath, "config.json");
        public static string AppDBFile = Path.Combine(AppDBPath, "tasks.sqlite");
        public static string AppLogFile = Path.Combine(AppLogPath, $"tuvalu-log-{DateTime.Now.ToString("yyyy-MM-dd")}.EXL");
        public static void CreateAppData()
        {
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }
            if (!Directory.Exists(AppLogPath))
            {
                Directory.CreateDirectory(AppLogPath);
            }
            if (!Directory.Exists(AppConfigPath))
            {
                Directory.CreateDirectory(AppConfigPath);
            }
            if (!Directory.Exists(AppDBPath))
            {
                Directory.CreateDirectory(AppDBPath);
            }
            if (!Directory.Exists(AppTempPath))
            {
                Directory.CreateDirectory(AppTempPath);
            }
        }
    }
}
