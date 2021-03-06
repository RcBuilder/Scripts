﻿interface Array<T> {
    findByProperty(propName: string, value: any): T;
    findIndexByProperty(propName: string, value: any): number;
    removeByProperty(propName: string, value: any): boolean;
}

interface Date {
    dateFormat(): string;
}

interface IItem {
    code: number,
    name: string,
    desc: string,
    saleDate: Date,
    image: string
};

interface IShopService {
    getItems(pageNum: number, pageSize: number): Promise<Item[]>,    
    addItem(item: Item): Promise<number>,
    updateItem(item: Item): Promise<number>,
    removeItem(code: number): Promise<boolean>,        
};