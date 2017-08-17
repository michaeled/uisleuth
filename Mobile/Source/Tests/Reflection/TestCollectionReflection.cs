using System;
using System.Collections.Generic;
using NUnit.Framework;
using UISleuth.Reflection;

namespace UISleuth.Tests.Reflection
{
    [TestFixture]
    public class TestCollectionReflection
    {
        [Test]
        public void Concrete_list_check()
        {
            var list = new List<object>();
            var result = list.GetType().IsKindOf(typeof (IList<>));
            Assert.IsTrue(result);
        }


        [Test]
        public void Interface_check()
        {
            IList<int> list = new List<int>();
            var result = list.GetType().IsKindOf(typeof (IList<>));

            Assert.IsTrue(result);
        }


        [Test]
        public void Same_type_check()
        {
            var result = typeof (IList<int>).IsKindOf(typeof (IList<>));
            Assert.IsTrue(result);
        }


        [Test]
        public void Get_list_typeparam()
        {
            var list = new List<DateTime>();
            var type = ReflectionMethods.GetEnumerableItemType(list.GetType());

            Assert.AreEqual(typeof (DateTime), type);
        }
    }
}
