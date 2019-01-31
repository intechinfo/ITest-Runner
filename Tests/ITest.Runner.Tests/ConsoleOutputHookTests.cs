using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITest.Runner.Tests
{
    [TestFixture]
    public class ConsoleOutputHookTests
    {
        [Test]
        public void CorrectlyCaptureOuputInScope()
        {
            Console.WriteLine( "Ya" );
            string conText = null;
            using( new ConsoleOutputHook( text => conText = text ) )
            {
                Console.WriteLine( "Yo1" );
                string conText2 = null;
                using( new ConsoleOutputHook( text => conText2 = text ) )
                {
                    Console.WriteLine( "NEVER" );
                }
                Console.WriteLine( "Yo2" );
            }
            Console.WriteLine( "Yu" );

            conText.Should().Contain( "Yo1" ).And.Contain( "Yo2" )
                            .And.NotContain( "Ya" ).And.NotContain( "Yu" ).And.NotContain( "NEVER" );
    }
    }
}
