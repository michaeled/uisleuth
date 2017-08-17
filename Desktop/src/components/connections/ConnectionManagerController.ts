declare var PubSub;
declare var require;

import { remote, clipboard } from 'electron';
import AndroidDeviceService, { IDeviceEvent, IDevice } from './AndroidDeviceService';
import StatusbarController from '../footer/StatusbarController';
import ConnectionService from './ConnectionService';
import StorageService from '../global/StorageService';
import { IDiscoveredApp } from './IDiscoveredApp';
import PollService from './PollService';

const os = require('os');
const DefaultPort: number = 9099;

export default class ConnectionManagerController {
    private activeTab : string = 'local';

    public os: string = null;
    public address: string;
    public port: number;
    public remember: boolean = false;
  
    public connecting: boolean = false;
    public devices: IDevice[] = [];
    public focus: boolean;
    public apps: IDiscoveredApp[] = [];

    static $inject = [
        "$rootScope",
        "$scope",
        "$location",
        "ConnectionService",
        "StorageService",
        "AndroidDeviceService",
        "DeviceDialogFactory",
        "PollService"
    ];
    
    constructor(private $rootScope, private $scope, private $location, private connection: ConnectionService,
        private storage: StorageService, private adb: AndroidDeviceService,
        private deviceDialog, private poll: PollService) {
            
        this.address = connection.address;
        this.port = connection.port;

        if (!connection.connected) {
            this.focus = true;
        }

        this.os = os.platform();

        this.hydrate();
        this.trackDevices();
        this.pollApps();

        this.poll.start();

        let adbToken = PubSub.subscribe(AndroidDeviceService.DevicesChangedEvent, (e, d: IDeviceEvent) => {
            this.devices = d.devices;
            $scope.$digest();
        });

        $scope.$on("$destroy", () => {
            this.poll.stop();
            PubSub.unsubscribe(adbToken);
        });

        console.trace("connection manager controller init.");
    }

    get label(): string {
        if (this.connecting) return "Connecting";
        return this.$scope.main.connected ? 'Disconnect' : 'Connect';
    }

    get tab(): string {
        return this.activeTab;
    }

    isAddressValid(el): boolean {
        return el.$submitted && el.address.$invalid;
    }

    isPortValid(el): boolean {
        return el.$submitted && el.port.$invalid;
    }

    onClose(): void {
        this.connection.close();
        StatusbarController.Info();
    }

    onTabClicked($event, name: string): void {
        $event.preventDefault();
        this.activeTab = name;
    }

    onAppAttach(app: IDiscoveredApp): void {
        if (this.connecting) return;
        this.connect(app.ip, app.port);
    }

    onDeviceAttach(device: IDevice): void {
        if (this.connecting) return;

        let nic = null;

        if (device.showNic && device.nic) {
            nic = device.nic;
        }

        if (device.validIp) {
            this.connect(device.ip, DefaultPort);
        } else {
            this.adb.client.getDHCPIpAddress(device.id, nic)
                .then(ip => {
                    this.connect(ip, DefaultPort);
                    this.$scope.$digest();
                }).catch(e => {
                    StatusbarController.Error(998, `${e.message}. Connect to ${device.ip} using the Manual Connection tab. Default port is ${DefaultPort}.`);
                    this.connecting = false;
                    this.$scope.$digest();
                });
        }
    }

    onConnect(form): void {
        if (form.$invalid) {
            return;
        }

        this.save();
        this.connect(this.address, this.port);
    }

    onOpen(n, d): void {
        this.$location.path("inspector");
        this.connecting = false;
        let reason: string = null;

        if (this.adb.attached && this.adb.attached) {
            reason = `Connected to ${this.adb.attached.name}.`;
        } else {
            reason = "Connected.";
        }

        StatusbarController.Info(reason);
        this.$scope.$apply();
    }

    onError(n, d): void {
        StatusbarController.Error(d.code, d.reason);
        this.connecting = false;
        this.$scope.$digest();
    }

    onServerKilled(n, d): void {
        this.$rootScope.$apply(() => {
            StatusbarController.Info("Disconnected.");
            this.$location.path("/").search({ force: true });
        })
    }

    onShowFeatures(device: IDevice): void {
        this.deviceDialog.open(device);
    }

    onCopyIpAddress($event, id): void {
        $event.preventDefault();
        let el = document.getElementById(id);

        if (el) {
            let text = el.innerText.trim();
            clipboard.writeText(text);
        }
    }

    private connect(address, port): void {
        this.connecting = true;
        StatusbarController.Info();

        this.adb.attachByIp(address);

        this.connection.connect({
            onOpen: this.onOpen.bind(this),
            onError: this.onError.bind(this),
            onServerKilled: this.onServerKilled.bind(this),
            address: address,
            port: port
        });
    }

    private save(): void {
        let defaults = this.connection.defaults;

        this.storage.quickConnect({
            address: this.remember ? this.address : defaults.address,
            port: this.remember ? this.port : defaults.port,
            remember: this.remember
        });
    }

    private hydrate(): void {
        this.storage.quickConnect((data) => {
            if (!this.connection.connected) {
                this.address = data.address;
                this.port = data.port;
            }

            this.remember = data.remember;
            this.$scope.$digest();
        });
    }

    private trackDevices(): void {
        try {
            this.adb.track();
            this.devices = this.adb.devices;
        } catch (ex) {
            StatusbarController.Error(996, `Error querying devices. Ensure ADB is in your path. Try connecting through the Manual Connection tab.`);
        }
    }

    private pollApps(): void {
        this.poll.on(PollService.AppOnline, () => {
            this.apps = this.poll.apps;
            this.$scope.$digest();
        });
    }
}