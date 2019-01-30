using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    public class DiscoverExecuteStrategy : IExecuteStrategy
    {
        public DiscoverExecuteStrategy()
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
