declare var PubSub;

import UltraBarAction from '../UltraBarAction';
import VisualTreeTabController from '../VisualTreeTabController';
import ConnectionService from '../../connections/ConnectionService';
import DesignerVisualElementsService from '../../global/DesignerVisualElementsService';
import DeleteWidgetRequest from '../../messages/DeleteWidgetRequest';

export default class DeleteNode extends UltraBarAction {
    public static DeletedWidgetEvent = "DeleteNode.DeletedWidgetEvent";

    constructor(connection: ConnectionService, elements: DesignerVisualElementsService) {
        super(connection, elements);
    }

    public execute($scope, ui, payload) {
        if (!payload) {
            console.error("payload not provided to delete node.");
        }

        let confirm = window.confirm(`Are you sure you want to delete this ${payload.node.type}?`);

        if (confirm) {
            let req = new DeleteWidgetRequest(payload.node.id);
            this.connection.send(req);

            // cleanup tree item
            payload.$childNodesScope.remove();
            PubSub.publish(VisualTreeTabController.SelectNode, payload.$parentNodeScope.$modelValue);
            this.publishDeletedWidgetEvent(payload.node);
        }
    }

    public static canExecute(payload): boolean {
        if (!payload || !payload.node) return;
        return payload.node.canDelete === true;
    }

    private publishDeletedWidgetEvent(node): void {
        let description = `Deleted view. Parent was ${node.parent.type}.`

        PubSub.publish(DeleteNode.DeletedWidgetEvent, {
            typeName: node.type,
            description: description
        });
    }
}