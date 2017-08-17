import EditorCollection from './EditorCollection';
import VisualTreeTabController from './VisualTreeTabController';

import SelectedElementDispatcher from './canvas/SelectedElementDispatcher';
import MessageDispatcherService from '../connections/MessageDispatcherService';
import DesignerDimensionsService from '../global/DesignerDimensionsService';

export default class PropertiesTabController {
    constructor($scope, editorFactory, categoryFactory, canvasFactory, dims) {
        this._scope = $scope;
        this._editorFactory = editorFactory;
        this._categoryFactory = categoryFactory;
        this._canvasFactory = canvasFactory;
        this._dims = dims;

        this._categories = [];
        this._editors = new EditorCollection();

        // tree & selected element
        this._tree = null;
        this._selected = null;

        // searchbox
        this.searchtext = undefined;

        let gwpToken = PubSub.subscribe(MessageDispatcherService.GetWidgetPropertiesResponse, (e, d) => {
            this._destroyEditors();

            // transform & copy
            let editors = editorFactory.create(d, $scope);

            if (editors.length === 0) {
                console.error("No properties were returned for the visual element. No editors created.")
                return;
            }

            // copy
            this._editors = editors.slice();

            // give a reference to the tree
            this._selected.editors = this._editors;

            // create
            this._categories = categoryFactory.create(editors);

            this._onEditorsLoaded();
            $scope.$apply();
        });

        let nncToken = PubSub.subscribe(VisualTreeTabController.NewNodeClicked, (m, d) => {
            this._tree = d.tree;
            this._selected = d.selected;

            $("#rbar").scrollTop(0);
        });

        $scope.$on("$destroy", () => {
            this._tree = null;
            this._selected = null;

            this._destroyEditors();

            PubSub.unsubscribe(gwpToken);
            PubSub.unsubscribe(nncToken);
        });

        console.trace("properties controller init.");
    }

    get showableCategories() {
        return this._categories.filter(c => c.editors.length > 0);
    }

    get categories() {
        return this._categories;
    }

    // all editors; not in category group
    get editors() {
        return this._editors;
    }

    changed(editor) {
        editor.changed();
    }

    refresh() {
        PubSub.publish(VisualTreeTabController.RefreshNodeData, true);
    }

    scrollTo($event, id) {
        document.getElementById(id).scrollIntoView();
        $event.preventDefault();
        $event.stopPropagation();
    }

    _destroyEditors() {
        for (const editor of this._editors) {
            editor.onDestroy();
        }
        this._editors = new EditorCollection();
    }

    _onEditorsLoaded() {
        if (this._canvasFactory.visualElementsSelectable) {
            let cmd = new SelectedElementDispatcher(this._canvasFactory, this._dims);
            cmd.execute(this._tree, this._selected, this.editors);
        }
    }
}

PropertiesTabController.$inject = [
    "$scope",
    "EditorFactory",
    "CategoryGroupFactory",
    "DeviceCanvasService",
    "DesignerDimensionsService"
];