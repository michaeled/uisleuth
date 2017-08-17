using System;

namespace UISleuth
{
    [Flags]
    internal enum RegistrarMatches
    {
        None = 0,
        TypeName = 1, 
        Enum = 2
    }
}