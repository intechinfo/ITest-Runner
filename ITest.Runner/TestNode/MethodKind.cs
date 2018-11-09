using System;
using System.Collections.Generic;
using System.Text;

namespace ITest.Runner
{
    [Flags]
    public enum MethodKind
    {
        None = 0,
        Test = 1,
        TestCase = 2,
        SetUp = 4,
        TearDown = 8,
        Async = 16,
        Explicit = 32
    }
}
