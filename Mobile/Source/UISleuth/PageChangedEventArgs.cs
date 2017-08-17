using System;
using Xamarin.Forms;

namespace UISleuth
{
    internal class PageChangedEventArgs : EventArgs
    {
        public Page NewPage { get; set; }
    }
}