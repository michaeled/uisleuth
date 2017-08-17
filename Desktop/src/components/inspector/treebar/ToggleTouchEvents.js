import DeviceViewController from '../DeviceViewController';

export default class ToggleTouchEvents {
    constructor(canvas) {
        this.canvas = canvas;
    }

    execute(enabled) {
        PubSub.publish(DeviceViewController.TouchEnabled, enabled);
    }
}