const {VM} = require('vm2');
const fs = require('fs-extra')

import TestRunnerStackService from './TestRunnerStackService';
import ITestRunnerOptions from './ITestRunnerOptions';
import TestRunnerSandbox from './TestRunnerSandbox';

export default class TestRunner {
    constructor (
        private stackService: TestRunnerStackService,
        private options: ITestRunnerOptions
    ) {
    }

    run(file: string) {
        const sandbox = new TestRunnerSandbox(this.options);
        const vm = this.createVM(sandbox);

        return new Promise((resolve, reject) => {

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
            });
        });
    }

    private createVM(sandbox: TestRunnerSandbox) {
        const options = {
            sandbox
        };

        const vm = new VM(options);

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