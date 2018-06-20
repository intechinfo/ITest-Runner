using ITest.Framework;
using System.Collections.Generic;

namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class TestCaseWithExplicit
    {
        public static int TestCaseAsTestCallCount { get; private set; }
        public static List<string> TestCaseWithExplicitMessages { get; } = new List<string>();
        public static void ResetCallMemory()
        {
            TestCaseAsTestCallCount = 0;
            TestCaseWithExplicitMessages.Clear();
        }

        // We allow TestCase used as a Test.
        [TestCase]
        public void testcase_as_test()
        {
            ++TestCaseAsTestCallCount;
        }

        [TestCase( "Implicit n°1." )]
        [TestCase( "This one is explicit.", Explicit = true )]
        [TestCase( "Implicit n°2." )]
        public void test_case_with_explicit( string msg )
        {
            TestCaseWithExplicitMessages.Add( msg );
        }

    }
}
