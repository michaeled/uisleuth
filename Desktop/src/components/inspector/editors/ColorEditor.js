import Editor from "../Editor";

export default class ColorEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this._prev = null;
        this._$scope = null;

        this.config.color = {
            onChange: this.colorChanged.bind(this),
            onClear: this.colorCleared.bind(this)
        };
    }

    get value() {
        return this._value === 0 ? "" : this._value;
    }

    set value(v) {
        if (this._prev != v) {
            this._value = v;
            this.changed();
        }
    }

    colorCleared(api, color, $event) {
        this.value = 0;
    }

    colorChanged(api, color, $event) {}

    togglePossibles() {
        super.togglePossibles();
        $(`#cp-${this.index}`).selectpicker("render");
    }

    onBound() {
        super.onBound();

        // prevent multiple event bindings
        if (ColorEditor._alreadyBound) {
            return;
        } else {
            ColorEditor._alreadyBound = true;
        }

        $("#tab-properties").on("changed.bs.select", ".color-editor .selectpicker", (e, newval, oldval) => {
            let val = e.target.options[newval].value;
            let scope = angular.element(e.target).scope();
            let editor = scope.editor;

            editor.value = this._colorValue(val);
            scope.$apply();
            e.target.focus();
        });
    }

    _colorValue(name) {
        switch (name) {
            case "Transparent":
                return "";
            case "Aqua":
                return "#00FFFF";
            case "Navy":
                return "#000080";
            case "Blue":
                return "#0000FF";
            case "Fuschia":
                return "#FF00FF";
            case "Gray":
                return "#808080";
            case "Lime":
                return "#00FF00";
            case "Pink":
                return "#FF66FF";
            case "Red":
                return "#FF0000";
            case "Teal":
                return "#008080";
            default:
                return name;
        }
    }
}

ColorEditor._alreadyBound = false;