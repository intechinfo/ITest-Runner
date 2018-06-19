using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Text;
using System.IO.Compression;

namespace ITest
{



    class Program
    {
        static string GetRequiredArgPath( string[] args, int idx, string description )
        {
            if( idx < args.Length )
            {
                string p = args[idx];
                if( p.Length > 0 ) return Path.GetFullPath( p );
            }
            DisplayUsage( $"Argument n°{idx} must be: {description}" );
            return null;
        }

        static int GetOptionIndex( string[] args, string option )
        {
            return Array.IndexOf( args, option );
        }

        static bool HasOption( string[] args, string option ) => GetOptionIndex( args, option ) >= 0;


        static int Main( string[] args )
        {
            var input = GetRequiredArgPath( args, 0, "The path to a zip or a solution directory." );
            if( input == null ) return -1;
            string output = GetRequiredArgPath( args, 1, "The output file path of the xml output." ); ;
            if( output == null ) return -1;
            bool isDebug = HasOption( args, "-debug" );
            bool noCopy = HasOption( args, "-nocopy" );

            string workPath;
            bool isZip = input.EndsWith( ".zip" );
            if( isZip )
            {
                if( !File.Exists( input ) )
                {
                    DisplayUsage( $"Unable to find .zip file: {input}" );
                    return -1;
                }
                workPath = Path.Combine( Path.GetTempPath(), "ITestRunner", Guid.NewGuid().ToString( "N" ) );
                ZipFile.ExtractToDirectory( input, workPath );
                noCopy = true;
            }
            else
            {
                if( !Directory.Exists( input ) )
                {
                    DisplayUsage( $"Unable to find directory: {input}" );
                    return -1;
                }
                workPath = input;
            }
            if( File.Exists( output ) )
            {
                DisplayUsage( $"Output file '{output}' exists." );
                return -1;
            }

            var r = Runner.SolutionRunner.Run( workPath, output, !noCopy, isDebug );
            return r.ExitCode;
        }

        static void DisplayUsage( string error = null )
        {
            if( error != null )
            {
                Console.WriteLine( "Error: " + error );
                Console.WriteLine( new String('-',20) );
            }
            Console.WriteLine( "Expected arguments: path-to-a-directory-or-a-zip-file output-file-path [-debug] [-nocopy]" );
            Console.WriteLine();
            Console.WriteLine( " -debug:  The solution will be executed in Debug mode. Default is Release." );
            Console.WriteLine( " -noCopy: The input solution folder will not be preserved by the execution." );
            Console.WriteLine();
            Console.WriteLine( "Example: \"C:\\Test\\The-Solution-Folder\" Result.xml -debug -nocopy" );
            Console.WriteLine();
            Console.WriteLine( "Note:" );
            Console.WriteLine( "- Both path-to-a-directory-or-a-zip-file and output-file-path can be absolute or relative to the current directory." );
            Console.WriteLine( "- The output file must not exist (its directory must exist)." );
        }

    }
}
