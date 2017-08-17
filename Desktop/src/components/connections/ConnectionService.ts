declare var PubSub;

const ConnectedEvent : string = "CS.connected";
const ErrorEvent : string = "CS.error";
const ServerKilledEvent : string = "CS.ServerKilledEvent";

export interface IConnectionOptions {
    address : string;
    port : number;
    onOpen : any;
    onError : any;
    onServerKilled : any;
}

export default class ConnectionService {
    public static MessageReceivedEvent: string = "CS.MessageReceivedEvent";

    private _ws: WebSocket = null;
    private _connected: boolean = false;
    private _options: IConnectionOptions = null;

    constructor() {
        console.trace("connection service init.");
    }

    public get uri() {
        //return `ws://uisleuth.azurewebsites.net/api/pair?clientKey=123::desktop`;
        return `ws://${this._options.address}:${this._options.port}/`;
    }

    public get connected() {
        return this._connected;
    }

    public get address() {
        return this._options ? this._options.address : null;
    }

    public get port() {
        return this._options ? this._options.port : null;
    }

    public get defaults(): IConnectionOptions {
        return {
            address: null,
            port: null,
            onOpen: null,
            onError: null,
            onServerKilled: null
        };
    }

    public connect(options: IConnectionOptions): void {
        if (this.connected) {
            throw "You must first disconnect.";
        }

        // cleanup previous connection
        PubSub.unsubscribe(ConnectedEvent);
        PubSub.unsubscribe(ErrorEvent);
        PubSub.unsubscribe(ServerKilledEvent);

        this._options = Object.assign({}, this.defaults, options);

        if (this._options.onOpen) {
            PubSub.subscribe(ConnectedEvent, this._options.onOpen);
        }

        if (this._options.onError) {
            PubSub.subscribe(ErrorEvent, this._options.onError);
        }

        if (this._options.onServerKilled) {
            PubSub.subscribe(ServerKilledEvent, this._options.onServerKilled);
        }

        console.log(`Connecting to ${this.uri}.`);

        this._ws = new WebSocket(this.uri);

        this._ws.onclose = (e) => {
            console.log("Connection closed.");
            PubSub.publish(ServerKilledEvent, e);
            let reason = this.getCloseReason(e);

            // error events are only raised in the onclose method. we have more info here.
            if (reason.error && !e.wasClean) {
                PubSub.publish(ErrorEvent, reason);
            }

            this.close();
        };

        this._ws.onopen = (e) => {
            console.log("Connected.");

            this._connected = true;
            PubSub.publish(ConnectedEvent, e);
        };

        this._ws.onmessage = (e) => {
            PubSub.publish(ConnectionService.MessageReceivedEvent, e.data);
        };
    }

    public send(data): void {
        if (!this._connected) throw "You cannot send data when disconnected from a device.";

        let msg = null;

        if (typeof data == "object") {
            msg = JSON.stringify(data);
        }

        this._ws.send(msg);

        console.trace("Message sent.");
    }

    public close(): void {
        if (this._ws) {
            this._ws.close();
        }

        this._connected = false;
        this._ws = null;
        this._options = null;
    }

    private getCloseReason(event): {code: number, error: boolean, reason: string} {
        let result = {
            code: event.code,
            error: true,
            reason: null
        };

        switch (event.code) {
            case 1000:
                result.error = false;
                result.reason = "The connection closed normally.";
                break;
            case 1001:
                result.reason = "The server or client forcefully closed the connection.";
                break;
            case 1002:
                result.reason = "The server terminated the connection due to a protocol error.";
                break;
            case 1003:
                result.reason = "The connection was terminated because the client or server has received an unknown data type.";
                break;
            case 1004:
            case 1005:
                result.reason = "The connection closed for unknown reason.";
                break;
            case 1006:
                // no control frame sent or received
                result.reason = "The connection was forcefully closed. The other party is not responding.";
                break;
            case 1007:
                result.reason = "The connection is terminating because it has received data that is not consistent with the type of the message.";
                break;
            case 1008:
                result.reason = "The connection is closing because a message has violated the application's policies.";
                break;
            case 1009:
                result.reason = "The connection has closed because a message was too big to process.";
                break;
            case 1010:
                result.reason = "The client terminated the connection because it expected the server to negotiate one or more extension.";
                break;
            case 1011:
                result.reason = "The server is terminating the connection because it encountered an unexpected condition that prevented it from fulfilling the request.";
                break;
            case 1015:
                result.reason = "The connection was closed due to a failure to perform a TLS handshake.";
                break;
            default:
                result.reason = "The connection closed due to an unknown reason.";
                break;
        }

        return result;
    }
}