import { app, Menu, autoUpdater } from 'electron';
if(require('electron-squirrel-startup')) app.quit();

import { devMenuTemplate } from './components/shell/dev_menu_template';
import { fileMenuTemplate } from './components/shell/file_menu_template';
import { helpMenuTemplate } from './components/shell/help_menu_template';
import { deviceMenuTemplate } from './components/shell/device_menu_template';

import createWindow from './components/shell/window';
import * as startup from './startup';
import env from './env';

app.commandLine.appendSwitch("ignore-certificate-errors");

var mainWindow;

app.on('ready', function() {
    setApplicationMenu();

    var mainWindow = createWindow('main', {
        width: 1024,
        height: 768,
        icon: __dirname + '/components/global/app.ico'
    });

    mainWindow.loadURL('file://' + __dirname + '/app.html');

    if (env.name === 'development') {
        mainWindow.openDevTools();
    }
});

app.on('window-all-closed', function() {
    app.quit();
});

app.on('certificate-error', (event, webContents, url, error, certificate, callback) => {
    if (certificate.fingerprint === "sha256/g4WSkq9+yK1jAMbO1QNKxZcqZVQ3qwH+qFemcmaEzMQ=") {
        callback(true);
    } else {
        callback(false);
    }
});

var setApplicationMenu = function() {
    var menus = [fileMenuTemplate, deviceMenuTemplate, helpMenuTemplate];
    if (env.name !== 'production') {
        menus.push(devMenuTemplate);
    }

    Menu.setApplicationMenu(Menu.buildFromTemplate(menus));
};

if (env.name !== 'production') {
    var userDataPath = app.getPath('userData');
    app.setPath('userData', userDataPath + ' (' + env.name + ')');
}

startup.init();