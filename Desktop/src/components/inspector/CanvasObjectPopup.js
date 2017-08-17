let $popup = null;

export default class CanvasObjectPopup {
    constructor() {

    }

    show(obj) {
        //createPopup(obj);
    }

    hide() {
        //hidePopup();
    }
}


function hidePopup() {
    if ($popup) {
        $popup.remove();
    }
}

function createPopup(obj) {
    $popup = $("<div>", {
            class: 'canvas-node-menu'
        })
        .html("<span class='glyphicon glyphicon-edit'></span>")
        .css("position", "fixed");

    $popup.appendTo("body");

    let x = obj.e.x - ($popup.width());
    let y = obj.e.y + ($popup.height() / 2) + 5;

    $popup
        .css("top", y)
        .css("left", x);
}