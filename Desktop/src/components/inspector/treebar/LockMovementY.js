export default class LockMovementY {
    constructor(canvas) {
        this.canvas = canvas;
    }

    execute(enabled) {
        for (const o of this.canvas.device.getObjects()) {
            o.lockMovementY = enabled;
        }
    }
}