using UISleuth.Messages;
using UISleuth.Networking;
using Xamarin.Forms;

namespace UISleuth.Reactions
{
    internal class CreateGridReaction : InspectorReaction
    {
        protected override void OnExecute(UIMessageContext ctx)
        {
            var req = ctx.Get<CreateGridRequest>();
            if (req == null) return;

            var target = Surface[req.ParentId];
            if (target == null) return;

            var attached = false;
            var rowCollection = new RowDefinitionCollection();
            var colCollection = new ColumnDefinitionCollection();

            for (var i = 0; i < req.Rows; i++)
            {
                var row = new RowDefinition();
                rowCollection.Add(row);
            }

            for (var j = 0; j < req.Columns; j++)
            {
                var col = new ColumnDefinition();
                colCollection.Add(col);
            }

            var view = new Grid
            {
                RowDefinitions = rowCollection,
                ColumnDefinitions = colCollection,
                ColumnSpacing = req.ColumnSpacing,
                RowSpacing = req.RowSpacing
            };

            Thread.Invoke(() =>
            {
                attached = Surface.SetParent(view, target);
            });

            if (!attached) return;

            var pair = Surface[view.Id];

            ctx.SetResponse<CreateWidgetResponse>(res =>
            {
                res.Widget = pair.UIWidget;
                res.Parent = target.UIWidget;
                res.Suggest<GetVisualTreeRequest>();
            });
        }
    }
}
