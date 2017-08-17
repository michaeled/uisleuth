using System;

namespace UISleuth.Widgets
{
    [Flags]
    public enum UIPropertyDescriptors
    {
        None = 0,
        Image = 1,
        Literals = 2,
        Flags = 4,
        Static = 8,
        ValueType = 16,
        Enumerable = 32,
        Collection = 64,
        List = 128,
        AttachedProperty = 256
    }
}