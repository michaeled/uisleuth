using System;

namespace UISleuth.Reflection
{
    internal struct TypeAttributeAssociation<T> where T : Attribute
    {
        public T[] Attributes { get; set; }
        public Type DecoratedType { get; set; }
    }


    internal interface IFindCustomAttributes<T> where T : Attribute
    {
        TypeAttributeAssociation<T>[] FindAll();
    }
}