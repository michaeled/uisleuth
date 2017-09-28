declare var PubSub;
declare var $;
declare var angular;

import DesignerVisualElementsService from './DesignerVisualElementsService';
import ConnectionService from '../connections/ConnectionService';
import MessageDispatcherService from '../connections/MessageDispatcherService';
import AutoUpdaterService from './AutoUpdaterService';
import AnnouncementService from '../connections/AnnouncementService';

export default class MainController {
    
    public static $inject = [
        "$scope",
        "ConnectionService",
        "MessageDispatcherService",
        "DesignerVisualElementsService",
        "AutoUpdaterService",
        "AnnouncementService"
    ];

    constructor(
        private $scope,
        private connection: ConnectionService, 
        private messages: MessageDispatcherService,
        private elements: DesignerVisualElementsService, 
        private auto: AutoUpdaterService, 
        private announcement: AnnouncementService) {

            auto.init();
            console.trace("main controller init.");
    }

    public get connected() {
        return this.connection.connected;
    }

    public onCloseMegaMenu(): void {
        let expanded = $("#demo-set-body").attr("aria-expanded") || false;

        if (expanded) {
            this.$scope.$broadcast("uisleuth.megamenu.closing");
        } else {
            this.$scope.$broadcast("uisleuth.megamenu.opening");
        }
    }
}