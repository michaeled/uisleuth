using System;

namespace UISleuth.Reflection
{
    /// <summary>
    /// Using reflection, implement methods that can find .NET object types.
    /// The reflection API is limited for PCLs, so we abstract those implementation details.
    /// </summary>
    internal interface IUIMessageFinder
    {
        /// <summary>
        /// Find and return a .NET type that matches a given <paramref name="typeName"/>.
        /// </summary>
        Type Find(string typeName);
    }
}