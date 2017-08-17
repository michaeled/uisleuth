declare var require;

import {IDiscoveredApp} from './IDiscoveredApp';

const http = require('http');
const EventEmitter = require('events').EventEmitter;
const util = require('util');

export interface IAnnouncementRequest {
    action: string;
    name: string;
    ip: string;
    port: number;
    os: string;
}

export default class AnnouncementService {
    private emit: any;
    public on: any;

    public static ServerOffline: string = "server-offline";
    public static ServerOnline: string = "server-online";

    public static AppOnline: string = "app-online";
    public static AppOffline: string = "app-offline";
    public static AppUpdated: string = "app-updated";
    public static Error: string = "error";
    public static BadRequest: string = "bad-request";
    public static DefaultAnnouncementPort: number = 9089;
    
    public apps: IDiscoveredApp[] = [];

    private _listening: boolean = false;
    private server: any;
    
    constructor() {
        EventEmitter.call(this);
        console.trace("AnnouncementService init.");
    }

    public get listening() {
        return this._listening;
    }

    public start(port: number): void {
        if (this.server) {
            console.error("AnnouncementService: stop the server before calling this method, again.");
            return;
        }

        this.apps = [];
        this.server = http.createServer();

        this.server.on("listening", () => {
            console.log("AnnouncementService: listening...");
            this._listening = true;
            this.emit(AnnouncementService.ServerOnline);
        });

        this.server.on("error", (e) => {
            console.log("AnnouncementService: Error", e);
        });

        this.server.on("request", this.onrequest.bind(this));

        this.server.on("close", () => {
            console.log("AnnouncementService: stopped listening...");
            this._listening = false;
        });

        this.server.listen(port);
    }

    public stop(): void {
        if (this.server) {
            this.server.close(() => {
                this.emit(AnnouncementService.ServerOffline);
                this.server = null;
            });
        }
    }

    private onrequest(req, res): void {
        if (req.method === "POST") {
            req.setEncoding("utf8");
            let data = "";

            req.on("data", chunk => {
                data += chunk;
            });

            req.on("end", () => {
                try {
                    const obj = JSON.parse(data);
                    let result = this.process(obj);
                    
                    if (result) {
                        res.writeHead(200);
                    }
                    else {
                        res.writeHead(400);
                        this.emit(AnnouncementService.BadRequest, "Unhandled announcement action.");
                    }
                }
                catch (err) {
                    this.emit(AnnouncementService.Error, err);
                    res.writeHead(500);
                }

                res.end();
            });
        }
        else {
            this.emit(AnnouncementService.BadRequest, "Unhandled HTTP verb.");
            res.writeHead(400);
            res.end();
        }
    }

    private process(request: IAnnouncementRequest): boolean {
        let handled = false;

        if (request.action == AnnouncementService.AppOnline) {
            let app = this.track(request, true);
            this.emit(AnnouncementService.AppOnline, request);

            handled = true;
        }
        else if (request.action == AnnouncementService.AppOffline) {
            let app = this.remove(request);
            this.emit(AnnouncementService.AppOffline, request);

            handled = true;
        }

        if (handled) {
            this.emit(AnnouncementService.AppUpdated);
        }

        return handled;
    }

    private track(request: IAnnouncementRequest, online: boolean): IDiscoveredApp {
        this.remove(request);

        let app = {
            ip: request.ip,
            name: request.name,
            online: online,
            os: request.os,
            port: request.port
        };

        this.apps.push(app);
        return app;
    }

    private remove(request: IAnnouncementRequest): boolean {
        let index = this.apps.findIndex(a => a.ip == request.ip);

        if (index != -1) {
            this.apps.splice(index, 1);
            return true;
        }

        return false;
    }
}

util.inherits(AnnouncementService, EventEmitter);