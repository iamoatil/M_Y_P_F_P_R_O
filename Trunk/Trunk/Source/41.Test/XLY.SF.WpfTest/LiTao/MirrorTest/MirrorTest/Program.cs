using System;
using System.Windows;
using System.Windows.Media;
using XLY.SF.Project.Views.Mirror;

namespace MirrorTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MainWindow win = new MainWindow();
            win.ShowDialog();
        }
    }
}
