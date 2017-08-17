import DesignerDimensionsService, {IDesignerDimensions} from '../../global/DesignerDimensionsService';

export default class DisplayPanelController {
    public static $inject = [
        "$scope",
        "DesignerDimensionsService"
    ];

    public display: IDesignerDimensions = null;
    public ready: boolean = false;

    constructor(private $scope, public dims: DesignerDimensionsService) {
        $scope.$watch("propctrl.editors", (newval, oldval) => {
            if (newval != oldval) {
                this.init();
            }
        });
    }

    public init(): void {
        this.display = this.dims.display;
        this.ready = true;
    }
}
