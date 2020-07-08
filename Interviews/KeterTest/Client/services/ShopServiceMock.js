var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ShopServiceMock {
    constructor() {
        this.items = [
            new Item(1, 'Item-1', '', new Date('2020-07-22'), null),
            new Item(2, 'Item-2', '', new Date('2020-07-22'), null),
            new Item(3, 'Item-3', '', new Date('2020-07-22'), null)
        ];
    }
    getItems(pageNum, pageSize) {
        return __awaiter(this, void 0, void 0, function* () {
            return this.items.slice(0);
        });
    }
    addItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            item.code = this.items.length + 1;
            this.items.push(item);
            return item.code;
        });
    }
    updateItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            this.items[this.items.findIndexByProperty('code', item.code)] = item;
            return item.code;
        });
    }
    removeItem(code) {
        return __awaiter(this, void 0, void 0, function* () {
            let item = this.items.findByProperty('code', code);
            if (!item)
                return false;
            this.items.splice(this.items.indexOf(item), 1);
            return true;
        });
    }
}
