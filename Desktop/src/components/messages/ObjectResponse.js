export default class ObjectResponse {
    constructor(obj) {
        this.object = obj;
    }

    get topic() {
        return ObjectResponse.topic(this.object);
    }
}

ObjectResponse.topic = function(obj) {
    return `${this.widgetId}.${this.objectName}`;
}