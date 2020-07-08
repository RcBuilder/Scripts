class ShopService implements IShopService {
    private static server: string = 'http://localhost:1166/';
    private static endpoints: Map<string, string> = new Map([
        ['getItems', 'store/items'],
        ['addItem', 'store/item'],
        ['updateItem', 'store/item'],
        ['removeItem', 'store/item/{code}'],
    ]);

    constructor() { }

    async getItems(pageNum: number, pageSize: number = 0 /* 0 = ALL */): Promise<Item[]> {
        let options = {
            method: 'GET',
        }

        try {            
            let response: any = await fetch(ShopService.server.concat(ShopService.endpoints.get('getItems')), options);
            return (await response.json()).map(x => new Item(x.code, x.name, x.desc, new Date(x.saleDate), x.image));          
        }
        catch (e) {
            console.error(e);
            return null;
        }
    }

    async addItem(item: Item): Promise<number> {
        let options = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        }

        try {
            let response = await fetch(ShopService.server.concat(ShopService.endpoints.get('addItem')), options);
            return response.json() as Promise<number>;
        }
        catch (e) {
            console.error(e);
            return null;
        }
    }

    async updateItem(item: Item): Promise<number> {                
        let options = {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(item)
        }

        try {
            let response = await fetch(ShopService.server.concat(ShopService.endpoints.get('updateItem')), options);
            return response.json() as Promise<number>; 
        }
        catch (e) {
            console.error(e);
            return null;
        }
    }

    async removeItem(code: number): Promise<boolean> {
        let options = {
            method: 'DELETE',         
        }

        try {
            let response = await fetch(ShopService.server.concat(ShopService.endpoints.get('removeItem')).replace('{code}', String(code)), options);
            return response.json() as Promise<boolean>;       
        }
        catch (e) {
            console.error(e);
            return false;
        }
    }
}