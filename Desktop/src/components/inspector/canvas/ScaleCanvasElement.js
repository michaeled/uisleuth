export default class ScaleCanvasElement {
    execute(tree, selected, editors, obj) {

        // is page selected?
        if (tree == selected) {
            obj.setControlsVisibility({
                bl: false,
                br: false,
                tl: false,
                tr: false,
                mt: false,
                mb: false,
                ml: false,
                mr: false,
                mtr: true
            });
        }

        obj.on("selected", function(e) {
            // restrict horizontal movement
            let parent = selected.parent;
            let orientation = parent.editors;
            console.log(orientation);

            obj.lockScalingX = true;
            obj.lockScalingY = true;
        });
    }
}