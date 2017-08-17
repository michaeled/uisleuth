declare var PubSub;
declare var $;
declare var fabric;

import CanvasObjectPopup from './CanvasObjectPopup';

const MinTabletWidth = 450;
const MaxTabletWidth = 1000;

export default class DeviceCanvasService {
    public static $inject = ["uisleuth-events"];
    
    public device: any = null;
    public visualElementsSelectable: boolean = false;
    private _ready: boolean = false;
    private _scaleX: number = 0;
    private _scaleY: number = 0;

    private get $output() {
        return $("#device-output");
    }

    private get $position() {
        return $("#device-position");
    }

    private get $frame() {
        return $("#device-frame");
    }

    private get availableSpace(): { width: number, height: number} {
        let width =  this.$position.width();
        let height = this.$position.height();
        return { width, height }
    }

    constructor(private uisevents) {
        PubSub.subscribe(uisevents.InspectorPageClosing, (e, d) => {
            console.trace("Resetting device canvas.");
            this.reset();
        });

        console.trace("DeviceCanvasService init");
    }

    public get ready(): boolean {
        return this._ready;
    }

    public get size(): { width: number, height: number} {
        let width = this.$output.width();
        let height = this.$output.height();
        return { width, height }
    }

    public get scale(): { scaleX: number, scaleY: number} {
        return {scaleX: this._scaleX, scaleY: this._scaleY};
    }

    public show(src : string): void {
        if (!this.ready) {
            console.trace("Device canvas has not been initialized.");
            return;
        }

        fabric.Image.fromURL(src, screenshot => {
            let scaleWidth = this.availableSpace.width;
            let scaleHeight = this.availableSpace.height;
            
            if (screenshot.width > screenshot.height) {
                this.$frame.removeClass("phone");
                this.$frame.addClass("tablet");

                // don't let the canvas shrink less than MinTabletWidth
                if (screenshot.width >= MinTabletWidth && this.availableSpace.width < MinTabletWidth) {
                    scaleWidth = MinTabletWidth;
                }

                if (screenshot.width >= MaxTabletWidth && this.availableSpace.width > MaxTabletWidth) {
                    scaleWidth = MaxTabletWidth;
                }
                
                screenshot.scaleToWidth(scaleWidth);
            }
            else {
                this.$frame.removeClass("tablet");
                this.$frame.addClass("phone");
                
                screenshot.scaleToHeight(scaleHeight);
            }
            
            let w = screenshot.getWidth();
            let h = screenshot.getHeight();

            this._scaleX = screenshot.scaleX;
            this._scaleY = screenshot.scaleY;
            
            this.$frame.height(h);
            this.$frame.width(w);

            this.device.setDimensions({
                width: w,
                height: h
            });

            this.device.setBackgroundImage(screenshot, this.device.renderAll.bind(this.device));
        });
    }

    public reset(): void {
        this._ready = false;
        this.device = null;
        this.visualElementsSelectable = false;
        window.removeEventListener('resize', this.resize.bind(this));
    }

    public init(): void {
        if (this.ready) {
            console.log("The device canvas is already ready; unecessary call.")
            return;
        }

        this.device = new fabric.StaticCanvas("device-output");
        this.device.defaultCursor = 'pointer';
                
        this.resize();
        window.addEventListener('resize', this.resize.bind(this));

        this.device.on({
            'object:scaling': function(e) {
                var obj = e.target;
                if (obj._uiStrokeWidth) {
                    var newStrokeWidth = obj._uiStrokeWidth / ((obj.scaleX + obj.scaleY) / 2);
                    obj.set('strokeWidth', newStrokeWidth);
                }
            }
        });

        this.device.on({
            'object:selected': function(e) {
                if (!e.target._uiPopup) {
                    e.target._uiPopup = new CanvasObjectPopup();
                }

                if (e.target._uiPopup.show) {
                    e.target._uiPopup.show(e);
                } else {
                    console.error("unable to show popup for canvas object");
                }
            }
        });

        this.device.on({
            'selection:cleared': function(e) {
                if (e.target && e.target._uiPopup) {
                    e.target._uiPopup.hide();
                }
            }
        });

        this._ready = true;
    }

    private resize(): void {
        this.device.setDimensions({
            width: this.size.width,
            height: this.size.height
        });

        if (this.device.backgroundImage) {
            this.device.backgroundImage.scaleToWidth(this.size.width);
        }
    }
}