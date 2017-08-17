export default class EditorCollection extends Array {
    constructor() {
        super();
    }

    get(name) {
        let editor = _.find(this, v => {
            if (!v) return false;
            if (!v.name) return false;
            return v.name === name
        });

        return editor;
    }

    getAllByType(type) {
        let editors = this.filter(v => {
            if (!v) return false;
            if (!v.type) return false;
            return v.type === type;
        });

        return editors;
    }
}