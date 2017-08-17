var env = require('./env');
import SetPropertyRequest from "../messages/SetPropertyRequest";

export default class Editor {
    constructor(widgetId, property, defaults = { value: undefined }) {
        this._widgetId = widgetId;
        this._designer = property;
        this._connection = null;
        this._possibles = [];
        this._placeholder = null;
        this._defaults = defaults;
        this._chosenPossible = null;
        this._index = null;
        this._bound = false;
        this._readonly = false;
        this._isAttachedProperty = false;

        this._parent = {
            widget: null,
            properties: null
        };

        this.config = {};
        this.showPossibles = false;

        // set default value
        this._value = property.value;
    }

    get isAttachedProperty() {
        return this._isAttachedProperty;
    }

    set isAttachedProperty(v) {
        this._isAttachedProperty = v;
    }

    get readonly() {
        return this._readonly;
    }

    set readonly(v) {
        this._readonly = v;
    }

    get index() {
        return this._index;
    }

    set index(v) {
        this._index = v;
    }

    get parent() {
        return this._parent;
    }

    set parent(v) {
        this._parent = v;
    }

    set chosenPossible(v) {
        this._chosenPossible = v;
    }

    get chosenPossible() {
        return this._chosenPossible;
    }

    get possibles() {
        return this._designer.uiType.possibleValues;
    }

    get widgetId() {
        return this._widgetId;
    }

    get name() {
        return this._designer.path[0];
    }

    get type() {
        return this._designer.uiType.fullName;
    }

    get value() {
        return this._value;
    }

    set value(v) {
        this._value = v;
    }

    get template() {
        if (!this._bound) {
            this.onBound();
            this._bound = true;
        }

        return `components/inspector/editors/${this.type}.htm`;
    }

    get placeholder() {
        return this._placeholder;
    }

    set placeholder(v) {
        this._placeholder = v;
    }

    get togglePossibleLabel() {
        return this.showPossibles ? "Hide Builtins" : "Show Builtins";
    }

    get defaults() {
        return this._defaults;
    }

    get hasDefaults() {
        if (this.defaults === null) return false;
        if (this.defaults.value === undefined) return false;
        return true;
    }

    get topic() {
        return Editor.topic(this.widgetId, this.name);
    }

    get helpUrl() {
        return `${env.help['xf.url.property']}${this.parent.widget.fullTypeName}.${this.name}`;
    }

    propertyChanged(path, value) {
        let req = new SetPropertyRequest(this.widgetId, value, path, this.isAttachedProperty);
        this._connection.send(req);

        PubSub.publish(Editor.PropertyChangedEvent, {
            propertyName: this.name,
            parentType: this.parent.widget.type,
            value: value,
        });
    }

    changed() {
        this.propertyChanged(this._designer.path, this._value);
    }

    togglePossibles() {
        this.showPossibles = !this.showPossibles;
    }

    setToDefault() {
        this.value = this.defaults.value;
    }

    static topic(id, name) {
        return `${id}.${name}`;
    }

    onReady($scope) {}
    onDestroy() {}
    onBound() {}

    _getProperty(properties, name, index) {
        let property = _.find(properties, v => {
            if (!v) return false;
            if (!v.path) return false;
            if (!v.path.length === index + 1) return false;
            return v.path[index] === name
        });

        return property;
    }
}

Editor.PropertyChangedEvent = "Editor.PCE";