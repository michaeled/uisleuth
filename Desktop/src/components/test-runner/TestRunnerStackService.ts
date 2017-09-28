declare var PubSub;

import MessageDispatcherService from '../connections/MessageDispatcherService';

export default class TestRunnerStackService {
    private stack = [];

    constructor() {
        let gvtToken = PubSub.subscribe(MessageDispatcherService.GetVisualTreeResponse, (e, d) => {
            this.push(d);
        });

        let gwpToken = PubSub.subscribe(MessageDispatcherService.GetWidgetPropertiesResponse, (e, d) => {
            console.log(d);
        });

        console.trace("init TestRunnerStackService.")
    }

    push(data) {
        this.stack.push(data);
    }

    pop() {
        return this.stack[0];
    }
}