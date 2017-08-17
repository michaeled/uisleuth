using System;
namespace UISleuth.Tests.Reactions.EmptyFakes
{
    public class FakeInspectorThread : IInspectorThread
    {
        public void Invoke(Action action)
        {
            action();
        }
    }
}