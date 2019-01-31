using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ITest.Runner.Tests
{
    [TestFixture]
    public class ExpliciteExecuteStrategyTests
    {
        [SetUp]
        public void onSetup()
        {
            LocalTests.ExpliciteExecute.ResetCallMemory();
        }

        [Test]
        public void ShouldCallSpecificTest()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() }, t => t == typeof( LocalTests.ExpliciteExecute ) );
            testRoot.HasInitializationError.Should().BeFalse();
            var strat = new ExplicitExecuteStrategy( testRoot.AllChildrenNodes
                                                        .Where( n => n.ToString() == "ITest.Runner.Tests/ITest.Runner.Tests.LocalTests.ExpliciteExecute/test_02" ) );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExpliciteExecute.Test01CallCount.Should().Be( 0 );
            LocalTests.ExpliciteExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.Test03CallCount.Should().Be( 0 );
            LocalTests.ExpliciteExecute.TestWithCaseCallCount.Should().Be( 0 );

            var d = testRoot.ResultDocument;
            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_01" ).Single()
                .Element( "Runs" ).Elements( "Run" )
                .All( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Untargeted.ToString() )
                .Should().Be( true );

            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_02" ).Single()
                .Element( "Runs" ).Elements( "Run" )
                .All( r => r.Attribute( "Skip" ) == null )
                .Should().Be( true );


            d.Save( SUTHelper.GetCleanResultFilePath( "Local-ExpliciteExecute" ) );
        }

        [Test]
        public void ShouldCallAllNotExplicitTestsOfFixture()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() } );
            testRoot.HasInitializationError.Should().BeFalse();
            var strat = new ExplicitExecuteStrategy( testRoot.AllChildrenNodes
                                                        .Where( n => n.ToString() == "ITest.Runner.Tests/ITest.Runner.Tests.LocalTests.ExpliciteExecute" ) );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExpliciteExecute.Test01CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.Test03CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.TestWithCaseCallCount.Should().Be( 2 );

            var d = testRoot.ResultDocument;
            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_with_case" ).Single()
                .Elements( "C" )
                .Where( e =>
                     e.Element( "Runs" ).Elements( "Run" ).Any( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Explicit.ToString() ))
                .Should().HaveCount( 1 );

            d.Save( SUTHelper.GetCleanResultFilePath( "Local-ExpliciteExecute" ) );
        }

        [Test]
        public void ShouldCallAllNotExplicitTestsOfDll()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() }, t => t == typeof( LocalTests.ExpliciteExecute ) );
            testRoot.HasInitializationError.Should().BeFalse();
            var strat = new ExplicitExecuteStrategy( testRoot.AllChildrenNodes
                                                        .Where( n => n.ToString() == "ITest.Runner.Tests" ) );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExpliciteExecute.Test01CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.Test03CallCount.Should().Be( 1 );
            LocalTests.ExpliciteExecute.TestWithCaseCallCount.Should().Be( 2 );

            var d = testRoot.ResultDocument;
            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_with_case" ).Single()
                .Elements( "C" )
                .Where( e =>
                     e.Element( "Runs" ).Elements( "Run" ).Any( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Explicit.ToString() ) )
                .Should().HaveCount( 1 );

            d.Save( SUTHelper.GetCleanResultFilePath( "Local-ExpliciteExecute" ) );
        }
    }
}
