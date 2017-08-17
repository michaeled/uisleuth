import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class PointEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
        this._gorToken = null;

        this._x = {};
        this._y = {};
    }

    get x() {
        return this._x.value;
    }

    set x(v) {
        if (angular.isObject(v)) {
            this._x = v;
            return;
        }

        if (this._x.value === v) return;
        this._x.value = v;
        this.propertyChanged([this.name, "X"], v);
    }

    get y() {
        return this._y.value;
    }

    set y(v) {
        if (angular.isObject(v)) {
            this._y = v;
            return;
        }

        if (this._y.value === v) return;
        this._y.value = v;
        this.propertyChanged([this.name, "Y"], v);
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

        let xProp = this._getProperty(properties, "X", 1);
        let yProp = this._getProperty(properties, "Y", 1);

        if (xProp) {
            xProp.value = parseFloat(xProp.value);
            this.x = xProp;
        }

        if (yProp) {
            yProp.value = parseFloat(yProp.value);
            this.y = yProp;
        }
    }
}