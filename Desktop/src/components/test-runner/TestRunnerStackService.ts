declare var PubSub;

import MessageDispatcherService from '../connections/MessageDispatcherService';

export default class TestRunnerStackService {
    private stack = [];
    private gvtToken;
    private gwpToken;

    constructor() {
        this.gvtToken = PubSub.subscribe(MessageDispatcherService.GetVisualTreeResponse, (e, d) => {
            console.log(d);
        });

        this.gwpToken = PubSub.subscribe(MessageDispatcherService.GetWidgetPropertiesResponse, (e, d) => {
            console.log(d);
        });

        console.trace("init TestRunnerStackService.")
    }

    cleanup() {
        PubSub.unsubscribe(this.gvtToken);
        PubSub.unsubscribe(this.gwpToken);

        console.trace("unsubscribed from TestRunnerStackService events.")
    }
}