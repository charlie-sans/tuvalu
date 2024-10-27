using Terminal.Gui;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuvalu.Tasks;
using Tuvalu.DB;
using Tuvalu.logger;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace Tuvalu.tui
{
    public class Tui
    {
        public static Terminal.Gui.MenuBar CreateMenuBar()
        {
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_Quit", "", () => { Application.RequestStop (); })
                }),
                new MenuBarItem ("_Edit", new MenuItem [] {
                    new MenuItem ("_Copy", "", null),
                    new MenuItem ("C_ut", "", null),
                    new MenuItem ("_Paste", "", null)
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
        
        public static void Wmain()
        {
            
    }
    }
}