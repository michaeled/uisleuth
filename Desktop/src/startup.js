import { app } from 'electron';
import env from './env';
import * as path from 'path';
import * as fs from 'fs-extra';

const defaultsdir = "defaults";

export function init() {
    copyDefaults();
    console.log("environment startup init.");
}

function copyDefaults() {
    let target = app.getPath("userData");
    let source = null;

    if (env.name === "development") {
        source = defaultsdir;
    } else if (env.name === "production") {
        source = path.join(path.dirname(__dirname), defaultsdir);
    }

    fs.copy(source, target, { overwrite: false }, function(err) {
        if (err) return console.error(err);
    });
}