export default class StringDialogController {
    constructor(ngDialog) {
        this._ngDialog = ngDialog;
    }

    showDialog($event, editor) {
        $event.preventDefault();

        this._ngDialog.open({
            template: './components/inspector/editors/StringDialog.htm',
            className: 'ngdialog-theme-default',
            controller: ["$scope", function($scope) {
                this.$scope = $scope;
                this.title = `${editor.name} Editor`;
                this.editor = editor;
                this.value = editor.value;

                this.apply = function() {
                    this.editor.value = this.value;
                    this.editor.changed();
                    this.close();
                }

                this.close = function() {
                    this.$scope.closeThisDialog();
                }
            }],
            controllerAs: "dlgctrl"
        });
    }
}

StringDialogController.$inject = ["ngDialog"];