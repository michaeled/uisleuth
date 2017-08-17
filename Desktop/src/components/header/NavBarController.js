export default class NavBarController {
    constructor($scope, adb, connection, deviceDialog) {
        this.adb = adb;
        this.connection = connection;
        this.deviceDialog = deviceDialog;
    }

    get canShowDeviceDialog() {
        return this.connection.connected && this.adb.attached != null;
    }

    showDeviceDialog($event) {
        $event.preventDefault();

        if (!this.adb.attached) return;
        this.deviceDialog.open(this.adb.attached)
    }
}

NavBarController.$inject = ["$scope", "AndroidDeviceService", "DeviceDialogFactory"]