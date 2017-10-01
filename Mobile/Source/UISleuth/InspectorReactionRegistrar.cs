using System;
using UISleuth.Messages;
using UISleuth.Reflection;
using UISleuth.Widgets;
using UISleuth.Reactions;
using Xamarin.Forms;

namespace UISleuth
{
    internal class InspectorReactionRegistrar
    {
        internal InspectorReactionRegistrar(ITypeFinder typeFinder)
        {
            var dr = DescriptorRegistrar.Create(typeFinder);

            dr.Add(typeof(ValueType), new StaticGenerator());
            dr.Add(typeof(Enum), new EnumGenerator());
            dr.Add(typeof(ImageSource), UIPropertyDescriptors.Image);
        }


        internal void Init(Page page)
        {
            Reaction.Reset();

            Reaction.Register<SupportedTypesRequest, SupportedTypesReaction>();
            InspectorReaction.Register<GetVisualTreeRequest, GetVisualTreeReaction>(page);
            InspectorReaction.Register<CreateWidgetRequest, CreateWidgetReaction>(page);
            InspectorReaction.Register<GetObjectRequest, GetObjectReaction>(page);
            InspectorReaction.Register<GetWidgetPropertiesRequest, GetWidgetPropertiesReaction>(page);
            InspectorReaction.Register<EditCollectionRequest, EditCollectionReaction>(page);
            InspectorReaction.Register<SetPropertyRequest, SetPropertyReaction>(page);
            InspectorReaction.Register<DeleteWidgetRequest, DeleteWidgetReaction>(page);
            InspectorReaction.Register<GetWidgetEventsRequest, GetWidgetEventsReaction>(page);
            InspectorReaction.Register<CreateStackLayoutRequest, CreateStackLayoutReaction>(page);
            InspectorReaction.Register<CreateGridRequest, CreateGridReaction>(page);
            InspectorReaction.Register<GetConstructorsRequest, GetConstructorsReaction>(page);
            InspectorReaction.Register<CallConstructorRequest, CallConstructorReaction>(page);
            InspectorReaction.Register<GetAttachedPropertiesRequest, GetAttachedPropertiesReaction>(page);
            InspectorReaction.Register<AddSupportedTypeRequest, AddSupportedTypesReaction>(page);
            InspectorReaction.Register<TouchScreenRequest, TouchScreenReaction>(page);
            InspectorReaction.Register<GetDisplayDimensionsRequest, GetDisplayDimensionsReaction>(page);
            InspectorReaction.Register<SetParentRequest, SetParentReaction>(page);
            InspectorReaction.Register<GestureRequest, GestureReaction>(page);
            InspectorReaction.Register<DesktopReady, DesktopReadyReaction>(page);
            InspectorReaction.Register<GetVisualElementsRequest, GetVisualElementsReaction>(page);
            InspectorReaction.Register<GetBindingContextRequest, GetBindingContextReaction>(page);
            InspectorReaction.Register<TraceEventsRequest, TraceEventsReaction>(page);
            InspectorReaction.Register<SetDeviceOrientationRequest, SetDeviceOrientationReaction>(page);
            InspectorReaction.Register<GetNavigablePagesRequest, GetNavigablePagesReaction>(page);

            Reaction.Register<ScreenShotRequest, ScreenShotReaction>();
        }
    }
}