using Terminal.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuvalu;
using Tuvalu.DB;
using Tuvalu.logger;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Data.Entity.Core.Common.EntitySql;
using System.Collections.ObjectModel;

namespace Tuvalu.tui
{
    public class Tui
    {
        public void Copy()
        {
            throw new NotImplementedException();
        }

        public void Cut()
        {
            throw new NotImplementedException();
        }

        public void Paste()
        {
            throw new NotImplementedException();
        }


        //create menubar takes in a dictonary containing the nemu item name and function that it should call
        public static Terminal.Gui.MenuBar CreateMenuBar(Dictionary<string, Action> menuItems)
        {
            var menu = new Terminal.Gui.MenuBar(new Terminal.Gui.MenuBarItem[] {
                new Terminal.Gui.MenuBarItem("_File", new Terminal.Gui.MenuItem[] {
                    new Terminal.Gui.MenuItem("_Quit", "", () => { Application.RequestStop(); })
                }),
                new Terminal.Gui.MenuBarItem("_Edit", new Terminal.Gui.MenuItem[] {
                    new Terminal.Gui.MenuItem("_Copy", "", menuItems["Copy"]),
                    new Terminal.Gui.MenuItem("C_ut", "", menuItems["Cut"]),
                    new Terminal.Gui.MenuItem("_Paste", "", menuItems["Paste"])
                })
            });
            return menu;
        }
        // Create a new top-level window
        public static Window CreateWindow(string title, int x, int y, int width, int height)
        {
            var win = new Window(title)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
            return win;
        }
        // create a new label
        public static Label CreateLabel(string text, int x, int y, int width, int height)
        {
            var label = new Label(text)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
            return label;
        }
        // create a new text field
        public static TextField CreateTextField(string text, int x, int y, int width, int height)
        {
            var textField = new TextField(text)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
            return textField;
        }
        // create a new button
        public static Button CreateButton(string text, int x, int y, int width, int height)
        {
            var button = new Button(text)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
            return button;
        }
        // create a new checkbox
        public static CheckBox CreateCheckBox(string text, int x, int y, int width, int height)
        {
            var checkBox = new CheckBox(text)
            {
                X = x,
                Y = y,
                Width = width,
                Height = height
            };
            return checkBox;
        }

        public static void Wmain(Tuvalu.Program.Data data)
        {
            Application.Init();
            var random = new Random();
            var top = Application.Top;

            var win = CreateWindow(Task_helper_lines.greetings(random.Next(0, 3)), 0, 1, top.Frame.Width, top.Frame.Height - 1);
            top.Add(win);

            var menuItems = new Dictionary<string, Action>
            {
                { "Copy", () => { MessageBox.ErrorQuery("Copy", "Copy", "Ok"); } },
                { "Cut", () => { MessageBox.ErrorQuery("Cut", "Cut", "Ok"); } },
                { "Paste", () => { MessageBox.ErrorQuery("Paste", "Paste", "Ok"); } }
            };
            var menu = CreateMenuBar(menuItems);
            top.Add(menu);

            // create the list pane on the left side of the window to hold the input fields

            var listPane = new FrameView("Task List")
            {
                X = 0,
                Y = 1,
                Width = Dim.Percent(30),
                Height = Dim.Fill()
            };
            win.Add(listPane);

            // create the input pane on the right side of the window to hold the input fields

            var inputPane = new FrameView("Task Details")
            {
                X = Pos.Right(listPane),
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.Add(inputPane);

            // create the list view to hold the tasks

            var listView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            listPane.Add(listView);

            // get the list of tasks from the database

            var DB = new DBconnector
            {
                DBPath = "Tasks.sqlite",
                Provider = "SQLite",
                DBConnectionString = "Data Source=Tasks.sqlite",
                DBName = "Tasks",
                DBType = "SQLite"
            };
            List<TTasks.TTask> tasks = new();

            if (DBconnector.DBExists(DB))
            {
                DB.DBCommand = "SELECT * FROM Tasks";
                tasks = DBconnector.GetTasks(DB);
            }
            var taskNames = tasks.Select(task => task.Name).ToList();
            listView.Source = new ListWrapper(taskNames);
            // create the input fields for the task details

            var nameLabel = CreateLabel("Name:", 1, 1, 10, 1);
            inputPane.Add(nameLabel);

            var nameField = CreateTextField("", 11, 1, 20, 1);
            inputPane.Add(nameField);

            var descriptionLabel = CreateLabel("Description:", 1, 3, 10, 1);
            inputPane.Add(descriptionLabel);

            var descriptionField = CreateTextField("", 11, 3, 20, 1);

            inputPane.Add(descriptionField);

            var statusLabel = CreateLabel("Status:", 1, 5, 10, 1);
            inputPane.Add(statusLabel);

            var statusField = CreateTextField("", 11, 5, 20, 1);

            inputPane.Add(statusField);

            var priorityLabel = CreateLabel("Priority:", 1, 7, 10, 1);
            inputPane.Add(priorityLabel);

            var priorityField = CreateTextField("", 11, 7, 20, 1);

            inputPane.Add(priorityField);

            var dueDateLabel = CreateLabel("Due Date:", 1, 9, 10, 1);

            inputPane.Add(dueDateLabel);

            var dueDateField = CreateTextField("", 11, 9, 20, 1);

            inputPane.Add(dueDateField);

            // create the buttons to add, update, and delete tasks

            var addButton = CreateButton("Add", 1, 11, 10, 1);

            inputPane.Add(addButton);

            var updateButton = CreateButton("Update", 12, 11, 10, 1);

            inputPane.Add(updateButton);

            var deleteButton = CreateButton("Delete", 23, 11, 10, 1);

            inputPane.Add(deleteButton);

            // whenever one of the list items is clicked, get the name and search for the task inside the DB
        listView.SelectedItemChanged += (args) =>
        {
            var selectedTaskName = taskNames[args.Item];
            TTasks.TTask selectedTask = tasks.FirstOrDefault(task => task.Name == selectedTaskName);

            if (selectedTask != null)
            {
                nameField.Text = selectedTask.Name;
                descriptionField.Text = selectedTask.Description;
                statusField.Text = selectedTask.Status;
                priorityField.Text = selectedTask.Priority.ToString();
                dueDateField.Text = selectedTask.DueDate.ToString();
            }
        };

        // if the task button for updating the task is clicked, use the ID to find the task and edit it
        updateButton.Clicked += () =>
        {
            var selectedTaskName = taskNames[listView.SelectedItem];
            TTasks.TTask selectedTask = tasks.FirstOrDefault(task => task.Name == selectedTaskName);

            if (selectedTask != null)
            {
                selectedTask.Name = nameField.Text.ToString(); // god i love getting null reference assignement errors. IT"S NOT FUCKING NULL YOU CUNT!
                selectedTask.Description = descriptionField.Text.ToString();
                selectedTask.Status = statusField.Text.ToString();
                selectedTask.Priority = priorityField.Text.ToString();
                selectedTask.DueDate = DateTime.Parse(dueDateField.Text.ToString()).ToString();

                DB.DBCommand = $"UPDATE Tasks SET Name = '{selectedTask.Name}', Description = '{selectedTask.Description}', Status = '{selectedTask.Status}', Priority = {selectedTask.Priority}, DueDate = '{selectedTask.DueDate}' WHERE ID = {selectedTask.ID}";
                DBconnector.ExecuteNonQuery(DB);

                taskNames[listView.SelectedItem] = selectedTask.Name;
                listView.SetNeedsDisplay();
            }
        };



            Application.Run();
        }
    }
}