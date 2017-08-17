export default class TypeDescriptor {
    static get None() { return 0; }
    static get Image() { return 1; }
    static get Literals() { return 2; };
    static get Flags() { return 4; }
    static get Static() { return 8; }
    static get ValueType() { return 16; }
    static get Enumerable() { return 32; }
    static get Collection() { return 64; }
    static get List() { return 128; }
    static get AttachedProperty() { return 256; }
}