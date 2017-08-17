using System;
using UISleuth.XAML;
using Xamarin.Forms;

namespace UISleuth
{
    internal class XamlLoader : IXamlLoader
    {
        public TView Load<TView>(TView view, string xaml, Action<XamlParseErrorInfo> onError = null) where TView : BindableObject
        {
            return view.Load(xaml, onError);
        }
    }
}