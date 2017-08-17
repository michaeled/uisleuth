import { ipcRenderer, remote } from 'electron';
import { ShowAboutDialog } from '../shell/events';

export default class AboutDialogController {
    public static $inject = ['$scope'];
    public show: boolean;
    public version: string;

    constructor(private $scope) {
        this.show = false;

        ipcRenderer.on(ShowAboutDialog, e => {
            this.version = remote.app.getVersion();
            this.show = !this.show;
            $scope.$digest();
        });

        console.trace("about dialog init");
    }
}