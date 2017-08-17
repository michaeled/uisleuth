import env from '../../../env';
import ThicknessEditor from "../editors/ThicknessEditor";

const DoubleTypeName = "System.Double";
const MarginPropertyName = "Margin";
const PaddingPropertyName = "Padding";
const WidthPropertyName = "Width";
const HeightPropertyName = "Height";
const XPropertyName = "X";
const YPropertyName = "Y";

export default class OverviewPanelController {
    constructor($scope) {
        this.ready = false;

        $scope.$watch(() => $scope.propctrl.editors, (newval, oldval) => {
            this.margin = null;
            this.padding = null;
            this.width = null;
            this.height = null;
            this.x = null;
            this.y = null;
            this.typeName = null;
            this.hasHelp = false;
            this.hasDetails = false;

            this.absX = 0;
            this.absY = 0;
            this.relX = 0;
            this.relY = 0;

            if (newval != oldval) {
                this.init(newval);
            }
        });
    }

    get template() {
        if (this.padding && this.margin && this.width && this.height) {
            return 'components/inspector/editors/shared/_computed.layout.htm'
        }

        if (!this.padding && this.margin && this.width && this.height) {
            return 'components/inspector/editors/shared/_computed.view.htm'
        }

        if (this.padding && !this.margin && this.width && this.height) {
            return 'components/inspector/editors/shared/_computed.page.htm'
        }

        return null;
    }

    get helpUrl() {
        if (!this.hasHelp) return null;
        return `${env.help['xf.url.type']}${this.typeName}`;
    }

    init(editors) {
        if (!editors) return;

        this.ready = false;

        for (const e of editors) {
            if (this._isMargin(e)) {
                this.margin = e;
            }

            if (this._isPadding(e)) {
                this.padding = e;
            }

            if (this._isHeight(e)) {
                this.height = e;
            }

            if (this._isWidth(e)) {
                this.width = e;
            }

            if (this._isX(e)) {
                this.x = e;
            }
            if (this._isY(e)) {
                this.y = e;
            }
        }

        // random editor selected for these sets
        if (this.x) {
            this.typeName = this.x.parent.widget.fullTypeName;

            this.absX = Math.round(this.x.parent.widget.dimensions.x);
            this.absY = Math.round(this.x.parent.widget.dimensions.y);
            this.relX = Math.round(this.x.value);
            this.relY = Math.round(this.y.value);

            if (this.typeName.startsWith("Xamarin.Forms")) {
                this.hasHelp = true;
            } else {
                this.hashelp = false;
            }

            this.hasDetails = true;
        }

        this.ready = true;
    }

    _isHeight(editor) {
        if (editor.type === DoubleTypeName && editor.name === HeightPropertyName) {
            return true;
        }

        return false;
    }

    _isWidth(editor) {
        if (editor.type === DoubleTypeName && editor.name === WidthPropertyName) {
            return true;
        }

        return false;
    }

    _isMargin(editor) {
        let ector = editor.constructor.name;
        let tctor = ThicknessEditor.name;

        if (ector === tctor && editor.name === MarginPropertyName) {
            return true;
        }

        return false;
    }

    _isPadding(editor) {
        let ector = editor.constructor.name;
        let tctor = ThicknessEditor.name;

        if (ector === tctor && editor.name === PaddingPropertyName) {
            return true;
        }

        return false;
    }

    _isX(editor) {
        if (editor.type === DoubleTypeName && editor.name === XPropertyName) {
            return true;
        }

        return false;
    }

    _isY(editor) {
        if (editor.type === DoubleTypeName && editor.name === YPropertyName) {
            return true;
        }

        return false;
    }
}

OverviewPanelController.$inject = ["$scope"];