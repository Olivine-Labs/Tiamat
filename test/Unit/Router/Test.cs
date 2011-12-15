using System;
using NUnit.Framework;

namespace Router
{
    [TestFixture]
    public class Test
    {
        private Router _router;

        [TestFixtureSetUp]
        public void SetUp()
        {
            _router = new Router();
            _router.Start();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            _router.Stop();
        }

        [Test]
        public void DoTest()
        {
        }
    }
}
