namespace XLY.SF.Project.DataMirrorApp
{
    class Program
    {
        static void Main(string[] args)
        {
            if(args.Length > 0
                && args[0] == "AndroidImg9008")
            {
                CommandPaser9008 cmder9008 = new CommandPaser9008();
                cmder9008.Run();
            }
            else
            {
                CommandPaser cmder = new CommandPaser();
                cmder.Run();
            }
        }
    }
}
