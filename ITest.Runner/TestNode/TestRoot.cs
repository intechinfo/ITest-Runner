using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public class TestRoot : TestNode
    {
        public static readonly XName xElementName = XNamespace.None + "TestResult";

        readonly XDocument _doc;
        readonly IReadOnlyList<TestAssembly> _assemblies;

        public TestRoot( Assembly main, params Assembly[] other )
            : this( new[] { main }.Concat( other ) )
        {
        }

        public TestRoot( IEnumerable<Assembly> assemblies, Func<Type,bool> typeFilter = null )
            : base( null, xElementName, null )
        {
            _doc = new XDocument( Result );
            _assemblies = assemblies.Select( a => new TestAssembly( this, a, typeFilter ) ).ToList();
        }

        public override TestNode Parent => null;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => _assemblies;

        public XDocument ResultDocument => _doc;

        public IEnumerable<TestNode> Locate( string filter )
        {
            var selector = new SelectionParser( filter );
            return TestNodeChildren.Where( n => selector.IsTarget( n ) );
        }

        public static XDocument UnattendedRun( Assembly main, params Assembly[] other )
        {
            return UnattendedRun( () => new TestRoot( main, other ) );
        }
        public static XDocument UnattendedRun( Assembly a, Func<Type, bool> typeFilter )
        {
            return UnattendedRun( () => new TestRoot( new[] { a }, typeFilter ) );
        }

        static XDocument UnattendedRun( Func<TestRoot> creator )
        {
            try
            {
                var r = creator();
                var ctx = new ExecutionContext() { Strategy = new DefaultExecuteStrategy( honorExplicit: true ) };
                if( r.Initialize() == null ) r.Execute( ctx );
                return r.ResultDocument;
            }
            catch( Exception ex )
            {
                return new XDocument( new XElement( "Fatal", ex.ToXml() ) );
            }
        }

    }
}
