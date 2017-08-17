export default class DeviceFeaturesController {
    constructor($scope) {
        this.features = [];
        this.init($scope.device);
    }

    init(device) {
        Object.getOwnPropertyNames(device.features).forEach((val, index, all) => {
            let item = {
                name: val,
                value: device.features[val]
            };

            this.features.push(item);
        });
    }
}

DeviceFeaturesController.$inject = ["$scope"];