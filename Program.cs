using System;
using System.Windows.Forms;

namespace FileIntegrityChecker.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // if missing, see note below
            Application.Run(new Forms.MainForm());
        }
    }
}
