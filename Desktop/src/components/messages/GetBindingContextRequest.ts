export default class GetBindingContextRequest {
    public action: string;
    
    constructor(public widgetId: string) {
        this.action = "GetBindingContextRequest";
    }
}