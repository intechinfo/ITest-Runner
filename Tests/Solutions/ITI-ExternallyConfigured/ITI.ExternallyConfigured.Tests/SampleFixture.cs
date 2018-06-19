using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ExternallyConfiguredTestsProject
{
    [TestFixture]
    public class SampleFixture
    {
        static string ThisFilePath( [CallerFilePath] string p = null ) => p;

        static string ConfigurationFilePath => Path.Combine( Path.GetDirectoryName( Path.GetDirectoryName( ThisFilePath() ) ), "ExternallyConfigured.config.xml" );

        static (bool Success, int DelayMS) ReadConfiguration( string name ) =>
            XDocument.Load( ConfigurationFilePath )
                                            .Root.Elements( "Test0" )
                                            .Select( e => ((string)e.Attribute( "Success" ) == "true", (int)e.Attribute( "DelayMS" )) )
                                            .Single();

        [Test]
        public void this_test_always_succeeds()
        {
            0.Should().Be( 0 );
        }

        [Test]
        public void this_test_outcome_depends_on_Test0_configuration()
        {
            var config = ReadConfiguration( "Test0" );
            Thread.Sleep( config.DelayMS );
            config.Success.Should().BeTrue( "Test0 Success is not true." );
        }

        [Test]
        public async Task this_test_outcome_depends_on_Test0_configuration_Async()
        {
            var config = ReadConfiguration( "Test0" );
            await Task.Delay( config.DelayMS );
            config.Success.Should().BeTrue( "Test0 Success is not true." );
        }

    }
}
