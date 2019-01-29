using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public sealed class TestMethod : TestNode
    {
        public static readonly XName xElementName = XNamespace.None + "M";

        readonly MethodDescriptor _desc;
        readonly IReadOnlyList<TestCaseMethod> _cases;

        // For [Test] method, this is not null: this is the actual test.
        // This is null for [TestCase].
        readonly ExecutionResult _thisResult;

        internal TestMethod( TestFixture f, MethodInfo m, MethodDescriptor d, string inheritedTypeName )
            : base( f, xElementName, inheritedTypeName != null ? inheritedTypeName + '.' + m.Name : m.Name )
        {
            Fixture = f;
            Method = m;
            _desc = d;
            Result.Add( new XAttribute( "Kind", _desc.MethodKind ) );
            _cases = d.TestCaseDetails.Select( c => new TestCaseMethod( this, c ) ).ToList();
            Debug.Assert( ((_desc.MethodKind & MethodKind.TestCase) != 0) == _cases.Count > 0 );
            if( (_desc.MethodKind & MethodKind.TestCase) == 0 ) _thisResult = new ExecutionResult( Result );
        }

        public override TestNode Parent => Fixture;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => _cases;

        public MethodKind Kind => _desc.MethodKind;

        public MethodInfo Method { get; }

        public TestFixture Fixture { get; }

        public bool IsTestCase => _thisResult == null;

        public bool IsExplicit => (_desc.MethodKind & MethodKind.Explicit) != 0;

        /// <summary>
        /// Gets the Execution result. Null if <see cref="IsTestCase"/> is true.
        /// </summary>
        public ExecutionResult ExecutionResult => _thisResult;

        public string InitializationError => _desc.InitializationError;

        private protected override string DoInitialize()
        {
            return _desc.InitializationError != null
                        ? _desc.InitializationError
                        : base.DoInitialize();
        }

        private protected override int DoExecute( ExecutionContext ctx )
        {
            if( _thisResult != null )
            {
                var skip = ctx.Strategy.ShouldRun( this );
                if( skip == RunSkipReason.None )
                {
                    if( !Fixture.RunSetupMethods( ctx ) ) return 1;
                    bool success = _thisResult.Run( ctx.ExecutionCount, (Kind & MethodKind.Async) != 0, Fixture.FixtureObject, Method, null );
                    success &= Fixture.RunTearDownMethods( ctx );
                    return success ? 0 : 1;
                }
                _thisResult.Skip( ctx.ExecutionCount, skip );
                return 0;
            }
            return base.DoExecute( ctx );
        }

    }
}
