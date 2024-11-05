using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.CSharp.RuntimeBinder;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlTypes;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Sql;
using Tuvalu.DB;
namespace Tuvalu{
    public class Task_helper_lines
    {
        public static string? greetings(int message_ID)
        {
            string? message = null;
            switch(message_ID)
            {
                case 0:
                    message = "hello there, welcome to tuvalu";
                    break;  
                case 1:
                    message = "welcome back! how can we help you today?";
                    break;
                
                default:
                    message = "welcome to tuvalu";
                    break;
            }
            return message;
        }
        public static string? goodbye(int message_ID)
        {
            string? message = null;
            switch(message_ID)
            {
                case 0:
                    message = "goodbye, have a great day!";
                    break;  
                case 1:
                    message = "take care, see you soon!";
                    break;
                
                default:
                    message = "goodbye!";
                    break;
            }
            return message;
        }
        

    }

    public class TTasks
        {
            DBconnector dBconnector = new DBconnector();
            String con;

            // public TTasks()
            // {
            //     con = dBconnector.DBConnectionString;
            // }
            public struct TTask{
                public string Name { get; set; }

                public string Description { get; set; }

                public string Status { get; set; }

                public string Priority { get; set; }
                public string DueDate { get; set; }
                public string CreatedDate { get; set; }
                public string CompletedDate { get; set; }
                public string ID { get; set; }

                public static bool operator ==(TTask left, TTask right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(TTask left, TTask right)
            {
                return !(left == right);
            }

            public override bool Equals(object? obj)
            {
                if (obj is TTask task)
                {
                    return Name == task.Name &&
                           Description == task.Description &&
                           Status == task.Status &&
                           Priority == task.Priority &&
                           DueDate == task.DueDate &&
                           CreatedDate == task.CreatedDate &&
                           CompletedDate == task.CompletedDate &&
                           ID == task.ID;
                }
                return false;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Name, Description, Status, Priority, DueDate, CreatedDate, CompletedDate, ID);
            }

                public TTask(string name, string description, string status, string priority, string dueDate, string createdDate, string completedDate, string id)
                {
                    Name = name;
                    Description = description;
                    Status = status;
                    Priority = priority;
                    DueDate = dueDate;
                    CreatedDate = createdDate;
                    CompletedDate = completedDate;
                    ID = id;
                }
            }

        

            public static TTask CreateTask(string name, string description, string status, string priority, string dueDate, string createdDate, string completedDate, string id)
            {
                TTask task = new TTask();
                task.Name = name;
                task.Description = description;
                task.Status = status;
                task.Priority = priority;
                task.DueDate = dueDate;
                task.CreatedDate = createdDate;
                task.CompletedDate = completedDate;
                task.ID = id;
                return task;
            }

            public static void AddTask(TTask task, string connectionString)
            {
                          
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "INSERT INTO Tasks (Name, Description, Status, Priority, DueDate, CreatedDate, CompletedDate, ID) VALUES (@Name, @Description, @Status, @Priority, @DueDate, @CreatedDate, @CompletedDate, @ID)";
                        command.Parameters.AddWithValue("@Name", task.Name);
                        command.Parameters.AddWithValue("@Description", task.Description);
                        command.Parameters.AddWithValue("@Status", task.Status);
                        command.Parameters.AddWithValue("@Priority", task.Priority);
                        command.Parameters.AddWithValue("@DueDate", task.DueDate);
                        command.Parameters.AddWithValue("@CreatedDate", task.CreatedDate);
                        command.Parameters.AddWithValue("@CompletedDate", task.CompletedDate);
                        command.Parameters.AddWithValue("@ID", task.ID);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }

            public static void UpdateTask(TTask task, string connectionString)
            {
                DBconnector dBconnector = new DBconnector();
                var con = dBconnector.DBConnectionString;
                using (SQLiteConnection connection = new SQLiteConnection(con))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "UPDATE Tasks SET Name = @Name, Description = @Description, Status = @Status, Priority = @Priority, DueDate = @DueDate, CreatedDate = @CreatedDate, CompletedDate = @CompletedDate WHERE ID = @ID";
                        command.Parameters.AddWithValue("@Name", task.Name);
                        command.Parameters.AddWithValue("@Description", task.Description);
                        command.Parameters.AddWithValue("@Status", task.Status);
                        command.Parameters.AddWithValue("@Priority", task.Priority);
                        command.Parameters.AddWithValue("@DueDate", task.DueDate);
                        command.Parameters.AddWithValue("@CreatedDate", task.CreatedDate);
                        command.Parameters.AddWithValue("@CompletedDate", task.CompletedDate);
                        command.Parameters.AddWithValue("@ID", task.ID);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                }
            }
            public static TTask? GetTask(string id, string connectionString)
            {
                TTask? task = null;
                
               
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT * FROM Tasks WHERE ID = @ID";
                        command.Parameters.AddWithValue("@ID", id);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                task = new TTask{
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Description = reader["Description"]?.ToString() ?? string.Empty,
                                    Status = reader["Status"]?.ToString() ?? string.Empty,
                                    Priority = reader["Priority"]?.ToString() ?? string.Empty,
                                    DueDate = reader["DueDate"]?.ToString() ?? string.Empty,
                                    CreatedDate = reader["CreatedDate"]?.ToString() ?? string.Empty,
                                    CompletedDate = reader["CompletedDate"]?.ToString() ?? string.Empty,
                                    ID = reader["ID"]?.ToString() ?? string.Empty
                                };
                            
                            }
                        }
                    }
                    connection.Close();
                }

                return task;
            }

            public static List<TTask> GetTasks(string connectionString)
            {
                List<TTask> tasks = new List<TTask>();
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(connection))
                    {
                        command.CommandText = "SELECT * FROM Tasks";
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TTask task = new TTask{
                                    Name = reader["Name"]?.ToString() ?? string.Empty,
                                    Description = reader["Description"]?.ToString() ?? string.Empty,
                                    Status = reader["Status"]?.ToString() ?? string.Empty,
                                    Priority = reader["Priority"]?.ToString() ?? string.Empty,
                                    DueDate = reader["DueDate"]?.ToString() ?? string.Empty,
                                    CreatedDate = reader["CreatedDate"]?.ToString() ?? string.Empty,
                                    CompletedDate = reader["CompletedDate"]?.ToString() ?? string.Empty,
                                    ID = reader["ID"]?.ToString() ?? string.Empty
                                };
                                tasks.Add(task);
                            }
                        }
                    }
                    connection.Close();
                }
                return tasks;
            }


        }
}