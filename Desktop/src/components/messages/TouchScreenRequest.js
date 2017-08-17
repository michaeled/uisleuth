export default class TouchScreenRequest {
    constructor(x, y, duration) {
        this.action = "TouchScreenRequest";
        this.x = x;
        this.y = y;
        this.duration = duration;
    }
}