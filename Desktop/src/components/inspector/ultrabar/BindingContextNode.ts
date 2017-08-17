declare var PubSub;

import UltraBarAction from '../UltraBarAction';
import ConnectionService from '../../connections/ConnectionService';
import DesignerVisualElementsService from '../../global/DesignerVisualElementsService';

export default class BindingContextNode extends UltraBarAction {
    constructor(connection: ConnectionService, elements: DesignerVisualElementsService) {
        super(connection, elements);
    }

    public execute($scope, ui, payload) {
        // todo: temp
    }

    public static canExecute(payload): boolean {
        if (!payload || !payload.node) return false;
        return payload.node.isDatabound === true;
    }
}