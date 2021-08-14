using mlgui;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MathLib
{
    public class MLAPP
    {
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Splash.ShowSplashScreen();
            // Background Loading of Main ML Window
            MLGUI ml_mw = new MLGUI();   
            Thread.Sleep(100);
            // End of Loading
            Splash.CloseForm();

            // Force ML Window to Front
            ml_mw.WindowState = FormWindowState.Minimized;
            ml_mw.Show();
            ml_mw.WindowState = FormWindowState.Normal;

            // Begin ML Application
            Application.Run(ml_mw);
        }
    }
}
