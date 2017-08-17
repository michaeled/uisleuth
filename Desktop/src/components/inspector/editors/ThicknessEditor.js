import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class ThicknessEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this._top = {};
        this._left = {};
        this._right = {};
        this._bottom = {};

        this._gorToken = null;
    }

    get top() {
        return this._top.value;
    }

    set top(v) {
        if (angular.isObject(v)) {
            this._top = v;
            return;
        }

        if (this._top.value === v) return;
        this._top.value = v;
        this.propertyChanged([this.name, "Top"], v);
    }

    get left() {
        return this._left.value;
    }

    set left(v) {
        if (angular.isObject(v)) {
            this._left = v;
            return;
        }

        if (this._left.value === v) return;
        this._left.value = v;
        this.propertyChanged([this.name, "Left"], v);
    }

    get right() {
        return this._right.value;
    }

    set right(v) {
        if (angular.isObject(v)) {
            this._right = v;
            return;
        }

        if (this._right.value === v) return;
        this._right.value = v;
        this.propertyChanged([this.name, "Right"], v);
    }

    get bottom() {
        return this._bottom.value;
    }

    set bottom(v) {
        if (angular.isObject(v)) {
            this._bottom = v;
            return;
        }

        if (this._bottom.value === v) return;
        this._bottom.value = v;
        this.propertyChanged([this.name, "Bottom"], v);
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

        let topProp = this._getProperty(properties, "Top");
        let leftProp = this._getProperty(properties, "Left");
        let rightProp = this._getProperty(properties, "Right");
        let bottomProp = this._getProperty(properties, "Bottom");

        if (topProp) this.top = topProp;
        if (leftProp) this.left = leftProp;
        if (rightProp) this.right = rightProp;
        if (bottomProp) this.bottom = bottomProp;
    }

    _getProperty(properties, name) {
        let property = super._getProperty(properties, name, 1);

        if (property) {
            property.value = parseInt(property.value);
        }

        return property;
    }
}