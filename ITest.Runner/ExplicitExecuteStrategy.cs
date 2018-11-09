using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITest.Runner
{
    public class ExplicitExecuteStrategy : IExecuteStrategy
    {
        readonly Dictionary<TestNode,bool> _nodes;

        public ExplicitExecuteStrategy( IEnumerable<TestNode> nodesToExecute )
        {
            _nodes = new Dictionary<TestNode, bool>();
            foreach( var n in nodesToExecute )
            {
                if( _nodes.TryGetValue( n, out bool already ) && already ) continue;
                _nodes[n] = true;
                var p = n.Parent;
                while( p != null )
                {
                    if( _nodes.ContainsKey( p ) ) break;
                    _nodes[p] = false;
                    p = p.Parent;
                }
            }
        }

        public virtual bool ShouldRun( TestAssembly a )
        {
            return _nodes.ContainsKey( a );
        }

        public bool ShouldRun( TestFixture f )
        {
            return _nodes.ContainsKey( f );
        }

        public bool ShouldRun( TestMethod m )
        {
            if( _nodes.ContainsKey( m ) ) return true;
            if( (_nodes.TryGetValue( m.Fixture, out bool f ) && f)
                || (_nodes.TryGetValue( m.Fixture.Assembly, out bool a) && a) )
            {
                return !m.IsExplicit;
            }
            return false;
        }

        public bool ShouldRun( TestCaseMethod c )
        {
            if( _nodes.ContainsKey( c ) ) return true;
            if( (_nodes.TryGetValue( c.Method, out bool m ) && m)
                || (_nodes.TryGetValue( c.Method.Fixture, out bool f) && f)
                || (_nodes.TryGetValue( c.Method.Fixture.Assembly, out bool a) && a) )
            {
                return !c.IsExplicit;
            }
            return false;
        }
    }
}
