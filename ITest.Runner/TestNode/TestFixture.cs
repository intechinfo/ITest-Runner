using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public sealed class TestFixture : TestNode
    {
        public static readonly XName xElementName = XNamespace.None + "F";
        public static readonly XName xElementInstanciateName = XNamespace.None + "FInstanciate";
        public static readonly XName xElementSetupName = XNamespace.None + "FSetup";
        public static readonly XName xElementTearDownName = XNamespace.None + "FTearDown";

        readonly Type _fixtureType;
        readonly FixtureDescriptor _desc;
        // _methods collects valid Test/TesCase methods and also Methods
        // that have initialization errors.
        readonly List<TestMethod> _methods;
        readonly ExecutionResult _instanciationResult;
        readonly ExecutionResult _setupExecResult;
        readonly ExecutionResult _tearDownExecResult;
        // Setup and TearDown are build from derived to base (the Setup list is reversed).
        readonly List<MethodDescriptor> _setupMethods;
        readonly List<MethodDescriptor> _tearDownMethods;

        object _fixtureObject;

        internal TestFixture( TestAssembly a, Type t, FixtureDescriptor desc )
            : base( a, xElementName, t.FullName, desc.IsExplicit )
        {
            Assembly = a;
            _fixtureType = t;
            _desc = desc;
            _instanciationResult = new ExecutionResult( this, xElementInstanciateName );
            _setupExecResult = new ExecutionResult( this, xElementSetupName );
            _tearDownExecResult = new ExecutionResult( this, xElementTearDownName );

            _methods = new List<TestMethod>();
            _setupMethods = new List<MethodDescriptor>();
            _tearDownMethods = new List<MethodDescriptor>();
            do
            {
                foreach( var m in t.GetMethods( BindingFlags.DeclaredOnly
                                                | BindingFlags.Instance
                                                | BindingFlags.Static
                                                | BindingFlags.Public ) )
                {
                    var d = NUnitBindings.GetMethodDescriptor( m );
                    if( d == null ) continue;
                    if( d.InitializationError != null || (d.MethodKind & (MethodKind.SetUp|MethodKind.TearDown)) == 0 )
                    {
                        var inheritedTypeName = t == _fixtureType ? null : t.Name;
                        _methods.Add( new TestMethod( this, m, d, inheritedTypeName ) );
                    }
                    else
                    {
                        if( (d.MethodKind & MethodKind.SetUp) != 0 ) _setupMethods.Add( d );
                        else
                        {
                            Debug.Assert( (d.MethodKind & MethodKind.TearDown) != 0 );
                            _tearDownMethods.Add( d );
                        }
                    }
                }
            }
            while( (t = t.BaseType) != typeof( object ) );
            _setupMethods.Reverse();
        }

        public override TestNode Parent => Assembly;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => _methods;

        public TestAssembly Assembly { get; }

        public bool IsExplicit => _desc.IsExplicit;

        internal object FixtureObject => _fixtureObject;

        private protected override int DoExecute( ExecutionContext ctx )
        {
            var skip = ctx.Strategy.ShouldRun( this );
            if( skip != RunSkipReason.None )
            {
                _instanciationResult.Skip( ctx.ExecutionCount, skip );
                return 0;
            }
            if( _fixtureObject == null )
            {
                if( !_instanciationResult.Run( ctx.ExecutionCount, () => _fixtureObject = Activator.CreateInstance( _fixtureType ) ) )
                {
                    return 1;
                }
            }
            int errorCount = 0;
            foreach( var m in _methods.Where( m => m.InitializationError == null ) )
            {
                errorCount += m.Execute( ctx );
            }
            return errorCount;
        }

        internal bool RunSetupMethods( ExecutionContext ctx )
        {
            foreach( var d in _setupMethods )
            {
                if( !_setupExecResult.Run( ctx.ExecutionCount, (d.MethodKind & MethodKind.Async) != 0, _fixtureObject, d.Method, null ) )
                {
                    return false;
                }
            }
            return true;
        }

        internal bool RunTearDownMethods( ExecutionContext ctx )
        {
            foreach( var d in _tearDownMethods )
            {
                if( !_tearDownExecResult.Run( ctx.ExecutionCount, (d.MethodKind & MethodKind.Async) != 0, _fixtureObject, d.Method, null ) )
                {
                    return false;
                }
            }
            return true;
        }

    }
}
