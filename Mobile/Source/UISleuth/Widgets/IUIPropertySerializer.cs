using System;

namespace UISleuth.Widgets
{
    public interface IUIPropertySerializer
    {
        Type Type { get; }
        object Convert(string json);
    }
}