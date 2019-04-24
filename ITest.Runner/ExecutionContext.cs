using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    /// <summary>
    /// Internal only class.
    /// </summary>
    class ExecutionContext
    {
        public IExecuteStrategy Strategy { get; set; }

        /// <summary>
        /// Gets the current execution round number.
        /// </summary>
        public int ExecutionCount { get; set; }
    }
}
