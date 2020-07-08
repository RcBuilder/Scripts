class ShopController {
    protected _items: Item[];
    protected _ItemTemplate: string = '';
    protected _canvas: HTMLElement;
    protected _sortBy: string = 'code';

    constructor(protected shopService: IShopService, protected document: HTMLDocument) {
        this.init();
    }

    protected init(): void {
        this._canvas = this.document.querySelector('#content');
        this.registerEvents();
        this.loadItems();        
    }

    protected async loadItems(): Promise<void> {
        this._items = await this.getItemsFromDB();       
        this.drawItems();
    }

    protected sortItems() {
        this._items.sort((a: Item, b: Item) => {
            if (a[this._sortBy] == b[this._sortBy]) return 0;
            return a[this._sortBy] > b[this._sortBy] ? 1 : -1;
        });
    }

    protected drawItems(predicate: any = null): void {
        this._canvas.innerHTML = '';

        let temp = this._items;
        if (predicate) temp = temp.filter(predicate);

        temp.forEach(item => {
            let node = document.createElement('div');                 
            node.innerHTML = `<div class="item">
	            <h3 class="truncate">${item.name}</h3>		                
	            <p>${item.saleDate.dateFormat()}</p>  
                <p>${item.desc}</p>
                <p><image class="img-thumbnail" src="${item.image || 'content/noImage.jpeg'}" /></p>
                <p>
                    <button class="btn btn-danger btn-sm" data-id="${item.code}" data-role="delete">Delete</button>
                    <button class="btn btn-primary btn-sm" data-id="${item.code}" data-role="edit" data-toggle="modal" data-target="#popupSaveItem">Edit</button>
                </p>
            </div>`;
            this._canvas.appendChild(node);
        });    
    }

    protected async registerEvents(): Promise<void> {
        this.document.querySelector('#btnSaveItem').addEventListener('click', async (e: any) => {
            let node: HTMLElement = this.document.querySelector('#popupSaveItem div.modal-body');

            this.clearValidations(node);

            let code: number = parseInt((node.querySelector('#code') as any).value); // 0 = new
            let name: string = (node.querySelector('#name') as any).value;
            let date: Date = new Date((node.querySelector('#saleDate') as any).value);
            let desc: string = (node.querySelector('#desc') as any).value;
            let image: any = await this.readFileAsData((node.querySelector('#file1')));

            if (name.trim() == '') {
                (node.querySelector('#name') as any).className += ' invalid';
                e.stopPropagation();
                return;
            }

            await this.saveItem(new Item(code, name, desc, new Date(date), image));
            this.sortItems();
            this.drawItems();
            this.clearInputs(node);
        }); 

        this.document.querySelector('[data-role="add"]').addEventListener('click', (e: any) => {
            let node: HTMLElement = this.document.querySelector('#popupSaveItem div.modal-body');
            this.clearValidations(node);
            this.clearInputs(node);
            (node.querySelector('#code') as any).value = 0; // new
            (node.querySelector('#saleDate') as any).value = new Date().dateFormat(); // new
        });

        this.document.querySelector('#ddlSort').addEventListener('change', (e: any) => {
            let ddl = (e.target as HTMLSelectElement);
            this._sortBy = ddl.options[ddl.selectedIndex].value;
            this.sortItems();
            this.drawItems();
        });

        this.document.querySelector('#btnSearch').addEventListener('click', (e: any) => {
            let searchPhrase: string = (this.document.querySelector('#txtSearch') as any).value;                        
            let predicate = x => x.code == searchPhrase || x.name == searchPhrase;
            this.drawItems(searchPhrase.trim() == '' ? null : predicate);
        });

        // monitor DOM new elements 
        this._canvas.addEventListener('DOMNodeInserted', (e: any) => {
            let rolesToProcess: HTMLElement[] = e.target.querySelectorAll('[data-role]');

            rolesToProcess.forEach(x => {
                switch (x.attributes['data-role'].value) {
                    case 'delete':
                        x.addEventListener('click', async (eInner: any) => {
                            await this.removeItem(parseInt(eInner.target.attributes['data-id'].value));
                            this.drawItems();
                        });
                    break;
                case 'edit':
                        x.addEventListener('click', (eInner: any) => {
                            let node: HTMLElement = this.document.querySelector('#popupSaveItem div.modal-body');
                            this.clearValidations(node);

                            let selected = parseInt(eInner.target.attributes['data-id'].value);
                            let item = this._items.findByProperty('code', selected);
                            (node.querySelector('#name') as any).value = item.name;
                            (node.querySelector('#saleDate') as any).value = item.saleDate.dateFormat();
                            (node.querySelector('#desc') as any).value = item.desc;
                            (node.querySelector('#code') as any).value = item.code;
                        });
                    break;            
                }
            });

        }, false)
    }

    protected async readFileAsData(inputFile) {
        return new Promise((resolve: any, reject: any) => {
            try {
                let file = inputFile.files[0];
                if (!file) {
                    resolve(null);
                    return;
                }
                let reader = new FileReader();
                reader.onload = () => {
                    resolve(reader.result);
                };
                reader.readAsDataURL(file);
            }
            catch{
                reject(null);
            }
        });        
    };

    protected async getItemsFromDB(pageNum: number = 1, pageSize: number = 10): Promise<Item[]> {
        let items: Item[] = await this.shopService.getItems(pageNum, pageSize);
        items.forEach(x => { x.saleDate = new Date(x.saleDate) });  // TODO ->> date binding issue
        return items;
    }

    protected async removeItem(code: number): Promise<void> {        
        let success = this.shopService.removeItem(code);
        if (success) this._items.removeByProperty('code', code);  
    }

    protected async saveItem(item: Item): Promise<boolean> {
        if (item.code == 0)
            return await this.addItem(item);
        return await this.updateItem(item);
    }

    protected async addItem(item: Item): Promise<boolean> {
        let newItemCode = await this.shopService.addItem(item);
        if (!newItemCode) return false;
        item.code = newItemCode;
        this._items.push(item);
        return true;
    }

    protected async updateItem(item: Item): Promise<boolean> {
        let itemIndex = this._items.findIndexByProperty('code', item.code);        
        item.image = item.image || this._items[itemIndex].image;

        let itemCode = await this.shopService.updateItem(item);
        if (!itemCode) return false;

        this._items[itemIndex] = item;     
        return true;
    }

    protected clearInputs(node: HTMLElement): void {
        node.querySelectorAll('input, textarea').forEach((x: any) => {
            // tagName_type
            switch (x.tagName.concat('_', x.type || '').toLowerCase()) {
                case 'textarea_textarea': x.value = '';
                case 'input_text':
                case 'input_hidden':
                case 'input_file': x.value = '';
                    break;
            }
        });
    }

    protected clearValidations(node: HTMLElement): void {
        node.querySelectorAll('input').forEach((x: any) => {
            x.className = x.className.replace(/\s+invalid/gi, '');
        });
    }
}