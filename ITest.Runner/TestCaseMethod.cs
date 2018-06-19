using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public sealed class TestCaseMethod : TestNode
    {
        public static readonly XName xElementName = XNamespace.None + "C";

        readonly ExecutionResult _execResult;

        internal TestCaseMethod( TestMethod m, TestCaseDetail d )
            : base( m, xElementName, d.CaseSignature )
        {
            Method = m;
            Detail = d;
            _execResult = new ExecutionResult( Result );
        }

        public override TestNode Parent => Method;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => Array.Empty<TestNode>();

        public TestMethod Method { get; }

        public TestCaseDetail Detail { get; }

        private protected override int DoExecute( ExecutionContext ctx )
        {
            if( ctx.Strategy.ShouldRun( this ) )
            {
                if( !Method.Fixture.RunSetupMethods( ctx ) ) return 1;
                bool success = _execResult.Run( (Method.Kind & MethodKind.Async) != 0, Method.Fixture.FixtureObject, Method.Method, Detail.ArrayValues );
                success &= Method.Fixture.RunTearDownMethods( ctx );
                return success ? 0 : 1;
            }
            _execResult.Skip();
            return 0;
        }

    }
}
