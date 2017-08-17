import path from 'path';
import os from 'os';
import { remote, shell } from 'electron';
import jetpack from 'fs-jetpack';
import env from './env';
import * as rounding from './components/global/utilities/math';

import ConnectionService from './components/connections/ConnectionService';
import StorageService from './components/global/StorageService';
import MessageDispatcherService from './components/connections/MessageDispatcherService';
import EditorFactory from './components/inspector/EditorFactory';
import CategoryGroupFactory from './components/inspector/CategoryGroupFactory';
import DeviceDialogFactory from './components/device/DeviceDialogFactory';

import MainController from './components/global/MainController';
import StatusbarController from './components/footer/StatusbarController';
import ConnectionManagerController from './components/connections/ConnectionManagerController';
import DeviceViewController from './components/inspector/DeviceViewController';
import VisualTreeTabController from './components/inspector/VisualTreeTabController';
import AttachedPropertiesTabController from './components/inspector/AttachedPropertiesTabController';
import CategoriesController from './components/header/CategoriesController';
import SettingsController from './components/header/SettingsController';
import PropertiesTabController from './components/inspector/PropertiesTabController';
import EventsTabController from './components/inspector/EventsTabController';
import TypesController from './components/header/TypesController';
import OverviewPanelController from './components/inspector/panels/OverviewPanelController';
import StringDialogController from './components/inspector/editors/StringDialogController';
import WebViewSourceDialogController from './components/inspector/editors/WebViewSourceDialogController';
import ConsoleController from './components/console/ConsoleController';
import UltraBarController from './components/inspector/UltraBarController';
import VisualTreeTabToolbarController from './components/inspector/VisualTreeTabToolbarController';
import DisplayPanelController from './components/inspector/panels/DisplayPanelController';
import HistoryTabController from './components/inspector/HistoryTabController';
import AboutDialogController from './components/help/AboutDialogController';
import DeviceFeaturesController from './components/device/DeviceFeaturesController';
import DevicePropertiesController from './components/device/DevicePropertiesController';
import DeviceProcessorController from './components/device/DeviceProcessorController';
import NavBarController from './components/header/NavBarController';
import WorkspaceController from './components/inspector/WorkspaceController';

import DialogDirective from './components/global/directives/DialogDirective';
import LastDirective from './components/global/directives/LastDirective';
import EscapeDirective from './components/global/directives/EscapeDirective';
import FocusDirective from './components/global/directives/FocusDirective';
import EnterDirective from './components/global/directives/EnterDirective';

import BindingContextDialogService from './components/inspector/ultrabar/BindingContextDialogService';
import XamlChangeTrackerService from './components/inspector/xaml/XamlChangeTrackerService';
import AndroidDeviceService from './components/connections/AndroidDeviceService';
import DeviceCanvasService from './components/inspector/DeviceCanvasService';
import DesignerDimensionsService from './components/global/DesignerDimensionsService';
import DesignerVisualElementsService from './components/global/DesignerVisualElementsService';
import AutoUpdaterService from './components/global/AutoUpdaterService';
import AnnouncementService from './components/connections/AnnouncementService';
import PollService from './components/connections/PollService';

var app = angular.module("uisleuth", [
    "ngRoute",
    "ya.nouislider",
    "color.picker",
    "ui.knob",
    "ngDialog",
    "angular.panels",
    "ui.tree"
]);

// controllers
app
    .controller("MainController", MainController)
    .controller("ConnectionManagerController", ConnectionManagerController)
    .controller("DeviceViewController", DeviceViewController)
    .controller("StatusbarController", StatusbarController)
    .controller("VisualTreeTabController", VisualTreeTabController)
    .controller("CategoriesController", CategoriesController)
    .controller("PropertiesTabController", PropertiesTabController)
    .controller("SettingsController", SettingsController)
    .controller("TypesController", TypesController)
    .controller("OverviewPanelController", OverviewPanelController)
    .controller("StringDialogController", StringDialogController)
    .controller("WebViewSourceDialogController", WebViewSourceDialogController)
    .controller("ConsoleController", ConsoleController)
    .controller("EventsTabController", EventsTabController)
    .controller("AttachedPropertiesTabController", AttachedPropertiesTabController)
    .controller("UltraBarController", UltraBarController)
    .controller("VisualTreeTabToolbarController", VisualTreeTabToolbarController)
    .controller("DisplayPanelController", DisplayPanelController)
    .controller("HistoryTabController", HistoryTabController)
    .controller("AboutDialogController", AboutDialogController)
    .controller("DeviceFeaturesController", DeviceFeaturesController)
    .controller("DevicePropertiesController", DevicePropertiesController)
    .controller("DeviceProcessorController", DeviceProcessorController)
    .controller("NavBarController", NavBarController)
    .controller("WorkspaceController", WorkspaceController);

// directives
app
    .directive("uisFocus", FocusDirective)
    .directive("uisEnter", EnterDirective)
    .directive("uisLast", LastDirective)
    .directive("uisEsc", EscapeDirective)
    .directive("uisDialog", () => new DialogDirective);

// factories
app
    .factory("EditorFactory", EditorFactory)
    .factory("CategoryGroupFactory", CategoryGroupFactory)
    .factory("DeviceDialogFactory", DeviceDialogFactory);

// services
app
    .service("ConnectionService", ConnectionService)
    .service("AndroidDeviceService", AndroidDeviceService)
    .service("MessageDispatcherService", MessageDispatcherService)
    .service("DeviceCanvasService", DeviceCanvasService)
    .service("DesignerDimensionsService", DesignerDimensionsService)
    .service("StorageService", StorageService)
    .service("DesignerVisualElementsService", DesignerVisualElementsService)
    .service("BindingContextDialogService", BindingContextDialogService)
    .service("AutoUpdaterService", AutoUpdaterService)
    .service("AnnouncementService", AnnouncementService)
    .service("PollService", PollService)
    .service("XamlChangeTrackerService", XamlChangeTrackerService);

// constants
app.constant('uisleuth-events', {
    InspectorPageClosing: 'InspectorPageClosing'
});

// configure routing
app.config(["$routeProvider", function($routeProvider) {
    $routeProvider.when("/", {
        controller: "ConnectionManagerController",
        controllerAs: "manager",
        templateUrl: "components/connections/manager.htm"
    });

    $routeProvider.when("/inspector", {
        templateUrl: "components/inspector/workspace.htm"
    });
}]);

//  configure color picker
app.config(["$provide", function($provide) {
    $provide.decorator('ColorPickerOptions', function($delegate) {
        var options = angular.copy($delegate);
        options.displayPrevious = true;
        options.round = false;
        options.alpha = true;
        options.hue = true;
        options.swatch = true;
        options.saturation = true;
        options.lightness = true;
        options.pos = 'left';
        options.case = 'upper';
        options.placeholder = 'Transparent';
        options.format = 'hex';
        options.clear = {
            show: true,
            label: 'Clear'
        };
        return options;
    });
}]);

// configure panel provider
app.config(['panelsProvider', function(panelsProvider) {
    panelsProvider
        .add({
            id: 'console',
            position: 'bottom',
            size: '33%',
            templateUrl: './components/console/console.htm',
            controller: 'ConsoleController as cpctrl',
            openCallbackFunction: 'cpctrl.showing'
        })
}]);

app.run(["$rootScope", "ConnectionService", "$location", "uisleuth-events", function($rootScope, connection, $location, uisevents) {
    $rootScope.$on("$routeChangeStart", function(event, next, current) {
        if (!connection.connected) {
            $location.path("/");
        }

        if (current == null) return;

        let nextPath = next.$$route.originalPath;
        let currPath = current.$$route.originalPath;

        // user clicked on tab they are currently viewing
        if (nextPath == currPath) return;

        if (currPath == "/inspector" && (nextPath == "/") || nextPath == "") {
            let forced = next.params.force;
            let ok = false;

            if (!forced) {
                ok = window.confirm("You are about disconnect from the current device. Continue?");
            }

            if (forced || ok) {
                PubSub.publish(uisevents.InspectorPageClosing, true);
                connection.close();
            } else {
                event.preventDefault();
            }
        }
    });
}]);

document.addEventListener("DOMContentLoaded", function() {
    $("body").tooltip({
        selector: "[data-toggle=tooltip]"
    });
});

$(document).on("click", "a[href^='http']", function(event) {
    event.preventDefault();
    shell.openExternal(this.href);
});

console.log("environment variables:", env);
console.log("app data path: " + remote.app.getPath("userData"));