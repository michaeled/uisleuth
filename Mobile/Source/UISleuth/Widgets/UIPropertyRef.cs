using UISleuth.Reflection;

namespace UISleuth.Widgets
{
    public class UIPropertyRef : IShallowClone<UIPropertyRef>
    {
        public string PropertyName { get; set; }
        public UIPropertyRef Parent { get; set; }
        public int? EnumerableIndex { get; set; }
        public bool IsEnumerable { get; set; }


        public UIPropertyRef ShallowClone()
        {
            return new UIPropertyRef
            {
                PropertyName = PropertyName,
                Parent = Parent.ShallowClone(),
                EnumerableIndex = EnumerableIndex,
                IsEnumerable = IsEnumerable,
            };
        }
    }
}