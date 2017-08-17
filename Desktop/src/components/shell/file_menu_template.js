import { app, BrowserWindow } from 'electron';

export var fileMenuTemplate = {
    label: 'File',
    submenu: [{
        label: 'Quit',
        accelerator: 'CmdOrCtrl+Q',
        click: function() {
            app.quit();
        }
    }]
};