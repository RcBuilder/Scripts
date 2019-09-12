export class MyModule3A {
    private _items: string[] = [];
    constructor(public orderId: number) {}

    public add = (name: string): void => {
        if (!this.validate(name)) return;
        this._items.push(name);
    }

    get items(): string[] {
        return this._items;
    }

    private validate(name: string): boolean {
        return name && name.trim().length > 0;
    }
};

export class MyModule3B {    
    public sayHello(name: string): void {
        console.log(`hello ${name}`)
    }
};

interface iConfig { key1: string, key2: string }
export const MyModule3Config: iConfig = {
    key1: 'value1',
    key2: 'value2'
};

export enum MyModule3eColors {
    RED, GREEN, BLUE
}