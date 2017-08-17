export default class LockMovementX {
    constructor(canvas) {
        this.canvas = canvas;
    }

    execute(enabled) {
        for (const o of this.canvas.device.getObjects()) {
            o.lockMovementX = enabled;
        }
    }
}