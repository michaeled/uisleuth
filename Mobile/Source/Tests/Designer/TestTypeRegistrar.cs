using NUnit.Framework;
using UISleuth.Reflection;

namespace UISleuth.Tests.Designer
{
    [TestFixture]
    public class TestTypeRegistrar
    {
        internal TypeRegistrar Tr => TypeRegistrar.Instance;


        [Test]
        public void Contains_using_UIReflectionProperty()
        {
            Tr.SetTypes(typeof(int));

            var xProp = new UIReflectionProperty
            {
                TargetType = typeof (int)
            };

            var usingTypeName = Tr.IsRegistered(xProp, RegistrarMatches.TypeName);
            var usingEnum = Tr.IsRegistered(xProp, RegistrarMatches.Enum);

            Assert.IsTrue(usingTypeName, "by TypeName");
            Assert.IsFalse(usingEnum, "by Enum");
        }


        [Test]
        public void Contains_using_type()
        {
            Tr.SetTypes(typeof(int));
            Assert.IsTrue(Tr.IsRegistered(typeof(int)));
        }
    }
}