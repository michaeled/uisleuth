declare var $: any;

import ConsoleBuilder from './ConsoleBuilder';

export default class ConsoleController {
    private builder: ConsoleBuilder;

    onReady() {
        this.builder = new ConsoleBuilder();
    }

    /**
     *  Invoked by console-target; configured in app.js.
     */
    showing() {
        setTimeout(() => {
            ConsoleBuilder.console.focus();
        }, 500);
    }
}