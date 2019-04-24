using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace ITest.Runner.Tests
{
    [TestFixture]
    public class SolutionRunnerTests
    {
        [Test]
        public void ITI_Primary_School_net461_works_fine()
        {
            var solution = SUTHelper.GetTestSolutionPath( "ITI-PrimarySchool" );
            var output = SUTHelper.GetCleanResultFilePath( "ITI-PrimarySchool" );
            File.Exists( output ).Should().BeFalse();

            var r = SolutionRunner.Run( solution, output, preserveSolutionFolder: true, debugBuild: true );
            r.ProcessSuccess.Should().BeTrue();
            r.OutputXmlPath.Should().Be( output );
            File.Exists( r.OutputXmlPath ).Should().BeTrue();
            var result = XDocument.Load( r.OutputXmlPath ).Root;
            result.Attribute( "LastRunErrorCount" ).Value.Should().Be( "0" );
        }

        [Test]
        public void ITI_Primary_School_netcoreapp21_works_fine()
        {
            DoRunPrimarySchoolNetCore();
        }

        static void DoRunPrimarySchoolNetCore()
        {
            var solutionPath = SUTHelper.GetTestSolutionPath( "ITI-PrimarySchool-NetCore" );
            var outputPath = SUTHelper.GetCleanResultFilePath( "ITI-PrimarySchool-NetCore" );
            File.Exists( outputPath ).Should().BeFalse();

            var r = SolutionRunner.Run( solutionPath, outputPath, preserveSolutionFolder: true, debugBuild: true );
            r.ProcessSuccess.Should().BeTrue();
            r.OutputXmlPath.Should().Be( outputPath );
            File.Exists( r.OutputXmlPath ).Should().BeTrue();
            var result = XDocument.Load( r.OutputXmlPath ).Root;
            result.Attribute( "LastRunErrorCount" ).Value.Should().Be( "0" );
        }

        [Test]
        public void Should_Fail_If_Solution_Path_Contain_Temp_Directory()
        {
            var solutionPath = SUTHelper.GetTestSolutionPath( "C:/" );
            var outputPath = SUTHelper.GetCleanResultFilePath( "EmptySolution" );
            var result = SolutionRunner.Run( solutionPath, outputPath, preserveSolutionFolder: true, debugBuild: true );
            Assert.That( result.ExitCode != 0 );
        }

        [Test]
        public void projecting_test_result_on_PrimarySchool_NetCore()
        {
            var p = SUTHelper.GetResultFilePath( "ITI-PrimarySchool-NetCore" );
            if( !File.Exists( p ) ) DoRunPrimarySchoolNetCore();
            XElement result = XDocument.Load( p ).Root;

            var pointResult = ToPointTestResult( result );

            pointResult.Elements().First().ToString()
                    .Should().Be( @"<P FullName=""ITI.PrimarySchool.Tests.PublicModelChecker"" Type=""F"" Success=""true"" />" );

        }

        [Test]
        public void projecting_test_result_on_LocalOneBugguyTestAmong4()
        {
            var p = SUTHelper.GetResultFilePath( "Local-OneBugguyTestAmong4" );
            Assume.That( File.Exists( p ) );

            XElement result = XDocument.Load( p ).Root;

            var pointResult = ToPointTestResult( result );

            pointResult.Elements().First().ToString()
                    .Should().Be( @"<P FullName=""ITest.Runner.Tests.LocalTests.OneBugguyTestAmong4"" Type=""F"" Success=""false"" />" );

        }

        XElement ToPointTestResult( XElement result )
        {

            string GetFullName( XElement e )
            {
                return String.Join( "", e.AncestorsAndSelf()
                                            .Reverse()
                                            .Skip( 2 )
                                            .Select( ( x, idx ) => x.Name != "C"
                                                                ? (idx > 0 ? "." : "") + (string)x.Attribute( "Name" )
                                                                : "(" + (string)x.Attribute( "Name" ) + ")" ) );
            }

            bool GetSuccess( XElement e )
            {
                return (int)e.Attribute( "LastRunErrorCount" ) == 0;
            }

            var pointResult = new XElement( "PointResult",
                                    result.Descendants()
                                          .Where( e => (e.Name == "F" || e.Name == "M" || e.Name == "C")
                                                        && e.Attribute( "IsExplicit" ) == null )
                                          .Select( e => new XElement( "P",
                                                                new XAttribute( "FullName", GetFullName( e ) ),
                                                                new XAttribute( "Type", e.Name ),
                                                                new XAttribute( "Success", GetSuccess( e ) )
                                                            ) ) );
            return pointResult;
        }

    }
}
