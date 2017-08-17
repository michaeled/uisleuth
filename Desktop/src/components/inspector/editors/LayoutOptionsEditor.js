import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class LayoutOptionsEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this.expands = {};
        this.alignment = {};

        this._gorToken = null;
    }

    onReady($scope) {
        let req = new GetObjectRequest(this.widgetId, [this.name]);

        this._gorToken = PubSub.subscribe(this.topic, (e, d) => {
            this._setProperties(d);
            $scope.$apply();
        });

        this._connection.send(req);
    }

    onDestroy() {
        PubSub.unsubscribe(this._gorToken);
    }

    start($event) {
        $event.preventDefault();
        this.alignment.value = "Start";
        this.propertyChanged(this.alignment.path, this.alignment.value);
    }

    center($event) {
        $event.preventDefault();
        this.alignment.value = "Center";
        this.propertyChanged(this.alignment.path, this.alignment.value);
    }

    end($event) {
        $event.preventDefault();
        this.alignment.value = "End";
        this.propertyChanged(this.alignment.path, this.alignment.value);
    }

    fill($event) {
        $event.preventDefault();
        this.alignment.value = "Fill";
        this.propertyChanged(this.alignment.path, this.alignment.value);
    }

    expand() {
        this.propertyChanged(this.expands.path, this.expands.value);
    }

    _setProperties(d) {
        let properties = d.property.value;

        let expandsProp = this._getProperty(properties, "Expands", 1);
        let alignmentProp = this._getProperty(properties, "Alignment", 1);

        if (expandsProp) {
            this.expands = expandsProp;
            this.expands.value = this.expands.value.toLocaleLowerCase() === "true";
        }

        if (alignmentProp) this.alignment = alignmentProp;
    }
}