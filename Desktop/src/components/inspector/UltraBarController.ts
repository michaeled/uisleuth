declare var $;
declare var angular;

import DeleteNode from './ultrabar/DeleteNode';
import InsertNode from './ultrabar/InsertNode';
import BindingContextNode from './ultrabar/BindingContextNode';
import ConnectionService from '../connections/ConnectionService';
import DesignerVisualElementsService from '../global/DesignerVisualElementsService';
import BindingContextDialogService from './ultrabar/BindingContextDialogService';

import * as htmlhelpers from '../global/utilities/viewport';

let $ultrabar = null;

export interface IUltraBarActionButton {
    key: string;
    type: any;
    ui: any;
}

export default class UltraBarController {
    public static $inject = [
        "$scope",
        "$compile",
        "ConnectionService",
        "DesignerVisualElementsService",
        "BindingContextDialogService"
    ];

    public actions: IUltraBarActionButton[];

    constructor(private $scope, private $compile, private connection: ConnectionService, private elements: DesignerVisualElementsService, private bcDialog: BindingContextDialogService) {
        this.setupActions();

        $scope.$on("$destroy", () => {
            this.connection = null;
            this.elements = null;
            this.actions = null;
        });

        console.trace("UltraBarController controller init.");
    }

    public action(key: string): IUltraBarActionButton {
        return this.actions.find(a => a.key === key);
    }

    public execute(key: string, payload): void {
        let action = this.action(key);

        if (!action) return;
        if (!action.type.canExecute(payload)) return;

        if (!action.type) {
            console.trace("Action not executed. No type defined.")
            return;
        };

        // todo: temp
        if (key == 'bindingcontext') {
            this.bcDialog.showDialog(payload.node);
        }
        else {
            let cmd = new action.type(this.connection, this.elements);
            cmd.execute(this.$scope, action.ui, payload);
        }
    }

    public canExecute(key: string, payload): boolean {
        let action = this.action(key);
        if (!action) return false;

        return action.type.canExecute(payload);
    }

    public show($event): void {
        destroyUltraBar();

        let tmpl = "'./components/inspector/ultrabar/ultrabar.htm'";
        $ultrabar = $(`<div class="ultrabar"><ng-include src="${tmpl}"></ng-include></div>`);

        $ultrabar
            .css("position", "fixed")
            .css("left", `calc(1em + ${$event.pageX}px)`)
            .css("top", `calc(1em + ${$event.pageY}px)`);

        $("body").append($ultrabar);
        this.$compile($ultrabar)(this.$scope);
    }

    public close(): void {
        destroyUltraBar();
    }

    public visible(): boolean {
        return $ultrabar != null;
    }

    public reset(): void {
        this.actions.forEach(a => {
            if (a.ui.reset) {
                a.ui.reset();
            }
        });
    }

    private setupActions() {
        this.actions = [{
                key: 'delete',
                type: DeleteNode,
                ui: {}
            },
            {
                key: 'insert',
                type: InsertNode,
                ui: {}
            },
            {
                key: 'bindingcontext',
                type: BindingContextNode,
                ui: {}
            }
        ];
    }
}


function destroyUltraBar() {
    if ($ultrabar) {

        let $scope = angular.element($ultrabar).scope();
        if ($scope.ubctrl) {
            $scope.ubctrl.reset();
        }

        $ultrabar.remove();
        $ultrabar = null;
    }
}

window.addEventListener("keydown", function(e) {
    if (e.keyCode === 27) {
        destroyUltraBar();
    }
});

window.addEventListener("click", function(e) {
    if ($ultrabar === null) return;
    if (!$ultrabar.is(":visible")) return;

    let $target = $(e.target);
    let activated = $target.closest(".ultrabar-toggle").length == 1;
    let within = $target.closest(".ultrabar").length === 1;

    if (activated) return;

    if (!within) {
        destroyUltraBar();
    } else {
        // snap to bottom of page if the ultrabar would otherwise be clipped
        if (!htmlhelpers.isElementInViewport($ultrabar)) {
            $ultrabar
                .css("top", "")
                .css("bottom", "15px");
        }
    }
});