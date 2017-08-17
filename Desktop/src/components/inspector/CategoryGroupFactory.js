const MiscCategory = "Miscellaneous";
import EditorCollection from "./EditorCollection";

export default class CategoryGroupFactory {
    constructor(storage) {
        this._storage = storage;
        this._categories = [];
        this.init();
    }

    init() {
        this._hydrate();
    }

    create(editors) {
        let result = [];
        let categories = _.cloneDeep(this._categories);

        // last category to be processed
        let misc = _.remove(categories, c => c.name === MiscCategory).pop();

        if (!misc) {
            throw "A Miscellaneous category must exist.";
        }

        for (const category of categories) {
            result.push(this._processCategory(editors, category));
        }

        result.push(this._processCategory(editors, misc));
        return result;
    }

    _processCategory(editors, category) {
        let result = {
            name: category.name,
            editors: new EditorCollection()
        };

        // breakout early if this is the miscellaneous category
        if (result.name === MiscCategory) {
            result.editors = editors;
            return result;
        }

        // every other category
        for (const property of category.properties) {
            let matches = _.remove(editors, e => {
                return e.name === property.name;
            });

            for (const editor of matches) {
                result.editors.push(editor);
            }
        }

        return result;
    }

    _hydrate() {
        this._storage.categories((data) => {
            if (!angular.equals({}, data)) {
                this._categories = data.categories;
            }
        });
    }
}

CategoryGroupFactory.$inject = ["StorageService"];