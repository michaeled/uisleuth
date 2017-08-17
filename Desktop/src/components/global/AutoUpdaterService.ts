declare var require;

import { remote } from 'electron';
import StorageService from './StorageService';

const http = require('http');
const os = require('os');
const env = require('./env');

const autoUpdater = remote.autoUpdater;

export default class AutoUpdaterService {
    public static $inject = ["StorageService"];

    constructor(private storage: StorageService) {
        console.trace("AutoUpdaterService init.");
    }

    public init() {
        if (env.name === "development") {
            console.log("AutoUpdateService: in development mode. stopping.")
            return;
        };

        autoUpdater.on('update-downloaded', (e) => {
            if (this.showUpdateDownloaded() == 0) {
                autoUpdater.quitAndInstall();
            }
        });

        this.update();
    }

    private update() {
        this.storage.general(data => {
            http.get(data.releases.url, res => {
                const feedUrl = this.getFeedUrl(data.releases.url);
                console.log("AutoUpdaterService: feed url " + feedUrl);

                autoUpdater.setFeedURL(feedUrl);
                autoUpdater.checkForUpdates();
            }).on('error', function(e) {
                console.log('AutoUpdateService: ', e);
            });
        });
    }

    private getFeedUrl(url: string): string {
        const platform = os.platform();
        const mac = platform + "_" + os.arch();
        const version = remote.app.getVersion();
        
        if (!url.endsWith("/")) {
            url += "/";
        }

        if (platform === "darwin") {
            return `${url}update/${mac}/${version}`;
        }
        else {
            return `${url}update/win32/${version}`
        }
    }

    private showUpdateDownloaded() {
        let image = remote.nativeImage.createFromPath("app/components/global/logo24x24.png");
        const response = remote.dialog.showMessageBox(remote.getCurrentWindow(), {
            title: "UI Sleuth",
            message: "An update has been downloaded.\nThis will cause the application to restart. Would you like to install it?",
            buttons: ["Restart and Install", "Later"]
        });

        return response;
    }
}