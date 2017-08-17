using System;
namespace UISleuth.Tests.EmptyFakes
{
    public class FakeImageGenerator : IGenerateValues
    {
        public string[] Get(Type type)
        {
            return new[]
            {
                "Test1",
                "Test2"
            };
        }
    }
}