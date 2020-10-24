using System;
using System.Collections.Generic;
using System.Text;

namespace GillenwaterChordRev3
{
    // Responsible for outputting information from various sources.
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

        // Exportable interface for objects that write output
        public interface IWriter
        {
            public void Write(object obj);
        }

        // Writes output to the console using default formatting.
        // Should be used for writing UI to the console.
        class UiWriter : IWriter
        {
            public void Write(object obj)
            {
                Console.ResetColor();
                Console.WriteLine(obj.ToString());
            }
        }

        // Writes output to the console, with a prefacing red "server" label.
        // Should be used for writing debug messages from a node's server component.
        class ServerInfoWriter : IWriter
        {
            public void Write(object obj)
            {
                
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Server:\t\t");
                Console.ResetColor();
                Console.WriteLine(obj.ToString());
            }
        }

        // Writes output to the console, with a prefacing blue "client" label.
        // Should be used for writing debug messages from a node's client component.
        class ClientInfoWriter : IWriter
        {
            public void Write(object obj)
            {
                Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("Client:\t\t");
                Console.ResetColor();
                Console.WriteLine(obj.ToString());
            }
        }
    }

    

    
}
