export default class SettingsController {
    constructor() {
        this.screenRateOptions = {
            start: 3000,
            step: 500,
            connect: 'lower',
            range: {
                'min': 0,
                'max': 10000
            }
        };

        this.modelUpdateOptions = {
            start: 500,
            step: 500,
            connect: 'lower',
            range: {
                'min': 500,
                'max': 10000
            }
        };
    }

    get screenRate() {
        return Math.floor(this.screenRateOptions.start);
    }

    get modelUpdateRate() {
        return Math.floor(this.modelUpdateOptions.start);
    }
}