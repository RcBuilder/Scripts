var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ShopController {
    constructor(shopService, document) {
        this.shopService = shopService;
        this.document = document;
        this._ItemTemplate = '';
        this._sortBy = 'code';
        this.init();
    }
    init() {
        this._canvas = this.document.querySelector('#content');
        this.registerEvents();
        this.loadItems();
    }
    loadItems() {
        return __awaiter(this, void 0, void 0, function* () {
            this._items = yield this.getItemsFromDB();
            this.drawItems();
        });
    }
    sortItems() {
        this._items.sort((a, b) => {
            if (a[this._sortBy] == b[this._sortBy])
                return 0;
            return a[this._sortBy] > b[this._sortBy] ? 1 : -1;
        });
    }
    drawItems(predicate = null) {
        this._canvas.innerHTML = '';
        let temp = this._items;
        if (predicate)
            temp = temp.filter(predicate);
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
    registerEvents() {
        return __awaiter(this, void 0, void 0, function* () {
            this.document.querySelector('#btnSaveItem').addEventListener('click', (e) => __awaiter(this, void 0, void 0, function* () {
                let node = this.document.querySelector('#popupSaveItem div.modal-body');
                this.clearValidations(node);
                let code = parseInt(node.querySelector('#code').value); // 0 = new
                let name = node.querySelector('#name').value;
                let date = new Date(node.querySelector('#saleDate').value);
                let desc = node.querySelector('#desc').value;
                let image = yield this.readFileAsData((node.querySelector('#file1')));
                if (name.trim() == '') {
                    node.querySelector('#name').className += ' invalid';
                    e.stopPropagation();
                    return;
                }
                yield this.saveItem(new Item(code, name, desc, new Date(date), image));
                this.sortItems();
                this.drawItems();
                this.clearInputs(node);
            }));
            this.document.querySelector('[data-role="add"]').addEventListener('click', (e) => {
                let node = this.document.querySelector('#popupSaveItem div.modal-body');
                this.clearValidations(node);
                this.clearInputs(node);
                node.querySelector('#code').value = 0; // new
                node.querySelector('#saleDate').value = new Date().dateFormat(); // new
            });
            this.document.querySelector('#ddlSort').addEventListener('change', (e) => {
                let ddl = e.target;
                this._sortBy = ddl.options[ddl.selectedIndex].value;
                this.sortItems();
                this.drawItems();
            });
            this.document.querySelector('#btnSearch').addEventListener('click', (e) => {
                let searchPhrase = this.document.querySelector('#txtSearch').value;
                let predicate = x => x.code == searchPhrase || x.name == searchPhrase;
                this.drawItems(searchPhrase.trim() == '' ? null : predicate);
            });
            // monitor DOM new elements 
            this._canvas.addEventListener('DOMNodeInserted', (e) => {
                let rolesToProcess = e.target.querySelectorAll('[data-role]');
                rolesToProcess.forEach(x => {
                    switch (x.attributes['data-role'].value) {
                        case 'delete':
                            x.addEventListener('click', (eInner) => __awaiter(this, void 0, void 0, function* () {
                                yield this.removeItem(parseInt(eInner.target.attributes['data-id'].value));
                                this.drawItems();
                            }));
                            break;
                        case 'edit':
                            x.addEventListener('click', (eInner) => {
                                let node = this.document.querySelector('#popupSaveItem div.modal-body');
                                this.clearValidations(node);
                                let selected = parseInt(eInner.target.attributes['data-id'].value);
                                let item = this._items.findByProperty('code', selected);
                                node.querySelector('#name').value = item.name;
                                node.querySelector('#saleDate').value = item.saleDate.dateFormat();
                                node.querySelector('#desc').value = item.desc;
                                node.querySelector('#code').value = item.code;
                            });
                            break;
                    }
                });
            }, false);
        });
    }
    readFileAsData(inputFile) {
        return __awaiter(this, void 0, void 0, function* () {
            return new Promise((resolve, reject) => {
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
                catch (_a) {
                    reject(null);
                }
            });
        });
    }
    ;
    getItemsFromDB(pageNum = 1, pageSize = 10) {
        return __awaiter(this, void 0, void 0, function* () {
            let items = yield this.shopService.getItems(pageNum, pageSize);
            items.forEach(x => { x.saleDate = new Date(x.saleDate); }); // TODO ->> date binding issue
            return items;
        });
    }
    removeItem(code) {
        return __awaiter(this, void 0, void 0, function* () {
            let success = this.shopService.removeItem(code);
            if (success)
                this._items.removeByProperty('code', code);
        });
    }
    saveItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            if (item.code == 0)
                return yield this.addItem(item);
            return yield this.updateItem(item);
        });
    }
    addItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let newItemCode = yield this.shopService.addItem(item);
            if (!newItemCode)
                return false;
            item.code = newItemCode;
            this._items.push(item);
            return true;
        });
    }
    updateItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let itemIndex = this._items.findIndexByProperty('code', item.code);
            item.image = item.image || this._items[itemIndex].image;
            let itemCode = yield this.shopService.updateItem(item);
            if (!itemCode)
                return false;
            this._items[itemIndex] = item;
            return true;
        });
    }
    clearInputs(node) {
        node.querySelectorAll('input, textarea').forEach((x) => {
            // tagName_type
            switch (x.tagName.concat('_', x.type || '').toLowerCase()) {
                case 'textarea_textarea': x.value = '';
                case 'input_text':
                case 'input_hidden':
                case 'input_file':
                    x.value = '';
                    break;
            }
        });
    }
    clearValidations(node) {
        node.querySelectorAll('input').forEach((x) => {
            x.className = x.className.replace(/\s+invalid/gi, '');
        });
    }
}
