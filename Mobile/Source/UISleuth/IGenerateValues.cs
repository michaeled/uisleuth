using System;

namespace UISleuth
{
    internal interface IGenerateValues
    {
        string[] Get(Type type);
    }
}