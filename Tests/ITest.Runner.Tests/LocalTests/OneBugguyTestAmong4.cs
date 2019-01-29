using ITest.Framework;
using FluentAssertions;
using System;


namespace ITest.Runner.Tests.LocalTests
{
    [TestFixture]
    public class OneBugguyTestAmong4
    {
        [SetUp]
        public void IWillSucceed()
        {
        }

        [TearDown]
        public void IWillSucceedToo()
        {
        }

        [Test]
        public void test_01()
        {
        }

        [Test]
        public void test_02()
        {
        }

        [Test]
        public void test_03_that_fails()
        {
            try
            {
                throw new Exception( "The inner error." );
            }
            catch( Exception ex )
            {
                throw new Exception( "Another exception above.", ex );
            }
        }

        [Test]
        public void test_04()
        {
        }

    }
}
