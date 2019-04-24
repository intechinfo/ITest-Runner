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
            : base( m, xElementName, d.CaseSignature, d.IsExplicit )
        {
            Method = m;
            Detail = d;
            _execResult = new ExecutionResult( Result );
        }

        public override TestNode Parent => Method;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => Array.Empty<TestNode>();

        public TestMethod Method { get; }

        public bool IsExplicit => Detail.IsExplicit;

        public TestCaseDetail Detail { get; }

        private protected override int DoExecute( ExecutionContext ctx )
        {
            var skip = ctx.Strategy.ShouldRun( this );
            if( skip == RunSkipReason.None )
            {
                if( !Method.Fixture.RunSetupMethods( ctx ) ) return 1;
                bool success = _execResult.Run( ctx.ExecutionCount, (Method.Kind & MethodKind.Async) != 0, Method.Fixture.FixtureObject, Method.Method, Detail.ArrayValues );
                success &= Method.Fixture.RunTearDownMethods( ctx );
                return success ? 0 : 1;
            }
            _execResult.Skip( ctx.ExecutionCount, skip );
            return 0;
        }

    }
}
