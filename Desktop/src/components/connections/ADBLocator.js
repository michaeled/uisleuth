import { remote } from 'electron';
import * as fs from 'fs';
import * as path from 'path';

export function installdir() {
    let path = null;

    if (process.platform === "darwin") {
        path = onDarwin();
    }
    else {
        path = onWindows();
    }

    return path;
}

function onWindows() {
    console.trace("windows: locating android-sdk path.");

    let winpath1 = path.join(remote.app.getPath("home"), "\\AppData\\Local\\Android\\android-sdk\\platform-tools\\adb.exe");
    let winpath2 = path.join(remote.app.getPath("home"), "\\AppData\\Local\\Android\\sdk\\platform-tools\\adb.exe");

    if (fs.existsSync(winpath1)) {
        return winpath1;
    }

    if (fs.existsSync(winpath2)) {
        return winpath2;
    }

    return null;
}

function onDarwin() {
    console.trace("osx: locating android-sdk path.");
    return null;
}