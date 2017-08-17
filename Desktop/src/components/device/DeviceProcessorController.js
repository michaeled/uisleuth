export default class DeviceProcessorController {
    constructor(adb) {
        this.adb = adb;
        this.stats = null;
        this.cpus = null;
        this.loads = [];
    }

    init(device) {
        this.adb.client.openProcStat(device.id).then(stats => {
            stats.on('load', load => {
                this.cpus = Object.keys(load);
                this.loads = [];

                for (const cpu of this.cpus) {
                    this.loads[cpu] = [];

                    Object.getOwnPropertyNames(load[cpu]).forEach((val, idx, vals) => {
                        let item = {
                            name: val,
                            value: load[cpu][val]
                        };

                        this.loads[cpu].push(item);
                    });
                }

                this.$scope.$digest();
            });

            this.stats = stats;
            this.$scope.$digest();
        });
    }

    close() {
        if (this.stats) {
            this.stats.end();
            this.stats = null;
        }
    }
}

DeviceProcessorController.$inject = ["AndroidDeviceService"];