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

        public virtual RunSkipReason ShouldRun( TestAssembly a )
        {
            return _nodes.ContainsKey( a ) ? RunSkipReason.None : RunSkipReason.Untargeted;
        }

        public RunSkipReason ShouldRun( TestFixture f )
        {
            if( _nodes.ContainsKey( f ) ) return RunSkipReason.None;
            if( (_nodes.TryGetValue( f.Assembly, out bool a ) && a) )
            {
                return f.IsExplicit ? RunSkipReason.Explicit : RunSkipReason.None;
            }
            return RunSkipReason.Untargeted;
        }
         
        public RunSkipReason ShouldRun( TestMethod m )
        {
            if( _nodes.ContainsKey( m ) ) return RunSkipReason.None;
            if( (_nodes.TryGetValue( m.Fixture, out bool f ) && f)
                || (_nodes.TryGetValue( m.Fixture.Assembly, out bool a) && a) )
            {
                return m.IsExplicit ? RunSkipReason.Explicit : RunSkipReason.None;
            }
            return RunSkipReason.Untargeted;
        }

        public RunSkipReason ShouldRun( TestCaseMethod c )
        {
            if( _nodes.ContainsKey( c ) ) return RunSkipReason.None;
            if( (_nodes.TryGetValue( c.Method, out bool m ) && m)
                || (_nodes.TryGetValue( c.Method.Fixture, out bool f) && f)
                || (_nodes.TryGetValue( c.Method.Fixture.Assembly, out bool a) && a) )
            {
                return c.IsExplicit ? RunSkipReason.Explicit : RunSkipReason.None;
            }
            return RunSkipReason.Untargeted;
        }
    }
}
