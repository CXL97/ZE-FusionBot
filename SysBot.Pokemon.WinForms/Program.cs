using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SysBot.Pokemon.WinForms;

static class Program
{
    public static readonly string WorkingDirectory = Environment.CurrentDirectory = Path.GetDirectoryName(Environment.ProcessPath)!;
    public static string ConfigPath { get; private set; } = Path.Combine(WorkingDirectory, "config.json");

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
#if NETCOREAPP
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
#endif
        var cmd = Environment.GetCommandLineArgs();
        var cfg = Array.Find(cmd, z => z.EndsWith(".json"));
        if (cfg != null)
            ConfigPath = cmd[0];

        // Enable visual styles and set compatible text rendering for the application on boot
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Create a Splash Screen and center it on the screen
        var splash = new SplashScreen();
        splash.StartPosition = FormStartPosition.Manual;
        splash.TopMost = true;
        int screenWidth = Screen.PrimaryScreen.Bounds.Width;
        int screenHeight = Screen.PrimaryScreen.Bounds.Height;
        int splashWidth = splash.Width;
        int splashHeight = splash.Height;
        int splashLeft = (screenWidth - splashWidth) / 2;
        int splashTop = (screenHeight - splashHeight) / 2;
        splash.Location = new Point(splashLeft, splashTop);
        splash.Show();
        var splashThread = new Thread(new ThreadStart(() =>
        {
        Thread.Sleep(2500); // How long in milliseconds the splash screen should be displayed
        splash.Invoke(new Action(() => splash.Close()));
        }));
        splashThread.Start();

        // Run the main form
        Application.Run(new Main());
        }
    }

