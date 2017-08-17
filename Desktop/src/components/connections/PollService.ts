declare var require;

import ConnectionService from './ConnectionService';
import OnlineRequest from '../messages/OnlineRequest';
import {IDiscoveredApp} from './IDiscoveredApp';

const EventEmitter = require('events').EventEmitter;
const util = require('util');
const os = require('os');

export interface IOnlineResponse {
    port: number;
    app: string;
    state: string;
}

export default class PollService {   
    private emit: any;
    private ws: any;
    public on: any;

    public static $inject = [
        "ConnectionService"
    ];

    public static TimeoutPeriod: number = 7000;
    public static AppOnline: string = "online";

    public apps: IDiscoveredApp[] = [];

    private _token: any = null;
    private _running: boolean = false;
    
    constructor(private connection: ConnectionService) {
        EventEmitter.call(this);
        console.trace("PollService init.");
    }

    public get running() {
        return this._running;
    }

    public start(): void {
        if (this._token) {
            console.error("PollService: stop the service before calling this method, again.");
            return;
        }

        if (os.platform() !== "darwin") {
            console.trace("PollService: stopping; not supported on windows.");
            return;
        }

        this.apps = [];

        this._token = setInterval(() => {
            this._running = true;

            if (this.connection.connected) {
                return;
            }

            this.discover();
        }, PollService.TimeoutPeriod);

        console.trace("PollService: started polling.");
    }

    public stop(): void {
        if (this._token) {
            clearInterval(this._token);

            this._token = null;
            this._running = false;

            if (this.ws) {
                this.ws.close();
                this.ws = null;
            }
            
            console.trace("PollService: stopped polling.");
        }
    }

    private _wait: boolean = false;
    private discover(): void {
        if (this._wait) return;

        this._wait = true;
        this.ws = new WebSocket("ws://localhost:9099/heartbeat");

        this.ws.onopen = () => {
            this.ws.send(new OnlineRequest());
        };

        this.ws.onmessage = (evt) => {
            const msg = JSON.parse(evt.data);
            this.process(msg);
            this._wait = false;
        };

        this.ws.onerror = () => {
            this._wait = false;
        };
    }

    private process(data: IOnlineResponse) {
        if (data.state == "online") {
            let app = this.track(data, true);
            this.emit(PollService.AppOnline, data);
        }
    }

    private track(request: IOnlineResponse, online: boolean): IDiscoveredApp {
        this.apps = [];

        let app = {
            ip: "localhost",
            name: request.app,
            online: online,
            os: "iOS",
            port: request.port
        };

        this.apps.push(app);
        return app;
    }
}

util.inherits(PollService, EventEmitter);