import EditorCollection from "./EditorCollection";
import MessageDispatcherService from "../connections/MessageDispatcherService";
import EditorFactory from "./EditorFactory";

export default class AttachedPropertiesTabController {
    constructor($scope, connection) {
        this.connection = connection;
        this._editors = new EditorCollection();

        let ef = new EditorFactory(connection);

        let apsToken = PubSub.subscribe(MessageDispatcherService.GetAttachedPropertiesResponse, (e, d) => {
            this._editors = ef.createAttached(d.widget, $scope);
            $scope.$digest();
        });

        $scope.$on("$destroy", () => {
            this._destroyEditors();
            PubSub.unsubscribe(apsToken);
        });

        console.trace("attached properties controller init.");
    }

    get editors() {
        return this._editors;
    }

    changed(editor) {
        editor.changed();
    }

    _destroyEditors() {
        for (const editor of this._editors) {
            editor.onDestroy();
        }

        this._editors = new EditorCollection();
    }
}

AttachedPropertiesTabController.$inject = ["$scope", "ConnectionService"];