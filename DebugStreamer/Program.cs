using System;
using System.Diagnostics;
using System.Threading;

namespace DebugStreamer
{
    internal class Program
    {
        private static void Main()
        {
            Debug.WriteLine("Starting the program");
            Thread.Sleep(TimeSpan.FromSeconds(5.0));
            Debug.WriteLine("Doing something");
            Thread.Sleep(TimeSpan.FromSeconds(5.0));
            Debug.WriteLine("Doing something else");
            Thread.Sleep(TimeSpan.FromSeconds(5.0));
            Debug.WriteLine("Doing a third thing");
            Thread.Sleep(TimeSpan.FromSeconds(5.0));
            Debug.WriteLine("Program completed!");
        }
    }
}
