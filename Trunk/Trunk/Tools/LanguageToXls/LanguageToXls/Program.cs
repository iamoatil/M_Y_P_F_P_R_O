using System;
using System.Windows;

namespace LanguageToXls
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                MainWindow window = new MainWindow();
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
