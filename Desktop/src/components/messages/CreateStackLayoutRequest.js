export default class CreateStackLayoutRequest {
    constructor(parentId, orientation, spacing) {
        this.parentId = parentId;
        this.orientation = orientation;
        this.spacing = spacing;
        this.action = "CreateStackLayoutRequest";
    }
}