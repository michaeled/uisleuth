import { BrowserWindow } from 'electron';
import { SetOrientation } from './events';

export var deviceMenuTemplate = {
    label: 'Device',
    submenu: 
    [
        {
            label: 'Orientation',
            submenu: [
                {
                    label: 'Portrait',
                    click: function() {
                        BrowserWindow.getFocusedWindow().webContents.send(SetOrientation, 'Portrait');
                    }
                },
                {
                    label: 'Reverse Portrait',
                    click: function() {
                        BrowserWindow.getFocusedWindow().webContents.send(SetOrientation, 'ReversePortrait');
                    }
                },
                {
                    type: 'separator'
                },
                {
                    label: 'Landscape',
                    click: function() {
                        BrowserWindow.getFocusedWindow().webContents.send(SetOrientation, 'Landscape');
                    }
                },
                {
                    label: 'Reverse Landscape',
                    click: function() {
                        BrowserWindow.getFocusedWindow().webContents.send(SetOrientation, 'ReverseLandscape');
                    }
                }
            ]
        }
    ]
};