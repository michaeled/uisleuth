export default class CreateWidgetRequest {
    constructor(parentId, typeName) {
        this.parentId = parentId;
        this.typeName = typeName;
        this.action = "CreateWidgetRequest";
    }
}