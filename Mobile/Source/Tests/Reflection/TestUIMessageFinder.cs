// ReSharper disable ClassNeverInstantiated.Local

using NUnit.Framework;
using UISleuth.Messages;
using UISleuth.Reflection;

namespace UISleuth.Tests.Reflection
{
    [TestFixture]
    public class TestUIMessageFinder
    {
        internal class TestUIMessage : UIMessage { }
        public class TestClass { }


        [Test]
        public void Find_class_that_inherits_UIMessage()
        {
            var finder = new UIMessageFinder();
            var result = finder.Find(nameof(TestUIMessage));
            Assert.IsNotNull(result);
        }


        [Test]
        public void Return_null_for_non_UIMessages()
        {
            var finder = new UIMessageFinder();
            var result = finder.Find(nameof(TestClass));
            Assert.IsNull(result);
        }


        [Test]
        public void Return_null_for_type_that_doesnt_exist()
        {
            var finder = new UIMessageFinder();
            var doesntExist = finder.Find("something_123");
            Assert.IsNull(doesntExist);

            var nullPassed = finder.Find(null);
            Assert.IsNull(nullPassed);
        }
    }
}