import Editor from "../Editor";

export default class OpacityEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        let self = this;

        this.config.options = {
            start: self.value,
            step: 0.01,
            connect: 'lower',
            range: {
                'min': 0,
                'max': 1
            }
        };

        this.config.events = {
            update: function(values, handle, unencoded) {
                if (self.value === self.config.options.start) {
                    return;
                }

                // todo debounce opacity?
                self.value = self.config.options.start;
                self.changed();
            }
        }
    }

    get sliderval() {
        return Math.floor(this.config.options.start * 100);
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/Opacity.htm`;
    }
}