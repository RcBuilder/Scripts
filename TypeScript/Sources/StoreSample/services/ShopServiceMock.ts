class ShopServiceMock implements IShopService {
    constructor() { }

    protected items: Item[] = [
        new Item(1, 'Item-1', '', new Date('2020-07-22'), null),
        new Item(2, 'Item-2', '', new Date('2020-07-22'), null),
        new Item(3, 'Item-3', '', new Date('2020-07-22'), null)
    ];    

    async getItems(pageNum: number, pageSize: number): Promise<Item[]> {
        return this.items.slice(0);
    }

    async addItem(item: Item): Promise<number> {
        item.code = this.items.length + 1;
        this.items.push(item);        
        return item.code;
    }

    async updateItem(item: Item): Promise<number> {                
        this.items[this.items.findIndexByProperty('code', item.code)] = item;
        return item.code;
    }

    async removeItem(code: number): Promise<boolean> {
        let item = this.items.findByProperty('code', code);
        if (!item) return false;
        this.items.splice(this.items.indexOf(item), 1);        
        return true;
    }
}