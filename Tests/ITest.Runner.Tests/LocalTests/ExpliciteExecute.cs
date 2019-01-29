using ITest.Framework;
using System;


namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class ExpliciteExecute
    {
        public static int Test01CallCount { get; private set; }
        public static int Test02CallCount { get; private set; }
        public static int Test03CallCount { get; private set; }
        public static int TestWithCaseCallCount { get; private set; }

        public static void ResetCallMemory()
        {
            TestWithCaseCallCount = Test01CallCount = Test02CallCount = Test03CallCount = 0;
        }

        [Test]
        public void test_01()
        {
            Console.WriteLine( "Running the first test !" );
            Test01CallCount++;
        }

        [Test]
        public void test_02()
        {
            Test02CallCount++;
        }

        [Test]
        public void test_03()
        {
            Test03CallCount++;
        }

        [TestCase( "Implicit n°1." )]
        [TestCase( "This one is explicit.", Explicit = true )]
        [TestCase( "Implicit n°2." )]
        public void test_with_case(string _)
        {
            TestWithCaseCallCount++;
        }

    }
}
