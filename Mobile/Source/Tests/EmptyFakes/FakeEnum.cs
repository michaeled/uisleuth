using System;

namespace UISleuth.Tests.EmptyFakes
{
    [Flags]
    public enum FakeEnumWithFlags
    {
        None = 0,
        Test1 = 1,
        Test2 = 2,
        Test3 = 4
    }


    public enum FakeEnumWithOutFlags
    {
        None = 0,
        Test1 = 1,
        Test2 = 2,
        Test3 = 3
    }
}
