using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tuvalu.DB;
using Tuvalu.logger;
using Tuvalu.Tasks;

namespace Tuvaluw
{
    public class Program
    {
        public struct Data
        {
            public DBconnector DB;
            public List<TTasks.TTask> Tasklist;
            public Dictionary<string, object>? json_config;

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
                    Console.WriteLine("Database exists");
                    if (File.Exists("config.json"))
                    {
                        using (StreamReader sr = new StreamReader("config.json"))
                        {
                            if (sr != null)
                            {
                                string json = sr.ReadToEnd();
                                Dictionary<string, object>? json_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                                Data data = new Data();
                                data.json_config = json_data;
                                Logger.Log("Config file exists");
                                configure(data);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Config file does not exist");
                        Logger.Log("Config file does not exist");
                        configure(new Data());
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
            var setup = (bool)data.json_config["is_setup"];
            if (setup == true)
            {
                print("already setup.\n");
                Logger.Log("Already setup");
                start(data);
                return;
            }
        }
        public static void start(Data data)
        {
            print("Starting Tuvalu\n");
            Logger.Log("Starting Tuvalu");
            bool running = true;
            while (running)
            {
                print("Enter a command: ");
                string command = Console.ReadLine();
                if (command == "exit")
                {
                    running = false;
                    Logger.Log("Exiting Tuvalu");
                    print("Exiting Tuvalu\n");
                }
                else
                {
                    switch (command)
                    {
                        case "add":
                            print("Adding task\n");
                            Logger.Log("Adding task");
                            addTask(data);
                            break;
                        case "list":
                            print("Listing tasks\n");
                            Logger.Log("Listing tasks");
                            listTasks(data);
                            break;
                        // case "complete":
                        //     print("Completing task\n");
                        //     Logger.Log("Completing task");
                        //     completeTask(data);
                        //     break;
                        // case "delete":
                        //     print("Deleting task\n");
                        //     Logger.Log("Deleting task");
                        //     deleteTask(data);
                        //     break;
                        case "help":
                            print("Commands: add, list, complete, delete, exit\n");
                            Logger.Log("Help");
                            break;
                        default:
                            print("Invalid command\n");
                            Logger.Log("Invalid command");
                            break;
                    }
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
