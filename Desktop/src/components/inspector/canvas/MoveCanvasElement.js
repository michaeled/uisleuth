export default class MoveCanvasElement {
    execute(tree, selected, editors, obj) {
        // is page selected?
        if (tree == selected) {
            obj.lockMovementX = true;
            obj.lockMovementY = true;
        }

        // first child; also locked
        if (tree.children && tree.children.length > 0) {
            if (tree.children[0] === selected) {
                obj.lockMovementX = true;
                obj.lockMovementY = true;
            }
        }
    }
}