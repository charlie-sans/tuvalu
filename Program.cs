using System;
using Tuvalu;
using Tuvalu.DB;
using Tuvalu.logger;
using Tuvalu.tui;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Tuvalu
{
    public class Program
    {
        public struct Data
        {
            public DBconnector DB;
            public List<TTasks.TTask> Tasklist;
            public string json_config;

            public Data(DBconnector db, List<TTasks.TTask> tasklist)
            {
                DB = db;
                Tasklist = tasklist;
                json_config = null;
            }
        }
        static DBconnector DB = new DBconnector
        {
            DBPath = "Tasks.sqlite",
            Provider = "SQLite",
            DBConnectionString = "Data Source=Tasks.sqlite",
            DBName = "Tasks",
            DBType = "SQLite"
        };
        public static void Main(string[] args)
        {
            try
            {


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
                    if (System.IO.File.Exists("config.json"))
                    {
                        using (System.IO.StreamReader sr = new System.IO.StreamReader("config.json"))
                        {
                            if (sr != null)
                            {
                                string json = sr.ReadToEnd();
                                Data data = new Data(DB, new List<TTasks.TTask>());
                                data.json_config = json;
                                configure(data);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Config file does not exist");
                        Logger.Log("Config file does not exist, going to see if i can create it for them :)");
                        Console.WriteLine("Creating config file");
                        string json = "{\"is_setup\": false}";
                        File.WriteAllText("config.json", json);

                        using (System.IO.StreamWriter file = new System.IO.StreamWriter("config.json"))
                        {
                            file.WriteLine(json);
                        }
                        Console.WriteLine("Config file created");
                        Logger.Log("Config file created, going to configure the application");
                       
                    }
                  
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                Logger.Log(ex);
            }
        }
        public static void print(string data)
        {
            Console.WriteLine(data);
        }
        public static void configure(Data data)
        {
            string json = data.json_config;
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            bool is_setup = jsonObject != null && jsonObject.ContainsKey("is_setup");
            if (is_setup)
            {
                Console.WriteLine("Configuration already done");
                tui.Tui.Wmain(data);
            }
            else
            {
                Console.WriteLine("Configuration not done");
                Console.WriteLine("Starting configuration");
                Console.WriteLine("Enter the name of the user using this application");
                string? user = Console.ReadLine();
                Console.WriteLine("have any pronouns? (y/n)");
                string? pronouns = Console.ReadLine();
                if (pronouns == "y")
                {
                    Console.WriteLine("Enter pronouns");
                    string? pronouns_input = Console.ReadLine();
                }
                Console.WriteLine("what is your preferred theme? (light/dark)");
                string? theme = Console.ReadLine();
                if (theme == "light")
                {
                    Console.WriteLine("Light theme selected");
                }
                else if (theme == "dark")
                {
                    Console.WriteLine("Dark theme selected");
                }
                else
                {
                    Console.WriteLine("Invalid theme selected, defaulting to dark theme");
                }

                // we now have a json object with the user's preferences
                jsonObject["is_setup"] = true;
                jsonObject["user"] = user;
                jsonObject["pronouns"] = pronouns;
                jsonObject["theme"] = theme; // bro, just stfu please respectfully
                // if we can't serialize the object, we'll just throw an exception, gracefully halt the program, and log the error
                try
                {
                    string new_json = JsonConvert.SerializeObject(jsonObject);
                    File.WriteAllText("config.json", new_json);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter("config.json"))
                    {
                        file.WriteLine(new_json);
                    }
                    Console.WriteLine("Configuration complete");
                    tui.Tui.Wmain(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                    Logger.Log(ex);
                    return;
                }


            }
        }
        public static void addTask(Data data)
        {
            print("Enter task name: ");
            string name = Console.ReadLine();
            print("Enter task description: ");
            string description = Console.ReadLine();
            print("Enter task status: ");
            string status = Console.ReadLine();
            print("Enter task priority: ");
            string priority = Console.ReadLine();
            print("Enter task due date: ");
            string dueDate = Console.ReadLine();
            print("Enter task created date: ");
            string createdDate = Console.ReadLine();
            print("Enter task completed date: ");
            string completedDate = Console.ReadLine();
            int ID = DBconnector.GetNextID(DB);
            TTasks.TTask task = new TTasks.TTask(name, description, status, priority, dueDate, createdDate, completedDate, ID.ToString());
            data.Tasklist.Add(task);
            DB.DBCommand = $"INSERT INTO Tasks (Name, Description, Status, Priority, DueDate, CreatedDate, CompletedDate, ID) VALUES ('{name}', '{description}', '{status}', '{priority}', '{dueDate}', '{createdDate}', '{completedDate}', '{ID}')";
            DBconnector.InsertData(DB);
            Logger.Log("Task added");
        }
        public static void listTasks(Data data)
        {
            DB.DBCommand = "SELECT * FROM Tasks";
            DBconnector.GetTasks(DB);
            foreach (TTasks.TTask task in data.Tasklist)
            {
                print($"Name: {task.Name}\nDescription: {task.Description}\nStatus: {task.Status}\nPriority: {task.Priority}\nDue Date: {task.DueDate}\nCreated Date: {task.CreatedDate}\nCompleted Date: {task.CompletedDate}\nID: {task.ID}\n");
            }
            Logger.Log("Tasks listed");
        }
    }
}
