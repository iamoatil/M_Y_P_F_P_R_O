
using System;

namespace XLY.SF.Project.DataMirrorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Consoler consoler = new Consoler();
            consoler.ParseArgs(args);
            consoler.Run();
        }

    }
}
