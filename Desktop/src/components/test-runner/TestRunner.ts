const { VM } = require('vm2');
const fs = require('fs-extra')
const util = require('util');

import TestRunnerStackService from './TestRunnerStackService';

export default class TestRunner {
    constructor(
        private stackService: TestRunnerStackService,
        private terminalEchoCb,
        private terminalErrorCb
    ) {
    }

    run(file) {
        const vm = this.createVM();

        return new Promise((resolve, reject) => {

            this.log(`Opening ${file}.`);

            fs.access(file, fs.constants.R_OK, (err) => {
                if (err) {
                    this.log(`The file ${file} was not found or you do not have read access.`, reject)
                }
                else {
                    fs.readFile(file, "utf8", (err, data) => {
                        if (err) {
                            this.log(`Unable to read ${file}.`, reject);
                        }
                        else {
                            this.log(`${file} found. executing.`);

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

    private createVM() {
        const self = this;

        const vm = new VM({
            sandbox: {

                assert: function(condition, msg) {
                    if (!condition) {
                        self.terminalErrorCb(msg);
                    }
                },

                log: function(msg) {
                    let output = msg;

                    if (typeof msg === "object") {
                        output = util.inspect(msg, { depth: 4 });
                    }

                    self.terminalEchoCb(output);
                },

                uisleuth() {
                    return self.getNextObjectModel();
                }
            }
        });

        return vm;
    }

    private log(msg, cb = null) {
        const output = `${msg}`;

        console.log(output);

        if (cb) {
            cb(output);
        }
    }

    private getNextObjectModel() {
        return this.stackService.pop();
    }
}