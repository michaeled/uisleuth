using System;
using System.Collections.Generic;
using System.Linq;

namespace UISleuth.Widgets
{
    public static class UIWidgetExtensions
    {
        public static IEnumerable<UIWidget> GetNodeAndDescendants(this UIWidget widget)
        {
            var descendants = widget.Children.RecursiveSelect(c => c.Children).Where(c => c != null);
            return new[] {widget}.Concat(descendants);
        }


        internal static IEnumerable<TSource> RecursiveSelect<TSource>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TSource>> childSelector)
        {
            var stack = new Stack<IEnumerator<TSource>>();
            var enumerator = source.GetEnumerator();

            try
            {
                while (true)
                {
                    if (enumerator.MoveNext())
                    {
                        var element = enumerator.Current;
                        yield return element;

                        stack.Push(enumerator);
                        enumerator = childSelector(element).GetEnumerator();
                    }
                    else if (stack.Count > 0)
                    {
                        enumerator.Dispose();
                        enumerator = stack.Pop();
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
            finally
            {
                enumerator.Dispose();

                while (stack.Count > 0) // Clean up in case of an exception.
                {
                    enumerator = stack.Pop();
                    enumerator.Dispose();
                }
            }
        }
    }
}