using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    public class SkipAllExecuteStrategy : IExecuteStrategy
    {
        public SkipAllExecuteStrategy()
        {
        }

        public virtual RunSkipReason ShouldRun( TestAssembly a )
        {
            return RunSkipReason.Untargeted;
        }

        public RunSkipReason ShouldRun( TestFixture f )
        {
            return RunSkipReason.Untargeted;
        }

        public RunSkipReason ShouldRun( TestMethod m )
        {
            return RunSkipReason.Untargeted;
        }

        public RunSkipReason ShouldRun( TestCaseMethod c )
        {
            return RunSkipReason.Untargeted;
        }
    }
}
