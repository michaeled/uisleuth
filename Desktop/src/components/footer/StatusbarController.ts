declare var PubSub;

const DefaultMessage : string = "Ready.";
const ErrorEvent : string = "SBC.ErrorEvent";
const InfoEvent : string = "SBC.InfoEvent";

export default class StatusbarController {
    static $inject = ["$scope", "panels"];

    public message : string = null;
    public showError : boolean = false;
    private _panels;
    
    static Info(reason: string = DefaultMessage): void {
        PubSub.publish(InfoEvent, {reason});
    }

    static Error(code: number, reason: string): void {
        PubSub.publish(ErrorEvent, {code, reason});
    }

    constructor($scope, panels) {
        this.message = DefaultMessage;
        this.showError = false;
        this._panels = panels;

        let ieToken = PubSub.subscribe(InfoEvent, (e, d) => {
            this.message = d.reason;
            this.showError = false;
            $scope.$digest();
        });

        let eeToken = PubSub.subscribe(ErrorEvent, (e, d) => {
            this.message = `Error Code ${d.code}: ${d.reason}`;
            this.showError = true;
            $scope.$digest();
        });

        console.trace("statusbar controller init.");
    }

    openConsole() {
        this._panels.open("console");
    }
}