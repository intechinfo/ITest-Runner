using ITest.Framework;
using System;


namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class BuggySetUp
    {
        [SetUp]
        public void IWillFail()
        {
            throw new Exception( "Error in Setup!" );
        }

        [Test]
        public void test_01()
        {
        }

    }
}
