export default class ToggleVisualElementsSelectable {
    constructor(canvas) {
        this.canvas = canvas;
    }

    execute(enabled) {
        if (!enabled) {
            this.canvas.device.deactivateAllWithDispatch();

            for (const o of this.canvas.device.getObjects()) {
                if (o._isHighlight) {
                    this.canvas.device.remove(o);
                }
            }

            this.canvas.device.renderAll();
        }

        this.canvas.device.selection = enabled;
        this.canvas.device.forEachObject(function(o) {
            o.selectable = enabled;
        });
    }
}