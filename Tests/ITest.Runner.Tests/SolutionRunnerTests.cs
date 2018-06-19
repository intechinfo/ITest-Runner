using FluentAssertions;
using NUnit.Framework;
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
    [TestFixture]
    public class SolutionRunnerTests
    {
        [Test]
        public void ITI_Primary_School_works_fine()
        {
            var solution = SUTHelper.GetTestSolutionPath( "ITI-PrimarySchool" );
            var output = SUTHelper.GetCleanResultFilePath( "ITI-PrimarySchool" );
            File.Exists( output ).Should().BeFalse();

            var r = SolutionRunner.Run( solution, output, preserveSolutionFolder: true, debugBuild: true );
            r.ProcessSuccess.Should().BeTrue();
            r.OutputXmlPath.Should().Be( output );
            File.Exists( r.OutputXmlPath ).Should().BeTrue();
            var result = XDocument.Load( r.OutputXmlPath ).Root;
            result.Attribute( "ErrorCount" ).Value.Should().Be( "0" );
        }

    }
}
