export default class SetParentRequest {
    public static SetParentEvent = "SetParentRequest.SetParentEvent";
    public action: string = "SetParentRequest";

    constructor(public childId: string, public parentId: string, public index: number) {
        // ignore
    }
}