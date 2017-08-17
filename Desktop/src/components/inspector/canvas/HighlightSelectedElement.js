const StrokeWidth = 1;

export default class HighlightSelectedElement {
    constructor(canvas, display) {
        this._canvas = canvas;
        this._display = display;
        this._rect = null;

        this._chcToken = PubSub.subscribe(HighlightSelectedElement.ChangeHighlightColor, (e, d) => {
            if (this._rect) {
                HighlightSelectedElement.HighlightColor = d;
                this._rect.set("stroke", d);
                this._canvas.device.renderAll();
            }
        });
    }

    execute(selected, editors) {
        let canvas = this._canvas.device;

        for (const o of canvas.getObjects()) {
            PubSub.unsubscribe(o._chcToken);
            canvas.remove(o);
        }

        let density = this._display.density;
        let densityX = this._canvas.scale.scaleX * density;
        let densityY = this._canvas.scale.scaleY * density;

        let x = Math.floor(selected.dimensions.x * densityX);
        let y = Math.floor(selected.dimensions.y * densityY);
        let width = Math.floor(selected.dimensions.width * densityX);
        let height = Math.floor(selected.dimensions.height * densityY);

        let rotation = editors.get("Rotation");
        let angle = 0;

        if (rotation) {
            angle = rotation.value;
        }

        let bars = Math.floor(this._display.statusBarHeight * this._canvas.scale.scaleY);
        let top = y + bars;

        if (width >= this._canvas.size.width) {
            width -= StrokeWidth;
        }

        if (height + top >= this._canvas.size.height) {
            height -= StrokeWidth;
        }

        this._rect = new fabric.Rect({
            angle: angle,
            left: x,
            top: top,
            width: width,
            height: height,
            fill: 'transparent',
            stroke: HighlightSelectedElement.HighlightColor,
            strokeWidth: StrokeWidth
        });

        // used in ToggleVisualElementsSelectable
        this._rect._isHighlight = true;
        this._rect._uiStrokeWidth = StrokeWidth;
        this._rect._chcToken = this._chcToken;

        this.disableMovement();

        canvas.add(this._rect);
        return this._rect;
    }

    disableMovement() {
        this._rect.setControlsVisibility({
            bl: false,
            br: false,
            tl: false,
            tr: false,
            mt: false,
            mb: false,
            ml: false,
            mr: false,
            mtr: false
        });

        this._rect.lockMovementX = true;
        this._rect.lockMovementY = true;
    }
}

HighlightSelectedElement.HighlightColor = 'red';
HighlightSelectedElement.ChangeHighlightColor = 'HighlightSelectedElement.ChangeHighlightColor';