using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITest.Runner.Tests
{
    public class SUTHelper
    {
        static string ThisFilePath( [CallerFilePath] string p = null ) => p;

        static string TestsFolderPath => Path.GetDirectoryName( Path.GetDirectoryName( ThisFilePath() ) );

        public static void ExternalConfigure( string testKey, bool success, int delayMS )
        {
            var f = Path.Combine( TestsFolderPath, "Solutions", "ExternallyConfigured.config.xml" );
            XDocument d = File.Exists( f )
                            ? XDocument.Load( f )
                            : new XDocument( new XElement( "Root" ) );
            var e = d.Root.Element( testKey );
            if( e == null ) d.Root.Add( e = new XElement( testKey ) );
            e.SetAttributeValue( "Success", success );
            e.SetAttributeValue( "DelayMS", delayMS );
            d.Save( f );
        }

        public static string GetTestSolutionPath( string solutionName )
        {
            return Path.Combine( TestsFolderPath, "Solutions", solutionName );
        }

        public static string GetResultFilePath( string testName )
        {
            var pResults = Path.Combine( TestsFolderPath, "ITest.Runner.Tests", "Results" );
            Directory.CreateDirectory( pResults );
            var p = Path.Combine( pResults, testName + ".xml" );
            return p;
        }

        public static string GetCleanResultFilePath( string testName )
        {
            var p = GetResultFilePath( testName );
            if( File.Exists( p ) ) File.Delete( p );
            return p;
        }

    }
}
