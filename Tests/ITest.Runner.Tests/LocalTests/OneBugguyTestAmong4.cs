using ITest.Framework;
using FluentAssertions;
using System;
using System.IO;

namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class OneBugguyTestAmong4
    {
        [SetUp]
        public void IWillSucceed()
        {
        }

        [TearDown]
        public void IWillSucceedToo()
        {
        }

        [Test]
        public void test_01()
        {
        }

        [Test]
        public void test_02()
        {
        }

        [Test]
        public void test_03_that_fails()
        {
            try
            {
                throw new Exception( "The inner error." );
            }
            catch( Exception ex )
            {
                throw new Exception( "Another exception above.", ex );
            }
        }

        class ConsoleOutputHook : IDisposable
        {
            readonly StringWriter _w;
            readonly Action<string> _collector;
            TextWriter _current;

            public ConsoleOutputHook( Action<string> c )
            {
                _collector = c;
                _w = new StringWriter();
                _current = Console.Out;
                Console.SetOut( _w );
            }

            public void Dispose()
            {
                if( _current != null )
                {
                    Console.SetOut( _current );
                    _current = null;
                    _collector( _w.ToString() );
                }
            }

            public override string ToString() => _w.ToString();
        }

        [Test]
        public void test_04()
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
