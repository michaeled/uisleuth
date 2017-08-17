namespace UISleuth
{
    internal enum UIDeviceOrientation
    {
        Unknown,
        Landscape,
        ReverseLandscape,
        Portrait,
        ReversePortrait
    }

    internal interface IDeviceOrientation
    {
        void SetOrientation(UIDeviceOrientation orientation);
        UIDeviceOrientation GetOrientation();
    }
}