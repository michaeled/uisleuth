declare var $: any;

import TestRunner from '../test-runner/TestRunner';
import TestRunnerStackService from '../test-runner/TestRunnerStackService';
import ITestRunnerOptions from '../test-runner/ITestRunnerOptions';

export default class ConsoleController {
    static $inject = ["TestRunnerStackService"];

    constructor (
        private stackService: TestRunnerStackService
    ) {
    }

    run(script: string) {
        const log = $("#terminalhost").terminal().echo;
        const error = $("#terminalhost").terminal().error;
        const tr = new TestRunner(this.stackService, { errorCallback: error, logCallback: log });

        return tr.run(script);
    }

    /**
     *  Invoked by terminalhost; configured in app.js.
     */
    showing() {
        setTimeout(() => {
            $("#terminalhost").terminal().focus();
        }, 500);
    }
}