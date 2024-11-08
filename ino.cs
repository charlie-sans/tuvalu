using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Tuvalu;
using Tuvalu.DB;
using Tuvalu.logger;
using Tuvalu.tui;

namespace Tuvalu
{
    public class ino
    {
        public static string welcome = $"Welcome to Tuvalu, a simple task manager for the terminal\n\n" +
            $"This program is of version {Globals.Version} with development starting early 2024\n" + 
            "thanks for using this program! it means the world to us at Finite and OpenStudio!\n\n" +   
            "before we begin, i [ivy] wish to get a bit more information before tuvalu starts.\n " +      
            "[Note]: this program is open source, if this program was sold to you or you bought it from somewhere\n" +        
            "i would highly recommend to get a refund as that's not something that should happen\n" +
            "this program will always be free\n\n" +
            "thanks for reading, now let's get started!\n\n";
        public static string user = "in this screen, all we need is some basic infomation relating to who you are.\n not too much, all it is is your name, pronouns, stuff like that. \n";

        public static string setup_complete = "Setup complete, starting Tuvalu\n";
    }
}