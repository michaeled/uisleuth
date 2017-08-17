import StorageService from "../global/StorageService";

export default class TypesController {
    constructor($scope, storage) {
        this._$scope = $scope;
        this._storage = storage;

        this.newType = false;
        this.newTypeName = null;
        this.focusTypeName = false;
        this.types = [];

        this.focusEditingType = false;
        this.selectedType = null;
        this.editingType = null;
        this.editingTypeName = null;

        this._hydrate();

        $scope.$on("uisleuth.megamenu.closing", e => {
            this.hideEditingType();
            this.hideNewType();

            this.selectedType = null;
        });
    }

    selectType(t) {
        this.hideNewType();

        if (this.selectedType != t) {
            this.hideEditingType();
        }

        this.selectedType = t;
    }

    onNewType() {
        this.newType = !this.newType;
        this.selectedType = null;

        if (this.newType) {
            this.focusTypeName = true;
        }
    }

    onEditType(t) {
        this.hideNewType();
        this.editingTypeName = t.fullName;
        this.editingType = t;
        this.focusEditingType = true;
    }

    addType(valid) {
        if (!valid) return;

        let match = this.types.find(t => t.fullName.toLowerCase() === this.newTypeName.toLowerCase());
        if (match) return;

        let newType = StorageService.newType(this.newTypeName);

        this.types.push(newType);
        this._save();

        this.newType = false;
        this.newTypeName = null;
    }

    editType(valid) {
        if (!valid) return;
        if (this._typeNameExists(this.editingTypeName)) return;

        this.editingType.fullName = this.editingTypeName;
        this._save();

        // reset
        this.hideEditingType();
        this.editingType = null;
        this.editingTypeName = null;
    }

    hideNewType() {
        this.newType = false;
        this.newTypeName = null;
        this.focusTypeName = false;
    }

    hideEditingType() {
        this.editingType = null;
        this.editingTypeName = null;
        this.focusEditingType = false;
    }

    onDeleteType(t) {
        let result = confirm("Are you sure you want to delete this type?");
        if (!result) return;

        let i = this.types.indexOf(t);
        if (i > -1) {
            this.types.splice(i, 1);
            this._save();
        }
    }

    _typeNameExists(name) {
        let match = this.types.find(t => t.fullName.toLowerCase() === name.toLowerCase());
        return match != null && match != undefined;
    }

    _hydrate() {
        this._storage.types((data) => {
            if (!angular.equals({}, data)) {
                this.types = data.types;
                this._$scope.$apply();
            }
        });
    }

    _save() {
        let stripped = angular.toJson(this.types);
        let obj = JSON.parse(stripped);

        this._storage.types({
            types: obj
        });
    }
}

TypesController.$inject = ["$scope", "StorageService"];