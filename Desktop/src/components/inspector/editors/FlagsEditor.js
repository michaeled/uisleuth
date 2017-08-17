import Editor from "../Editor";

export default class FlagsEditor extends Editor {
    constructor(id, property, defaults) {
        super(id, property, defaults);

        // flags are returned comma seperated
        // flags are sent pipe seperated
    }

    get template() {
        let discard = super.template;
        return `components/inspector/editors/Flags.htm`;
    }

    onShown() {
        let self = this;
        let re = /\s*,\s*/;

        setTimeout(function() {
            let flags = null;

            if (self.value) {
                flags = self.value.split(re);
            }

            let select = $('.flags-editor select');
            select.selectpicker("refresh");
            select.selectpicker('val', flags);

        }, 100);
    }

    onBound() {
        super.onBound();

        // prevent multiple event bindings
        if (FlagsEditor._alreadyBound) {
            return;
        } else {
            FlagsEditor._alreadyBound = true;
        }

        let self = this;
        $("#tab-properties").on("changed.bs.select", ".flags-editor select", function(e) {
            let val = $(this).val() || "";

            if (val && val.join) {
                val = val.join(" | ");
            }

            self.value = val;
            self.changed();
        });
    }
}

FlagsEditor._alreadyBound = false;