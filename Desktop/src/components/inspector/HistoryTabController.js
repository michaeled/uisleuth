import Editor from './Editor';
import InsertNode from './ultrabar/InsertNode';
import DeleteNode from './ultrabar/DeleteNode';
import SetParentRequest from '../messages/SetParentRequest';

export default class HistoryTabController {
    constructor($scope) {
        this._$scope = $scope;
        this.history = [];

        let pceToken = PubSub.subscribe(Editor.PropertyChangedEvent, (e, d) => {
            this._addPropertyChanged(d);
            this._$scope.$digest();
        });

        let cweToken = PubSub.subscribe(InsertNode.CreatedWidgetEvent, (e, d) => {
            this._addCreatedWidget(d);
            this._$scope.$digest();
        });

        let dweToken = PubSub.subscribe(DeleteNode.DeletedWidgetEvent, (e, d) => {
            this._addDeletedWidget(d);
            this._$scope.$digest();
        });

        let speToken = PubSub.subscribe(SetParentRequest.SetParentEvent, (e, d) => {
            this._addSetParent(d);
            this._$scope.$digest();
        });

        $scope.$on("$destroy", () => {
            PubSub.unsubscribe(pceToken);
            PubSub.unsubscribe(cweToken);
            PubSub.unsubscribe(dweToken);
            PubSub.unsubscribe(speToken);
        });

        console.trace("history tab controller init.");
    }

    _addPropertyChanged(data) {
        let header = data.parentType;
        let description = `${data.propertyName} was changed to ${data.value}.`;
        this._add(header, description, 'changed');
    }

    _addCreatedWidget(data) {
        let header = data.typeName;
        let description = data.description || "Created with default values.";
        this._add(header, description, 'created');
    }

    _addDeletedWidget(data) {
        let header = data.typeName;
        let description = data.description || "Deleted view.";
        this._add(header, description, 'deleted');
    }

    _addSetParent(data) {
        let header = data.typeName;
        let description = data.description || "Moved view.";
        this._add(header, description, 'moved');
    }

    _add(header, description, classname) {
        // show last 50 changes
        if (this.history.length >= 50) {
            this.history.splice(-1);
        }

        this.history.unshift({
            header,
            description,
            classname
        });
    }
}

HistoryTabController.$inject = ["$scope"];