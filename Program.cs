#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
// #define DEBUG 

using System;
using Tuvalu;
using Tuvalu.DB;
using Tuvalu.logger;
using Tuvalu.tui;
using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Tuvalu
{
    public class Program
    {
        public static DBconnector DB = new DBconnector
        {
            DBPath = Globals.AppDBFile,
            Provider = "SQLite",
            DBConnectionString = $"Data Source={Globals.AppDBFile}",
            DBName = "Tasks",
            DBType = "SQLite"
        };  
       
        public static async Task Main(string[] args)
        {
            try
            {
                await InitializeApplicationAsync();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"An unexpected error occurred: {ex.Message}");
                await Task.Run(() => Logger.Log(ex));
            }
        }

        private static async Task InitializeApplicationAsync()
        {
            await Task.Run(() => Globals.CreateAppData());
            
            if (!DBconnector.DBExists(DB))
            {
                await InitializeNewDatabaseAsync();
            }
            
            await LoadConfigurationAsync();
        }

        private static async Task LoadConfigurationAsync()
        {
            if (File.Exists(Globals.AppConfigFile))
            {
                string json = await File.ReadAllTextAsync(Globals.AppConfigFile);
                var data = new Data(DB, new List<TTasks.TTask>()) { json_config = json };
                await Misc.ConfigureAsync(data);
            }
            else
            {
                await CreateDefaultConfigAsync();
            }
        }

        private static async Task CreateDefaultConfigAsync()
        {
            string json = "{\"is_setup\": false}";
            await File.WriteAllTextAsync(Globals.AppConfigFile, json);
            await Task.Run(() => Logger.Log("Config file created, going to configure the application"));
            await Misc.ConfigureAsync(new Data(DB, new List<TTasks.TTask>()));
        }

        private static async Task InitializeNewDatabaseAsync()
        {
            await Console.Out.WriteLineAsync("Database does not exist");
            
            await Task.Run(() => {
                DBconnector.CreateDB(DB);
                DB.DBCommand = "CREATE TABLE IF NOT EXISTS Tasks (Name TEXT, Description TEXT, Status TEXT, Priority TEXT, DueDate TEXT, CreatedDate TEXT, CompletedDate TEXT, ID TEXT PRIMARY KEY)";
                DBconnector.CreateTable(DB);
            });
            
            await Console.Out.WriteLineAsync("Database initialization complete");
        }
         
        public static async Task PrintAsync(string data)
        {
            await Console.Out.WriteLineAsync(data);
        }
    }
}
