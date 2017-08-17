using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Xamarin.Forms;
using UISleuth.Reflection;

namespace UISleuth.Tests.Reflection
{
    [TestFixture]
    public class TestUIReflection
    {
        public struct TestValueType
        {
            public string String1 { get; set; }
            public string String2 { get; set; }
            public int Int1 { get; set; }
            public int Int2 { get; set; }
            public DateTime D2 { get; set; }
            private DateTime D1 { get; set; }
        }


        [Test]
        public void Test_ParseEnumerableProperty()
        {
            // Doesn't have an indexer
            int? nIndex;
            string nStripped;
            var noIndexer = ReflectionMethods.ParseIndexer("Normal", out nIndex, out nStripped);
            Assert.IsFalse(noIndexer);
            Assert.IsNull(nIndex);
            Assert.AreEqual("Normal", nStripped);

            // Has an indexer
            int? yIndex;
            string yStripped;
            var yesIndexer = ReflectionMethods.ParseIndexer("Columns[5]", out yIndex, out yStripped);
            Assert.IsTrue(yesIndexer);
            Assert.AreEqual(5, yIndex);
            Assert.AreEqual("Columns", yStripped);
        }


        [Test]
        public void Test_GetIndexerValue()
        {
            var nullArg = ReflectionMethods.GetIndexerValue(null);
            Assert.IsNull(nullArg);

            var noIndexer = ReflectionMethods.GetIndexerValue("TextColor");
            Assert.IsNull(noIndexer);

            var yesIndexer = ReflectionMethods.GetIndexerValue("Rows[555]");
            Assert.AreEqual(555, yesIndexer);
        }


        [Test]
        public void Test_StripIndexer()
        {
            var nullArg = ReflectionMethods.StripIndexer(null);
            Assert.IsNull(nullArg);

            var noIndexer = ReflectionMethods.StripIndexer("MyTest");
            Assert.AreEqual("MyTest", noIndexer);

            var yesIndexer = ReflectionMethods.StripIndexer("Rows[5]");
            Assert.AreEqual("Rows", yesIndexer);
        }


        [Test]
        public void Test_GetPrimitiveValueTypeProperties()
        {
            var v1 = new TestValueType();
            var v1Props = UIPropertyMethods.GetPrimitiveValueTypeProperties(v1);
            Assert.AreEqual(2, v1Props.Length);

            var int1 = v1Props.FirstOrDefault(p => p.PropertyName == "Int1");
            var int2 = v1Props.FirstOrDefault(p => p.PropertyName == "Int2");

            Assert.IsNotNull(int1, "Int1");
            Assert.IsNotNull(int2, "Int2");
        }


        [Test]
        public void Test_IsNotPrimitiveValueType()
        {
            var v1 = new TestValueType();
            Assert.IsTrue(ReflectionMethods.IsNotPrimitiveValueType(v1));

            var v2 = 0;
            Assert.IsFalse(ReflectionMethods.IsNotPrimitiveValueType(v2));
        }


        [Test]
        public void Test_GetEnumerableElementAt()
        {
            var array1 = new[] {"i1", "i2", "i3"};
            Assert.AreEqual("i1", ReflectionMethods.GetItem(array1, 0));
            Assert.AreEqual("i2", ReflectionMethods.GetItem(array1, 1));

            var al1 = new ArrayList(2) {5, "test2"};
            Assert.AreEqual(5, ReflectionMethods.GetItem(al1, 0));
            Assert.AreEqual("test2", ReflectionMethods.GetItem(al1, 1));

            var dict1 = new Dictionary<string, string>
            {
                {"key1", "value1"},
                {"key2", "value2"}
            };

            Assert.IsNotNull(ReflectionMethods.GetItem(dict1, 0));

            var list1 = new List<string> { "list1", "list2" };
            Assert.AreEqual("list1", ReflectionMethods.GetItem(list1, 0));
            Assert.AreEqual("list2", ReflectionMethods.GetItem(list1, 1));
        }


        [Test]
        public void Test_IsValueType()
        {
            Assert.IsTrue(ReflectionMethods.IsValueType(typeof (DateTime)));
            Assert.IsTrue(ReflectionMethods.IsValueType(typeof (Int32)));
            Assert.IsTrue(ReflectionMethods.IsValueType(typeof (TestValueType)));
            Assert.IsFalse(ReflectionMethods.IsValueType(typeof(string)));
            Assert.IsFalse(ReflectionMethods.IsValueType(typeof(Exception)));
        }


        [Test]
        public void Test_GetShortTypeName()
        {
            const string aqtn = @"System.Web.Security.SqlMembershipProvider, System.Web, Version=2.0.0.0,Culture=neutral,PublicKeyToken=b03f5f7f11d50a3a";

            Assert.AreEqual(string.Empty, ReflectionMethods.GetShortTypeName(null));
            Assert.AreEqual(string.Empty, ReflectionMethods.GetShortTypeName(string.Empty));
            Assert.AreEqual("ClassName", ReflectionMethods.GetShortTypeName("ClassName"));
            Assert.AreEqual("ExceptionMessage", ReflectionMethods.GetShortTypeName("Namespace1.Namespace2.ExceptionMessage"));
            Assert.AreEqual("SqlMembershipProvider", ReflectionMethods.GetShortTypeName(aqtn));
        }


        [Test]
        public void Test_GetUIRefProperties()
        {
            var label = new Label();
            var refProps = label.GetRefProperties();
            CollectionAssert.IsNotEmpty(refProps);
        }


        [Test]
        public void Test_GetUIRefProperties_with_simple_Path()
        {
            var page = new ContentPage();
            var cv1 = new ContentView();
            var cv2 = new ContentView();
            var label = new Label { Text = "Hello!" };

            cv2.Content = label;
            cv1.Content = cv2;
            page.Content = cv1;
            
            // should be path to the label
            var props = page.GetRefProperties(new[] { "Content", "Content", "Content"});
            var l = props[0].GetTargetObject() as Label;

            Assert.IsNotNull(l);
            Assert.AreEqual("Hello!", l.Text);
        }


        [Test]
        public void Test_GetUIRefProperties_with_single_path()
        {
            var label = new Label();
            var props = label.GetRefProperties(new[] {"HorizontalOptions"});

            CollectionAssert.IsNotEmpty(props);
            Assert.IsNotNull(props.First(p => p.TargetName == "HorizontalOptions"));
        }


        [Test]
        public void Test_GetUIRefProperties_with_null_path()
        {
            var page = new ContentPage();
            Assert.Throws<InvalidOperationException>(() => page.GetRefProperties(null));
        }


        [Test]
        public void Test_GetUIRefProperties_with_indexer()
        {
            var grid = new Grid();
            var row1 = new RowDefinition {Height = new GridLength(1)};
            var row2 = new RowDefinition {Height = new GridLength(2)};

            grid.RowDefinitions.Add(row1);
            grid.RowDefinitions.Add(row2);

            var page = new ContentPage {Content = grid};

            // row 1 assert
            var row1HeightProps = page.GetRefProperties(new[] {"Content", "RowDefinitions[0]", "Height"});
            CollectionAssert.IsNotEmpty(row1HeightProps);

            var height1Prop = row1HeightProps.First(p => p.TargetName == "Height");
            var height1 = (GridLength) height1Prop.GetTargetObject();
            Assert.AreEqual(1.0D, height1.Value);

            // row 2 assert
            var row2HeightProps = page.GetRefProperties(new[] { "Content", "RowDefinitions[1]", "Height" });
            CollectionAssert.IsNotEmpty(row2HeightProps);

            var height2Prop = row2HeightProps.First(p => p.TargetName == "Height");
            var height2 = (GridLength) height2Prop.GetTargetObject();
            Assert.AreEqual(2.0D, height2.Value);
        }
    }
}