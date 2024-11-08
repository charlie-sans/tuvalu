using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Tuvalu;
using Tuvalu.logger;
using Terminal.Gui;
using Tuvalu.tui;
using Tuvalu.DB;
using System.Threading.Tasks;

namespace Tuvalu
{
    public class Misc
    {
        public static async Task ConfigureAsync(Data data)
        {
            string json = data.json_config;
            var jsonObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            bool is_setup = jsonObject != null && jsonObject.ContainsKey("is_setup");
            
            if (is_setup)
            {
                await Console.Out.WriteLineAsync("Configuration already done");
                await Tui.WmainAsync(data);
            }
            else
            {
                await Console.Out.WriteLineAsync("Configuration not done");
                await Console.Out.WriteLineAsync("Starting configuration");

                await Task.Run(() => setup_screen());

                try
                {
                    string new_json = JsonConvert.SerializeObject(jsonObject);
                    await File.WriteAllTextAsync(Globals.AppConfigFile, new_json);
                    await Console.Out.WriteLineAsync("Configuration complete");
                    await Tui.WmainAsync(data);
                }
                catch (Exception ex)
                {
                    await Console.Out.WriteLineAsync($"An unexpected error occurred: {ex.Message}");
                    await Task.Run(() => Logger.Log(ex));
                }
            }
        }
        public static void setup_screen()
        {
            Application.Init();

            var wizard = new Wizard("Setup Wizard");

            // Add 1st step
            var firstStep = new Wizard.WizardStep("Tuvalu Setup Wizard");
            wizard.AddStep(firstStep);
            firstStep.NextButtonText = "Accept!";
            firstStep.HelpText = "Welcome to the Tuvalu Setup Wizard!";

            // Add 2nd step
            var secondStep = new Wizard.WizardStep("User Information");
            wizard.AddStep(secondStep);
            secondStep.HelpText = "Please enter your user information.";

            var name_lbl = new Label("Name:") { X = 1, Y = 1 };
            var name_field = new TextField("") { X = Pos.Right(name_lbl) + 1, Y = Pos.Top(name_lbl), Width = 40 };
            var pronouns_lbl = new Label("Pronouns:") { X = 1, Y = Pos.Bottom(name_lbl) + 1 };
            var pronouns_field = new TextField("") { X = Pos.Right(pronouns_lbl) + 1, Y = Pos.Top(pronouns_lbl), Width = 40 };
            var theme_lbl = new Label("Theme:") { X = 1, Y = Pos.Bottom(pronouns_lbl) + 1 };
            var theme_field = new ListView(new string[] { "Light", "Dark" }) { X = Pos.Right(theme_lbl) + 1, Y = Pos.Top(theme_lbl), Width = 40, Height = 2 };

            secondStep.Add(name_lbl, name_field, pronouns_lbl, pronouns_field, theme_lbl, theme_field);

            // Add 3rd step
            var thirdStep = new Wizard.WizardStep("Setup Complete");
            wizard.AddStep(thirdStep);
            thirdStep.HelpText = "Setup is complete! Thank you for configuring Tuvalu.";

            wizard.Finished += (args) =>
            {
                MessageBox.Query("Wizard", "Thanks for using Tuvalu! just start the app again and you should have the setup complete! If you have any issues, please make an issue at https://github.com/charlie-sans/tuvalu", "Ok");
                Application.RequestStop();
            };

            Application.Top.Add(wizard);
            Application.Run();
            Application.Shutdown();
            

        }
    }
}