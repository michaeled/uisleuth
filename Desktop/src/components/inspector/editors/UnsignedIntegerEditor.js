import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class UnsignedIntegerEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/UnsignedInteger.htm`;
    }
}