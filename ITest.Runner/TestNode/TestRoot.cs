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
        int _executionCount;

        public TestRoot( Assembly main, params Assembly[] other )
            : this( new[] { main }.Concat( other ) )
        {
        }

        public TestRoot( IEnumerable<Assembly> assemblies, Func<Type,bool> typeFilter = null )
            : base( null, xElementName, null )
        {
            _doc = new XDocument( Result );
            _assemblies = assemblies.Select( a => new TestAssembly( this, a, typeFilter ) ).ToList();
            Initialize();
        }

        public override TestNode Parent => null;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => _assemblies;


        /// <summary>
        /// Root entry point of execution.
        /// </summary>
        /// <param name="strategy">The strategy to use. Can npt be null.</param>
        /// <returns>The number of errors.</returns>
        public int Execute( IExecuteStrategy strategy )
        {
            if( strategy == null ) throw new ArgumentNullException( nameof( strategy ) );
            return Execute( new ExecutionContext { Strategy = strategy } );
        }

        private protected override int DoExecute( ExecutionContext ctx )
        {
            ctx.ExecutionCount = ++_executionCount;
            return base.DoExecute( ctx );
        }

        public XDocument ResultDocument => _doc;


        public static XDocument UnattendedRun( Func<TestRoot, IExecuteStrategy> strategyBuilder, Assembly main, params Assembly[] other )
        {
            return UnattendedRun( () => new TestRoot( main, other ), strategyBuilder );
        }

        public static XDocument UnattendedRun( Assembly a, Func<Type, bool> typeFilter, Func<TestRoot, IExecuteStrategy> strategyBuilder = null )
        {
            return UnattendedRun( () => new TestRoot( new[] { a }, typeFilter ), strategyBuilder );
        }

        static XDocument UnattendedRun( Func<TestRoot> creator, Func<TestRoot,IExecuteStrategy> strategyBuilder )
        {
            try
            {
                var r = creator();
                if( !r.HasInitializationError )
                {
                    if( strategyBuilder == null ) strategyBuilder = root => new DefaultExecuteStrategy( true );
                    var strat = strategyBuilder( r );
                    if( strat != null )
                    {
                        r.Execute( strat );
                    }
                }
                return r.ResultDocument;
            }
            catch( Exception ex )
            {
                return new XDocument( new XElement( "Fatal", ex.ToXml() ) );
            }
        }
    }
}
