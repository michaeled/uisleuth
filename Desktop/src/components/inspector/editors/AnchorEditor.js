import Editor from "../Editor";

export default class AnchorEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this._anchorX = defaults.value;
        this._anchorY = defaults.value;
    }

    onReady($scope) {
        let x = this.parent.skippedProperties.find(v => v.name === "AnchorX");
        let y = this.parent.properties.find(v => v.name === "AnchorY");

        if (x) {
            this.anchorX = x.value;
        }

        if (y) {
            this.anchorY = y.value;
        }
    }

    setToDefault() {
        this.anchorX = this.defaults.value;
        this.anchorY = this.defaults.value;
    }

    get output() {
        return (`(${this.anchorX}, ${this.anchorY})`);
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/Anchor.htm`;
    }

    get anchorX() {
        return this._anchorX;
    }

    set anchorX(v) {
        if (v === this._anchorX) return;

        this._anchorX = v;
        this.propertyChanged(["AnchorX"], v);
    }

    get anchorY() {
        return this._anchorY;
    }

    set anchorY(v) {
        if (v === this._anchorY) return;

        this._anchorY = v;
        this.propertyChanged(["AnchorY"], v);
    }
}