export default class DeviceDialogFactory {
    constructor(dialog) {
        this.dialog = dialog;
    }

    open(device) {
        this.dialog.open({
            template: './components/device/capabilities.htm',
            className: 'ngdialog-theme-default device',
            controller: ["$scope", function DeviceDialogController($scope) {
                $scope.device = device;
                this.title = device.name;

                this.close = function() {
                    $scope.closeThisDialog();
                }
            }],
            controllerAs: "ctrl",
        });
    }
}

DeviceDialogFactory.$inject = ["ngDialog"];