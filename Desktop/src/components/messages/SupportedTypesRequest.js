export default class SupportedTypesRequest {
    constructor(types) {
        if (!types) throw "No types sent to designer.";

        this.action = "SupportedTypesRequest";
        this.types = types;
    }
}