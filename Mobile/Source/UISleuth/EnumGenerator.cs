using System;

namespace UISleuth
{
    internal class EnumGenerator : IGenerateValues
    {
        public string[] Get(Type t)
        {
            return t == null ? null : Enum.GetNames(t);
        }
    }
}