using System;
using UISleuth.XAML;
using Xamarin.Forms;

namespace UISleuth
{
    internal interface IXamlLoader
    {
        TView Load<TView>(TView view, string xaml, Action<XamlParseErrorInfo> onError = null) where TView : BindableObject;
    }
}