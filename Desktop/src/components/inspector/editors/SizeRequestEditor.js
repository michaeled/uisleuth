import Editor from "../Editor";

export default class SizeRequestEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
    }

    get placeholder() {
        if (this._value === -1) {
            return "Default";
        }

        return null;
    }

    get value() {
        if (this._value === -1) return null;
        return this._value;
    }

    set value(v) {
        super.value = v;
    }
}