using System;

namespace UISleuth.Tests.Reactions.EmptyFakes
{
    public class FakeScreenShot : IScreenShot
    {
        public byte[] Capture()
        {
            throw new NotImplementedException();
        }
    }
}