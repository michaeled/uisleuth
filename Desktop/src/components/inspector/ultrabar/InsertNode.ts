declare var PubSub;

import UltraBarAction from '../UltraBarAction';
import CreateWidgetRequest from '../../messages/CreateWidgetRequest';
import CreateGridRequest from '../../messages/CreateGridRequest';
import CreateStackLayoutRequest from '../../messages/CreateStackLayoutRequest';
import VisualTreeTabController from '../VisualTreeTabController';
import ConnectionService from '../../connections/ConnectionService';
import DesignerVisualElementsService from '../../global/DesignerVisualElementsService';

export default class InsertNode extends UltraBarAction {
    public static CreatedWidgetEvent: string = "InsertNode.CreatedWidgetEvent";

    private ui: any = null;
    private $scope: any = null;
    private payload: any = null;

    constructor(connection: ConnectionService, elements: DesignerVisualElementsService) {
        super(connection, elements);
    }

    public static canExecute(payload): boolean {
        if (!payload || !payload.node) return false;
        return payload.node.isLayout;
    }

    public execute($scope, ui, payload): void {
        ui.show = !ui.show;
        if (ui.ready) return;

        this.$scope = $scope;
        this.ui = ui;
        this.payload = payload;

        this.setPage('list');
        this.setupTabs();

        ui.clicked = this.itemClicked.bind(this);
        ui.reset = this.reset.bind(this);
        ui.shortname = this.shortname.bind(this);
        ui.setPage = this.setPage.bind(this);

        ui.filter = "";
        ui.ready = true;
    }

    public setPage(name: string): void {
        this.ui.activePage = name;
    }

    private setupTabs(): void {
        let tabs = [
            {'usage': 'Views', 'items': this.elements.elements.views.map(mapper)},
            {'usage': 'Layouts', 'items': this.elements.elements.layouts.map(mapper)},
            {'usage': 'Other', 'items': this.elements.elements.others.map(mapper)}
        ];

        this.ui.tabs = tabs;
        this.ui.selected = tabs[0];

        function mapper(a) { return {'fullName': a}; }
    }

    private reset(): void {
        this.ui.show = false;
        this.ui.selected = this.ui.tabs[0];
        this.ui.filter = "";
    }

    private shortname(longname): string {
        if (longname) {
            let shortname = longname.split(".");
            if (shortname.length === 0) return longname;
            return shortname[shortname.length - 1];
        }
        return longname;
    }

    private setupGrid(): void {
        let ubctrl = this.$scope.ubctrl;
        let ui = this.ui;
        let parentId = this.payload.node.id;

        ui.grid = {};
        ui.grid.props = {
            columns: 1,
            rows: 1,
            columnSpacing: 5,
            rowSpacing: 5
        }

        ui.grid.create = () => {
            let req = new CreateGridRequest(parentId,
                ui.grid.props.columns,
                ui.grid.props.rows,
                ui.grid.props.columnSpacing,
                ui.grid.props.rowSpacing);

            this.connection.send(req);

            PubSub.publish(VisualTreeTabController.RefreshVisualTreeAndSelectNode, parentId);
            this.publishCreatedEvent("Grid", `Created with ${ui.grid.props.columns} column(s) and ${ui.grid.props.rows} row(s).`)

            ubctrl.close();
        };

        this.setPage('grid');
    }

    private setupStackLayout(): void {
        let ubctrl = this.$scope.ubctrl;
        let ui = this.ui;
        let parentId = this.payload.node.id;

        ui.stacklayout = {};
        ui.stacklayout.props = {
            orientation: "vertical",
            spacing: 5
        }

        ui.stacklayout.create = () => {
            let req = new CreateStackLayoutRequest(parentId,
                ui.stacklayout.props.orientation,
                ui.stacklayout.props.spacing);

            this.connection.send(req);

            PubSub.publish(VisualTreeTabController.RefreshVisualTreeAndSelectNode, parentId);
            this.publishCreatedEvent("StackLayout", `Created with a ${ui.stacklayout.props.orientation} orientation.`)

            ubctrl.close();
        };

        this.setPage('stacklayout');
    }

    private itemClicked(item): void {
        let ubctrl = this.$scope.ubctrl;
        let parentId = this.payload.node.id;

        if (item.fullName === "Xamarin.Forms.Grid") {
            this.setupGrid();
        } else if (item.fullName === "Xamarin.Forms.StackLayout") {
            this.setupStackLayout();
        } else {
            let req = new CreateWidgetRequest(parentId, item.fullName);
            this.connection.send(req);

            PubSub.publish(VisualTreeTabController.RefreshVisualTreeAndSelectNode, parentId);
            this.publishCreatedEvent(this.shortname(item.fullName), null);

            ubctrl.close();
        }
    }

    private publishCreatedEvent(typeName: string, description: string): void {
        PubSub.publish(InsertNode.CreatedWidgetEvent, {
            typeName,
            description
        })
    }
}