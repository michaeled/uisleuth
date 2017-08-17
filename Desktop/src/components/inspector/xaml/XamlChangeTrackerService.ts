declare var PubSub;

import Editor from '../Editor';
import InsertNode from '../ultrabar/InsertNode';
import DeleteNode from '../ultrabar/DeleteNode';
import SetParentRequest from '../../messages/SetParentRequest';

interface IPropertyChange {
    elementName: string;
    propertyName: string;
    propertyValue: any;
}

export default class XamlChangeTrackerService {
    constructor() {
        let pceToken = PubSub.subscribe(Editor.PropertyChangedEvent, (e, d) => {
            console.log(e);
        });

        let cweToken = PubSub.subscribe(InsertNode.CreatedWidgetEvent, (e, d) => {
        });

        let dweToken = PubSub.subscribe(DeleteNode.DeletedWidgetEvent, (e, d) => {
        });

        let speToken = PubSub.subscribe(SetParentRequest.SetParentEvent, (e, d) => {
        });

        console.trace("xaml change tracker service init.");
    }
}