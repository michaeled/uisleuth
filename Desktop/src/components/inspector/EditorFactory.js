import Editor from "./Editor";
import TypeDescriptor from "./editors/TypeDescriptor";
import ColorEditor from "./editors/ColorEditor";
import RotationEditor from "./editors/RotationEditor";
import ThicknessEditor from "./editors/ThicknessEditor";
import SizeRequestEditor from "./editors/SizeRequestEditor";
import TextAlignmentEditor from "./editors/TextAlignmentEditor";
import RectangleEditor from "./editors/RectangleEditor";
import LayoutOptionsEditor from "./editors/LayoutOptionsEditor";
import EditorCollection from "./EditorCollection";
import OpacityEditor from "./editors/OpacityEditor";
import AnchorEditor from "./editors/AnchorEditor";
import FlagsEditor from "./editors/FlagsEditor";
import EnumEditor from "./editors/EnumEditor";
import PointEditor from "./editors/PointEditor";
import SizeEditor from "./editors/SizeEditor";
import UnsignedIntegerEditor from "./editors/UnsignedIntegerEditor";
import CharEditor from "./editors/CharEditor";

export default class EditorFactory {
    constructor(connection) {
        this._connection = connection;

        console.trace("editor factory init.");
    }

    createAttached(widget, $scope) {
        let editors = new EditorCollection();
        let id = widget.id;
        let skipped = [];

        for (let property of widget.attachedProperties) {
            let editor = this._createEditor(id, property);
            editor.readonly = !property.canWrite;
            editor._connection = this._connection;

            if (editor.skip) {
                skipped.push(editor);
            } else {
                editors.push(editor);
            }
        }

        // give each editor a reference to all the widget's properties
        for (let editor of editors) {
            editor.parent.attachedProperties = editors;
            editor.parent.skippedAttachedProperties = skipped;
            editor.isAttachedProperty = true;

            // call ready on each editor
            editor.onReady($scope);
        }

        return editors;
    }

    create(propertyResponse, $scope) {
        let editors = new EditorCollection();

        if (!propertyResponse.widget) {
            console.error("Unable to create editor for property.", propertyResponse);
            return;
        }

        let id = propertyResponse.widget.id;
        let skipped = [];

        for (let property of propertyResponse.properties) {
            let editor = this._createEditor(id, property);
            editor.readonly = !property.canWrite;
            editor._connection = this._connection;

            if (editor.skip) {
                skipped.push(editor);
            } else {
                editors.push(editor);
            }
        }

        // give each editor a reference to all the widget's properties
        for (let editor of editors) {
            editor.parent.widget = propertyResponse.widget;
            editor.parent.properties = editors;
            editor.parent.skippedProperties = skipped;

            // call ready on each editor
            editor.onReady($scope);
        }

        return editors;
    }

    _createEditor(id, property) {
        let type = property.uiType.fullName || '';
        let name = property.path[property.path.length - 1];
        let descriptor = property.uiType.descriptor;

        // skip anchorx; only use anchory
        if (type === "System.Double" && name === "AnchorX") {
            let anchorx = new Editor(id, property, { property: 0.5 });
            anchorx.skip = true;
            return anchorx;
        }

        if (type === "System.Single") {
            return new Editor(id, property);
        }

        if (type === "System.Char") {
            return new CharEditor(id, property);
        }

        if (type === "System.Double" && name === "AnchorY") {
            return new AnchorEditor(id, property, { value: 0.5 });
        }

        if (name.startsWith("Rotation")) {
            return new RotationEditor(id, property, { value: 0 });
        }

        if (type === "System.Double" && name.endsWith("Opacity")) {
            return new OpacityEditor(id, property);
        }

        if (type === "Xamarin.Forms.Color") {
            return new ColorEditor(id, property);
        }

        if (type === "Xamarin.Forms.Thickness") {
            return new ThicknessEditor(id, property);
        }

        if (type === "System.Double" && name.endsWith("Request")) {
            return new SizeRequestEditor(id, property, { value: -1 });
        }

        if (type === "Xamarin.Forms.TextAlignment") {
            return new TextAlignmentEditor(id, property);
        }

        if (type === "Xamarin.Forms.Rectangle") {
            return new RectangleEditor(id, property);
        }

        if (type === "Xamarin.Forms.LayoutOptions") {
            return new LayoutOptionsEditor(id, property);
        }

        if (type === "Xamarin.Forms.Point") {
            return new PointEditor(id, property);
        }

        if (type === "Xamarin.Forms.Size") {
            return new SizeEditor(id, property);
        }

        switch (type) {
            case "System.UInt16":
            case "System.UInt32":
            case "System.UInt64":
                return new UnsignedIntegerEditor(id, property);
        }

        if ((descriptor & TypeDescriptor.Flags) && (descriptor & TypeDescriptor.Literals)) {
            return new FlagsEditor(id, property);
        }

        // todo: better way?
        if (descriptor & TypeDescriptor.Literals && !type.startsWith("System")) {
            return new EnumEditor(id, property);
        }

        return new Editor(id, property);
    }
}

EditorFactory.$inject = ["ConnectionService"];