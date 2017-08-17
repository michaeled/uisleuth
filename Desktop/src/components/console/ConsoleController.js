export default class ConsoleController {
    constructor($scope) {
        this.opened = false;
    }

    showing() {
        setTimeout(function() {
            $("#terminalhost").terminal().focus();
        }, 500);
    }
}

ConsoleController.$inject = ["$scope"];