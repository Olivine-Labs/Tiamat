using System;
using NUnit.Framework;

namespace Vocale.Tests
{
    class StaticClass
    {
        public static Object AStaticCommand(params Object[] parameters)
        {
            return "4";
        }
    }

    class DynamicClass : StaticClass
    {
        public Object ADynamicCommand(params Object[] parameters)
        {
            return "2";
        }
    }

    [TestFixture]
    public class Main
    {
        private Vocale _vocale;

        [TestFixtureSetUp]
        public void SetUp()
        {
             _vocale = new Vocale();
        }

        [TearDown]
        public void TearDown()
        {
            _vocale.RemoveAll();
        }

        [Test]
        public void BindToStatic()
        {
            _vocale.Register(typeof(StaticClass));
        }

        [Test]
        public void BindToDynamic()
        {
            var dynamicClass = new DynamicClass();
            _vocale.Register(dynamicClass);
        }

        [Test]
        public void Execute()
        {
            var dynamicClass = new DynamicClass();
            _vocale.Register(dynamicClass);
            var result = _vocale.Execute("ADynamicCommand", new object[3]) as String;
            Assert.AreEqual("2", result);
            result = _vocale.Execute("AStaticCommand", new object[3]) as String;
            Assert.AreEqual("4", result);
        }
    }
}
