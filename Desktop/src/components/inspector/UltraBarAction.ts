import ConnectionService from '../connections/ConnectionService';
import DesignerVisualElementsService from '../global/DesignerVisualElementsService';

export default class UltraBarAction {
    constructor(protected connection: ConnectionService, protected elements: DesignerVisualElementsService) {}

    public execute($scope, ui, payload): void {}
    public static canExecute(payload): boolean { return true; }
}