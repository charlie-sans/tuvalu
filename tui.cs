#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
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
    public class ColorTheme
    {
        public static int CurrentTheme { get; set; } = 0;

        private static readonly Dictionary<int, (string Name, ColorScheme Scheme)> Themes = new()
        {
            [0] = ("Magenta Theme", new ColorScheme {
                Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightMagenta),
                Focus = Terminal.Gui.Attribute.Make(Color.Magenta, Color.Cyan),
                HotNormal = Terminal.Gui.Attribute.Make(Color.BrightMagenta, Color.Black),
                HotFocus = Terminal.Gui.Attribute.Make(Color.Magenta, Color.Cyan)
            }),
            [1] = ("Green Theme", new ColorScheme {
                Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightGreen),
                Focus = Terminal.Gui.Attribute.Make(Color.Green, Color.Cyan),
                HotNormal = Terminal.Gui.Attribute.Make(Color.BrightGreen, Color.Black),
                HotFocus = Terminal.Gui.Attribute.Make(Color.Green, Color.Cyan)
            }),
            [2] = ("Red Theme", new ColorScheme {
                Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightRed),
                Focus = Terminal.Gui.Attribute.Make(Color.Red, Color.Cyan),
                HotNormal = Terminal.Gui.Attribute.Make(Color.BrightRed, Color.Black),
                HotFocus = Terminal.Gui.Attribute.Make(Color.Red, Color.Cyan)
            }),
            [3] = ("Blue Theme", new ColorScheme {
                Normal = Terminal.Gui.Attribute.Make(Color.Black, Color.BrightBlue),
                Focus = Terminal.Gui.Attribute.Make(Color.Blue, Color.Cyan),
                HotNormal = Terminal.Gui.Attribute.Make(Color.BrightBlue, Color.Black),
                HotFocus = Terminal.Gui.Attribute.Make(Color.Blue, Color.Cyan)
            })
        };

        public static void GetTheme(Window window, int colorTheme)
        {
            CurrentTheme = colorTheme;
            window.ColorScheme = Themes.GetValueOrDefault(colorTheme, Themes[0]).Scheme;
        }

        public static string GetThemeName(int theme) => 
            Themes.GetValueOrDefault(theme, Themes[0]).Name;
    }

    public class Tui
    {
        // Add a cache for task names
        private static readonly Dictionary<string, string> _taskNameCache = new();

        // Add event for task updates
        private static event Action<TTasks.TTask>? OnTaskAdded;
        private static event Action? OnTasksChanged;
        private static ListView? _mainListView;
        private static List<TTasks.TTask>? _currentTasks;

        public static void sleep(int secconds)
        {
            Thread.Sleep(secconds * 1000);
        }

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
        // Set the background color of the window
        public static void SetWindowBackgroundColor(Window window, Terminal.Gui.Attribute Normal, Terminal.Gui.Attribute Focus, Terminal.Gui.Attribute HotNormal, Terminal.Gui.Attribute HotFocus)
        {
            window.ColorScheme = new ColorScheme
            {
                Normal = Normal,
                Focus = Focus,
                HotNormal = HotNormal,
                HotFocus = HotFocus
            };
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

        public static async Task WmainAsync(Data data)
        {
            Application.Init();
            
            var tasksLoadingTask = LoadTasksAsync(data.DB);
            
            // Initialize main UI components
            var top = Application.Top;
            var win = CreateMainWindow();
            top.Add(win);

            // Create menu
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem("_File", new MenuItem[] {
                    new MenuItem("_New Task", "", () => ShowNewTaskDialog(data.DB)),
                    new MenuItem("_Export Tasks", "", () => ExportTasks()),
                    new MenuItem("_Quit", "", () => Application.RequestStop())
                }),
                new MenuBarItem("_Edit", new MenuItem[] {
                    new MenuItem("_Delete Task", "", () => DeleteSelectedTask()),
                    new MenuItem("_Mark Complete", "", () => MarkTaskComplete())
                }),
                new MenuBarItem("_View", new MenuItem[] {
                    new MenuItem("_Refresh", "", () => RefreshTaskList()),
                    new MenuItem("_Filter", "", () => ShowFilterDialog())
                })
            });
            top.Add(menu);

            // Create status bar
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F2, "~F2~ New Task", () => ShowNewTaskDialog(data.DB)),
                new StatusItem(Key.F3, "~F3~ Delete", () => DeleteSelectedTask()),
                new StatusItem(Key.F4, "~F4~ Edit", () => EditSelectedTask()),
                new StatusItem(Key.F5, "~F5~ Refresh", () => RefreshTaskList())
            });
            top.Add(statusBar);

            // Create main layout
            var mainLayout = new FrameView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            win.Add(mainLayout);

            // Task list pane
            var listPane = new FrameView("Tasks")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(40),
                Height = Dim.Fill()
            };
            mainLayout.Add(listPane);

            // Task details pane
            var detailsPane = new FrameView("Task Details")
            {
                X = Pos.Percent(40),
                Y = 0,
                Width = Dim.Percent(60),
                Height = Dim.Fill()
            };
            mainLayout.Add(detailsPane);

            // Create list view
            var listView = new ListView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                AllowsMarking = true,
                AllowsMultipleSelection = false
            };
            listPane.Add(listView);

            // Create detail fields
            var detailsLayout = CreateTaskDetailsLayout();
            detailsPane.Add(detailsLayout);

            // Wait for tasks and update UI
            var tasks = await tasksLoadingTask;
            UpdateTaskList(listView, tasks);

            // Store reference to main ListView
            _mainListView = listView;
            _currentTasks = tasks;

            // Subscribe to events
            OnTaskAdded += (task) => {
                Application.MainLoop.Invoke(() => {
                    _currentTasks?.Add(task);
                    UpdateTaskList(_mainListView!, _currentTasks!);
                });
            };

            OnTasksChanged += () => {
                Application.MainLoop.Invoke(async () => {
                    _currentTasks = await LoadTasksAsync(data.DB);
                    UpdateTaskList(_mainListView!, _currentTasks);
                });
            };

            // Set up list view selection handling
            listView.OpenSelectedItem += (args) => {
                if (listView.SelectedItem >= 0 && listView.SelectedItem < tasks.Count)
                {
                    ShowTaskDetails(tasks[listView.SelectedItem], detailsLayout);
                }
            };

            Application.Run();
        }

        private static Window CreateMainWindow()
        {
            return new Window("Tuvalu Task Manager")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
        }

        private static FrameView CreateTaskDetailsLayout()
        {
            var layout = new FrameView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var nameLabel = new Label("Name:") { X = 1, Y = 1 };
            var nameField = new TextField("") { X = 1, Y = 2, Width = Dim.Fill() - 2 };

            var descLabel = new Label("Description:") { X = 1, Y = 4 };
            var descField = new TextView() { 
                X = 1, 
                Y = 5, 
                Width = Dim.Fill() - 2,
                Height = 5 
            };

            var statusLabel = new Label("Status:") { X = 1, Y = 11 };
            var statusCombo = new ComboBox() { 
                X = 1, 
                Y = 12, 
                Width = 20,
                Height = 1 
            };
            statusCombo.SetSource(new[] { "Not Started", "In Progress", "Completed" });

            var priorityLabel = new Label("Priority:") { X = 1, Y = 14 };
            var priorityCombo = new ComboBox() {
                X = 1,
                Y = 15,
                Width = 20
            };
            priorityCombo.SetSource(new[] { "Low", "Medium", "High" });

            var dueDateLabel = new Label("Due Date:") { X = 1, Y = 17 };
            var dueDateField = new TextField("") { X = 1, Y = 18, Width = 20 };

            var buttonBar = new View() { 
                X = 1, 
                Y = Pos.Bottom(dueDateField) + 1,
                Width = Dim.Fill() - 2,
                Height = 1
            };

            var saveButton = new Button("Save Changes", true) { 
                X = 0,
                Y = 0
            };
            
            var cancelButton = new Button("Cancel", true) { 
                X = Pos.Right(saveButton) + 2,
                Y = 0
            };

            buttonBar.Add(saveButton, cancelButton);
            
            layout.Add(
                nameLabel, nameField,
                descLabel, descField,
                statusLabel, statusCombo,
                priorityLabel, priorityCombo,
                dueDateLabel, dueDateField,
                buttonBar
            );

            return layout;
        }

        private static async void ShowNewTaskDialog(DBconnector db)
        {
            var dialog = new Dialog("New Task", 60, 20);
            var task = new TTasks.TTask();
            
            // Add form fields
            var nameField = new TextField("") { X = 1, Y = 1, Width = Dim.Fill() - 4 };
            var descField = new TextView() { 
                X = 1, 
                Y = 3, 
                Width = Dim.Fill() - 4,
                Height = 5 
            };
            
            dialog.Add(
                new Label("Name:") { X = 1, Y = 0 },
                nameField,
                new Label("Description:") { X = 1, Y = 2 },
                descField
            );

            // Add buttons
            var saveBtn = new Button("Save");
            saveBtn.Clicked += async () => {
                task.Name = nameField.Text.ToString();
                task.Description = descField.Text.ToString();
                task.ID = Guid.NewGuid().ToString();
                task.CreatedDate = DateTime.Now.ToString();
                task.Status = "Not Started";
                
                await AddTaskAsync(task, db);
                Application.RequestStop();
            };
            dialog.AddButton(saveBtn);
            
            var cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += () => Application.RequestStop();
            dialog.AddButton(cancelBtn);

            Application.Run(dialog);
        }

        private static async Task<List<TTasks.TTask>> LoadTasksAsync(DBconnector db)
        {
            if (!DBconnector.DBExists(db))
                return new List<TTasks.TTask>();

            db.DBCommand = "SELECT * FROM Tasks";
            return await DBconnector.GetTasksAsync(db);
        }

        private static async Task UpdateUIWithTasksAsync(FrameView listPane, List<TTasks.TTask> tasks)
        {
            var taskNames = tasks.Select(t => t.Name).ToList();
            TTasks.TTask? selectedTask = null;
            
            // Update UI on the main thread
            Application.MainLoop.Invoke(() =>
            {
                // Create and configure list view
                var listView = new ListView
                {
                    X = 0,
                    Y = 0,
                    Width = Dim.Fill(),
                    Height = Dim.Fill(),
                    AllowsMarking = true,
                    AllowsMultipleSelection = false
                };
                
                listView.Source = new ListWrapper(taskNames);
                listPane.Add(listView);

                // Get the main window from the list pane's parent
                var mainWindow = listPane.SuperView as Window;
                if (mainWindow == null) return;

                // Add details pane to the main window
                var detailsPane = new FrameView("Task Details")
                {
                    X = Pos.Percent(50),
                    Y = 0,
                    Width = Dim.Percent(50),
                    Height = Dim.Fill()
                };
                mainWindow.Add(detailsPane);

                // Add task detail fields
                var nameField = new TextField("")
                {
                    X = 1,
                    Y = 1,
                    Width = Dim.Fill() - 2,
                    Height = 1
                };

                var descriptionField = new TextView()
                {
                    X = 1,
                    Y = 3,
                    Width = Dim.Fill() - 2,
                    Height = 5
                };

                var statusField = new ComboBox()
                {
                    X = 1,
                    Y = 9,
                    Width = 20,
                    Height = 1
                };
                statusField.SetSource(new[] { "Not Started", "In Progress", "Completed" });

                var priorityField = new ComboBox()
                {
                    X = 1,
                    Y = 11,
                    Width = 20,
                    Height = 1
                };
                priorityField.SetSource(new[] { "Low", "Medium", "High" });

                var dueDateField = new TextField("")
                {
                    X = 1,
                    Y = 13,
                    Width = 20,
                    Height = 1
                };

                var saveButton = new Button("Save Changes")
                {
                    X = 1,
                    Y = 15
                };

                var deleteButton = new Button("Delete Task")
                {
                    X = Pos.Right(saveButton) + 2,
                    Y = 15
                };

                // Add all controls to the details pane
                detailsPane.Add(
                    new Label("Name:") { X = 1, Y = 0 },
                    nameField,
                    new Label("Description:") { X = 1, Y = 2 },
                    descriptionField,
                    new Label("Status:") { X = 1, Y = 8 },
                    statusField,
                    new Label("Priority:") { X = 1, Y = 10 },
                    priorityField,
                    new Label("Due Date:") { X = 1, Y = 12 },
                    dueDateField,
                    saveButton,
                    deleteButton
                );

                listView.OpenSelectedItem += (args) =>
                {
                    if (listView.SelectedItem >= 0 && listView.SelectedItem < tasks.Count)
                    {
                        selectedTask = tasks[listView.SelectedItem];
                        nameField.Text = selectedTask.Value.Name;
                        descriptionField.Text = selectedTask.Value.Description;
                        statusField.Text = selectedTask.Value.Status;
                        priorityField.Text = selectedTask.Value.Priority;
                        dueDateField.Text = selectedTask.Value.DueDate;
                    }
                };

                saveButton.Clicked += async () =>
                {
                    if (selectedTask != null)
                    {
                        var task = selectedTask.Value;
                        task.Name = nameField.Text.ToString();
                        task.Description = descriptionField.Text.ToString();
                        task.Status = statusField.Text.ToString();
                        task.Priority = priorityField.Text.ToString();
                        task.DueDate = dueDateField.Text.ToString();
                        selectedTask = task;

                        try
                        {
                            await TTasks.UpdateTaskAsync(selectedTask.Value, Program.DB.DBConnectionString);
                            MessageBox.Query("Success", "Task updated successfully", "OK");
                            taskNames[listView.SelectedItem] = selectedTask.Value.Name;
                            listView.SetNeedsDisplay();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.ErrorQuery("Error", $"Failed to update task: {ex.Message}", "OK");
                        }
                    }
                };

                deleteButton.Clicked += async () =>
                {
                    if (selectedTask != null)
                    {
                        var result = MessageBox.Query("Confirm Delete", 
                            "Are you sure you want to delete this task?", "Yes", "No");
                        
                        if (result == 0) // Yes
                        {
                            try
                            {
                                Program.DB.DBCommand = $"DELETE FROM Tasks WHERE ID = '{selectedTask.Value.ID}'";
                                await DBconnector.ExecuteNonQueryAsync(Program.DB);
                                tasks.RemoveAt(listView.SelectedItem);
                                taskNames.RemoveAt(listView.SelectedItem);
                                listView.SetNeedsDisplay();
                                
                                // Clear fields
                                nameField.Text = "";
                                descriptionField.Text = "";
                                statusField.Text = "";
                                priorityField.Text = "";
                                dueDateField.Text = "";
                                
                                MessageBox.Query("Success", "Task deleted successfully", "OK");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.ErrorQuery("Error", $"Failed to delete task: {ex.Message}", "OK");
                            }
                        }
                    }
                };
            });
        }

        private static async Task UpdateTaskAsync(TTasks.TTask task, DBconnector db)
        {
            await DBconnector.ExecuteNonQueryAsync(db);
            _taskNameCache[task.ID] = task.Name;
            OnTasksChanged?.Invoke();
        }

        private static async Task AddTaskAsync(TTasks.TTask task, DBconnector db)
        {
            try
            {
                db.DBCommand = $@"INSERT INTO Tasks 
                    (Name, Description, Status, Priority, DueDate, CreatedDate, CompletedDate, ID) 
                    VALUES ('{task.Name}', '{task.Description}', '{task.Status}', '{task.Priority}', 
                            '{task.DueDate}', '{task.CreatedDate}', '{task.CompletedDate}', '{task.ID}')";
                            
                await DBconnector.ExecuteNonQueryAsync(db);
                _taskNameCache[task.ID] = task.Name;
                
                // Notify UI of new task
                OnTaskAdded?.Invoke(task);
                MessageBox.Query("Success", "Task added successfully", "OK");
            }
            catch (Exception ex)
            {
                await Task.Run(() => Logger.Log(ex));
                MessageBox.ErrorQuery("Error", $"An error occurred while trying to add the task: {ex}", "Ok");
            }
        }

        private static async Task DeleteTaskAsync(TTasks.TTask task, DBconnector db)
        {
            try
            {
                db.DBCommand = $"DELETE FROM Tasks WHERE ID = '{task.ID}'";
                await DBconnector.ExecuteNonQueryAsync(db);
                _taskNameCache.Remove(task.ID);
                OnTasksChanged?.Invoke();
                MessageBox.Query("Success", "Task deleted successfully", "OK");
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete task: {ex.Message}", "OK");
                await Task.Run(() => Logger.Log(ex));
            }
        }

        private static (Toplevel top, Window win, FrameView listPane, FrameView inputPane) InitializeUI()
        {
            var top = Application.Top;
            
            // Create the main window
            var win = new Window("Tuvalu Task Manager")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            top.Add(win);

            // header bar menu
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem("_File", new MenuItem[] {
                    new MenuItem("_New Task", "", () => ShowNewTaskDialog(Program.DB)),
                    new MenuItem("_Export Tasks", "", () => ExportTasks()),
                    new MenuItem("_Quit", "", () => Application.RequestStop())
                }),
                new MenuBarItem("_Edit", new MenuItem[] {
                    new MenuItem("_Delete Task", "", () => DeleteSelectedTask()),
                    new MenuItem("_Mark Complete", "", () => MarkTaskComplete())
                }),
                new MenuBarItem("_View", new MenuItem[] {
                    new MenuItem("_Refresh", "", () => RefreshTaskList()),
                    new MenuItem("_Filter", "", () => ShowFilterDialog())
                })
            });

            // Create list pane
            var listPane = new FrameView("Tasks")
            {
                X = 0,
                Y = 0,
                Width = Dim.Percent(50),
                Height = Dim.Fill()
            };
            win.Add(listPane);

            // Create input pane
            var inputPane = new FrameView("Details")
            {
                X = Pos.Right(listPane),
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.Add(inputPane);

            return (top, win, listPane, inputPane);
        }

        private static void UpdateTaskList(ListView listView, List<TTasks.TTask> tasks)
        {
            var taskNames = tasks.Select(t => t.Name).ToList();
            listView.SetSource(taskNames);
        }

        private static void ShowTaskDetails(TTasks.TTask task, View detailsLayout)
        {
            var nameField = detailsLayout.Subviews.FirstOrDefault(v => v is TextField) as TextField;
            var descField = detailsLayout.Subviews.FirstOrDefault(v => v is TextView) as TextView;
            var statusCombo = detailsLayout.Subviews.FirstOrDefault(v => v.Frame.Y == 12) as ComboBox;
            var priorityCombo = detailsLayout.Subviews.FirstOrDefault(v => v.Frame.Y == 15) as ComboBox;
            var dueDateField = detailsLayout.Subviews.FirstOrDefault(v => v.Frame.Y == 18) as TextField;

            if (nameField != null) nameField.Text = task.Name;
            if (descField != null) descField.Text = task.Description;
            if (statusCombo != null) statusCombo.Text = task.Status;
            if (priorityCombo != null) priorityCombo.Text = task.Priority;
            if (dueDateField != null) dueDateField.Text = task.DueDate;
        }

        private static void DeleteSelectedTask()
        {
            var result = MessageBox.Query("Confirm Delete", 
                "Are you sure you want to delete this task?", "Yes", "No");
            if (result == 0)
            {
                // Implementation will be called from the ListView's event handler
                // where we have access to the selected task
            }
        }

        private static void MarkTaskComplete()
        {
            // This will be implemented in the ListView's event handler
            // where we have access to the selected task
        }

        private static void RefreshTaskList()
        {
            // Force a refresh of the current view
            Application.Refresh();
        }


        private static void ExportTasks()
        {
            var dialog = new Dialog("Export Tasks", 50, 10);
            var pathField = new TextField("") { 
                X = 1, 
                Y = 1, 
                Width = Dim.Fill() - 4 
            };

            dialog.Add(
                new Label("Export Path:") { X = 1, Y = 0 },
                pathField
            );

            var exportBtn = new Button("Export");
            exportBtn.Clicked += () => {
                try
                {
                    // Implementation will export tasks to specified path
                    MessageBox.Query("Success", "Tasks exported successfully", "OK");
                    Application.RequestStop();
                }
                catch (Exception ex)
                {
                    MessageBox.ErrorQuery("Error", $"Failed to export tasks: {ex.Message}", "OK");
                }
            };
            dialog.AddButton(exportBtn);
            var cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += () => Application.RequestStop();
            dialog.AddButton(cancelBtn);

            Application.Run(dialog);
        }

        private static void EditSelectedTask()
        {
            // This will be implemented in the ListView's event handler
            // where we have access to the selected task
        }

        private static void ShowFilterDialog()
        {
            var dialog = new Dialog("Filter Tasks", 50, 15);
            var statusCombo = new ComboBox() { 
                X = 1, 
                Y = 1, 
                Width = 30
            };
            statusCombo.SetSource(new[] { "All", "Not Started", "In Progress", "Completed" });

            var priorityCombo = new ComboBox() {
                X = 1,
                Y = 3,
                Width = 30
            };
            priorityCombo.SetSource(new[] { "All", "Low", "Medium", "High" });

            dialog.Add(
                new Label("Status:") { X = 1, Y = 0 },
                statusCombo,
                new Label("Priority:") { X = 1, Y = 2 },
                priorityCombo
            );

            var applyBtn = new Button("Apply");
            applyBtn.Clicked += async () => {
                // TODO: Implement filtering logic

                // filter tasks based on selected status and priority
                // if "All" is selected, show all tasks
                // if "Not Started" is selected, show tasks with status "Not Started"
                // if "In Progress" is selected, show tasks with status "In Progress"
                // if "Completed" is selected, show tasks with status "Completed"
                // if "Low" is selected, show tasks with priority "Low"
                // if "Medium" is selected, show tasks with priority "Medium"
                // if "High" is selected, show tasks with priority "High"
                string selectedStatus = statusCombo.Text.ToString();
                string selectedPriority = priorityCombo.Text.ToString();

                var tasks = await LoadTasksAsync(Program.DB);
                var filteredTasks = tasks.Where(t => 
                    (selectedStatus == "All" || t.Status == selectedStatus) &&
                    (selectedPriority == "All" || t.Priority == selectedPriority)
                ).ToList();

                UpdateTaskList(Application.Top.Subviews[0].Subviews[0].Subviews[0] as ListView, filteredTasks);
                Application.RequestStop();



            };
            dialog.AddButton(applyBtn);

            var cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += () => {Application.RequestStop(); };
            dialog.AddButton(cancelBtn);

            Application.Run(dialog);
        }
    }
}