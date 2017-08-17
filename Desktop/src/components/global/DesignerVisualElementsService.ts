declare var PubSub;

import MessageDispatcherService from '../connections/MessageDispatcherService';

export interface IDesignerVisualElements {
    views: string[];
    layouts: string[];
    others: string[];
} 

export default class DesignerVisualElementsService {
    public elements: IDesignerVisualElements = null;

    constructor() {
        let ddToken = PubSub.subscribe(MessageDispatcherService.GetVisualElementsResponse, (e, d) => {
            this.elements = d;
        });
    }
}