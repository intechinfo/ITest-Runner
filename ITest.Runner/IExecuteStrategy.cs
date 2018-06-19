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
        /// <returns>True to execute its <see cref="TestFixture"/>s, false to skip it.</returns>
        bool ShouldRun( TestAssembly a );

        /// <summary>
        /// Gets whether the given fixture should be executed.
        /// </summary>
        /// <param name="f">The fixture to execute.</param>
        /// <returns>True to execute its <see cref="TestMethod"/>s, false to skip it.</returns>
        bool ShouldRun( TestFixture f );

        /// <summary>
        /// Gets whether the given method should be executed.
        /// </summary>
        /// <param name="m">The method to execute.</param>
        /// <returns>True to execute the method or its <see cref="TestCaseMethod"/>s, false to skip it.</returns>
        bool ShouldRun( TestMethod m );

        /// <summary>
        /// Gets whether the given test case should be executed.
        /// </summary>
        /// <param name="c">The test case to execute.</param>
        /// <returns>True to execute the test case, false to skip it.</returns>
        bool ShouldRun( TestCaseMethod c );
    }
}
