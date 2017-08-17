export default class SetDeviceOrientationRequest {
    public action: string = "SetDeviceOrientationRequest";

    constructor(public orientation: "Portrait" | "Landscape") {
    }
}