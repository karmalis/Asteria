using System;

namespace Asteria
{
    class Program
    {
        static void Main(string[] args)
        {
            CmdParser cmdParser = new CmdParser();
            cmdParser.AddOption("in", "The input file", true, true);
            cmdParser.AddOption("out", "The output file", true, true);
            cmdParser.AddOption("cnum", "Characters per line", true, true);

            if (!cmdParser.ParseArgs(args))
            {
                cmdParser.PrintErrors();
                cmdParser.PrintHelp();
                return;
            }

            try
            {
                Processor processor = new Processor(cmdParser.GetArg("in"), cmdParser.GetArg("out"), cmdParser.GetArg("cnum"));
                processor.Run();
            } catch (Exception exception)
            {
                Console.WriteLine("Program has encountered an exception: " + exception.Message);
            }
        }
    }
}
