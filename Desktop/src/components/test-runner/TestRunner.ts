const {VM} = require('vm2');
const fs = require('fs-extra')

import TestRunnerStackService from './TestRunnerStackService';
import ITestRunnerOptions from './ITestRunnerOptions';
import TestRunnerSandbox from './TestRunnerSandbox';

export default class TestRunner {
    private stackService: TestRunnerStackService;

    constructor (
        private options: ITestRunnerOptions
    ) {
        this.stackService = new TestRunnerStackService();
    }

    run(file: string) {
        return new Promise((resolve, reject) => {
            const sandbox = new TestRunnerSandbox(this.options);
            const vm = this.createVM(sandbox);

            this.log(`Opening ${file}.`);

            fs.access(file, fs.constants.R_OK, (err) => {
                if (err) {
                    this.log(`The file ${file} was not found or you do not have read access.`, reject)
                }
                else {
                    fs.readFile(file, 'utf8', (err, data) => {
                        if (err) {
                            this.log(`Unable to read ${file}.`, reject);
                        }
                        else {
                            this.log(`${file} found. Executing.`);

                            const output = vm.run(data);

                            if (output) {
                                this.log(output);
                            }
                            
                            resolve();
                        }
                    });
                }

                this.stackService.cleanup();
            });
        });
    }

    private createVM(sandbox: TestRunnerSandbox) {
        const vm = new VM({sandbox});
        return vm;
    }

    private log(msg, cb = null) {
        const output = `${msg}`;

        console.log(output);

        if (cb) {
            cb(output);
        }
    }
}