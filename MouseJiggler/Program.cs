#region header

// CoffeeWizz - Program.cs
// 
// Created by: Alistair J R Young (avatar) at 2021/01/22 4:12 PM.
// Forked by: xcy7e, Oct 2024

#endregion

#region using

using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Threading;
using System.Windows.Forms;

using Xcy7e.CoffeeWizz.Properties;

using JetBrains.Annotations;

using PInvoke;

#endregion

namespace Xcy7e.CoffeeWizz
{
    [PublicAPI]
    public static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        public static int Main (string[] args)
        {
            // Attach to the parent process's console so we can display help, version information, and command-line errors.
            Kernel32.AttachConsole (dwProcessId: Helpers.AttachParentProcess);

            // Ensure that we are the only instance of the Mouse Wizzler currently running.
            var instance = new Mutex (initiallyOwned: false, name: "single instance: Xcy7e.CoffeeWizz");

            try
            {
                if (instance.WaitOne (millisecondsTimeout: 0))

                    // Parse arguments and do the appropriate thing.
                {
                    return Program.GetCommandLineParser ().Invoke (args: args);
                }
                else
                {
                    Console.WriteLine (value: "CoffeeWizz is already running. Aborting.");

                    return 1;
                }
            }
            finally
            {
                instance.Close ();

                // Detach from the parent console.
                Kernel32.FreeConsole ();
            }
        }

        private static int RootHandler (bool wizzle, bool minimized, bool zen, int seconds)
        {
            // Prepare Windows Forms to run the application.
            Application.SetHighDpiMode (highDpiMode: HighDpiMode.SystemAware);
            Application.EnableVisualStyles ();
            Application.SetCompatibleTextRenderingDefault (defaultValue: false);

            // Run the application.
            var mainForm = new MainForm (wizzleOnStartup: wizzle,
                                         minimizeOnStartup: minimized,
                                         zenWizzleEnabled: zen,
                                         wizzlePeriod: seconds);

            Application.Run (mainForm: mainForm);

            return 0;
        }

        private static RootCommand GetCommandLineParser ()
        {
            // Create root command.
            var rootCommand = new RootCommand
                              {
                                  Description = "Virtually wizzles the mouse, making the computer seem not idle.",
                                  Handler =
                                      CommandHandler.Create (action: new Func<bool, bool, bool, int, int> (Program.RootHandler)),
                              };

            // -j --wizzle
            Option optWizzling = new (aliases: new[] {"--wizzle", "-j",}, description: "Start with wizzling enabled.");
            optWizzling.Argument = new Argument<bool> ();
            optWizzling.Argument.SetDefaultValue (value: false);
            rootCommand.AddOption (option: optWizzling);

            // -m --minimized
            Option optMinimized = new (aliases: new[] {"--minimized", "-m",}, description: "Start minimized.");
            optMinimized.Argument = new Argument<bool> ();
            optMinimized.Argument.SetDefaultValue (value: Settings.Default.MinimizeOnStartup);
            rootCommand.AddOption (option: optMinimized);

            // -z --zen
            Option optZen = new (aliases: new[] {"--zen", "-z",}, description: "Start with zen (invisible) wizzling enabled.");
            optZen.Argument = new Argument<bool> ();
            optZen.Argument.SetDefaultValue (value: Settings.Default.ZenWizzle);
            rootCommand.AddOption (option: optZen);

            // -s 60 --seconds=60
            Option optPeriod = new (aliases: new[] {"--seconds", "-s",},
                                    description: "Set number of seconds for the wizzle interval.");

            optPeriod.Argument = new Argument<int> ();

            optPeriod.AddValidator (validate: p => p.GetValueOrDefault<int> () < 1
                                                       ? "Period cannot be shorter than 1 second."
                                                       : null);

            optPeriod.AddValidator (validate: p => p.GetValueOrDefault<int> () > 60
                                                       ? "Period cannot be longer than 60 seconds."
                                                       : null);

            optPeriod.Argument.SetDefaultValue (value: Settings.Default.WizzlePeriod);
            rootCommand.AddOption (option: optPeriod);

            // Build the command line parser.
            return rootCommand;
        }
    }
}
