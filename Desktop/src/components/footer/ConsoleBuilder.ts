declare var $;

import TestRunner from '../test-runner/TestRunner';
import ITestRunnerOptions from '../test-runner/ITestRunnerOptions';

export default class ConsoleBuilder {
    public static get console() {
        return $("#consolehost").terminal();
    }

    constructor() {
        this.configure({
            "run-test": this.run
        });
    }

    private configure(options) {
        return $("#consolehost").terminal(options, {
            prompt: '>',
            greetings: `
██╗   ██╗██╗    ███████╗██╗     ███████╗██╗   ██╗████████╗██╗  ██╗
██║   ██║██║    ██╔════╝██║     ██╔════╝██║   ██║╚══██╔══╝██║  ██║
██║   ██║██║    ███████╗██║     █████╗  ██║   ██║   ██║   ███████║
██║   ██║██║    ╚════██║██║     ██╔══╝  ██║   ██║   ██║   ██╔══██║
╚██████╔╝██║    ███████║███████╗███████╗╚██████╔╝   ██║   ██║  ██║
 ╚═════╝ ╚═╝    ╚══════╝╚══════╝╚══════╝ ╚═════╝    ╚═╝   ╚═╝  ╚═╝           
`
        });
    }

    private run(script: string) {
        const tr = new TestRunner({ 
            error: ConsoleBuilder.console.error,
            log: ConsoleBuilder.console.echo 
        });

        tr.run(script).then(r => {
            ConsoleBuilder.console.echo(r);
        })
        .catch(r => {
            ConsoleBuilder.console.error(r);
        });
    }
}