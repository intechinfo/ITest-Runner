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
    public class DiscoverExecuteStrategyTests
    {
        [Test]
        public void DiscoverProperly()
        {
            var testRoot = new TestRoot( new[] { Assembly.GetExecutingAssembly() } );
            testRoot.HasInitializationError.Should().BeFalse();
            testRoot.Execute( new DiscoverExecuteStrategy() ).Should().Be( 0 );
            
            var d = testRoot.ResultDocument;

            d.Root.Element( "A" ).Elements( "F" )
            .All( f => f.Elements( "M" ).All( m =>
            {
                if (m.Element("Runs") == null)
                    return m.Elements( "C" ).All( c => c.Element( "Runs" ).Elements( "Run" ).Count() == 0 );

                return m.Element( "Runs" ).Elements( "Run" ).Count() == 0;
            } ) )
            .Should().BeTrue();

            d.Save( SUTHelper.GetCleanResultFilePath( "Local-ExpliciteExecute" ) );
        }
    }
}
