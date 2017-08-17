using System;
using System.Diagnostics;
using System.Threading;
using Xamarin.Forms;

namespace UISleuth
{
    internal class InspectorThread : IInspectorThread
    {
        public void Invoke(Action action)
        {
            var done = false;

            Device.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    action();
                    done = true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine("UI Sleuth: An error occurred. " + e);
                }
            });

            SpinWait.SpinUntil(() => done, TimeSpan.FromSeconds(3));
        }
    }
}