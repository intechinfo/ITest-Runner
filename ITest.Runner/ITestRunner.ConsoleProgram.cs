using System;
using System.IO;
using System.Reflection;

namespace ITest.Runner
{
    public class ConsoleProgram
    {
        public static void Main( string[] args )
        {
            var outputPath = args[0];
            Console.WriteLine( $"Output path = {outputPath}" );
            TestRoot.UnattendedRun( _ => new DefaultExecuteStrategy(true),  Assembly.GetExecutingAssembly() ).Save( outputPath );
        }

    }
}
