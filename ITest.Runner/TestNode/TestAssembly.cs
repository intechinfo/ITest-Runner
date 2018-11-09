using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace ITest.Runner
{
    public sealed class TestAssembly : TestNode
    {
        public static readonly XName xElementName = XNamespace.None + "A";

        readonly Assembly _a;
        readonly IReadOnlyList<TestFixture> _fixtures;

        public TestAssembly( TestRoot r, Assembly a, Func<Type, bool> typeFilter )
            : base( r, xElementName, a.GetName().Name )
        {
            Root = r;
            _a = a;
            _fixtures = a.GetTypes()
                    .Where( t => typeFilter?.Invoke( t ) ?? true )
                    .Select( t => new { T = t, D = NUnitBindings.GetFixtureDescriptor( t ) } )
                    .Where( c => c.D != null )
                    .Select( c => new TestFixture( this, c.T, c.D ) )
                    .ToList();
        }

        public TestRoot Root { get; }

        public override TestNode Parent => Root;

        private protected override IReadOnlyList<TestNode> TestNodeChildren => _fixtures;

    }
}
