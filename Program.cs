using System;
using Tuvalu.Tasks;
using Tuvalu.DB;
using Tuvalu.logger;
using Terminal.Gui;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Data.SqlClient;
using Tuvalu.tui;
namespace Tuvalu
{
    public class Program
    {
        public struct Data
        {
            public DBconnector DB;
            public List<TTasks.TTask> Tasklist;

            public Data(DBconnector db, List<TTasks.TTask> tasklist)
            {
                DB = db;
                Tasklist = tasklist;
            }
            //config
            public string json_config;
            
        }
        public static void Main(string[] args)
        {
            try
            {
                
                var DB = new DBconnector
                {
                    DBPath = "Tasks.sqlite",
                    Provider = "SQLite",
                    DBConnectionString = "Data Source=Tasks.sqlite",
                    DBName = "Tasks",
                    DBType = "SQLite"
                };
                if (!DBconnector.DBExists(DB))
                {
                    Console.WriteLine("Database does not exist");
                    DBconnector.CreateDB(DB);
                    Console.WriteLine("Database created");

                    DB.DBCommand = "CREATE TABLE IF NOT EXISTS Tasks (Name TEXT, Description TEXT, Status TEXT, Priority TEXT, DueDate TEXT, CreatedDate TEXT, CompletedDate TEXT, ID TEXT)";
                    DBconnector.CreateTable(DB);
                }
                else
                {
                    Console.WriteLine("Database exists, lets goo");
                    using (StreamReader sr = new StreamReader("config.json"))
                    {
                        if (sr != null)
                        {
                            string json = sr.ReadToEnd();
                            string json_data = JsonConvert.DeserializeObject<string>(json) ?? string.Empty;
                            Data data = new();
                            data.json_config = json_data;
                            configure(data);
                        }
                    }
                }

            }
            catch (SqlException sqlEx)
            {
                Console.WriteLine($"Database error: {sqlEx.Message}");
                Logger.Log(sqlEx);
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"File I/O error: {ioEx.Message}");
                Logger.Log(ioEx);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Logger.Log(ex);
            }
        }
        public static void configure(Data data)
        {
            string json = data.json_config;
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            bool? is_setup = jsonObject != null && jsonObject.ContainsKey("is_setup") ? jsonObject["is_setup"] as bool? : null;

        }
    }
}