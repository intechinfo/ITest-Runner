using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ITest.Runner
{
    public class SolutionRunResult
    {
        public bool ProcessSuccess => ExitCode == 0;

        public int ExitCode { get; }

        public string RunError { get; }

        public string StdError { get; }

        public string StdOutput { get; }

        public string OutputXmlPath { get; }

        internal SolutionRunResult( string runError )
        {
            ExitCode = Int32.MaxValue;
            RunError = runError;
        }

        internal SolutionRunResult( int exitCode, string stdErr, string stdOut, string outputXmlPath )
        {
            ExitCode = exitCode;
            StdError = stdErr;
            StdOutput = stdOut;
            OutputXmlPath = outputXmlPath;
        }

    }
}
