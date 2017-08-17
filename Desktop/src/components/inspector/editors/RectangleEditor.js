import Editor from "../Editor";
import GetObjectRequest from "../../messages/GetObjectRequest";

export default class RectangleEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        this.top = {};
        this.left = {};
        this.right = {};
        this.bottom = {};
        this.width = {};
        this.height = {};
        this.x = {};
        this.y = {};
        this.isEmpty = {};

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

    _setProperties(d) {
        let properties = d.property.value;

        let topProp = this._getProperty(properties, "Top");
        let leftProp = this._getProperty(properties, "Left");
        let rightProp = this._getProperty(properties, "Right");
        let bottomProp = this._getProperty(properties, "Bottom");
        let heightProp = this._getProperty(properties, "Height");
        let widthProp = this._getProperty(properties, "Width");
        let xProp = this._getProperty(properties, "X");
        let yProp = this._getProperty(properties, "Y");
        let isEmptyProp = this._getProperty(properties, "IsEmpty");

        if (topProp) this.top = topProp;
        if (leftProp) this.left = leftProp;
        if (rightProp) this.right = rightProp;
        if (bottomProp) this.bottom = bottomProp;
        if (heightProp) this.height = heightProp;
        if (widthProp) this.width = widthProp;
        if (xProp) this.x = xProp;
        if (yProp) this.y = yProp;
        if (isEmptyProp) this.isEmpty = isEmptyProp;
    }

    _getProperty(properties, name) {
        let property = _.find(properties, v => {
            if (!v) return false;
            if (!v.path) return false;
            if (!v.path.length === 2) return false;
            return v.path[1] === name
        });

        if (property) {
            property.value = parseInt(property.value);
        }

        return property;
    }
}