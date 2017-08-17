namespace UISleuth.Reflection
{
    internal interface IShallowClone<out T>
    {
        T ShallowClone();
    }
}