declare var PubSub;

import MessageDispatcherService from '../connections/MessageDispatcherService';

export interface IDesignerDimensions {
    width: number;
    height: number;
    density: number;
    navigationBarHeight: number;
    statusBarHeight: number;
} 

export default class DesignerDimensionsService {
    public display: IDesignerDimensions = null;

    constructor() {
        let ddToken = PubSub.subscribe(MessageDispatcherService.GetDisplayDimensionsResponse, (e, d) => {
            this.display = d;
        });

        let orToken = PubSub.subscribe(MessageDispatcherService.SetDeviceOrientationResponse, (e, d) => {
            this.display.width = d.width;
            this.display.height = d.height;
        });
    }
}