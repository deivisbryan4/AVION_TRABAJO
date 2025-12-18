using System;
using System.Windows.Forms;

namespace JuegoDeAvion
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Menu_Principal()); // Iniciar desde el menú
        }
    }
}