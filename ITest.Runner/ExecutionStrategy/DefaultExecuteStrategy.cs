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

        public virtual RunSkipReason ShouldRun( TestAssembly a )
        {
            return RunSkipReason.None;
        }

        public RunSkipReason ShouldRun( TestFixture f )
        {
            return (f.IsExplicit && _honorExplicit) ? RunSkipReason.Explicit : RunSkipReason.None;
        }

        public RunSkipReason ShouldRun( TestMethod m )
        {
            return (m.IsExplicit && _honorExplicit) ? RunSkipReason.Explicit : RunSkipReason.None;
        }

        public RunSkipReason ShouldRun( TestCaseMethod c )
        {
            return (c.IsExplicit && _honorExplicit) ? RunSkipReason.Explicit : RunSkipReason.None;
        }
    }
}
