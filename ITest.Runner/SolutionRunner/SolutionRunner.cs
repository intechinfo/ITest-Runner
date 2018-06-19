using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    /// <summary>
    /// Helper that processes a Solution folder that must have one and only one .Tests folder
    /// and generates the Xml of the tests run.
    /// </summary>
    public static class SolutionRunner
    {
        /// <summary>
        /// Runs the unit tests in the given solution, by default preserving the original solution
        /// folder in a copy.
        /// </summary>
        /// <param name="workSolutionPath">The original solution folder.</param>
        /// <param name="outputXmlFilePath">The xml file path to output the results.</param>
        /// <param name="preserveSolutionFolder">
        /// False to direclty work in the <paramref name="workSolutionPath"/>. This MUST be a copy
        /// because this process modifies the solution.
        /// </param>
        /// <param name="debugBuild">True to run in Debug mode instead of Release.</param>
        /// <returns>The operation result. Never null.</returns>
        public static SolutionRunResult Run( string workSolutionPath, string outputXmlFilePath, bool preserveSolutionFolder = true, bool debugBuild = false )
        {
            if( String.IsNullOrWhiteSpace( outputXmlFilePath )
                || outputXmlFilePath.Any( c => Path.GetInvalidPathChars().Contains( c ) ) )
            {
                return new SolutionRunResult( $"Invalid output file path '{outputXmlFilePath}': it must not exist and be a valid file path." );
            }
            if( File.Exists( outputXmlFilePath ) )
            {
                return new SolutionRunResult( $"Output file '{outputXmlFilePath}' exists. It must not exist." );
            }
            if( preserveSolutionFolder )
            {
                string safeWorkPath = null;
                try
                {
                    safeWorkPath = Path.Combine( Path.GetTempPath(), "ITestRunner", Guid.NewGuid().ToString( "N" ) );
                    CopyDirectory( new DirectoryInfo( workSolutionPath ), new DirectoryInfo( safeWorkPath ),
                                    withHiddenFiles: false,
                                    withHiddenFolders: false,
                                    fileFilter: null,
                                    dirFilter: d => d.Name != "obj" && d.Name != "bin" );
                    workSolutionPath = safeWorkPath;
                }
                catch( Exception ex )
                {
                    return new SolutionRunResult( $"Error while copying folder '{workSolutionPath}' to '{safeWorkPath}': {ex}" );
                }
            }
            var testDirectories = Directory.GetDirectories( workSolutionPath, "*.Tests" );
            if( testDirectories.Length != 1 )
            {
                return new SolutionRunResult( $"There must be one and only one .Tests directory in '{workSolutionPath}'." );
            }
            var testProjectPath = testDirectories[0];
            var testProjectName = Path.GetFileName( testProjectPath );
            var csProjPath = Path.Combine( testProjectPath, testProjectName + ".csproj" );

            var project = ModifyProjectFile( csProjPath );
            if( project.Error != null ) return new SolutionRunResult( project.Error );

            try
            {
                // Copy source code runner files (with the expected Main).
                var a = typeof( SolutionRunner).Assembly;
                var prefix = a.GetName().Name + ".Embedded.";
                foreach( var source in a.GetManifestResourceNames().Where( name => name.StartsWith( prefix ) ) )
                {
                    using( var s = a.GetManifestResourceStream( source ) )
                    using( var target = File.OpenWrite( Path.Combine( testProjectPath, source ) ) )
                    {
                        s.CopyTo( target );
                    }
                }

                // If a Properties/launchSettings.json file exists, removes it.
                var launchSettings = Path.Combine( testProjectPath, "Properties", "launchSettings.json" );
                if( File.Exists( launchSettings ) ) File.Delete( launchSettings );
                var r = DotNetRun( testProjectPath, debugBuild, project.TargetFramework, outputXmlFilePath );
                return new SolutionRunResult( r.ExitCode, r.StdErr, r.StdOut, outputXmlFilePath );
            }
            catch( Exception ex )
            {
                return new SolutionRunResult( $"Error while running: {ex.Message}" );
            }
        }

        struct ModifyProjectFileResult
        {
            public readonly string Error;
            public readonly string TargetFramework;

            public ModifyProjectFileResult( string err, string targetFramework )
            {
                Error = err;
                TargetFramework = targetFramework;
            }
        }

        static ModifyProjectFileResult ModifyProjectFile( string csProjPath )
        {
            if( !File.Exists( csProjPath ) )
            {
                return new ModifyProjectFileResult( $"Unable to find file '{csProjPath}'.", null);
            }
            try
            {
                XDocument d = XDocument.Load( csProjPath );
                // Ensures that OutputType is exe.
                var firstPropertyGroup = d.Root.Element( "PropertyGroup" );
                firstPropertyGroup.Add( new XElement( "OutputType", "exe" ) );
                // Extracts the first TargetFramework.
                var targetFramework = (string)firstPropertyGroup.Element( "TargetFramework" );
                if( targetFramework == null )
                {
                    string[] t = firstPropertyGroup.Element( "TargetFrameworks" )?.Value?.Split( ';' );
                    if( t == null || t.Length == 0 || String.IsNullOrWhiteSpace( targetFramework = t[0] ) )
                    {
                        return new ModifyProjectFileResult( $"Unable to find TargetFramework or TargetFrameworks element in first <PropertyGroup> of '{csProjPath}'.", null);
                    }
                }
                // Required for code source injection (uses private protected).
                d.Root.Elements( "PropertyGroup" ).Elements( "LangVersion" ).Remove();
                firstPropertyGroup.Add( new XElement( "LangVersion", "7.2" ) );
                d.Save( csProjPath );
                return new ModifyProjectFileResult( null, targetFramework);
            }
            catch( Exception ex )
            {
                return new ModifyProjectFileResult( $"Error while modifying '{csProjPath}': {ex.Message}", null);
            }
        }

        struct DotNetRunResult
        {
            public readonly int ExitCode;
            public readonly string StdOut;
            public readonly string StdErr;

            public DotNetRunResult( int exitCode, string stdOut, string stdErr )
            {
                ExitCode = exitCode;
                StdOut = stdOut;
                StdErr = stdErr;
            }
        }

        static DotNetRunResult DotNetRun( string projectPath, bool debugBuild, string framework, string args )
        {
            var pI = new ProcessStartInfo()
            {
                WorkingDirectory = projectPath,
                FileName = "dotnet",
                Arguments = $"run -c {(debugBuild ? "Debug" : "Release")} -f {framework} -- {args} ",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using( Process cmdProcess = new Process() )
            {
                StringBuilder bErr = new StringBuilder();
                StringBuilder bOut = new StringBuilder();
                cmdProcess.StartInfo = pI;
                cmdProcess.ErrorDataReceived += ( o, e ) => { if( !string.IsNullOrEmpty( e.Data ) ) bErr.AppendLine( e.Data ); };
                cmdProcess.OutputDataReceived += ( o, e ) => { if( e.Data != null ) bOut.AppendLine( e.Data ); };
                cmdProcess.Start();
                cmdProcess.BeginErrorReadLine();
                cmdProcess.BeginOutputReadLine();
                cmdProcess.WaitForExit();
                return new DotNetRunResult( cmdProcess.ExitCode, bOut.ToString(), bErr.ToString());
            }
        }

        /// <summary>
        /// Recursively copy a directory, creates it if it does not already exists. 
        /// Throws an IOException, if a same file exists in the target directory.
        /// </summary>
        /// <param name="src">The source directory.</param>
        /// <param name="target">The target directory.</param>
        /// <param name="withHiddenFiles">False to skip hidden files.</param>
        /// <param name="withHiddenFolders">False to skip hidden folders.</param>
        /// <param name="fileFilter">Optional predicate for directories.</param>
        /// <param name="dirFilter">Optional predicate for files.</param>
        public static void CopyDirectory( DirectoryInfo src, DirectoryInfo target, bool withHiddenFiles = true, bool withHiddenFolders = true, Func<FileInfo, bool> fileFilter = null, Func<DirectoryInfo, bool> dirFilter = null )
        {
            if( src == null ) throw new ArgumentNullException( "src" );
            if( target == null ) throw new ArgumentNullException( "target" );
            if( !target.Exists ) target.Create();
            DirectoryInfo[] dirs = src.GetDirectories();
            foreach( DirectoryInfo d in dirs )
            {
                if( (withHiddenFolders || ((d.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
                    && (dirFilter == null || dirFilter( d )) )
                {
                    CopyDirectory( d, new DirectoryInfo( Path.Combine( target.FullName, d.Name ) ), withHiddenFiles, withHiddenFolders );
                }
            }
            FileInfo[] files = src.GetFiles();
            foreach( FileInfo f in files )
            {
                if( (withHiddenFiles || ((f.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
                    && (fileFilter == null || fileFilter( f )) )
                {
                    f.CopyTo( Path.Combine( target.FullName, f.Name ) );
                }
            }
        }

    }
}
