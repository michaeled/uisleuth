import Editor from "../Editor";

export default class RotationEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this._showNegatives = false;

        this.config.knob = {
            fontSize: 11,
            size: 100,
            min: 0,
            max: 360,
            skin: {
                type: 'tron',
                width: 5,
                color: '#E5E9F0',
                spaceWidth: 3
            },
            barColor: '#7AA0C7',
            trackWidth: 30,
            barWidth: 30,
            textColor: '#E5E9F0',
            step: 1,
        };
    }

    get value() {
        return super.value;
    }

    set value(v) {
        super.value = v;
        this.changed();
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/Rotation.htm`;
    }

    toggleNegatives() {
        this._showNegatives = !this._showNegatives;

        if (this._showNegatives) {
            this.config.knob.min = -360;
        } else {
            this.config.knob.min = 0;
        }
    }

    get negativeLabel() {
        if (this._showNegatives) {
            return "Disable Negatives";
        }
        return "Enable Negatives";
    }

    _isNumeric(n) {
        return !isNaN(parseFloat(n)) && isFinite(n);
    }
}