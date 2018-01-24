// Copyright 2013 Michael F. Collins, III

// This program will demonstrate how to capture the output from
// OutputDebugString and will write the output to standard output.

using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading;

namespace OutputDebugStringWriter
{
    internal class Program
    {
        private static void Main()
        {
            // To capture the output from OutputDebugString, we need a
            // shared memory buffer and two events.
            MemoryMappedFile memoryMappedFile = null;
            EventWaitHandle bufferReadyEvent = null;
            EventWaitHandle dataReadyEvent = null;
            try
            {
                memoryMappedFile = MemoryMappedFile.CreateNew("DBWIN_BUFFER", 4096L);

                // We try to create the events. If the events exist, we
                // will report an error and abort.
                bufferReadyEvent = new EventWaitHandle(
                    false,
                    EventResetMode.AutoReset,
                    "DBWIN_BUFFER_READY",
                    out var created);
                if (!created)
                {
                    Console.Error.WriteLine("The DBWIN_BUFFER_READY event exists.");
                    return;
                }

                dataReadyEvent = new EventWaitHandle(
                    false,
                    EventResetMode.AutoReset,
                    "DBWIN_DATA_READY",
                    out created);
                if (!created)
                {
                    Console.Error.WriteLine("The DBWIN_DATA_READY event exists.");
                    return;
                }

                // I am using a cancellation token to control the lifetime of
                // the program. You can terminate the program by pressing
                // CTRL+C and the handler that I defined will set the cancellation
                // token.
                var cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                Console.CancelKeyPress += (sender, e) =>
                {
                    cancellationTokenSource.Cancel();
                    e.Cancel = true;
                };

                // During the message processing loop, I want to check every
                // second to see if the user has pressed the CTRL+C key to end
                // the program.
                var timeout = TimeSpan.FromSeconds(1.0);
                bufferReadyEvent.Set();
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!dataReadyEvent.WaitOne(timeout))
                    {
                        continue;
                    }

                    using (var stream = memoryMappedFile.CreateViewStream())
                    {
                        using (var reader = new BinaryReader(stream, Encoding.Default))
                        {
                            var processId = reader.ReadUInt32();

                            // Because I specified the Encoding.Default object as the
                            // encoding for the BinaryReader object, the characters
                            // will be read out of memory as 8-bit octets and converted
                            // to Unicode characters by .NET.
                            var chars = reader.ReadChars(4092);

                            // The debug message will be null-terminated in memory,
                            // so I am looking for the null character in the character
                            // array to determine the bounds of the message to output.
                            var index = Array.IndexOf(chars, '\0');
                            var message = new string(chars, 0, index);
                            Console.Out.Write("{0}: {1}", processId, message);
                        }
                    }

                    // The message has been processed, so trigger the
                    // DBWIN_BUFFER_READY event in order to receive the next message
                    // from the process being debugged.
                    bufferReadyEvent.Set();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                Console.ReadLine();
            }
            finally
            {
                memoryMappedFile?.Dispose();
                bufferReadyEvent?.Dispose();
                dataReadyEvent?.Dispose();
            }
        }
    }
}
