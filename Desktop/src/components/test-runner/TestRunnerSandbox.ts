const util = require('util');

import ITestRunnerOptions from './ITestRunnerOptions';

export default class TestRunnerSandbox {
    constructor (
        private options: ITestRunnerOptions
    ) {
    }
    
    public assert = (condition, msg) => {
        if (!condition) {
            this.options.error(msg);
        }
    };

    public log = (msg) => {
        let output = msg;

        if (typeof msg === "object") {
            output = util.inspect(msg, { depth: 3 });
        }

        this.options.log(output);
    }
}