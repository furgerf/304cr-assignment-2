using System;
using System.Windows.Forms;
using Ai2dShooter.View;

namespace Ai2dShooter.Common
{
    /********
     * TODO *
     ********
     * 
     * let DT player use DT
     * fix graphics bug after game terminates
     * add influence map
     * fix movement -^
     * fix sound FX stuff
     */ 

    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
