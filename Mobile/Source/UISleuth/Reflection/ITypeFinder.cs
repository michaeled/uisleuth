using System;

namespace UISleuth.Reflection
{
    internal interface ITypeFinder
    {
        Type Find(string typeName);
    }
}