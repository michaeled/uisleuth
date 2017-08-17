export default class CategoriesController {
    constructor($scope, storage, orderBy) {
        this._$scope = $scope;
        this._storage = storage;
        this._orderBy = orderBy;

        this._selectedCategory = null;
        this._selectedProperty = null;

        this.categories = [];

        // these determine if the textboxes are shown or not
        this.newCategory = false;
        this.newProperty = false;

        // these are the values entered in the textboxes
        this.newCategoryName = null;
        this.newPropertyName = null;

        // focus
        this.focusCategoryName = false;
        this.focusPropertyName = false;
        this.focusEditingCategory = false;
        this.focusEditingProperty = false;

        // current category being modified
        this.editingCategory = null;
        this.editingProperty = null;

        // this is the temporary names entered by the user
        this.editingCategoryName = null;
        this.editingPropertyName = null;

        this._hydrate();

        $scope.$on("uisleuth.megamenu.closing", e => {
            this.hideEditingCategory();
            this.hideEditingProperty();
            this.hideNewCategory();
            this.hideNewProperty();

            this._selectedProperty = null;
        });
    }

    get selectedCategory() {
        return this._selectedCategory;
    }

    get selectedProperty() {
        return this._selectedProperty;
    }

    hideNewCategory() {
        this.newCategory = false;
        this._selectFirstCategory();
    }

    hideNewProperty() {
        this.newProperty = false;
    }

    hideEditingCategory() {
        this.focusEditingCategory = false;
        this.editingCategory = null;
        this.editingCategoryName = null;
    }

    hideEditingProperty() {
        this.focusEditingProperty = false;
        this.editingProperty = null;
        this.editingPropertyName = null;
    }

    endOfLoop() {
        this._selectFirstCategory();
    }

    selectProperty(property) {
        let typeName = typeof(property);

        if (typeName === "object") {
            if (this._selectedProperty != property) {
                this.hideNewProperty();
                this.hideEditingProperty();
                this.hideEditingCategory();
            }

            this._selectedProperty = property;
        }
    }

    selectCategory(category) {
        // hide new category
        if (this.categories.length > 0 && this._selectedCategory == null && this.newCategory) {
            this.newCategory = false;
        }

        let typeName = typeof(category);

        // category object passed; otherwise the name was passed.
        if (typeName === "object") {
            if (this._selectedCategory != category) {
                this.hideEditingCategory();
                this.hideEditingProperty();
                this._selectedProperty = null;
            }

            this._selectedCategory = category;
        } else {
            let match = this._getCategoryByName(category);

            if (this._selectedCategory != match) {
                this.hideEditingCategory();
                this.hideEditingProperty();
                this._selectedProperty = null;
            }

            this._selectedCategory = match;
        }

        this.hideNewProperty();
    }

    onNewCategory() {
        this.newCategory = !this.newCategory;
        this.hideEditingCategory();

        if (this.newCategory) {
            this.newCategoryName = null;
            this.selectCategory(null);
        } else {
            this._selectFirstCategory();
        }

        this.focusCategoryName = this.newCategory;
    }

    onNewProperty() {
        // don't show property textbox if new category textbox is showing too
        if (this.newCategory) return;
        if (this.categories.length == 0) return;

        this.hideEditingCategory();
        this.hideEditingProperty();
        this._selectedProperty = null;
        this.newProperty = !this.newProperty;
        this.newPropertyName = null;
        this.focusPropertyName = this.newProperty;
    }

    addCategory(valid) {
        if (!valid) return;
        if (this.categoryNameExists(this.newCategoryName)) return;

        let newCat = {
            name: this.newCategoryName,
            properties: []
        };

        this.categories.push(newCat);
        this._sortCategories();
        this._save();

        // hide textbox
        this.newCategory = false;
        this.selectCategory(this.newCategoryName);
        this.newCategoryName = null;
    }

    editCategory(valid) {
        if (!valid) return;
        if (this.categoryNameExists(this.editingCategoryName)) return;

        this.editingCategory.name = this.editingCategoryName;
        this._save();
        this._sortCategories();

        // reset
        this.selectCategory(this.editingCategoryName);
        this.editingCategory = null;
        this.editingCategoryName = null;
    }

    addProperty(valid) {
        if (!valid) return;
        if (this.propertyNameExists(this.newPropertyName)) return;

        let newProp = {
            name: this.newPropertyName
        };

        this.selectedCategory.properties.push(newProp);
        this._save();

        this.newProperty = false;
    }

    editProperty(valid) {
        if (!valid) return;
        if (this.propertyNameExists(this.editingPropertyName)) return;

        this.editingProperty.name = this.editingPropertyName;
        this._save();

        // reset
        this.editingProperty = null;
        this.editingPropertyName = null;
    }


    categoryNameExists(name) {
        let match = this._getCategoryByName(name);
        return match != null && match != undefined;
    }

    propertyNameExists(name) {
        let match = this.selectedCategory.properties.find(p => p.name.toLowerCase() === name.toLowerCase());
        return match != null && match != undefined;
    }

    onEditCategory(c) {
        this.editingCategory = c;
        this.focusEditingCategory = true;
        this.editingCategoryName = c.name;
    }

    onEditProperty(p) {
        this.editingProperty = p;
        this.focusEditingProperty = true;
        this.editingPropertyName = p.name;
    }

    onDeleteCategory(c) {
        let result = confirm("Are you sure you want to delete this category?");
        if (!result) return;

        let i = this.categories.indexOf(c);
        if (i > -1) {
            this._selectNextCategory(c);
            this.categories.splice(i, 1);
            this._save();
        }
    }

    onDeleteProperty(p) {
        let result = confirm("Are you sure you want to delete this property?");
        if (!result) return;

        let i = this.selectedCategory.properties.indexOf(p);
        if (i > -1) {
            this.selectedCategory.properties.splice(i, 1);
            this._save();
        }
    }

    _selectFirstCategory() {
        if (this.selectedCategory == null) {
            this._sortCategories();
            this.selectCategory(this.categories[0]);
        }
    }

    _selectNextCategory(c) {
        let i = this.categories.indexOf(c);

        if (i + 1 >= this.categories.length) {
            let previous = this.categories[this.categories.length - 2];
            if (previous) {
                this.selectCategory(previous);
            }
        } else {
            let next = this.categories[i + 1];
            if (next) {
                this.selectCategory(next);
            }
        }
    }

    _save() {
        let stripped = angular.toJson(this.categories);
        let obj = JSON.parse(stripped);

        this._storage.categories({
            categories: obj
        });
    }

    _hydrate() {
        this._storage.categories((data) => {
            if (!angular.equals({}, data)) {
                this.categories = data.categories;
                this._sortCategories();
                this._$scope.$apply();
            }
        });
    }

    _sortCategories() {
        this.categories = this._orderBy(this.categories, "name", false, this._compare);
    }

    _compare(v1, v2) {
        // If we don't get strings, just compare by index
        if (v1.type !== 'string' || v2.type !== 'string') {
            return (v1.index < v2.index) ? -1 : 1;
        }

        // Compare strings alphabetically, taking locale into account
        return v1.value.toLowerCase().localeCompare(v2.value.toLowerCase());
    };

    _getCategoryByName(name) {
        return this.categories.find(i => i.name.toLowerCase() === name.toLowerCase());
    }
}


CategoriesController.$inject = ["$scope", "StorageService", "orderByFilter"];