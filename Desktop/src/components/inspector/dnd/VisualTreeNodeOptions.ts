declare var PubSub;
declare var $;

import SetParentRequest from '../../messages/SetParentRequest'
import VisualTreeTabController from '../VisualTreeTabController';

let $tip = null;
let altDown = false;
let escDown = false;
let currentNode = null;

export function getNodeOptions(controller: VisualTreeTabController) {
    let options = {
        beforeDrag: function(e) {
            let mv = e.$modelValue;
            if (mv && mv.isCell) {
                return false;
            }
            return true;
        },
        beforeDrop: function(e) {
            if (escDown) {
                return false;
            } else {
                return true;
            }
        },
        dropped: function(e) {

            // did the node move to a new position?
            if (currentNode !== null) {
                if (e.dest.index == currentNode.index && e.dest.nodesScope.$id == currentNode.nodesScope.$id) {
                    currentNode = null;
                    return;
                }
            }

            let childId = e.source.nodeScope.$modelValue.id;
            let parentId = e.dest.nodesScope.$nodeScope.$modelValue.id;
            let index = e.dest.index;
            let req = new SetParentRequest(childId, parentId, index);

            controller.connection.send(req);

            let childType = e.source.nodeScope.$modelValue.type;
            let parentType = e.dest.nodesScope.$nodeScope.$modelValue.type;
            let description = `Moved to position ${index} of ${parentType}.`;

            PubSub.publish(SetParentRequest.SetParentEvent, {
                typeName: childType,
                description: description
            });
        },
        dragStop: function(e) {
            removeDnDEvents();
        },
        dragStart: function(e) {
            if (!e.source.nodeScope) return;

            addDnDEvents();

            controller.onNodeClicked(e.source.nodeScope.$modelValue);
            currentNode = e.dest;
        },
        accept: function(sourceNodeScope, destNodesScope, destIndex) {
            if (destNodesScope.$nodeScope == null) return;
            removeTip();

            let dest = destNodesScope.$nodeScope.$modelValue;
            let source = sourceNodeScope.$modelValue;
            let destDepth = destNodesScope.depth();
            let sourceDepth = sourceNodeScope.depth();

            let result = false;
            let tipText = null;

            // allow drop to original position
            if (sourceNodeScope.$parentNodeScope.$modelValue === dest) {
                result = true;
            }

            // depth 1 is the page; depth 2 is first child
            // hovering a layout visual element

            if (dest.isLayout && (dest.isContentPropertyViewType || destDepth === 1)) {
                if (dest.allowsManyChildren) {
                    result = true;
                } else {
                    if (controller.hasChildren(dest) && sourceDepth !== 2) {

                        if (altDown) {
                            tipText = "Replace current visual element.";
                        } else {
                            tipText = "Press [alt] to replace current visual element."
                        }

                        createTip(destNodesScope, tipText);

                        // wants to replace visual element
                        if (altDown === true) {
                            result = true;
                        } else {
                            result = false;
                        }

                    } else {
                        result = true;
                    }
                }
            }

            return result;
        },
    };

    return options;
}


function createTip(destNodesScope, text) {
    let offset = destNodesScope.$element.offset();

    // remove old tip; if exists
    removeTip();

    if (!text) return;

    $tip = $("<div>", {
            class: 'tip',
            text: text
        })
        .css("top", offset.top)
        .css("left", offset.left)
        .css("position", "absolute");

    $tip.appendTo("#lbar");
}

function addDnDEvents() {
    altDown = false;
    escDown = false;
    window.addEventListener("keydown", keydown, true);
    window.addEventListener("keyup", keyup, true);
}

function removeDnDEvents() {
    altDown = false;
    escDown = false;
    window.removeEventListener("keydown", keydown, true);
    window.removeEventListener("keyup", keyup, true);
    removeTip();
}

function keydown(e) {
    if (e.keyCode === 18) {
        altDown = true;
    }

    if (e.keyCode === 27) {
        escDown = true;
    }
}

function keyup(e) {
    if (e.keyCode === 18) {
        altDown = false;
    }

    if (e.keyCode === 27) {
        escDown = false;
    }
}

function removeTip() {
    if ($tip) {
        $tip.remove();
    }
}