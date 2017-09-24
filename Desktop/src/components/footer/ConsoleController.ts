declare var $: any;

import TestRunner from '../test-runner/TestRunner';
import TestRunnerStackService from '../test-runner/TestRunnerStackService';

export default class ConsoleController {
    static $inject = ["TestRunnerStackService"];

    constructor(
        private stackService: TestRunnerStackService
    ) {
    }

    run(script) {
        const echo = $("#terminalhost").terminal().echo;
        const error = $("#terminalhost").terminal().error;
        const tr = new TestRunner(this.stackService, echo, error);

        return tr.run(script);
    }

    showing() {
        setTimeout(() => {
            $("#terminalhost").terminal().focus();
        }, 500);
    }
}