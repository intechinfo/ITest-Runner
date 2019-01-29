using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    /// <summary>
    /// Injectable strategy to <see cref="TestNode.Execute"/>.
    /// </summary>
    public interface IExecuteStrategy
    {
        /// <summary>
        /// Gets whether the given assembly should be executed.
        /// </summary>
        /// <param name="a">The assembly to execute.</param>
        /// <returns>The reason to skip the assembly or <see cref="RunSkipReason.None"/> to execute it.</returns>
        RunSkipReason ShouldRun( TestAssembly a );

        /// <summary>
        /// Gets whether the given fixture should be executed.
        /// </summary>
        /// <param name="f">The fixture to execute.</param>
        /// <returns>The reason to skip the fixture or <see cref="RunSkipReason.None"/> to execute it.</returns>
        RunSkipReason ShouldRun( TestFixture f );

        /// <summary>
        /// Gets whether the given method should be executed.
        /// </summary>
        /// <param name="m">The method to execute.</param>
        /// <returns>
        /// The reason to skip the test or its <see cref="TestCaseMethod"/>s.
        /// <see cref="RunSkipReason.None"/> to execute it (or them).
        /// </returns>
        RunSkipReason ShouldRun( TestMethod m );

        /// <summary>
        /// Gets whether the given test case should be executed.
        /// </summary>
        /// <param name="c">The test case to execute.</param>
        /// <returns>The reason to skip the test case or <see cref="RunSkipReason.None"/> to execute it.</returns>
        RunSkipReason ShouldRun( TestCaseMethod c );
    }
}
