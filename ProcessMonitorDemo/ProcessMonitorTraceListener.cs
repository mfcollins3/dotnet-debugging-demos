using System.Diagnostics;

namespace ProcessMonitorDemo
{
    public class ProcessMonitorTraceListener : TraceListener
    {
        private readonly ProcessMonitor processMonitor;

        public ProcessMonitorTraceListener()
        {
            try
            {
                this.processMonitor = new ProcessMonitor();
            }
            catch
            {
                this.processMonitor = null;
            }
        }

        public override void Close()
        {
            processMonitor?.Dispose();
        }

        public override void Write(string message)
        {
            try
            {
                processMonitor?.WriteMessage(message);
            }
            catch
            {

            }
        }

        public override void WriteLine(string message)
        {
            try
            {
                processMonitor?.WriteMessage(message);
            }
            catch
            {

            }
        }
    }
}
