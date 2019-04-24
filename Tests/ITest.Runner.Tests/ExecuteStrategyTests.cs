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
    public class ExecuteStrategyTests
    {

        [Test]
        public void SkipAllExecuteStrategy_skips_all_tests()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() } );
            testRoot.HasInitializationError.Should().BeFalse();
            testRoot.Execute( new SkipAllExecuteStrategy() ).Should().Be( 0 );

            var d = testRoot.ResultDocument;

            d.Root.Element( "A" ).Elements( "F" )
            .All( f => f.Elements( "M" ).All( m =>
            {
                if( m.Element( "Runs" ) == null )
                    return m.Elements( "C" ).All( c => c.Element( "Runs" ).Elements( "Run" ).Count() == 0 );

                return m.Element( "Runs" ).Elements( "Run" ).Count() == 0;
            } ) )
            .Should().BeTrue();

            d.Save( SUTHelper.GetCleanResultFilePath( "SkipAllExecuteStrategy_skips_all_tests" ) );
        }


        [SetUp]
        public void OnSetup()
        {
            LocalTests.ExplicitExecute.ResetCallMemory();
        }

        [Test]
        public void targeting_one_and_only_one_test_method()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() }, t => t == typeof( LocalTests.ExplicitExecute ) );
            testRoot.HasInitializationError.Should().BeFalse();
            var toExecute = testRoot.AllChildrenNodes
                                    .Where( n => n.ToString() == "ITest.Runner.Tests/ITest.Runner.Tests.LocalTests.ExplicitExecute/test_02" );
            var strat = new ExplicitExecuteStrategy( toExecute );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExplicitExecute.Test01CallCount.Should().Be( 0 );
            LocalTests.ExplicitExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.Test03CallCount.Should().Be( 0 );
            LocalTests.ExplicitExecute.TestWithCaseCallCount.Should().Be( 0 );

            var d = testRoot.ResultDocument;
            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) != "test_02" )
                .Elements( "Runs" ).Elements( "Run" )
                .All( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Untargeted.ToString() )
                .Should().Be( true );

            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_02" ).Single()
                .Element( "Runs" ).Elements( "Run" )
                .All( r => r.Attribute( "Skip" ) == null )
                .Should().Be( true );


            d.Save( SUTHelper.GetCleanResultFilePath( "targeting_one_and_only_one_test_method" ) );
        }

        [Test]
        public void targeting_a_fixture()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() } );
            testRoot.HasInitializationError.Should().BeFalse();

            var toExecute = testRoot.AllChildrenNodes
                                .Where( n => n.ToString() == "ITest.Runner.Tests/ITest.Runner.Tests.LocalTests.ExplicitExecute" );
            var strat = new ExplicitExecuteStrategy( toExecute );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExplicitExecute.Test01CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.Test03CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.TestWithCaseCallCount.Should().Be( 2 );

            var d = testRoot.ResultDocument;
            d.Root.Elements( "A" ).Elements( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_with_case" ).Single()
                .Elements( "C" )
                .Where( e =>
                     e.Element( "Runs" ).Elements( "Run" ).Any( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Explicit.ToString() ) )
                .Should().HaveCount( 1 );

            d.Save( SUTHelper.GetCleanResultFilePath( "Local-ExplicitExecute" ) );
        }

        [Test]
        public void executing_all_non_explicit_tests_of_an_assembly()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() }, t => t == typeof( LocalTests.ExplicitExecute ) );
            testRoot.HasInitializationError.Should().BeFalse();
            var toExecute = testRoot.AllChildrenNodes
                                .Where( n => n.ToString() == "ITest.Runner.Tests" );
            var strat = new ExplicitExecuteStrategy( toExecute );
            testRoot.Execute( strat ).Should().Be( 0 );

            LocalTests.ExplicitExecute.Test01CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.Test02CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.Test03CallCount.Should().Be( 1 );
            LocalTests.ExplicitExecute.TestWithCaseCallCount.Should().Be( 2 );

            var d = testRoot.ResultDocument;
            d.Root.Element( "A" ).Element( "F" ).Elements( "M" )
                .Where( m => (string)m.Attribute( "Name" ) == "test_with_case" ).Single()
                .Elements( "C" )
                .Where( e =>
                     e.Element( "Runs" ).Elements( "Run" ).Any( r => (string)r.Attribute( "Skip" ) == RunSkipReason.Explicit.ToString() ) )
                .Should().HaveCount( 1 );

            d.Save( SUTHelper.GetCleanResultFilePath( "executing_all_non_explicit_tests_of_an_assembly" ) );
        }
    }
}
