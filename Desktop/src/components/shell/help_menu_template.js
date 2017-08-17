import { BrowserWindow, shell } from 'electron';
import { ShowAboutDialog } from './events';

export var helpMenuTemplate = {
    label: 'Help',
    submenu: 
    [
        {
            label: 'About',
            click: function() {
                BrowserWindow.getFocusedWindow().webContents.send(ShowAboutDialog, null);
            },
        },
        {
            type: 'separator'
        },
        {
            label: 'Feedback',
            click: function() {
                shell.openExternal("https://uisleuth.wufoo.com/forms/mbszwut0x31ua4/");
            },
        },
        {
            label: 'Release Notes',
            click: function() {
                shell.openExternal("http://www.uisleuth.com/release-notes/");
            },
        }
    ]
};