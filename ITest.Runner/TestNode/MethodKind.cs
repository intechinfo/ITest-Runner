using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    /// <summary>
    /// Defines a test method.
    /// </summary>
    [Flags]
    public enum MethodKind
    {
        /// <summary>
        /// Not a test method.
        /// </summary>
        None = 0,

        /// <summary>
        /// Test method (normal one).
        /// </summary>
        Test = 1,

        /// <summary>
        /// Test case: parameters apply.
        /// </summary>
        TestCase = 2,

        /// <summary>
        /// Setup method (called before each execution of <see cref="Test"/> and <see cref="TestCase"/>).
        /// </summary>
        SetUp = 4,

        /// <summary>
        /// Teardown method (called after each execution of <see cref="Test"/> and <see cref="TestCase"/>).
        /// </summary>
        TearDown = 8,

        /// <summary>
        /// Flags that denotes an async method.
        /// </summary>
        Async = 16,

        /// <summary>
        /// Flags that denotes an explicit test method.
        /// </summary>
        Explicit = 32
    }
}
