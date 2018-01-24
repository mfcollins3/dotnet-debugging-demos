using System.Threading;

namespace ProcessMonitorDemo
{
    internal class Program
    {
        private static void Main()
        {
            using (var procmon = new ProcessMonitor())
            {
                procmon.WriteMessage("The program is starting.");
                Thread.Sleep(5000);
                procmon.WriteMessage("The program is running.");
                Thread.Sleep(5000);
                procmon.WriteMessage("The program is ending.");
            }
        }
    }
}
