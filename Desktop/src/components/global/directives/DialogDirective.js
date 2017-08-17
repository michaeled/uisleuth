export default class DialogDirective {
    constructor() {
        this.scope = {
            show: '@',
            title: '@'
        };

        this.controller = ['$scope', DialogController];
        this.controllerAs = 'ctrl';
        this.bindToController = true;
        this.restrict = 'E';
        this.transclude = true;
        this.templateUrl = './components/global/directives/dialog.htm';
    }

    link(scope, element, attrs, ctrl) {
        let dialog = element[0].firstChild;

        ctrl.dialog = dialog;
        scope.title = attrs.title;

        scope.$watch(() => element.attr('show'), (newval, oldval) => {
            if (newval === oldval) return;
            ctrl.showModal(newval);
        });
    }
}

class DialogController {
    constructor($scope) {
        this.$scope = $scope;
        this.dialog = null;
    }

    showModal(value) {
        if (value) {
            if (this.dialog) {
                this.dialog.showModal();
            }
        } else {
            this.close();
        }
    }

    close() {
        if (this.dialog && this.dialog.open) {
            this.dialog.close();
        }
    }
}