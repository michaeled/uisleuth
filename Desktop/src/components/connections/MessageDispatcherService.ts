declare var PubSub;

import ConnectionService from './ConnectionService';
import ObjectResponse from '../messages/ObjectResponse';

export default class MessageDispatcherService {
    public static MobileReady : string = "MDS.MobileReady";
    public static ScreenShotResponse : string = "MDS.ScreenShotResponse";
    public static GetVisualTreeResponse : string = "MDS.GetVisualTreeResponse";
    public static GetWidgetPropertiesResponse : string = "MDS.GetWidgetPropertiesResponse";
    public static GetDisplayDimensionsResponse : string = "MDS.GetDisplayDimensionsResponse";
    public static GetWidgetEventsResponse : string = "MDS.GetWidgetEventsResponse";
    public static GetAttachedPropertiesResponse : string = "MDS.GetAttachedPropertiesResponse";
    public static SetParentResponse : string= "MDS.SetParentResponse";
    public static PageChanged : string = "MDS.PageChanged";
    public static GetVisualElementsResponse : string = "MDS.GetVisualElementsResponse";
    public static GetBindingContextResponse : string = "MDS.GetBindingContextResponse";
    public static SetDeviceOrientationResponse : string = "MDS.SetDeviceOrientationResponse";

    constructor() {
        PubSub.subscribe(ConnectionService.MessageReceivedEvent, this.dispatch);
        console.trace("message dispatcher init.");
    }

    private dispatch(error, data : string) {
        let inc = JSON.parse(data);
        if (inc == null || !inc.action) throw `Incoming message for ${error} was null. Action was ${inc.action}.`;

        console.trace(`dispatching message ${inc.action}.`);
        switch (inc.action) {
            case "MobileReady":
                PubSub.publish(MessageDispatcherService.MobileReady, true);
                break;
            case "ScreenShotResponse":
                PubSub.publish(MessageDispatcherService.ScreenShotResponse, inc);
                break;
            case "GetVisualTreeResponse":
                PubSub.publish(MessageDispatcherService.GetVisualTreeResponse, inc);
                break;
            case "GetWidgetPropertiesResponse":
                PubSub.publish(MessageDispatcherService.GetWidgetPropertiesResponse, inc);
                break;
            case "GetDisplayDimensionsResponse":
                PubSub.publish(MessageDispatcherService.GetDisplayDimensionsResponse, inc);
                break;
            case "GetWidgetEventsResponse":
                PubSub.publish(MessageDispatcherService.GetWidgetEventsResponse, inc);
                break;
            case "GetAttachedPropertiesResponse":
                PubSub.publish(MessageDispatcherService.GetAttachedPropertiesResponse, inc);
                break;
            case "SetParentResponse":
                PubSub.publish(MessageDispatcherService.SetParentResponse, inc);
                break;
            case "ObjectResponse":
                let topic = `${inc.widgetId}.${inc.objectName}`;
                PubSub.publish(topic, inc);
                break;
            case "PageChanged":
                PubSub.publish(MessageDispatcherService.PageChanged, inc);
                break;
            case "GetVisualElementsResponse":
                PubSub.publish(MessageDispatcherService.GetVisualElementsResponse, inc);
                break;
            case "GetBindingContextResponse":
                PubSub.publish(MessageDispatcherService.GetBindingContextResponse, inc);
                break;
            case "SetDeviceOrientationResponse":
                PubSub.publish(MessageDispatcherService.SetDeviceOrientationResponse, inc);
                break;
            default:
                console.trace(`unhandled message: ${inc.action}.`);
                break;
        }
    }
}