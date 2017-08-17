declare var PubSub;
declare var angular;

import GetVisualTreeRequest from '../messages/GetVisualTreeRequest';
import SupportedTypesRequest from '../messages/SupportedTypesRequest';
import GetDisplayDimensionsRequest from '../messages/GetDisplayDimensionsRequest';
import GetVisualElementsRequest from '../messages/GetVisualElementsRequest';
import DesktopReady from '../messages/DesktopReady';

import StorageService from '../global/StorageService';
import ConnectionService from '../connections/ConnectionService';
import MessageDispatcherService from '../connections/MessageDispatcherService';
import XamlChangeTrackerService from './xaml/XamlChangeTrackerService';

export default class WorkspaceController {
    public static $inject = [
        "$scope",
        "ConnectionService",
        "StorageService",
        "XamlChangeTrackerService"
    ];
    
    private _mobileReady: boolean = false;

    constructor(private $scope, private connection: ConnectionService, private storage: StorageService, private xaml: XamlChangeTrackerService) {
        let drToken = PubSub.subscribe(MessageDispatcherService.MobileReady, (e, data) => {
            this.onMobileReady(data);
        });

        let pcToken = PubSub.subscribe(MessageDispatcherService.PageChanged, (e, data) => {
            this.sendPageChanged();
        });

        $scope.$on("$destroy", () => {
            PubSub.unsubscribe(drToken);
            PubSub.unsubscribe(pcToken);
        });

        this.sendDesktopReady();
        console.trace("WorkspaceController init.");
    }

    public set mobileReady(value: boolean) {
        this._mobileReady = value;
    }

    public get mobileReady(): boolean {
        return this._mobileReady;
    }

    private onMobileReady(ready: boolean): void {
        this._mobileReady = ready;
        this.$scope.$apply();

        this.sendSupportedTypes();
        this.sendDisplayDimensionRequest();
        this.sendVisualTreeRequest();
        this.sendVisualElementsRequest();
        
        this.$scope.$broadcast("uisleuth.inspector.ready");
    }

    private sendDesktopReady(): void {
        this.connection.send(new DesktopReady());
    }

    private sendDisplayDimensionRequest(): void {
        let req = new GetDisplayDimensionsRequest();
        this.connection.send(req);
    }

    private sendVisualElementsRequest(): void {
        let req = new GetVisualElementsRequest();
        this.connection.send(req);
    }

    private sendVisualTreeRequest(): void {
        let req = new GetVisualTreeRequest();
        this.connection.send(req);
    }

    private sendPageChanged(): void {
        let req = new GetVisualTreeRequest();
        this.connection.send(req);
    }

    private sendSupportedTypes(): void {
        this.storage.types((data) => {
            if (!angular.equals({}, data)) {
                let req = new SupportedTypesRequest(data.types);
                this.connection.send(req);
            } else {
                console.trace("no types loaded to send to the designer.");
            }
        });
    }
}