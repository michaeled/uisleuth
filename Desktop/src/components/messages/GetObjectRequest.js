export default class GetObjectRequest {
    constructor(widgetId, path) {
        this.widgetId = widgetId;
        this.path = path;
        this.action = "GetObjectRequest";
    }
}