export default class DevicePropertiesController {
    constructor($scope) {
        this.properties = [];
        this.init($scope.device);
    }

    init(device) {
        Object.getOwnPropertyNames(device.properties).forEach((val, index, all) => {
            let item = {
                name: val,
                value: device.properties[val]
            };

            this.properties.push(item);
        });
    }
}

DevicePropertiesController.$inject = ["$scope"];