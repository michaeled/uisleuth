export default class GestureRequest {
    public action: string;
    
    constructor(public path: {x: number, y: number}[], public duration: number) {
        this.action = "GestureRequest";
    }
}