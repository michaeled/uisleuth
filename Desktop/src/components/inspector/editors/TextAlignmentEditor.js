import Editor from "../Editor";

export default class TextAlignmentEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
    }

    start($event) {
        $event.preventDefault();
        this.value = "Start";
        this.changed();
    }

    center($event) {
        $event.preventDefault();
        this.value = "Center";
        this.changed();
    }

    end($event) {
        $event.preventDefault();
        this.value = "End";
        this.changed();
    }

    get isStart() {
        return this._pv === "Start" || this.value === "Start";
    }

    get isCenter() {
        return this._pv === "Center" || this.value === "Center";
    }

    get isEnd() {
        return this._pv === "End" || this.value === "End";
    }

    get _pv() {
        return this._designer.uiType.possibleValues[this.value];
    }
}