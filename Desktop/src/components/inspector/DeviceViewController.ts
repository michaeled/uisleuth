declare var PubSub;
declare var require;
declare var fabric;

import StatusbarController from '../footer/StatusbarController';
import MessageDispatcherService from "../connections/MessageDispatcherService";
import ScreenShotRequest from "../messages/ScreenShotRequest";
import TouchScreenRequest from "../messages/TouchScreenRequest";
import SetDeviceOrientationRequest from "../messages/SetDeviceOrientationRequest";
import GestureRequest from "../messages/GestureRequest";
import ConnectionService from '../connections/ConnectionService';
import AndroidDeviceService from '../connections/AndroidDeviceService';
import DeviceCanvasService from './DeviceCanvasService';
import DesignerDimensionsService from '../global/DesignerDimensionsService';
import { SetOrientation } from '../shell/events';
import { ipcRenderer, remote } from 'electron';

const Promise = require('bluebird');
const adb = require('adbkit');
const assert = require('assert');

export default class DeviceViewController {
    public static $inject = [
        "$scope",
        "ConnectionService",
        "DeviceCanvasService",
        "DesignerDimensionsService",
        "AndroidDeviceService"
    ];

    public static TouchEnabled = "DeviceViewController.TouchEnabled";
    public src : string = null;
    public touchEnabled : boolean = true;

    private _screenshotToken = null;
    private _screenshotTimeout : number = 5000;
    
    constructor(private $scope, private connections: ConnectionService, private canvas: DeviceCanvasService, private dims: DesignerDimensionsService, private adb: AndroidDeviceService) {
        let ssrToken = PubSub.subscribe(MessageDispatcherService.ScreenShotResponse, (e, d) => {
            this.update(d);
        });

        let teToken = PubSub.subscribe(DeviceViewController.TouchEnabled, (e, d) => {
            this.touchEnabled = d;
            $scope.$apply();
        })

        ipcRenderer.on(SetOrientation, (e, m) => {
            if (!this.connections.connected) {
                return;
            }

            let req = new SetDeviceOrientationRequest(m);
            this.connections.send(req);
        });

        $scope.$on("$destroy", () => {
            $scope.wsctrl.mobileReady = false;

            clearTimeout(this._screenshotToken);
            PubSub.unsubscribe(ssrToken);
            PubSub.unsubscribe(teToken);
            ipcRenderer.removeAllListeners(SetOrientation);
        });

        $scope.$watch(() => $scope.wsctrl.mobileReady, (newval, oldval) => {
            console.trace("mobile device ready; requesting device view.");
            canvas.init();

            if (newval) {
                this.screenshot();
                this.startScreenshotTimer();
            }
        });

        console.trace("device view controller init.");
    }

    public gesture(points: {x:number, y:number}[]): void {
        if (points.length === 0) return;
        if (!this.touchEnabled) return;

        this.removeGestures();

        let pencil = new fabric.PencilBrush(this.canvas.device);
        let res = pencil.convertPointsToSVGPath(points);
        let path = pencil.createPath(res.toString());
        
        path.set({ strokeWidth: 3, stroke: "rgb(0,159,214)" });
        path._isGesture = true;

        this.canvas.device.add(path);
        setTimeout(() => this.removeGestures(), 500);

        let display = this.dims.display;
        let scaleX = display.width / this.canvas.size.width;
        let scaleY = display.height / this.canvas.size.height;

        let scaled = points.map(p => {
            p.x = Math.ceil(p.x * scaleX);
            p.y = Math.ceil(p.y * scaleY);

            return p;
        });

        let gesture = new GestureRequest(scaled, 100);
        this.connections.send(gesture);
    }

    public touch($event): void {
        if (!this.touchEnabled) return;

        const duration = 100;

        let display = this.dims.display;
        let scaleX = display.width / this.canvas.size.width;
        let scaleY = display.height / this.canvas.size.height;

        let x = Math.ceil($event.offsetX * scaleX);
        let y = Math.ceil($event.offsetY * scaleY);

        let req = new TouchScreenRequest(x, y, duration);
        this.connections.send(req);
    }

    private update(d): void {
        this.src = "data:image/jpg;base64," + d.capture;
        this.canvas.show(this.src);
        this.$scope.$digest();
    }

    private startScreenshotTimer(): void {
        if (this._screenshotToken != null) {
            clearInterval(this._screenshotToken);
            console.trace("device view refresh timer cleared.");
        }

        this._screenshotToken = setInterval(() => {
            this.screenshot();
        }, this._screenshotTimeout);
    }

    private screenshot(): void {
        let req = new ScreenShotRequest();
        this.connections.send(req);
    }

    private removeGestures(): void {
        for (const o of this.canvas.device.getObjects()) {
            if (o._isGesture) {
                this.canvas.device.remove(o);
            }
        }
    }
}