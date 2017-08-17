import Editor from "../Editor";

export default class EnumEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/System.Enum.htm`;
    }

    onShown() {
        let self = this;

        setTimeout(function() {
            if (self.value) {
                let select = $('.enum-editor select');
                select.selectpicker("refresh");
                select.selectpicker('val', self.value);
            }
        }, 100);
    }

    onBound() {
        super.onBound();

        // prevent multiple event bindings
        if (EnumEditor._alreadyBound) {
            return;
        } else {
            EnumEditor._alreadyBound = true;
        }

        let self = this;
        $("#tab-properties").on("changed.bs.select", ".enum-editor select", function(e) {
            let val = $(this).val() || "";
            self.value = val;
            self.changed();
        });
    }
}

EnumEditor._alreadyBound = false;