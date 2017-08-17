declare var require;

const storage = require('electron-json-storage');
const quickConnectKey = "settings.quickconnect";
const categoriesKey = "settings.propcats";
const typesKey = "settings.types";
const generalKey = "settings.general";
const appsKey = "settings.apps";

export default class StorageService {
    constructor() {
        console.trace("storage service init.");
    }

    /**
     * 
     * @param options Callback fn's act as a 'get'. It will be invoked with the saved data; passing an object will save the object.
     */
    public general(options: any) {
        // get
        if (typeof options === "function") {
            if (!options) throw "Provide a callback function.";

            storage.get(generalKey, (error, data) => {
                if (error) throw error;

                var isEmpty = Object.keys(data).length === 0;

                if (isEmpty) {
                    console.trace("No general settings retrieved.");
                }

                options(data);
            });
        }
        // set
        else {
            storage.set(generalKey, options, (error) => {
                if (error) throw error;
            });
        }
    }

    /**
     * 
     * @param options Callback fn's act as a 'get'. It will be invoked with the saved data; passing an object will save the object.
     */
    public types(options: any) {
        // get
        if (typeof options === "function") {
            if (!options) throw "Provide a callback function.";
            
            storage.get(typesKey, (error, data) => {
                if (error) throw error;

                var isEmpty = Object.keys(data).length === 0;

                if (isEmpty) {
                    console.trace("No types retrieved.");
                }

                options(data);
            });
        }
        // set
        else {
            storage.set(typesKey, options, (error) => {
                if (error) throw error;
            });
        }
    }

    /**
     * 
     * @param options Callback fn's act as a 'get'. It will be invoked with the saved data; passing an object will save the object.
     */
    public categories(options: any) {
        // get
        if (typeof options === "function") {
            if (!options) throw "Provide a callback function.";
            storage.get(categoriesKey, (error, data) => {
                if (error) throw error;

                var isEmpty = Object.keys(data).length === 0;

                if (isEmpty) {
                    console.trace("No categories retrieved.");
                }

                options(data);
            });
        }
        // set
        else {
            storage.set(categoriesKey, options, (error) => {
                if (error) throw error;
            });
        }
    }

    /**
     * 
     * @param options Callback fn's act as a 'get'. It will be invoked with the saved data; passing an object will save the object.
     */
    public quickConnect(options: any) {
        // get
        if (typeof options === "function") {
            if (!options) throw "Provide a callback function.";
            storage.get(quickConnectKey, (error, data) => {
                if (error) throw error;

                var isEmpty = Object.keys(data).length === 0;

                if (isEmpty) {
                    console.trace("No connection settings retrieved.");
                }

                options(data);
            });
        }
        // set
        else {
            var val = Object.assign({}, this.connectDefaults, options);
            storage.set(quickConnectKey, val, (error) => {
                if (error) throw error;
            });
        }
    }

    public static newType(name: string): {fullName: string} {
        return { fullName: name };
    }

    public get categoriesDefault(): {name: string, properties: any[]} {
        return { name: null, properties: [] };
    }

    public get connectDefaults(): {address: string, port: number, remember: boolean} {
        return { address: null, port: null, remember: false };
    }
}