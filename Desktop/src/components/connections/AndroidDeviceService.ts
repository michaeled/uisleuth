declare var PubSub;
declare var require;

import { remote } from 'electron';
import * as netutils from '../global/utilities/net';
import * as adblocator from './ADBLocator';
import StatusbarController from '../footer/StatusbarController';

const adb = require('adbkit');
const Promise = require('bluebird'); 

export interface IDeviceEvent {
    devices: any;
}

export interface IDevice {
    name: string;
    id: string;
    ip: string;
    port: any;
    nic: any;
    showNic: boolean;
    validIp: boolean;

    features: any;
    properties: any;
}

export default class AndroidDeviceService {
    private _adbFound: boolean;
    private _client: any;
    private _devices: IDevice[] = [];
    private _attached: IDevice = null;
    private _ready: boolean = false;

    static DevicesChangedEvent = "ADBF.DevicesChangedEvent";

    constructor() {
        const adbloc = adblocator.installdir();
        this._adbFound = adbloc != null;
        
        this._client = adbloc != null ? adb.createClient({ bin: adbloc }) : adb.createClient();
    }

    get client(): any {
        return this._client;
    }

    get devices(): IDevice[] {
        return this._devices;
    }

    get attached(): IDevice {
        return this._attached;
    }

    get adbFound(): boolean {
        return this._adbFound;
    }

    attachByIp(ip: string): boolean {
        if (!ip) return false;

        if (this.devices) {
            this._attached = this.devices.find(d => d.ip === ip);
        }

        return this.attached != null;
    }

    track(): void {
        if (this._ready) return;

        this.client.trackDevices()
            .then(tracker => {
                tracker.on('add', device => {
                    this.add(device);
                    console.log('Android device %s was plugged-in.', device.id)
                });

                tracker.on('remove', device => {
                    this.remove(device);
                    console.log('Android device %s was unplugged.', device.id)
                });

                tracker.on('change', device => {
                    this.add(device);
                    console.log('Android device %s changed.', device.id);
                });

                tracker.on('end', () => {
                    this._devices = [];
                })
            })
            .catch(err => {
                this._ready = false;
            })

        this._ready = true;
    }

    async add(device: IDevice) {
        this.remove(device);

        // add device
        this._devices[this._devices.length] = device;

        // set networking info
        device.nic = null;
        device.showNic = false;

        this.setIp(device.id, device).then(result => {
            device.validIp = result;
            PubSub.publish(AndroidDeviceService.DevicesChangedEvent, { devices: this.devices });
            this.setDeviceCapabilities(device);
        });
    }

    private setIp(input, device: IDevice) {
        return new Promise(resolve => {
            this.checkForVisualStudioEmulator(input, device)
                .then(result => {
                    if (result) {
                        resolve(result);
                    } else {
                        this.checkForGoogleEmulator(device)
                            .then(result => {
                                if (result) {
                                    resolve(result);
                                } else {
                                    this.checkUsingIpAddr(device)
                                        .then(result => {
                                            if (result) {
                                                resolve(result);
                                            } else {
                                                this.checkUsingIpRoute(device)
                                                    .then(result => {
                                                        resolve(result);
                                                    });
                                            }
                                        })
                                    }
                            });
                    }
                });
        });
    }

    private checkForVisualStudioEmulator(input, device): Promise<boolean> {
        return new Promise(resolve => {
            let ip: string = null;
            let port: number = null;

            // Check if a Visual Studio Emulator
            if (input.includes(":")) {
                let address = input.split(":");
                ip = address[0];
                port = address[1];
            }

            if (netutils.isIPV4(ip)) {
                device.ip = ip;
                device.port = port;
                resolve(true);
            } else {
                resolve(false);
            }
        });
    }

    private checkForGoogleEmulator(device): Promise<boolean> {
        return new Promise(resolve => {
            this.client.shell(device.id, 'getprop net.gprs.local-ip')
                .then(adb.util.readAll)
                .then(output => {
                    if (!output) {
                        resolve(false);
                        return;
                    }
                
                    const converted = output.toString('utf-8').trim();
                    
                    if (netutils.isIPV4(converted)) {
                        device.ip = converted;
                        device.port = null;
                        resolve(true);
                    } else {
                        resolve(false);
                    }
                });
        });
    }

    private checkUsingIpRoute(device: IDevice): Promise<boolean> {
        return new Promise(resolve => {
            // use linux commands
            this.client.shell(device.id, 'ip route show table local | grep host | grep -v " lo " | cut -d " " -f2')
                .then(adb.util.readAll)
                .then(output => {
                    if (!output) {
                        resolve(false);
                        return;
                    }

                    const converted = output.toString('utf-8').trim();
                    
                    if (netutils.isIPV4(converted)) {
                        device.ip = converted;
                        device.port = null;
                        resolve(true);
                    }
                    else {
                        resolve(false);
                    }
                });
        });
    }

    private checkUsingIpAddr(device: IDevice): Promise<boolean> {
        return new Promise(resolve => {
            // use linux commands
            this.client.shell(device.id, 'ip addr show | grep global')
                .then(adb.util.readAll)
                .then(output => {
                    if (!output) {
                        resolve(false);
                        return;
                    }

                    const converted = output.toString('utf-8').trim();
                    const lines = converted.split('\n');
                    
                    if (lines.length > 0) {
                        // returns first ip address found.
                        const match = lines[0].match(netutils.IPV4Pattern);

                        if (match != null && netutils.isIPV4(match[0])) {
                            device.ip = match[0];
                            device.port = null;
                            resolve(true);
                        }
                        else {
                            resolve(false);
                        }
                    }
                    else {
                        resolve(false);
                    }
                });
        });
    }

    private setDeviceCapabilities(device): void {
        this.client.getProperties(device.id)
            .then(properties => {
                this.setProperties(device, properties);
            });

        this.client.getFeatures(device.id)
            .then(features => {
                this.setFeatures(device, features);
            });
    }

    private setFeatures(device: IDevice, features): void {
        device.features = features;
        PubSub.publish(AndroidDeviceService.DevicesChangedEvent, { devices: this.devices });
    }

    private setProperties(device: IDevice, properties): void {
        device.properties = properties;
        device.name = properties["ro.product.model"];
        PubSub.publish(AndroidDeviceService.DevicesChangedEvent, { devices: this.devices });
    }

    private remove(device: IDevice): void {
        let index = this.devices.findIndex(d => d.id == device.id);

        if (index != -1) {
            this._devices.splice(index, 1);
            PubSub.publish(AndroidDeviceService.DevicesChangedEvent, { devices: this.devices });
        }
    }
}