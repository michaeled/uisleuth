import Editor from "../Editor";

export default class CharEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
    }

    get value() {
        if (super.value) {
            if (super.value.charCodeAt(0) === 0) {
                return null;
            }
        }

        return super.value;
    }

    set value(v) {
        if (v == "") {
            super.value = null;
        } else {
            super.value = v;
        }
    }
}