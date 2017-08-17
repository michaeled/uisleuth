export default class SetPropertyRequest {
    constructor(widgetId, value, path, isAttachedProperty) {
        this.widgetId = widgetId;
        this.value = value;
        this.path = path;
        this.isAttachedProperty = isAttachedProperty;
        this.action = "SetPropertyRequest";
    }
}