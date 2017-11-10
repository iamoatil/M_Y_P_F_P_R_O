using System;

namespace MirrorTest
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {

            CmdMainWindow win = new CmdMainWindow();
            win.ShowDialog();

            //WcfMainWindow win2 = new WcfMainWindow();
            //win2.ShowDialog();
        }
    }
}
