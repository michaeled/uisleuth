import HighlightSelectedElement from "./HighlightSelectedElement";
import RotateCanvasObject from "./RotateCanvasObject";
import ScaleCanvasElement from "./ScaleCanvasElement";
import MoveCanvasElement from "./MoveCanvasElement";

export default class SelectedElementDispatcher {
    constructor(canvas, dims) {
        this.canvas = canvas;
        this.display = dims.display;
    }

    execute(tree, selected, editors) {
        // highlight on click; need to be #1
        let highlight = new HighlightSelectedElement(this.canvas, this.display);
        let hco = highlight.execute(selected, editors);

        /*
        // on rotate
        let rotate = new RotateCanvasObject();
        rotate.execute(editors, hco);

        // show scale handles
        let scale = new ScaleCanvasElement();
        scale.execute(tree, selected, editors, hco);

        // movement
        let move = new MoveCanvasElement();
        move.execute(tree, selected, editors, hco);
        */
    }
}