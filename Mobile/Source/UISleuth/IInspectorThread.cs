using System;

namespace UISleuth
{
    internal interface IInspectorThread
    {
        void Invoke(Action action);
    }
}