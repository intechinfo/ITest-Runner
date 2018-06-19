using ITest.Framework;
using System.Collections.Generic;

namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class Fix1
    {
        public static int Test01CallCount { get; private set; }
        public static int Test02CallCount { get; private set; }
        public static List<string> Test02Messages { get; } = new List<string>();
        public static void ResetCallMemory()
        {
            Test01CallCount = Test02CallCount = 0;
            Test02Messages.Clear();
        }

        [Test]
        public void test_01()
        {
            ++Test01CallCount;
        }

        [TestCase( "Hip" )]
        [TestCase( "Hop" )]
        public void test_01( string msg )
        {
            ++Test02CallCount;
            Test02Messages.Add( msg );
        }

    }
}
