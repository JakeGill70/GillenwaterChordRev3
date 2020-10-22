using System;
using System.Collections.Generic;
using System.Text;

namespace GillenwaterChordRev3
{
    public class OutputManager
    {
        protected static ConsoleColor defaultBg = ConsoleColor.Black;
        protected static ConsoleColor defaultFg = ConsoleColor.White;

        public static IWriter Ui = new UiWriter();
        public static IWriter Server = new ServerInfoWriter();
        public static IWriter Client = new ClientInfoWriter();

        public static void UpdateDefaults()
        {
            defaultBg = ConsoleColor.Black;
            defaultFg = ConsoleColor.White;
        }

        public interface IWriter
        {
            public void Write(object obj);
        }

        class UiWriter : IWriter
        {
            public void Write(object obj)
            {
                Console.ForegroundColor = OutputManager.defaultFg;
                Console.BackgroundColor = OutputManager.defaultBg;
                Console.WriteLine(obj.ToString());
            }
        }

        class ServerInfoWriter : IWriter
        {
            public void Write(object obj)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Server:\t\t");
                Console.BackgroundColor = OutputManager.defaultBg;
                Console.WriteLine(obj.ToString());
            }
        }

        class ClientInfoWriter : IWriter
        {
            public void Write(object obj)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Client:\t\t");
                Console.BackgroundColor = OutputManager.defaultBg;
                Console.WriteLine(obj.ToString());
            }
        }
    }

    

    
}
