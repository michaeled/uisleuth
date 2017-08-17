import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class SizeEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
        this._gorToken = null;

        this._width = {};
        this._height = {};
    }

    get width() {
        return this._width.value;
    }

    set width(v) {
        if (angular.isObject(v)) {
            this._width = v;
            return;
        }

        if (this._width.value === v) return;
        this._width.value = v;
        this.propertyChanged([this.name, "Width"], v);
    }

    get height() {
        return this._height.value;
    }

    set height(v) {
        if (angular.isObject(v)) {
            this._height = v;
            return;
        }

        if (this._height.value === v) return;
        this._height.value = v;
        this.propertyChanged([this.name, "Height"], v);
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

    _setProperties(d) {
        let properties = d.property.value;

        let wProp = this._getProperty(properties, "Width", 1);
        let hProp = this._getProperty(properties, "Height", 1);

        if (wProp) {
            wProp.value = parseFloat(wProp.value);
            this.width = wProp;
        }

        if (hProp) {
            hProp.value = parseFloat(hProp.value);
            this.height = hProp;
        }
    }
}