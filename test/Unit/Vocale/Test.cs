using System;
using NUnit.Framework;

namespace Vocale
{
    class StaticClass
    {
        public static Object AStaticCommand(params Object[] parameters)
        {
            return "static";
        }
    }

    class DynamicClass : StaticClass
    {
        public Object ADynamicCommand(params Object[] parameters)
        {
            return "dynamic";
        }
    }

    class FailClass
    {
        public static Object AFailCommand()
        {
            return "fail";
        }
    }

    [TestFixture]
    public class Test
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
            Assert.True(_vocale.Exists("AStaticCommand"));
        }

        [Test]
        public void BindToDynamic()
        {
            var dynamicClass = new DynamicClass();
            _vocale.Register(dynamicClass);
            Assert.True(_vocale.Exists("ADynamicCommand"));
            Assert.True(_vocale.Exists("AStaticCommand"));
        }

        [Test]
        public void Exists()
        {
            _vocale.Register(typeof(StaticClass));
            Assert.True(_vocale.Exists("AStaticCommand"));
        }

        [Test]
        public void NotExists()
        {
            _vocale.Register(typeof(StaticClass));
            Assert.False(_vocale.Exists("ANonexistantCommand"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void BindToFail()
        {
            _vocale.Register(typeof(FailClass));
        }

        [Test]
        public void ExecuteStatic()
        {
            _vocale.Register(typeof(StaticClass));
            var result = _vocale.Execute("AStaticCommand", new object[3]) as String;
            Assert.AreEqual("static", result);
        }

        [Test]
        public void ExecuteDynamic()
        {
            var dynamicClass = new DynamicClass();
            _vocale.Register(dynamicClass);
            var result = _vocale.Execute("ADynamicCommand", new object[3]) as String;
            Assert.AreEqual("dynamic", result);
            result = _vocale.Execute("AStaticCommand", new object[3]) as String;
            Assert.AreEqual("static", result);
        }

        [Test]
        public void ExecuteFail()
        {
            _vocale.Register(typeof(StaticClass));
            var result = _vocale.Execute("ANonexistantCommand", new object[3]) as String;
            Assert.Null(result);
        }

        [Test]
        public void Remove()
        {
            _vocale.Register(typeof(StaticClass));
            Assert.True(_vocale.Exists("AStaticCommand"));
            _vocale.Remove("AStaticCommand");
            Assert.False(_vocale.Exists("AStaticCommand"));
        }

        [Test]
        public void RemoveMultiple()
        {
            var dynamicClass = new DynamicClass();
            _vocale.Register(dynamicClass);
            Assert.True(_vocale.Exists("AStaticCommand"));
            Assert.True(_vocale.Exists("ADynamicCommand"));
            _vocale.Remove("AStaticCommand");
            Assert.False(_vocale.Exists("AStaticCommand"));
            _vocale.Remove("ADynamicCommand");
            Assert.False(_vocale.Exists("ADynamicCommand"));
        }

        [Test]
        public void RemoveFail()
        {
            _vocale.Register(typeof(StaticClass));
            Assert.True(_vocale.Exists("AStaticCommand"));
            _vocale.Remove("ANonexistantCommand");
        }
    }
}
