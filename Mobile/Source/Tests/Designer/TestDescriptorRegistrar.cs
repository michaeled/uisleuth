using NUnit.Framework;
using UISleuth.Reflection;
using UISleuth.Tests.EmptyFakes;
using UISleuth.Widgets;

namespace UISleuth.Tests.Designer
{
    [TestFixture]
    public class TestDescriptorRegistrar
    {
        private DescriptorRegistrar Dr { get; set; }


        [SetUp]
        public void BeforeTest()
        {
            Dr = DescriptorRegistrar.Create(new TypeFinder());
            Dr.Reset();
        }


        [Test]
        public void Test_flags_and_staticlist()
        {
            Dr.Add(typeof(FakeEnumWithFlags), UIPropertyDescriptors.None);

            var t1 = FakeEnumWithFlags.Test1;

            var xRef = new UIReflectionProperty
            {
                TargetType = typeof (FakeEnumWithFlags),
            };

            var xProp = new UIProperty
            {
                Value = t1
            };

            Dr.SetPossibleValues(xRef, xProp, t1);

            Assert.IsTrue(xProp.UIType.Descriptor.HasFlag(UIPropertyDescriptors.Flags | UIPropertyDescriptors.Literals));
        }


        [Test]
        public void Test_Add_using_UIPropertyDescriptor()
        {
            Dr.Add(typeof (FakeImage), UIPropertyDescriptors.Image);
            var usingType = Dr.GetDescriptors(typeof (FakeImage));
            Assert.AreEqual(usingType, UIPropertyDescriptors.Image);
        }


        [Test]
        public void Test_Add_using_Generator()
        {
            Dr.Add(typeof(FakeImage), UIPropertyDescriptors.Image, new FakeImageGenerator());
        }
    }
}
