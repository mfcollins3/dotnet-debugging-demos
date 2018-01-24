using System;
using System.Diagnostics;

namespace DebugWriter
{
    internal class Program
    {
        private static void Main()
        {
            Debug.WriteLine("Debuggee is running; waiting for input");
            Console.Out.WriteLine("Type a message and press ENTER. Enter a blank line to terminate");
            while (true)
            {
                var line = Console.In.ReadLine();
                if (string.Empty == line)
                {
                    break;
                }

                Debug.WriteLine(line);
            }

            Debug.WriteLine("Debuggee is completed");
        }
    }
}
