using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    public class DefaultExecuteStrategy : IExecuteStrategy
    {
        readonly bool _honorExplicit;

        public DefaultExecuteStrategy( bool honorExplicit )
        {
            _honorExplicit = honorExplicit;
        }

        public virtual bool ShouldRun( TestAssembly a )
        {
            return true;
        }

        public bool ShouldRun( TestFixture f )
        {
            return !f.IsExplicit || !_honorExplicit;
        }

        public bool ShouldRun( TestMethod m )
        {
            return !m.IsExplicit || !_honorExplicit;
        }

        public bool ShouldRun( TestCaseMethod c )
        {
            return true;
        }
    }
}
