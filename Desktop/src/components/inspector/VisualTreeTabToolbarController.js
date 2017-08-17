import GetVisualTreeRequest from '../messages/GetVisualTreeRequest';
import HighlightSelectedElement from './canvas/HighlightSelectedElement';

import ToggleVisualElementsSelectable from './treebar/ToggleVisualElementsSelectable';
import ToggleTouchEvents from './treebar/ToggleTouchEvents';
import LockMovementX from './treebar/LockMovementX';
import LockMovementY from './treebar/LockMovementY';

export default class VisualTreeTabToolbarController {
    constructor($scope, connection, canvas) {
        this._connection = connection;
        this._canvas = canvas;

        this.selectable = false;
        this.touchable = true;
        this.lockx = false;
        this.locky = false;

        console.trace("VisualTreeTabToolbarController init");
    }

    refresh() {
        let req = new GetVisualTreeRequest();
        this._connection.send(req);
    }

    changeHighlightColor(color) {
        PubSub.publish(HighlightSelectedElement.ChangeHighlightColor, color);
    }

    toggleSelectable() {
        this.selectable = !this.selectable;
        this._canvas.visualElementsSelectable = this.selectable;

        let cmd = new ToggleVisualElementsSelectable(this._canvas);
        cmd.execute(this.selectable);
    }

    toggleTouchable() {
        this.touchable = !this.touchable;
        let cmd = new ToggleTouchEvents(this._canvas);
        cmd.execute(this.touchable);
    }

    lockMovementX() {
        this.lockx = !this.lockx;
        let cmd = new LockMovementX(this._canvas);
        cmd.execute(this.lockx);
    }

    lockMovementY() {
        this.locky = !this.locky;
        let cmd = new LockMovementY(this._canvas);
        cmd.execute(this.locky);
    }
}

VisualTreeTabToolbarController.$inject = ["$scope", "ConnectionService", "DeviceCanvasService"];